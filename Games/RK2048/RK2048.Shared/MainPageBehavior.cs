#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
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

using FrozenSky.Infrastructure;
using FrozenSky.Multimedia.Views;
using FrozenSky.Util;
using Microsoft.Xaml.Interactivity;
using RK2048.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace RK2048
{
    public class MainPageBehavior : DependencyObject, IBehavior
    {
        private FrozenSkyPanelPainter m_painter;
        private SwapChainBackgroundPanel m_currentTarget;
        private UIGestureCatcher m_gestureCatcher;
        private GameCore m_gameCore;

        public MainPageBehavior()
        {
            m_painter = new FrozenSkyPanelPainter();
        }

        public void Attach(DependencyObject associatedObject)
        {
            this.AssociatedObject = associatedObject;
        }

        public void Detach()
        {
            this.AssociatedObject = null;
        }

        /// <summary>
        /// Initializes the game using the given background panel.
        /// </summary>
        /// <param name="backgroundPanel">The BackgroundPanel to be used as the render target.</param>
        private async void TriggerInitialize(SwapChainBackgroundPanel backgroundPanel)
        {
            // Check whether we are in design mode..
            if (!FrozenSkyApplication.IsInitialized) { return; }

            // Create new GameCore object
            m_gameCore = m_currentTarget.DataContext as GameCore;
            if (m_gameCore == null)
            {
                m_gameCore = new GameCore();
                m_currentTarget.DataContext = m_gameCore;
            }

            // Register events
            m_currentTarget.KeyDown += OnCurrentTarget_KeyDown;
            m_currentTarget.PointerPressed += OnCurrentTArget_PointerPressed;
            m_currentTarget.SizeChanged += OnCurrentTarget_SizeChanged;

            // Attach this view to the loaded GameCore
            m_painter.Attach(m_currentTarget);

            await m_gameCore.InitializeAsync(m_painter);
            if (m_currentTarget != backgroundPanel) { return; }  //<-- do we still have the same state?

            // Create the GestureRecognizer and register corresponding events
            m_gestureCatcher = new UIGestureCatcher(m_currentTarget);
            m_gestureCatcher.MoveDown += OnGestureCatcher_MoveDown;
            m_gestureCatcher.MoveLeft += OnGestureCatcher_MoveLeft;
            m_gestureCatcher.MoveRight += OnGestureCatcher_MoveRight;
            m_gestureCatcher.MoveTop += OnGestureCatcher_MoveTop;
        }

        /// <summary>
        /// Unloads the game.
        /// </summary>
        /// <param name="backgroundPanel">The target backround panel.</param>
        private void Unload()
        {
            m_painter.Detach();

            m_currentTarget.KeyDown -= OnCurrentTarget_KeyDown;
            m_currentTarget.SizeChanged -= OnCurrentTarget_SizeChanged;
            m_currentTarget.PointerPressed -= OnCurrentTArget_PointerPressed;
            m_currentTarget.DataContext = null;

            m_gameCore = null;

            m_gestureCatcher.MoveDown -= OnGestureCatcher_MoveDown;
            m_gestureCatcher.MoveLeft -= OnGestureCatcher_MoveLeft;
            m_gestureCatcher.MoveRight -= OnGestureCatcher_MoveRight;
            m_gestureCatcher.MoveTop -= OnGestureCatcher_MoveTop;
            m_gestureCatcher = null;
        }

        private void OnCurrentTarget_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (m_gameCore == null) { return; }

            m_gameCore.UpdateCameraLocation();
        }

        /// <summary>
        /// Called when the pointer is pressed inside the rendering panel.
        /// </summary>
        private void OnCurrentTArget_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (m_gameCore == null) { return; }
            if (m_gameCore.IsAnyTaskRunning()) { return; }

            m_gameCore.IsMenuOpened = false;
        }

        /// <summary>
        /// Called when the gesture catcher recognized a touch gesture.
        /// </summary>
        private async void OnGestureCatcher_MoveTop(object sender, EventArgs e)
        {
            if (m_gameCore == null) { return; }
            if (m_gameCore.IsAnyTaskRunning()) { return; }

            await m_gameCore.TryMoveUpAsync();
        }

        /// <summary>
        /// Called when the gesture catcher recognized a touch gesture.
        /// </summary>
        private async void OnGestureCatcher_MoveRight(object sender, EventArgs e)
        {
            if (m_gameCore == null) { return; }
            if (m_gameCore.IsAnyTaskRunning()) { return; }

            await m_gameCore.TryMoveRightAsync();
        }

        /// <summary>
        /// Called when the gesture catcher recognized a touch gesture.
        /// </summary>
        private async void OnGestureCatcher_MoveLeft(object sender, EventArgs e)
        {
            if (m_gameCore == null) { return; }
            if (m_gameCore.IsAnyTaskRunning()) { return; }

            await m_gameCore.TryMoveLeftAsync();
        }

        /// <summary>
        /// Called when the gesture catcher recognized a touch gesture.
        /// </summary>
        private async void OnGestureCatcher_MoveDown(object sender, EventArgs e)
        {
            if (m_gameCore == null) { return; }
            if (m_gameCore.IsAnyTaskRunning()) { return; }

            await m_gameCore.TryMoveDownAsync();
        }

        /// <summary>
        /// Called when user presses a key.
        /// </summary>
        private async void OnCurrentTarget_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (m_gameCore == null) { return; }
            if (m_gameCore.IsAnyTaskRunning()) { return; }

            // Trigger movement of displayed tiles
            switch (e.Key)
            {
                case VirtualKey.Down:
                case VirtualKey.NumberPad2:
                case VirtualKey.S:
                    await m_gameCore.TryMoveDownAsync();
                    break;

                case VirtualKey.Left:
                case VirtualKey.NumberPad4:
                case VirtualKey.A:
                    await m_gameCore.TryMoveLeftAsync();
                    break;

                case VirtualKey.Up:
                case VirtualKey.NumberPad8:
                case VirtualKey.W:
                    await m_gameCore.TryMoveUpAsync();
                    break;

                case VirtualKey.Right:
                case VirtualKey.NumberPad6:
                case VirtualKey.D:
                    await m_gameCore.TryMoveRightAsync();
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the currently associated object.
        /// </summary>
        public DependencyObject AssociatedObject
        {
            get { return m_currentTarget; }
            set
            {
                if (m_currentTarget != value)
                {
                    // Trigger game unload
                    if (m_currentTarget != null)
                    {
                        Unload();
                    }

                    // Set main target property
                    m_currentTarget = value as SwapChainBackgroundPanel;

                    // Trigger game initialization
                    if (m_currentTarget != null)
                    {
                        TriggerInitialize(m_currentTarget);
                    }
                }
            }
        }
    }
}
