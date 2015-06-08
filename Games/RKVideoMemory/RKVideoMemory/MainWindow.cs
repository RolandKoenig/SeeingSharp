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
#endregion License information (SeeingSharp and all based games/applications)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RKVideoMemory.Data;
using RKVideoMemory.Game;
using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;

namespace RKVideoMemory
{
    public partial class MainWindow : Form
    {
        #region initial state
        private Icon m_initialIcon;
        private string m_initialTitel;
        #endregion initial state

        #region game container
        private GameCore m_game;
        #endregion game container

        #region state variables
        private bool m_onTickProcessing;
        private List<SceneObject> m_objectsBelowCursor;
        private bool m_isFullscreen;
        private bool m_lastFullscreenState;
        private bool m_isLoading;
        #endregion state variables

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Remember initial state
            m_initialIcon = this.Icon;
            m_initialTitel = this.Text;

            m_objectsBelowCursor = new List<SceneObject>();
        }

        /// <summary>
        /// Generic update logic for this dialog.
        /// </summary>
        private void UpdateDialogStates()
        {
            this.Enabled =
                (m_game != null) &&
                (m_game.IsInitialized);

            m_barStatus.Visible = m_isLoading;

            // Handle fullscreen mode
            m_chkFullscreen.Checked = m_isFullscreen;

            // Handle changed state
            if (m_lastFullscreenState != m_isFullscreen)
            {
                m_lastFullscreenState = m_isFullscreen;
                if (m_isFullscreen)
                {
                    this.WindowState = FormWindowState.Normal;

                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    this.WindowState = FormWindowState.Maximized;
                    this.TopMost = true;
                }
                else
                {
                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                    this.WindowState = FormWindowState.Normal;
                    this.TopMost = false;
                }
            }
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!SeeingSharpApplication.IsInitialized) { return; }

            // Subscribe all handlers to the UIMessenger
            SeeingSharpApplication.Current.UIMessenger
                .SubscribeAllOnControl(this);

            // Associate game logic to UI
            m_game = new GameCore();
            m_ctrlRenderer.RenderLoop.Camera = m_game.Camera;
            m_ctrlRenderer.RenderLoop.SetScene(m_game.Scene);

            // Initialize game logic
            await m_game.InitializeAsync();

            // Try to load the default level
            string defaultFolder = Path.Combine(
                Path.GetDirectoryName(this.GetType().Assembly.Location),
                Constants.DEFAULT_FOLDER_INITIAL_LEVEL);
            if (Directory.Exists(defaultFolder))
            {
                m_isLoading = true;
                try
                {
                    this.UpdateDialogStates();
                    await m_game.LoadLevelAsync(defaultFolder);
                }
                finally
                {
                    m_isLoading = false;
                    this.UpdateDialogStates();
                }
            }
        }

        private void OnMessage_Received(GameInitializedMessage message)
        {
            this.UpdateDialogStates();
        }

        /// <summary>
        /// Called when the game requests to display a video.
        /// </summary>
        private async void OnMessage_Received(PlayMovieRequestMessage message)
        {
            m_ctrlRenderer.DiscardPresent = true;
            try
            {
                // Perform rendering here
                await m_mediaPlayer.OpenAndShowVideoFileAsync(message.VideoLink);
            }
            catch (Exception)
            {
                m_ctrlRenderer.DiscardPresent = false;

                // Trigger MovieFinished message to ensure correct game logic
                SeeingSharpApplication.Current.UIMessenger
                    .Publish<PlayMovieFinishedMessage>();

                // Rethrow exception
                throw;
            }
        }

        private void OnMessage_Received(LevelLoadedMessage message)
        {
            // Replace icon
            if (!string.IsNullOrEmpty(message.LoadedLevel.AppIconPath))
            {
                this.Icon = new Icon(message.LoadedLevel.AppIconPath);
            }
            else
            {
                this.Icon = m_initialIcon;
            }

            // Replace titel
            if (!string.IsNullOrEmpty(message.LoadedLevel.LevelSettings.AppName))
            {
                this.Text = "RK Video Memory - " + message.LoadedLevel.LevelSettings.AppName;
            }
            else
            {
                this.Text = m_initialTitel;
            }
        }

        /// <summary>
        /// Called when video playing is completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnMediaPlayer_VideoFinished(object sender, EventArgs e)
        {
            m_ctrlRenderer.DiscardPresent = false;

            // Raise video-play finished message
            SeeingSharpApplication.Current.UIMessenger
                .Publish<PlayMovieFinishedMessage>();
        }

        private void OnCtrlRenderer_MouseClick(object sender, MouseEventArgs e)
        {
            if (m_objectsBelowCursor.Count > 0)
            {
                SeeingSharpApplication.Current.UIMessenger.Publish(
                    new ObjectsClickedMessage(m_objectsBelowCursor.ToList()));
            }
        }

        private async void OnTimerPicking_Tick(object sender, EventArgs e)
        {
            this.UpdateDialogStates();

            if (m_onTickProcessing) { return; }

            m_onTickProcessing = true;
            try
            {
                // Perform simple picking test
                List<SceneObject> objectsBelowCursor = await m_ctrlRenderer.GetObjectsBelowCursorAsync();
                if (this.IsDisposed) { return; }

                // Look, what is new and what is old
                List<SceneObject> removedObjects = new List<SceneObject>(m_objectsBelowCursor.Count);
                List<SceneObject> addedObjects = new List<SceneObject>(objectsBelowCursor.Count);
                foreach (var actPickedObject in m_objectsBelowCursor)
                {
                    if (!objectsBelowCursor.Contains(actPickedObject))
                    {
                        removedObjects.Add(actPickedObject);
                    }
                }
                foreach (var actObjectBelowCurser in objectsBelowCursor)
                {
                    if (!m_objectsBelowCursor.Contains(actObjectBelowCurser))
                    {
                        addedObjects.Add(actObjectBelowCurser);
                    }
                }

                // Update local collection
                foreach (var actRemovedObject in removedObjects) { m_objectsBelowCursor.Remove(actRemovedObject); }
                foreach (var actAddedObject in addedObjects) { m_objectsBelowCursor.Add(actAddedObject); }

                // Notify changes to game system
                HoveredObjectsChangedMessage hoveredChangedMessage = new HoveredObjectsChangedMessage(
                    removedObjects, addedObjects);
                SeeingSharpApplication.Current.UIMessenger.Publish(hoveredChangedMessage);
            }
            finally
            {
                m_onTickProcessing = false;
            }
        }

        private async void OnCmdLoadLevel_Click(object sender, EventArgs e)
        {
            if (m_dlgOpenDir.ShowDialog(this) == DialogResult.OK)
            {
                m_isLoading = true;
                try
                {
                    this.UpdateDialogStates();
                    await m_game.LoadLevelAsync(m_dlgOpenDir.SelectedPath);
                }
                finally
                {
                    m_isLoading = false;
                    this.UpdateDialogStates();
                }
            }
        }

        private void OnChkFullscreen_Click(object sender, EventArgs e)
        {
            m_isFullscreen = !m_isFullscreen;
            this.UpdateDialogStates();
        }

        private void OnCmdClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnMainMenu_MenuActivate(object sender, EventArgs e)
        {
            m_behaviorHideMenubar.IsHidingActive = false;
        }

        private void OnMainMenu_MenuDeactivate(object sender, EventArgs e)
        {
            m_behaviorHideMenubar.IsHidingActive = true;
        }
    }
}