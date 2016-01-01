#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Checking;
using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;

namespace SeeingSharp.Multimedia.Input
{
    public class InputHandlerFactory
    {
        private List<IInputHandler> m_inputHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputHandlerFactory"/> class.
        /// </summary>
        internal InputHandlerFactory()
        {
            m_inputHandlers = SeeingSharpApplication.Current.TypeQuery
                .GetAndInstanciateByContract<IInputHandler>();
        }

        /// <summary>
        /// Updates all currently active input handlers for the given view.
        /// </summary>
        /// <param name="viewObject">The object of the view control.</param>
        /// <param name="inputHandlers">The collection of input handlers managed by the view object.</param>
        /// <param name="renderLoop">The renderloop used by the view object.</param>
        /// <param name="currentlyDispsoing">Is the view currently disposing?</param>
        internal static void UpdateInputHandlerList(
            IInputEnabledView viewObject,
            List<IInputHandler> inputHandlers,
            RenderLoop renderLoop,
            bool currentlyDispsoing)
        {
            viewObject.EnsureNotNull("viewObject");
            inputHandlers.EnsureNotNull("inputHandlers");
            renderLoop.EnsureNotNull("renderLoop");

            // Clear previous input handlers
            if (inputHandlers.Count > 0)
            {
                foreach (var actHandler in inputHandlers)
                {
                    actHandler.Stop();
                }
                inputHandlers.Clear();
            }

            // Check whether this object is disposed
            if (currentlyDispsoing) { return; }

            // Check for other dependencies
            if ((renderLoop == null) ||
                (renderLoop.Camera == null))
            {
                return;
            }

            // Get all possible input handlers
            inputHandlers.AddRange(GraphicsCore.Current.InputHandlers.GetInputHandler(
                viewObject,
                viewObject.GetType(),
                renderLoop.Camera.GetType()));

            // Start them all
            foreach (var actInputHandler in inputHandlers)
            {
                actInputHandler.Start(viewObject, renderLoop.Camera);
            }
        }

        /// <summary>
        /// Gets all possible GraphicsInputHandlers for the given view and camera types.
        /// </summary>
        /// <typeparam name="ViewType">Gets the type of the view.</typeparam>
        /// <typeparam name="CameraType">Gets the type of the camera.</typeparam>
        /// <param name="viewObject">The view for which to the input handlers.</param>
        public List<IInputHandler> GetInputHandler<ViewType, CameraType>(IInputEnabledView viewObject)
            where ViewType : class
            where CameraType : class
        {
            Type givenViewType = typeof(ViewType);
            Type givenCameraType = typeof(CameraType);

            return GetInputHandler(viewObject, givenViewType, givenCameraType);
        }

        /// <summary>
        /// Gets all possible GraphicsInputHandlers for the given view and camera types.
        /// Pass null to all parameters to return all generic input handlers.
        /// </summary>
        /// <param name="viewObject">The view for which to query the input object.</param>
        /// <param name="givenCameraType">The type of the view.</param>
        /// <param name="givenViewType">The type of the camera.</param>
        public List<IInputHandler> GetInputHandler(IInputEnabledView viewObject, Type givenViewType, Type givenCameraType)
        {
            List<IInputHandler> result = new List<IInputHandler>();
            foreach (var actInputHandler in m_inputHandlers)
            {
                // Query for the input handler's information
                Type[] actSupportedViewTypes = actInputHandler.GetSupportedViewTypes();
                Type[] actSupportedCameraTypes = actInputHandler.GetSupportedCameraTypes();
                SeeingSharpInputMode[] actSupportedInputModes = actInputHandler.GetSupportedInputModes();
                bool viewTypeSupported = false;
                bool cameraTypeSupported = false;
                bool inputModeSupported = false;

                // Check for view-type support
                if (actSupportedViewTypes == null)
                {
                    viewTypeSupported = givenViewType == null;
                }
                else if(givenViewType != null)
                {
                    foreach (Type actViewType in actSupportedViewTypes)
                    {
                        if (actViewType.GetTypeInfo().IsAssignableFrom(givenViewType.GetTypeInfo()))
                        {
                            viewTypeSupported = true;
                            break;
                        }
                    }
                }
                if (!viewTypeSupported) { continue; }

                // Check for camera-type support
                if (actSupportedCameraTypes == null)
                {
                    cameraTypeSupported = givenCameraType == null;
                }
                else if(givenCameraType != null)
                {
                    foreach (Type actCameraType in actSupportedCameraTypes)
                    {
                        if (actCameraType.GetTypeInfo().IsAssignableFrom(givenCameraType.GetTypeInfo()))
                        {
                            cameraTypeSupported = true;
                            break;
                        }
                    }
                }
                if (!cameraTypeSupported) { continue; }

                // Check for input-mode support
                if (viewObject != null)
                {
                    if (actSupportedInputModes == null) { inputModeSupported = true; }
                    else
                    {
                        foreach (SeeingSharpInputMode actInputMode in actSupportedInputModes)
                        {
                            if (actInputMode == viewObject.InputMode)
                            {
                                inputModeSupported = true;
                                break;
                            }
                        }
                    }
                    if (!inputModeSupported) { continue; }
                }

                // Create a new input handler
                result.Add(Activator.CreateInstance(actInputHandler.GetType()) as IInputHandler);
            }
            return result;
        }

        /// <summary>
        /// Gets the total count of loaded input handlers
        /// </summary>
        public int Count
        {
            get { return m_inputHandlers.Count; }
        }
    }
}