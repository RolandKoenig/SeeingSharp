#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2015 Roland König (RolandK)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion

using SeeingSharp.Util;
using SeeingSharp.Checking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;

//Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace SeeingSharp.Multimedia.Core
{
    public abstract partial class SceneObject : IDisposable, IAnimatableObject
    {
        #region Generic members
        private IndexBasedDynamicCollection<VisibilityCheckData> m_visibilityData;
        private DetailLevel m_targetDetailLevel;
        private dynamic m_customData;
        private object m_tag1;
        private object m_tag2;
        private bool m_isStatic;
        #endregion

        #region Some information about parent containers
        private Scene m_scene;
        private SceneLayer m_sceneLayer;
        private IEnumerable<MessageSubscription> m_sceneMessageSubscriptions;
        #endregion

        #region Members for behaviors
        private List<SceneObjectBehavior> m_behaviors;
        #endregion

        #region Members for animations
        private AnimationHandler m_animationHandler;
        #endregion

        #region Collections for describing object hierarchies
        private List<SceneObject> m_dependencies;
        private SceneObject m_owner;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneObject"/> class.
        /// </summary>
        protected SceneObject()
        {
            m_targetDetailLevel = DetailLevel.All;

            m_dependencies = new List<SceneObject>();
            m_owner = null;

            m_behaviors = new List<SceneObjectBehavior>();
            m_animationHandler = new AnimationHandler(this);
            m_visibilityData = new IndexBasedDynamicCollection<VisibilityCheckData>();

            //Create a dynamic container for custom data
            m_customData = new ExpandoObject();

            this.TransormationChanged = true;
            this.IsPickingTestVisible = true;
        }

        /// <summary>
        /// Registers the given scene and layer on this object.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="sceneLayer">The scene layer.</param>
        internal void SetSceneAndLayer(Scene scene, SceneLayer sceneLayer)
        {
            scene.EnsureNotNull("scene");
            sceneLayer.EnsureNotNull("sceneLayer");
            m_scene.EnsureNull("m_scene");
            m_sceneLayer.EnsureNull("m_sceneLayer");

            m_scene = scene;
            m_sceneLayer = sceneLayer;

            if(m_scene.Messenger != null)
            {
                m_sceneMessageSubscriptions = m_scene.Messenger.SubscribeAll(this);
            }
        }

        /// <summary>
        /// Deregisters the current scene and layer from this object.
        /// </summary>
        internal void ResetSceneAndLayer()
        {
            m_scene.EnsureNotNull("m_scene");
            m_sceneLayer.EnsureNotNull("m_sceneLayer");

            if (m_sceneMessageSubscriptions != null)
            {
                foreach (MessageSubscription actSubscription in m_sceneMessageSubscriptions)
                {
                    actSubscription.Unsubscribe();
                }
                m_sceneMessageSubscriptions = null;
            }

            m_scene = null;
            m_sceneLayer = null;
        }

        /// <summary>
        /// Registers the given behavior on this object.
        /// </summary>
        /// <param name="behavior">The behavior to be registered.</param>
        public void RegisterBehavior(SceneObjectBehavior behavior)
        {
            behavior.SetHostObject(this);
            m_behaviors.Add(behavior);
        }

        /// <summary>
        /// Gets the behavior of the given type.
        /// </summary>
        /// <typeparam name="BehaviorType">The type of the behavior to get.</typeparam>
        public BehaviorType GetBehavior<BehaviorType>()
            where BehaviorType : SceneObjectBehavior
        {
            Type typeToSearch = typeof(BehaviorType);
            foreach(SceneObjectBehavior actBehavior in m_behaviors)
            {
                if (actBehavior.GetType() == typeToSearch) { return actBehavior as BehaviorType; }
            }
            return null;
        }

        /// <summary>
        /// Gets the behavior of the given type.
        /// </summary>
        /// <param name="behaviorType">The type of the behavior to get.</param>
        public SceneObjectBehavior GetBehavior(Type behaviorType)
        {
            foreach (SceneObjectBehavior actBehavior in m_behaviors)
            {
                if (actBehavior.GetType() == behaviorType) { return actBehavior; }
            }
            return null;
        }

        /// <summary>
        /// Check all dependencies of this object.
        /// </summary>
        /// <param name="other">The object to check for.</param>
        /// <returns>true if the given object is a dependency of this one.</returns>
        public bool DependsOn(SceneObject other)
        {
            foreach (SceneObject actDependency in m_dependencies)
            {
                if (actDependency == other) { return true; }
                if (actDependency.DependsOn(other)) { return true; }
            }

            return false;
        }

        /// <summary>
        /// Determines whether this object already contains this dependency (no lower level check).
        /// </summary>
        /// <param name="dependency">The dependency to check for.</param>
        public virtual bool DependsOnSingleLevel(SceneObject dependency)
        {
            return m_dependencies.Contains(dependency);
        }

        /// <summary>
        /// Queries for all dependencies (also lower level).
        /// </summary>
        public IEnumerable<SceneObject> GetAllDependencies()
        {
            foreach (SceneObject actDependency in m_dependencies)
            {
                yield return actDependency;

                foreach (SceneObject actLowerDependency in actDependency.GetAllDependencies())
                {
                    yield return actLowerDependency;
                }
            }
        }

        /// <summary>
        /// Adds the given object as a dependency.
        /// </summary>
        /// <param name="dependency">The object which is be be located under this one within object hierarchy.</param>
        public virtual void AddDependency(SceneObject dependency)
        {
            if (dependency.Scene != this.Scene) { throw new SeeingSharpGraphicsException("Dependency musst have the same scene!"); }
            if (dependency.m_owner != null) { throw new SeeingSharpGraphicsException("Dependency has already an owner!"); }
            if (dependency.DependsOn(this)) { throw new SeeingSharpGraphicsException("Cyclic dependency detected!"); }
            if (m_dependencies.Contains(dependency)) { throw new SeeingSharpGraphicsException("Dependency is already added!"); }

            // Create owner-dependency relation
            m_dependencies.Add(dependency);
            dependency.m_owner = this;
        }

        /// <summary>
        /// Removes the given object from the dependency list.
        /// </summary>
        /// <param name="dependency">The object which is to be removed from the dependency list.</param>
        public virtual void RemoveDependency(SceneObject dependency)
        {
            if (dependency.Scene!= this.Scene) { throw new ArgumentException("Dependency musst have the same scene!"); }

            // Destroy owner-dependency relation
            m_dependencies.Remove(dependency);
            if (dependency.m_owner == this) { dependency.m_owner = this; }
        }

        /// <summary>
        /// Is this object visible in the given view?
        /// </summary>
        /// <param name="viewInfo">The view info to check.</param>
        public bool IsVisible(ViewInformation viewInfo)
        {
            if (viewInfo.ViewIndex < 0) { throw new SeeingSharpGraphicsException("Given ViewInformation object is not assoziated to any view!"); }
            if (viewInfo.Scene == null) { throw new SeeingSharpGraphicsException("Given ViewInformation object is not attached to any scene!"); }
            if (viewInfo.Scene != this.Scene) { throw new SeeingSharpGraphicsException("Given ViewInformation object is not attached to this scene!"); }

            VisibilityCheckData checkData = m_visibilityData[viewInfo.ViewIndex];
            if (checkData == null) { return false; }

            return checkData.IsVisible;
        }

        /// <summary>
        /// Clears current visibility data.
        /// </summary>
        internal void ClearVisibilityStageData()
        {
            foreach(VisibilityCheckData actCheckData in m_visibilityData)
            {
                actCheckData.FilterStageData.Clear();
            }
        }

        /// <summary>
        /// Gets the data object used for visibility checking.
        /// </summary>
        /// <param name="viewInfo">The VisibilityCheckData for this object for the given view.</param>
        internal VisibilityCheckData GetVisibilityCheckData(ViewInformation viewInfo)
        {
            VisibilityCheckData checkData = m_visibilityData[viewInfo.ViewIndex];
            if (checkData == null)
            {
                checkData = m_visibilityData.AddObject(
                    new VisibilityCheckData(),
                    viewInfo.ViewIndex);
            }
            return checkData;
        }

        /// <summary>
        /// Picks an object in 3D-World.
        /// </summary>
        /// <param name="rayStart">Start of picking ray.</param>
        /// <param name="rayDirection"></param>
        /// <param name="viewInfo">Information about the view that triggered picking.</param>
        /// <param name="pickingOptions">Some additional options for picking calculations.</param>
        /// <returns>Returns the distance to the object or float.NaN if object is not picked.</returns>
        internal virtual float Pick(Vector3 rayStart, Vector3 rayDirection, ViewInformation viewInfo, PickingOptions pickingOptions)
        {
            return float.NaN;
        }

        /// <summary>
        /// Is this object visible currently?
        /// </summary>
        /// <param name="viewInfo">Information about the view that triggered bounding volume testing.</param>
        /// <param name="boundingFrustum">The bounding frustum to check.</param>
        internal virtual bool IsInBoundingFrustum(ViewInformation viewInfo, ref BoundingFrustum boundingFrustum)
        {
            return true;
        }

        /// <summary>
        /// Registers a layer view subset with the given index.
        /// </summary>
        /// <param name="layerViewSubset">The layer view subset to register.</param>
        internal void RegisterLayerViewSubset(ViewRelatedSceneLayerSubset layerViewSubset)
        {
            m_viewRelatedSubscriptions.AddObject(new List<RenderPassSubscription>(), layerViewSubset.ViewIndex);
        }

        /// <summary>
        /// Deregisters a layer view subset with the given index.
        /// </summary>
        /// <param name="layerViewSubset">The layer view subset to deregister.</param>
        internal void DeregisterLayerViewSubset(ViewRelatedSceneLayerSubset layerViewSubset)
        {
            if (m_viewRelatedSubscriptions.HasObjectAt(layerViewSubset.ViewIndex))
            {
                m_viewRelatedSubscriptions.RemoveObject(layerViewSubset.ViewIndex);
            }
        }

        /// <summary>
        /// Is the given view index registered on this object?
        /// </summary>
        /// <param name="viewIndex">The index to check for.</param>
        internal bool IsLayerViewSubsetRegistered(int viewIndex)
        {
            return m_viewRelatedSubscriptions.HasObjectAt(viewIndex);
        }

        /// <summary>
        /// Fills the given collection with all referenced resources.
        /// </summary>
        /// <param name="resourceCollection">The collection to be filled,</param>
        public virtual void GetReferencedResources(SingleInstanceCollection<Resource> resourceCollection)
        {

        }

        /// <summary>
        /// Loads all resources of the object.
        /// </summary>
        /// <param name="device">Current graphics device.</param>
        /// <param name="resourceDictionary">Current resource dicionary.</param>
        public abstract void LoadResources(EngineDevice device, ResourceDictionary resourceDictionary);

        /// <summary>
        /// Updates this object.
        /// </summary>
        /// <param name="updateState">State of update process.</param>
        public void Update(UpdateState updateState)
        {
            // Update all behaviors first
            foreach (SceneObjectBehavior actBehavior in m_behaviors)
            {
                actBehavior.Update(updateState);
            }

            // Update the object
            UpdateInternal(updateState);
        }

        /// <summary>
        /// Update logic for overall updates.
        /// This method should be used for update logic that also depends on other object.
        /// UpdateOverall methods are called sequentially object by object.
        /// </summary>
        /// <param name="updateState">Current update state of the scene.</param>
        public void UpdateOverall(UpdateState updateState)
        {
            // Update all dependencies finally
            if (m_dependencies.Count > 0)
            {
                UpdateDependenciesInternal(updateState, m_dependencies);
            }

            // TODO: Move animation handler update back to standard update (=> But make it threadsafe!)
            // Update current animation state
            if (m_animationHandler != null)
            {
                m_animationHandler.Update(updateState);
            }

            // Update all behaviors first
            foreach (SceneObjectBehavior actBehavior in m_behaviors)
            {
                actBehavior.UpdateOverall(updateState);
            }
        }

        /// <summary>
        /// Disposes all unmanaged resources of this object.
        /// </summary>
        public void Dispose()
        {
            this.UnloadResources();
        }

        /// <summary>
        /// Updates this object for the given view.
        /// </summary>
        /// <param name="updateState">Current state of the update pass.</param>
        /// <param name="layerViewSubset">The layer view subset wich called this update method.</param>
        public void UpdateForView(UpdateState updateState, ViewRelatedSceneLayerSubset layerViewSubset)
        {
            UpdateForViewInternal(updateState, layerViewSubset);
        }

        /// <summary>
        /// Unloads all resources of the object.
        /// </summary>
        public virtual void UnloadResources()
        {

        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        /// <param name="updateState">Current update state.</param>
        protected abstract void UpdateInternal(UpdateState updateState);

        /// <summary>
        /// Updates this object for the given view.
        /// </summary>
        /// <param name="updateState">Current state of the update pass.</param>
        /// <param name="layerViewSubset">The layer view subset wich called this update method.</param>
        protected abstract void UpdateForViewInternal(UpdateState updateState, ViewRelatedSceneLayerSubset layerViewSubset);

        /// <summary>
        /// Updates all dependencies of this object. Override this to change default behavior.
        /// </summary>
        /// <param name="updateState">The current update state.</param>
        /// <param name="dependencies">The full dependency list that should be updated.</param>
        protected virtual void UpdateDependenciesInternal(UpdateState updateState, List<SceneObject> dependencies)
        {
            // Trigger updates of all dependencies
            foreach (SceneObject actDependency in m_dependencies)
            {
                actDependency.Update(updateState);
            }
        }

        /// <summary>
        /// Are resources loaded for the given device?
        /// </summary>
        public abstract bool IsLoaded(EngineDevice device);

        /// <summary>
        /// Gets a dynamic container for custom data.
        /// </summary>
        public dynamic CustomData
        {
            get { return m_customData; }
        }

        /// <summary>
        /// Gets or sets an additional data object.
        /// </summary>
        public object Tag1
        {
            get { return m_tag1; }
            set { m_tag1 = value; }
        }

        /// <summary>
        /// Gets or sets an additional data object.
        /// </summary>
        public object Tag2
        {
            get { return m_tag2; }
            set { m_tag2 = value; }
        }

        /// <summary>
        /// Is this object visible for picking-test?
        /// </summary>
        public bool IsPickingTestVisible 
        {
            get; 
            set; 
        }

        /// <summary>
        /// Gets current scene.
        /// </summary>
        public Scene Scene
        {
            get { return m_scene; }
        }

        /// <summary>
        /// Gets the messenager associated to this object.
        /// This object is null unless the registerForMessaging argument of the
        /// Scene's constructor was set to true.
        /// </summary>
        public SeeingSharpMessenger Messenger
        {
            get 
            {
                if (m_scene == null) { return null; }
                return m_scene.Messenger; 
            }
        }

        /// <summary>
        /// Gets the synchronization context.
        /// This object is null unless the registerForMessaging argument of the
        /// Scene's constructor was set to true.
        /// </summary>
        public SynchronizationContext SynchronizationContext
        {
            get 
            {
                if (m_scene == null) { return null; }
                return m_scene.SynchronizationContext; 
            }
        }

        /// <summary>
        /// Gets or sets the scene layer.
        /// </summary>
        public SceneLayer SceneLayer
        {
            get { return m_sceneLayer; }
        }

        /// <summary>
        /// Gets current AnimationHandler object.
        /// </summary>
        public virtual AnimationHandler AnimationHandler
        {
            get { return m_animationHandler; }
        }

        /// <summary>
        /// Is this object a static object?
        /// </summary>
        public bool IsStatic
        {
            get { return m_isStatic; }
            set
            {
                if (this.Scene != null) { throw new SeeingSharpException("Unable to change IsStatic state when the object is already assigned to a scene!"); }
                m_isStatic = value;
            }
        }

        /// <summary>
        /// Indicates whether transformation data has changed during last update calls.
        /// This member is used for viewbox-culling to ignore objects which haven't changed their state.
        /// </summary>
        internal bool TransormationChanged;

        /// <summary>
        /// Gets or sets the target detail level.
        /// </summary>
        public DetailLevel TargetDetailLevel
        {
            get { return m_targetDetailLevel; }
            set
            {
                if (this.Scene != null) { throw new SeeingSharpGraphicsException("Unable to change TargetDetailLevel when object is already added to a scene!"); }
                m_targetDetailLevel = value;
            }
        }

        /// <summary>
        /// Gets the total count of owners where this object is a direct dependency of.
        /// </summary>
        public bool HasOwner
        {
            get { return m_owner != null; }
        }

        /// <summary>
        /// Gets the total count of direct dependencies of this object.
        /// </summary>
        public int CountDependencies
        {
            get { return m_dependencies.Count; }
        }
    }
}