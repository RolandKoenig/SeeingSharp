using FrozenSky.Infrastructure;
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using RKVideoMemory.Data;
using RKVideoMemory.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RKVideoMemory
{
    public partial class MainWindow : Form
    {
        private GameCore m_game;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Generic update logic for this dialog.
        /// </summary>
        private void UpdateDialogStates()
        {
            this.Enabled =
                (m_game != null) &&
                (m_game.IsInitialized);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!FrozenSkyApplication.IsInitialized) { return; }

            // Subscribe all handlers to the UIMessenger
            FrozenSkyApplication.Current.UIMessenger
                .SubscribeAllOnControl(this);

            // Associate game logic to UI
            m_game = new GameCore();
            m_ctrlRenderer.RenderLoop.Camera = m_game.Camera;
            m_ctrlRenderer.RenderLoop.SetScene(m_game.Scene);

            // Initialize game logic
            await m_game.InitializeAsync();
        }

        private void OnMessage_Received(GameInitializedMessage message)
        {
            this.UpdateDialogStates();
        }

        private void OnMessage_Received(LevelLoadedMessage message)
        {
            this.UpdateDialogStates();
        }

        private void OnMessage_Received(LevelUnloadedMessage message)
        {
            this.UpdateDialogStates();
        }

        private async void OnCmdLoadLevel_Click(object sender, EventArgs e)
        {
            if(m_dlgOpenDir.ShowDialog(this) == DialogResult.OK)
            {
                await m_game.LoadLevelAsync(m_dlgOpenDir.SelectedPath);
            }
        }
    }
}
