﻿#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
	Exception are projects where it is noted otherwhise.
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Util;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// Some namespace mappings
using D3D = SharpDX.Direct3D;

namespace SeeingSharp.Multimedia.Core
{
    public class ViewRelatedSceneLayerSubset : IDisposable
    {
        private const int DEFAULT_PASS_SUBSCRIPTION_LENGTH = 1024;

        #region State members
        private bool m_disposed;
        #endregion

        #region Configuration member
        private Scene m_scene;
        private SceneLayer m_sceneLayer;
        private ViewInformation m_viewInformation;
        private EngineDevice m_device;
        private ResourceDictionary m_resources;
        #endregion

        #region Temporary collections
        private List<Tuple<SceneObject, bool, bool>> m_tmpChangedVisibilities;
        #endregion

        #region Special members for subscribe/unsubscribe pass logic
        private bool m_isSubscribeUnsubscribeAllowed;
        private Action m_changedVisibilitiesAction;
        #endregion

        #region Objects that raises exceptions during render
        private Dictionary<SceneObject, object> m_invalidObjects;
        private Queue<SceneObject> m_invalidObjectsToDeregister;
        #endregion

        #region Resources for rendering
        private RenderPassLineRender m_renderPassLineRender;
        private RenderPassDefaultTransparent m_renderPassTransparent;
        private RenderPass2DOverlay m_renderPass2DOverlay;
        private ViewRenderParameters m_renderParameters;
        #endregion

        #region Subscription collections
        // All collections needed to link all scene objects to corresponding render passes
        // => This collections are updated using UpdateForView logic
        private Dictionary<RenderPassInfo, PassSubscribionProperties> m_objectsPerPassDict;
        private List<PassSubscribionProperties> m_objectsPerPass;
        private PassSubscribionProperties m_objectsPassPlainRender;
        private PassSubscribionProperties m_objectsPassLineRender;
        private PassSubscribionProperties m_objectsPassTransparentRender;
        private PassSubscribionProperties m_objectsPassSpriteBatchRender;
        private PassSubscribionProperties m_objectsPass2DOverlay;
        private bool m_anythingUnsubscribed;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewRelatedSceneLayerSubset" /> class.
        /// </summary>
        internal ViewRelatedSceneLayerSubset(SceneLayer sceneLayer, ViewInformation viewInformation, ResourceDictionary resources, int viewIndex)
        {
            m_scene = sceneLayer.Scene;
            m_sceneLayer = sceneLayer;
            m_viewInformation = viewInformation;
            m_device = ViewInformation.Device;
            m_resources = resources;
            ViewIndex = viewIndex;

            m_invalidObjects = new Dictionary<SceneObject, object>();
            m_invalidObjectsToDeregister = new Queue<SceneObject>();

            // Create temporary collections
            m_tmpChangedVisibilities = new List<Tuple<SceneObject, bool, bool>>();

            // Create all specialized render pass lists
            m_objectsPassPlainRender = new PassSubscribionProperties();
            m_objectsPassLineRender = new PassSubscribionProperties();
            m_objectsPassTransparentRender = new PassSubscribionProperties();
            m_objectsPassSpriteBatchRender = new PassSubscribionProperties();
            m_objectsPass2DOverlay = new PassSubscribionProperties();

            // Create dictionary for fast access to all render pass list
            m_objectsPerPassDict = new Dictionary<RenderPassInfo, PassSubscribionProperties>();
            m_objectsPerPassDict[RenderPassInfo.PASS_PLAIN_RENDER] = m_objectsPassPlainRender;
            m_objectsPerPassDict[RenderPassInfo.PASS_LINE_RENDER] = m_objectsPassLineRender;
            m_objectsPerPassDict[RenderPassInfo.PASS_TRANSPARENT_RENDER] = m_objectsPassTransparentRender;
            m_objectsPerPassDict[RenderPassInfo.PASS_SPRITE_BATCH] = m_objectsPassSpriteBatchRender;
            m_objectsPerPassDict[RenderPassInfo.PASS_2D_OVERLAY] = m_objectsPass2DOverlay;
            m_objectsPerPass = new List<PassSubscribionProperties>(m_objectsPerPassDict.Values);

            m_anythingUnsubscribed = false;

            // Create and load all render pass relevant resources
            RefreshDeviceDependentResources();
        }

        /// <summary>
        /// Führt anwendungsspezifische Aufgaben aus, die mit dem Freigeben, Zurückgeben oder Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
        /// </summary>
        public void Dispose()
        {
            if (m_disposed) { return; }

            m_renderParameters = null;
            m_renderPassLineRender = null;
            m_renderPassTransparent = null;

            m_disposed = true;
        }

        /// <summary>
        /// Registers the given collection of objects on this view subset.
        /// </summary>
        /// <param name="sceneObjects">The scene objects to be registered.</param>
        internal void RegisterObjectRange(params SceneObject[] sceneObjects)
        {
            if (m_disposed) { throw new ObjectDisposedException("ViewRelatedLayerSubset"); }

            int length = sceneObjects.Length;
            for (int loop = 0; loop < length; loop++)
            {
                SceneObject actSceneObject = sceneObjects[loop];
                if (m_invalidObjects.ContainsKey(actSceneObject)) { continue; }

                if((actSceneObject.TargetDetailLevel & m_device.SupportedDetailLevel) == m_device.SupportedDetailLevel)
                {
                    sceneObjects[loop].RegisterLayerViewSubset(this);
                }
            }
        }

        /// <summary>
        /// Clears all resources created by this view subset.
        /// </summary>
        internal void ClearResources()
        {
            m_renderParameters = null;
            m_renderPass2DOverlay = null;
            m_renderPassLineRender = null;
            m_renderPassTransparent = null;
        }

        /// <summary>
        /// Deregisters the given object form this view subset.
        /// </summary>
        /// <param name="sceneObject">The object to be removed.</param>
        internal void DeregisterObject(SceneObject sceneObject)
        {
            if (m_disposed) { throw new ObjectDisposedException("ViewRelatedLayerSubset"); }

            // Perform unsubscription here (allowed because this call comes from update process)
            m_isSubscribeUnsubscribeAllowed = true;
            try
            {
                if (sceneObject.IsLayerViewSubsetRegistered(this.ViewIndex))
                {
                    sceneObject.UnsubsribeFromAllPasses(this);
                    sceneObject.DeregisterLayerViewSubset(this);
                }
            }
            finally
            {
                m_isSubscribeUnsubscribeAllowed = false;
            }
        }

        /// <summary>
        /// Clears all subscriptions 
        /// </summary>
        /// <param name="allObjects">A collection containing all objects of the current layer.</param>
        internal void ClearAllSubscriptions(List<SceneObject> allObjects)
        {
            if (m_disposed) { throw new ObjectDisposedException("ViewRelatedLayerSubset"); }

            // Clear all subscriptions on the SceneObject intances
            int length = allObjects.Count;
            for (int loop = 0; loop < length; loop++)
            {
                allObjects[loop].ClearSubscriptionsWithoutUnsubscribeCall(this);
                allObjects[loop].DeregisterLayerViewSubset(this);
            }

            // Clear local subscription information
            foreach(var actSubscriptionList in m_objectsPerPassDict.Values)
            {
                actSubscriptionList.Subscriptions.Clear();
            }
        }

        /// <summary>
        /// Executes view update using the given update state object.
        /// </summary>
        /// <param name="updateState">The update state.</param>
        internal void UpdateForView(SceneRelatedUpdateState updateState)
        {
            if (m_disposed) { throw new ObjectDisposedException("ViewRelatedLayerSubset"); }

            // TODO: Trigger some other logic to update transparent object order
            // TODO: Performance improvement!!!
            bool anyOrderChanges = false;
            Camera3DBase camera = m_viewInformation.Camera;
            m_objectsPassTransparentRender.Subscriptions.Sort(new Comparison<RenderPassSubscription>((left, right) =>
            {
                SceneSpacialObject leftSpacial = left.SceneObject as SceneSpacialObject;
                SceneSpacialObject rightSpacial = right.SceneObject as SceneSpacialObject;
                if ((leftSpacial != null) && (rightSpacial != null))
                {
                    float leftDistance = (camera.Position - leftSpacial.Position).LengthSquared();
                    float rightDistance = (camera.Position - rightSpacial.Position).LengthSquared();
                    anyOrderChanges = true;
                    return rightDistance.CompareTo(leftDistance);
                }
                else if (leftSpacial != null) { anyOrderChanges = true; return -1; }
                else if (rightSpacial != null) { anyOrderChanges = true; return 1; }
                {
                    return 0;
                }
            }));
            if(anyOrderChanges)
            {
                // Synchronize ordering changes with corresponding scene object
                for(int loop=0 ; loop<m_objectsPassTransparentRender.Subscriptions.Count; loop++)
                {
                    var actSubscription = m_objectsPassTransparentRender.Subscriptions[loop];
                    actSubscription.SubscriptionIndex = loop;
                    actSubscription.SceneObject.UpdateSubscription(actSubscription, this);
                    m_objectsPassTransparentRender.Subscriptions[loop] = actSubscription;
                }
            }

            // Update all objects related to this view 
            m_isSubscribeUnsubscribeAllowed = true;
            try
            {
                // Update subscriptions based on visibility check result
                if(m_changedVisibilitiesAction != null)
                {
                    m_changedVisibilitiesAction();
                    m_changedVisibilitiesAction = null;
                }

                // Unsubscribe all invalid objects
                SceneObject actInvalidObject = null;
                while(m_invalidObjectsToDeregister.Count > 0)
                {
                    actInvalidObject = m_invalidObjectsToDeregister.Dequeue();
                    actInvalidObject.UnsubsribeFromAllPasses(this);
                }

                // Update subsccriptions based on boject state
                List<SceneObject> allObjects = m_sceneLayer.ObjectsInternal;
                int allObjectsLength = allObjects.Count;
                int visibleObjectCount = m_viewInformation.Owner.VisibleObjectCountInternal;
                for (int loop = 0; loop < allObjectsLength; loop++)
                {
                    SceneObject actSceneObject = allObjects[loop];
                    if (m_invalidObjects.ContainsKey(actSceneObject)) { continue; }

                    if (actSceneObject.IsLayerViewSubsetRegistered(this.ViewIndex) &&
                        actSceneObject.IsVisible(m_viewInformation))
                    {
                        actSceneObject.UpdateForView(updateState, this);
                        visibleObjectCount++;
                    }
                }
                m_viewInformation.Owner.VisibleObjectCountInternal = visibleObjectCount;
            }
            finally
            {
                m_isSubscribeUnsubscribeAllowed = false;
            }

            // Reorganize subscriptions if there is anything unsubscribed
            if(m_anythingUnsubscribed)
            {
                m_anythingUnsubscribed = false;
                foreach(var actPassProperties in m_objectsPerPass)
                {
                    if (actPassProperties.UnsubscribeCallCount <= 0) { continue; }

                    // Variables for consistency checking
                    int givenUnsubscribeCount = actPassProperties.UnsubscribeCallCount;
                    int trueUnsubscribeCount = 0;

                    // Handle case where we have unsubscribed some
                    //  => Build new subscription list and ignore all whith 'IsSubscribed' == false
                    List<RenderPassSubscription> newSubscriptionList = new List<RenderPassSubscription>(
                        (actPassProperties.Subscriptions.Count - actPassProperties.UnsubscribeCallCount) + 128);
                    for(int loop=0 ; loop<actPassProperties.Subscriptions.Count; loop++)
                    {
                        RenderPassSubscription actSubscription = actPassProperties.Subscriptions[loop];
                        if (!actSubscription.IsSubscribed) 
                        {
                            actSubscription.SceneObject.ClearSubscriptionsWithoutUnsubscribeCall(this, actSubscription);
                            trueUnsubscribeCount++;
                            continue; 
                        }

                        // Add this item to new subscription list
                        actSubscription.SubscriptionIndex = newSubscriptionList.Count;
                        newSubscriptionList.Add(actSubscription);

                        actSubscription.SceneObject.UpdateSubscription(actSubscription, this);
                    }
                    actPassProperties.Subscriptions = newSubscriptionList;
                    actPassProperties.UnsubscribeCallCount = 0;

                    // Check for consistency: Does unsubscribe-count match true unsubscriptions using IsSubscribed flag
                    if(givenUnsubscribeCount != trueUnsubscribeCount)
                    {
                        throw new SeeingSharpException("Inconsistency: Given unsubscribe count does not mach true count of unsubscriptions!");
                    }
                }
            }
        }

        /// <summary>
        /// Update logic beside rendering.
        /// </summary>
        /// <param name="updateState">The update state.</param>
        /// <param name="sceneObjectsForSingleUpdateCall">A collection of scene objects for a single update call. These are normally a list of newly inserted static objects.</param>
        internal void UpdateBesideRender(SceneRelatedUpdateState updateState, Queue<SceneObject> sceneObjectsForSingleUpdateCall)
        {
            List<SceneObjectFilter> filters = m_viewInformation.Filters;
            m_tmpChangedVisibilities.Clear();

            // Perform some pre-logic on filters
            bool anyFilterChanged = false;
            foreach (SceneObjectFilter actFilter in filters)
            {
                actFilter.SetEnvironmentData(m_sceneLayer, m_viewInformation);
                if (actFilter.ConfigurationChanged) { anyFilterChanged = true; }
            }

            // Check whether we have to update all objects
            bool refreshAllObjects = 
                m_viewInformation.Camera.StateChanged ||
                anyFilterChanged;

            // Perform viewbox culling for all standard objects
            List<SceneObject> allObjects = m_sceneLayer.ObjectsInternal;
            int allObjectsLength = allObjects.Count;
            if (allObjectsLength > 0)
            {
                SceneObject[] allObjectsArray = allObjects.GetBackingArray();
                for (int loop = 0; loop < allObjectsLength; loop++)
                {
                    SceneObject actObject = allObjectsArray[loop];

                    // Don't handle static objects here if we don't want to handle them
                    if(!refreshAllObjects)
                    {
                        if (actObject.IsStatic) { continue; }
                        if (!actObject.TransormationChanged) { continue; }
                    }

                    // Perform culling
                    PerformViewboxCulling(
                        actObject,
                        filters,
                        refreshAllObjects);
                }
            }

            // Update objects which are passed for a single update call (normally newly inserted static objects)
            int singleUpdateCallCount = sceneObjectsForSingleUpdateCall.Count;
            if((!refreshAllObjects) && (singleUpdateCallCount  >0))
            {
                SceneObject[] singleUpdateArray = sceneObjectsForSingleUpdateCall.GetBackingArray();
                for(int loop=0 ; loop<singleUpdateCallCount; loop++)
                {
                    // Perform culling
                    PerformViewboxCulling(
                        singleUpdateArray[loop],
                        filters,
                        refreshAllObjects);
                }
            }

            // Handle changed visibility in standard update logic
            if (m_tmpChangedVisibilities.Count > 0)
            {
                Scene startingScene = m_scene;
                m_changedVisibilitiesAction = () =>
                {
                    if (m_disposed) { return; }
                    if (startingScene != m_scene) { return; }

                    foreach (var actChangedVisibility in m_tmpChangedVisibilities)
                    {
                        // Check whether this objet is still here..
                        if (actChangedVisibility.Item1.Scene != m_scene) { continue; }
                        if (actChangedVisibility.Item1.SceneLayer != m_sceneLayer) { continue; }

                        // Handle changed visibility
                        HandleObjectVisibilityChanged(
                            actChangedVisibility.Item1,
                            actChangedVisibility.Item2,
                            actChangedVisibility.Item3);
                    }
                };
            }
        }

        /// <summary>
        /// Renders this view subset.
        /// </summary>
        /// <param name="renderState">The RenderState object holding all relevant data for current render pass</param>
        internal void Render(RenderState renderState)
        {
            if (m_disposed) { throw new ObjectDisposedException("ViewRelatedLayerSubset"); }

            // Skip rendering if there is nothing to do..
            if ((m_objectsPassLineRender.Subscriptions.Count == 0) &&
               (m_objectsPassPlainRender.Subscriptions.Count == 0) &&
               (m_objectsPassSpriteBatchRender.Subscriptions.Count == 0) &&
               (m_objectsPassTransparentRender.Subscriptions.Count == 0))
            {
                return;
            }

            List<SceneObject> invalidObjects = null;
            ResourceDictionary resources = renderState.CurrentResources;

            if (renderState.Device != m_device) { throw new SeeingSharpGraphicsException("Rendering of a ViewRelatedSceneLayoutSubset is called with a wrong device object!"); }
            if (renderState.CurrentResources != m_resources) { throw new SeeingSharpGraphicsException("Rendering of a ViewRelatedSceneLayoutSubset is called with a wrong ResourceDictionary object!"); }

            // Get current view configuration
            GraphicsViewConfiguration viewConfiguration = m_viewInformation.ViewConfiguration;

            // Update device dependent resources here
            RefreshDeviceDependentResources();

            // Update render parameters
            CBPerView cbPerView = new CBPerView();
            cbPerView.Accentuation = viewConfiguration.AccentuationFactor;
            cbPerView.GradientFactor = viewConfiguration.GeneratedColorGradientFactor;
            cbPerView.BorderFactor = viewConfiguration.GeneratedBorderFactor;
            cbPerView.Ambient = viewConfiguration.AmbientFactor;
            cbPerView.CameraPosition = this.ViewInformation.Camera.Position;
            cbPerView.ScreenPixelSize = this.ViewInformation.Camera.GetScreenSize();
            cbPerView.LightPower = viewConfiguration.LightPower;
            cbPerView.StrongLightFactor = viewConfiguration.StrongLightFactor;
            cbPerView.ViewProj = Matrix4x4.Transpose(this.ViewInformation.Camera.ViewProjection);
            m_renderParameters.UpdateValues(renderState, cbPerView);

            // Query for postprocess effect
            PostprocessEffectResource postprocessEffect = m_renderParameters.GetPostprocessEffect(
                m_sceneLayer.PostprocessEffectKey,
                resources);

            // Clear current depth buffer
            if (m_sceneLayer.ClearDepthBufferBeforeRendering)
            {
                renderState.ClearCurrentDepthBuffer();
            }

            // Perform main renderpass logic
            m_renderParameters.Apply(renderState);

            int passID = 0;
            bool continueWithNextPass = true;
            while (continueWithNextPass)
            {
                // Notify state before rendering
                if (postprocessEffect != null) { postprocessEffect.NotifyBeforeRender(renderState, passID); }

                try
                {
                    // All following objects are build using only triangle lists
                    m_device.DeviceImmediateContextD3D11.InputAssembler.PrimitiveTopology = D3D.PrimitiveTopology.TriangleList;

                    // Perform all plain renderings
                    RenderPass(null, m_objectsPassPlainRender, renderState, ref invalidObjects);

                    // Notify state after plain rendering
                    if (postprocessEffect != null) { postprocessEffect.NotifyAfterRenderPlain(renderState, passID); }

                    // Render all lines
                    RenderPass(
                        m_renderPassLineRender, m_objectsPassLineRender,
                        renderState, ref invalidObjects);
                    renderState.ApplyMaterial(null);

                    // Perform all transparent renderings
                    RenderPass(
                        m_renderPassTransparent, m_objectsPassTransparentRender,
                        renderState, ref invalidObjects);
                }
                finally
                {
                    // Notify state after rendering
                    if (postprocessEffect != null) { continueWithNextPass = postprocessEffect.NotifyAfterRender(renderState, passID); }
                    else { continueWithNextPass = false; }

                    // Increment passID value
                    passID++;
                }
            }

            // Clear current depth buffer
            if (m_sceneLayer.ClearDepthBufferAfterRendering)
            {
                renderState.ClearCurrentDepthBuffer();
            }

            //Remove all invalid objects
            if (invalidObjects != null)
            {
                HandleInvalidObjects(invalidObjects);
            }
        }

        /// <summary>
        /// Subscribes the given object to the given render pass.
        /// </summary>
        internal RenderPassSubscription SubscribeForPass(
            RenderPassInfo passInfo, 
            SceneObject sceneObject, Action<RenderState> renderMethod,
            int zOrder)
        {
            if (!m_isSubscribeUnsubscribeAllowed) { throw new SeeingSharpException("Subscription is not allowed currently!"); }

            var subscriptionProperties = m_objectsPerPassDict[passInfo];

            // Append new subscription to subscription list
            List<RenderPassSubscription> subscriptions = subscriptionProperties.Subscriptions;
            int subscriptionsCount = subscriptions.Count;
            RenderPassSubscription newSubscription = new RenderPassSubscription(this, passInfo, sceneObject, renderMethod, zOrder);
            if (!passInfo.IsSorted)
            {
                // No sort, so put the new subscription to the end of the collection
                newSubscription.SubscriptionIndex = subscriptionsCount;
                subscriptions.Add(newSubscription);
            }
            else
            {
                // Perform BinaryInsert to the correct position
                int newIndex = CommonTools.BinaryInsert(subscriptions, newSubscription, SubscriptionZOrderComparer.Instance);

                // Increment all subscription indices after the inserted position
                subscriptionsCount++;
                RenderPassSubscription actSubscription;
                for (int loop = newIndex; loop < subscriptionsCount; loop++)
                {
                    actSubscription = subscriptions[loop];
                    if (actSubscription.SubscriptionIndex != loop)
                    {
                        actSubscription.SubscriptionIndex = loop;
                        subscriptions[loop] = actSubscription;

                        actSubscription.SceneObject.UpdateSubscription(actSubscription, this);
                    }
                }
                newSubscription = subscriptions[newIndex];
            }

            return newSubscription;
        }

        /// <summary>
        /// Unsubscribes the given object from the given render pass.
        /// </summary>
        internal void UnsubscribeForPass(RenderPassSubscription subscription)
        {
            if (!m_isSubscribeUnsubscribeAllowed) { throw new SeeingSharpException("Subscription is not allowed currently!"); }

            if (subscription.IsSubscribed)
            {
                // Set unsubscribed flag
                m_anythingUnsubscribed = true;

                // Register unsubscription call
                var subscriptionInfo = m_objectsPerPassDict[subscription.RenderPass];

                subscriptionInfo.UnsubscribeCallCount++;

                // Update subscription info and reinsert it on the pass collection
                subscription.IsSubscribed = false;
                subscriptionInfo.Subscriptions[subscription.SubscriptionIndex] = subscription;

                // Post changes to the scene object holding this info too
                subscription.SceneObject.UpdateSubscription(subscription, this);
            }
        }

        /// <summary>
        /// Main method for object filtering. This method checks whether an object is visible or not.
        /// </summary>
        /// <param name="actObject">The object to be tested.</param>
        /// <param name="filters">All currently active filters.</param>
        /// <param name="refreshStaticObjects">A flag inicating whether we have to update all objects.</param>
        private void PerformViewboxCulling(SceneObject actObject, List<SceneObjectFilter> filters, bool refreshStaticObjects)
        {
            if (!actObject.IsLayerViewSubsetRegistered(this.ViewIndex)) { return; }
            if (m_invalidObjects.ContainsKey(actObject)) { return; }

            // Get visiblity check data about current object
            VisibilityCheckData checkData = actObject.GetVisibilityCheckData(m_viewInformation);
            if (checkData == null) { return; }

            // Execute all filters in configured order step by step
            int filterCount = filters.Count;
            bool previousFilterExecuted = false;
            bool previousFilterResult = true;
            VisibilityCheckFilterStageData lastFilterStageData = null;
            for (int actFilterIndex = 0; actFilterIndex < filterCount; actFilterIndex++)
            {
                SceneObjectFilter actFilter = filters[actFilterIndex];

                // Get data about current filter stage
                VisibilityCheckFilterStageData filterStageData = checkData.FilterStageData[actFilterIndex];
                if (filterStageData == null)
                {
                    filterStageData = checkData.FilterStageData.AddObject(
                        new VisibilityCheckFilterStageData(),
                        actFilterIndex);
                }

                // Remember last filter stage data
                lastFilterStageData = filterStageData;

                // Execute filter if needed
                if ((!filterStageData.HasExecuted) ||   // <-- Execute the filter if it was not executed for this object before
                    (actFilter.ConfigurationChanged) || // <-- Execute the filter if its configuraiton has changed
                    previousFilterExecuted ||           // <-- Execute the filter if one of the previous was executed
                    actFilter.UpdateEachFrame)          // <-- Execute the filter if it requests it on each frame (e. g. clipping filter)
                {
                    if (previousFilterResult)
                    {
                        // Re-Filter this object because any above condition has passed and
                        // this object successfully past the previous filter
                        bool isObjectVisible = actFilter.IsObjectVisible(actObject, m_viewInformation);

                        filterStageData.HasExecuted = true;
                        filterStageData.HasPassed = isObjectVisible;

                        previousFilterResult = isObjectVisible;
                        previousFilterExecuted = true;
                    }
                    else
                    {
                        // Set this object to unvisible because previous filter has thrown out
                        // this object
                        filterStageData.HasExecuted = true;
                        filterStageData.HasPassed = false;
                    }
                }
                else
                {
                    previousFilterResult = filterStageData.HasPassed;
                    previousFilterExecuted = false;
                }
            }

            // Handle changed visibility of the object
            bool oldVisible = checkData.IsVisible;
            bool newVisible = lastFilterStageData != null ? lastFilterStageData.HasPassed : true;
            if (oldVisible != newVisible)
            {
                checkData.IsVisible = newVisible;
                m_tmpChangedVisibilities.Add(Tuple.Create(actObject, oldVisible, newVisible));
            }
        }

        /// <summary>
        /// Handles changed object visibility.
        /// This method is called from default update thread.
        /// </summary>
        /// <param name="sceneObject">The scene object to be handled.</param>
        /// <param name="oldVisibility">Previous visibility flag value.</param>
        /// <param name="newVisibility">New visibility flag value.</param>
        private void HandleObjectVisibilityChanged(SceneObject sceneObject, bool oldVisibility, bool newVisibility)
        {
            if (!newVisibility)
            {
                // Deregister this object completely from the local layer subset
                sceneObject.UnsubsribeFromAllPasses(this);
            }
        }

        /// <summary>
        /// Refreshes device dependent resources of this class.
        /// </summary>
        private void RefreshDeviceDependentResources()
        {
            if ((m_renderParameters == null) ||
                (!m_renderParameters.IsLoaded))
            {
                m_renderParameters = m_resources.AddAndLoadResource(
                    GraphicsCore.GetNextGenericResourceKey(),
                    new ViewRenderParameters());
            }

            if ((m_renderPassTransparent == null) ||
                (!m_renderPassTransparent.IsLoaded))
            {
                m_renderPassTransparent = m_resources.GetResourceAndEnsureLoaded(
                    new NamedOrGenericKey(typeof(RenderPassDefaultTransparent)),
                    () => new RenderPassDefaultTransparent());
            }

            if ((m_renderPassLineRender == null) ||
                (!m_renderPassLineRender.IsLoaded))
            {
                m_renderPassLineRender = m_resources.GetResourceAndEnsureLoaded(
                    new NamedOrGenericKey(typeof(RenderPassLineRender)),
                    () => new RenderPassLineRender());
            }

            if ((m_renderPass2DOverlay == null) ||
                (!m_renderPass2DOverlay.IsLoaded))
            {
                m_renderPass2DOverlay = m_resources.GetResourceAndEnsureLoaded(
                    new NamedOrGenericKey(typeof(RenderPass2DOverlay)),
                    () => new RenderPass2DOverlay());
            }
        }

        /// <summary>
        /// Rendering logic for lines renderings.
        /// </summary>
        private void RenderPass(
            RenderPassBase renderPass, PassSubscribionProperties subscriptions,
            RenderState renderState, ref List<SceneObject> invalidObjects)
        {
            if (subscriptions.Subscriptions.Count > 0)
            {
                if (renderPass != null)
                {
                    //Ensure loaded resources for transparency pass
                    if (!renderPass.IsLoaded)
                    {
                        renderPass.LoadResource();
                    }

                    //Render all subscriptions
                    renderPass.Apply(renderState);
                }
                try
                {
                    int subscriptionCount = subscriptions.Subscriptions.Count;
                    for (int loopPass = 0; loopPass < subscriptionCount; loopPass++)
                    {
                        RenderPassSubscription actSubscription = subscriptions.Subscriptions[loopPass];
                        try
                        {
                            actSubscription.RenderMethod(renderState);
                        }
                        catch (Exception ex)
                        {
                            GraphicsCore.PublishInternalExceptionInfo(
                                ex, InternalExceptionLocation.Rendering3DObject);

                            // Mark this object as invalid
                            if (invalidObjects == null) { invalidObjects = new List<SceneObject>(); }
                            invalidObjects.Add(actSubscription.SceneObject);
                        }
                    }
                }
                finally
                {
                    if (renderPass != null)
                    {
                        renderPass.Discard(renderState);
                    }
                }
            }
        }

        /// <summary>
        /// Renders the 2D overlay of this view subset.
        /// </summary>
        /// <param name="renderState">The RenderState object holding all relevant data for current render pass</param>
        internal void Render2DOverlay(RenderState renderState)
        {
            if (m_disposed) { throw new ObjectDisposedException("ViewRelatedLayerSubset"); }
            if (m_objectsPass2DOverlay.Subscriptions.Count == 0) { return; }

            // Render all 2D objects
            List<SceneObject> invalidObjects = null;
            RenderPass(
                m_renderPass2DOverlay, m_objectsPass2DOverlay,
                renderState, ref invalidObjects);

            // Remove all invalid objects
            if (invalidObjects != null)
            {
                HandleInvalidObjects(invalidObjects);
            }
        }

        /// <summary>
        /// Handles invalid objects.
        /// </summary>
        /// <param name="invalidObjects">List containing all invalid objects to handle.</param>
        private void HandleInvalidObjects(List<SceneObject> invalidObjects)
        {
            // Register the given objects as invalid
            foreach(SceneObject actInvalidObject in invalidObjects)
            {
                m_invalidObjects.Add(actInvalidObject, null);
                m_invalidObjectsToDeregister.Enqueue(actInvalidObject);
            }
        }

        /// <summary>
        /// Gets or sets the index of this view subset within the scene.
        /// </summary>
        public int ViewIndex;

        /// <summary>
        /// Gets the corresponding ViewInformation object.
        /// </summary>
        public ViewInformation ViewInformation
        {
            get { return m_viewInformation; }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        /// <summary>
        /// Helper class holding some information abount subscriptions per pass.
        /// </summary>
        private class PassSubscribionProperties
        {
            internal int UnsubscribeCallCount = 0;
            internal List<RenderPassSubscription> Subscriptions = new List<RenderPassSubscription>(DEFAULT_PASS_SUBSCRIPTION_LENGTH);
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        /// <summary>
        /// Helper for sorting render pass subscriptions by z order.
        /// </summary>
        private class SubscriptionZOrderComparer : IComparer<RenderPassSubscription>
        {
            public static readonly SubscriptionZOrderComparer Instance = new SubscriptionZOrderComparer();

            public int Compare(RenderPassSubscription x, RenderPassSubscription y)
            {
                return x.ZOrder.CompareTo(y.ZOrder);
            }
        }
    }
}