#region License information (SeeingSharp and all based games/applications)
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

using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Views;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using SeeingSharp.Util;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.Multimedia.Input.CoreWindowKeyAndMouseInputHandler),
    contractType: typeof(SeeingSharp.Multimedia.Input.IInputHandler))]

namespace SeeingSharp.Multimedia.Input
{
    class CoreWindowKeyAndMouseInputHandler : IInputHandler
    {
        private const float MOVEMENT = 0.3f;
        private const float ROTATION = 0.01f;
        private static readonly Dictionary<VirtualKey, WinVirtualKey> s_keyMappingDict;

        #region objects from outside
        private SeeingSharpCoreWindowPainter m_painter;
        private IInputEnabledView m_viewInterface;
        private RenderLoop m_renderLoop;
        private CoreWindow m_coreWindow;
        private CoreDispatcher m_dispatcher;
        #endregion

        #region Input states
        private MouseOrPointerState m_stateMouseOrPointer;
        private KeyboardState m_stateKeyboard;
        #endregion

        #region state variables for camera movement
        private PointerPoint m_lastDragPoint;
        #endregion

        /// <summary>
        /// Initializes the <see cref="CoreWindowKeyAndMouseInputHandler"/> class.
        /// </summary>
        static CoreWindowKeyAndMouseInputHandler()
        {
            s_keyMappingDict = new Dictionary<VirtualKey, WinVirtualKey>();
            foreach(VirtualKey actVirtualKey in Enum.GetValues(typeof(VirtualKey)))
            {
                short actVirtualKeyCode = (short)actVirtualKey;
                WinVirtualKey actWinVirtualKey = (WinVirtualKey)actVirtualKeyCode;
                s_keyMappingDict[actVirtualKey] = actWinVirtualKey;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WinRTKeyAndMouseInputHandler"/> class.
        /// </summary>
        public CoreWindowKeyAndMouseInputHandler()
        {
            m_stateMouseOrPointer = new MouseOrPointerState();
            m_stateMouseOrPointer.Type = MouseOrPointerType.Mouse;

            m_stateKeyboard = new KeyboardState();
        }

        /// <summary>
        /// Gets a list containing all supported view types.
        /// </summary>
        public Type[] GetSupportedViewTypes()
        {
            return new Type[]
            {
                typeof(SeeingSharpCoreWindowPainter)
            };
        }

        /// <summary>
        /// Starts input handling.
        /// </summary>
        /// <param name="viewObject">The view object (e. g. Direct3D11Canvas).</param>
        public void Start(IInputEnabledView viewObject)
        {
            m_painter = viewObject as SeeingSharpCoreWindowPainter;
            if (m_painter == null) { throw new ArgumentException("Unable to handle given view object!"); }

            m_viewInterface = m_painter as IInputEnabledView;
            if (m_viewInterface == null) { throw new ArgumentException("Unable to handle given view object!"); }

            m_renderLoop = m_viewInterface.RenderLoop;
            if (m_renderLoop == null) { throw new ArgumentException("Unable to handle given view object!"); }

            m_dispatcher = m_painter.Disptacher;
            if(m_dispatcher == null) { throw new ArgumentException("Unable to get CoreDisptacher from target panel!"); }

            // Deligate start logic to UI thread
            m_dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Register all events
                m_painter.TargetWindow.PointerExited += OnTargetPanel_PointerExited;
                m_painter.TargetWindow.PointerEntered += OnTargetPanel_PointerEntered;
                m_painter.TargetWindow.PointerWheelChanged += OnTargetPanel_PointerWheelChanged;
                m_painter.TargetWindow.PointerPressed += OnTargetPanel_PointerPressed;
                m_painter.TargetWindow.PointerReleased += OnTargetPanel_PointerReleased;
                m_painter.TargetWindow.PointerMoved += OnTargetPanel_PointerMoved;

                m_coreWindow = CoreWindow.GetForCurrentThread();
                m_coreWindow.KeyDown += OnCoreWindow_KeyDown;
                m_coreWindow.KeyUp += OnCoreWindow_KeyUp;

            }).FireAndForget();
        }

        /// <summary>
        /// Stops input handling.
        /// </summary>
        public void Stop()
        {
            if(m_painter == null) { return; }
            if(m_dispatcher == null) { return; }

            // Deregister all events on UI thread
            SeeingSharpCoreWindowPainter painter = m_painter;
            CoreWindow coreWindow = m_coreWindow;
            m_dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Deregister all events
                painter.TargetWindow.PointerExited -= OnTargetPanel_PointerExited;
                painter.TargetWindow.PointerEntered -= OnTargetPanel_PointerEntered;
                painter.TargetWindow.PointerWheelChanged -= OnTargetPanel_PointerWheelChanged;
                painter.TargetWindow.PointerPressed -= OnTargetPanel_PointerPressed;
                painter.TargetWindow.PointerReleased -= OnTargetPanel_PointerReleased;
                painter.TargetWindow.PointerMoved -= OnTargetPanel_PointerMoved;

                // Deregister events from CoreWindow
                coreWindow.KeyDown -= OnCoreWindow_KeyDown;
                coreWindow.KeyUp -= OnCoreWindow_KeyUp;

            }).FireAndForget();

            // set all references to zero
            m_painter = null;
            m_coreWindow = null;
            m_dispatcher = null;

        }

        /// <summary>
        /// Querries all current input states.
        /// </summary>
        public IEnumerable<InputStateBase> GetInputStates()
        {
            yield return m_stateMouseOrPointer;
            yield return m_stateKeyboard;
        }

        private void OnCoreWindow_KeyDown(CoreWindow sender, KeyEventArgs e)
        {
            if (m_painter == null) { return; }

            m_stateKeyboard.NotifyKeyDown(s_keyMappingDict[e.VirtualKey]);
        }

        private void OnCoreWindow_KeyUp(CoreWindow sender, KeyEventArgs e)
        {
            if (m_painter == null) { return; }

            m_stateKeyboard.NotifyKeyUp(s_keyMappingDict[e.VirtualKey]);
        }

        private void OnTargetPanel_PointerReleased(CoreWindow sender, PointerEventArgs e)
        {
            if (m_painter == null) { return; }

            // Track mouse/pointer state
            PointerPointProperties pointProperties = e.CurrentPoint.Properties;
            if (pointProperties.IsPrimary)
            {
                m_stateMouseOrPointer.NotifyButtonStates(
                    pointProperties.IsLeftButtonPressed,
                    pointProperties.IsMiddleButtonPressed,
                    pointProperties.IsRightButtonPressed,
                    pointProperties.IsXButton1Pressed,
                    pointProperties.IsXButton2Pressed);
            }

            // Needed here because we loose focus again by default on left mouse button
            e.Handled = true;
        }

        private void OnTargetPanel_PointerPressed(CoreWindow sender, PointerEventArgs e)
        {
            if (m_painter == null) { return; }

            // Track mouse/pointer state
            PointerPointProperties pointProperties = e.CurrentPoint.Properties;
            if (pointProperties.IsPrimary)
            {
                m_stateMouseOrPointer.NotifyButtonStates(
                    pointProperties.IsLeftButtonPressed,
                    pointProperties.IsMiddleButtonPressed,
                    pointProperties.IsRightButtonPressed,
                    pointProperties.IsXButton1Pressed,
                    pointProperties.IsXButton2Pressed);
            }
            m_lastDragPoint = e.CurrentPoint;

            // Needed here because we loose focus again by default on left mouse button
            e.Handled = true;
        }

        private void OnTargetPanel_PointerMoved(CoreWindow sender, PointerEventArgs e)
        {
            if (m_painter == null) { return; }

            // Calculate move distance
            PointerPoint currentPoint = e.CurrentPoint;
            if(m_lastDragPoint == null) { m_lastDragPoint = currentPoint; }
            Vector2 moveDistance = new Vector2(
                (float)(currentPoint.Position.X - m_lastDragPoint.Position.X),
                (float)(currentPoint.Position.Y - m_lastDragPoint.Position.Y));
            Vector2 currentLocation = new Vector2(
                (float)currentPoint.Position.X,
                (float)currentPoint.Position.Y);

            // Track mouse/pointer state
            PointerPointProperties pointProperties = currentPoint.Properties;
            if (pointProperties.IsPrimary)
            {
                m_stateMouseOrPointer.NotifyButtonStates(
                    pointProperties.IsLeftButtonPressed,
                    pointProperties.IsMiddleButtonPressed,
                    pointProperties.IsRightButtonPressed,
                    pointProperties.IsXButton1Pressed,
                    pointProperties.IsXButton2Pressed);

                m_stateMouseOrPointer.NotifyMouseLocation(
                    currentLocation, moveDistance, m_painter.ActualSize.ToVector2());
            }

            // Store last drag point
            m_lastDragPoint = currentPoint;
        }

        private void OnTargetPanel_PointerWheelChanged(CoreWindow sender, PointerEventArgs e)
        {
            if (m_painter == null) { return; }

            // Track mouse/pointer state
            PointerPointProperties pointProperties = e.CurrentPoint.Properties;
            int wheelDelta = pointProperties.MouseWheelDelta;
            if (pointProperties.IsPrimary)
            {
                m_stateMouseOrPointer.NotifyButtonStates(
                    pointProperties.IsLeftButtonPressed,
                    pointProperties.IsMiddleButtonPressed,
                    pointProperties.IsRightButtonPressed,
                    pointProperties.IsXButton1Pressed,
                    pointProperties.IsXButton2Pressed);
                m_stateMouseOrPointer.NotifyMouseWheel(wheelDelta);
            }
        }

        /// <summary>
        /// Called when mouse leaves the target panel.
        /// </summary>
        private void OnTargetPanel_PointerExited(CoreWindow sender, PointerEventArgs e)
        {
            if (m_painter == null) { return; }

            m_stateMouseOrPointer.NotifyInside(false);
        }

        private void OnTargetPanel_PointerEntered(CoreWindow sender, PointerEventArgs e)
        {
            if (m_painter == null) { return; }

            m_stateMouseOrPointer.NotifyInside(true);
        }
    }
}