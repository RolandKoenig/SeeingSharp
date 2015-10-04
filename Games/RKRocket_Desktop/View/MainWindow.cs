using RKRocket.ViewModel;
using SeeingSharp.Infrastructure;
using SeeingSharp.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RKRocket.View
{
    public partial class MainWindow : SeeingSharpForm
    {
        #region State
        private MainUIViewModel m_viewModel;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!SeeingSharpApplication.IsInitialized) { return; }

            // Create the main viewmodel
            m_viewModel = new MainUIViewModel();
            m_renderPanel.Scene = m_viewModel.Game.GameScene;
            m_renderPanel.Camera = m_viewModel.Game.Camera;
            m_viewModel.PropertyChanged += OnViewModel_PropertyChanged;

            // Set initial label contents
            m_lblLevelValue.Text = m_viewModel.CurrentLevel.ToString();
            m_lblScoreValue.Text = m_viewModel.CurrentScore.ToString();
            m_lblMaxReachedValue.Text = m_viewModel.MaxReachedScore.ToString();
            m_lblHealthValue.Text = m_viewModel.CurrentHealth.ToString();

            m_mnuGame.DropDownOpened += OnMenu_DropDownOpened;
            m_mnuInfo.DropDownOpened += OnMenu_DropDownOpened;
            m_mnuGame.DropDownClosed += OnMenu_DropDownClosed;
            m_mnuInfo.DropDownClosed += OnMenu_DropDownClosed;
        }



        private void OnMenu_DropDownOpened(object sender, EventArgs e)
        {
            base.Messenger.Publish<MessageMenuOpened>();
        }

        private void OnMenu_DropDownClosed(object sender, EventArgs e)
        {
            base.Messenger.Publish<MessageMenuClosed>();
        }

        /// <summary>
        /// Manually react on property changes within the viewmodel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(MainUIViewModel.CurrentLevel):
                    m_lblLevelValue.Text = m_viewModel.CurrentLevel.ToString();
                    break;

                case nameof(MainUIViewModel.CurrentScore):
                    m_lblScoreValue.Text = m_viewModel.CurrentScore.ToString();
                    break;

                case nameof(MainUIViewModel.CurrentHealth):
                    m_lblHealthValue.Text = m_viewModel.CurrentHealth.ToString();
                    break;

                case nameof(MainUIViewModel.MaxReachedScore):
                    m_lblMaxReachedValue.Text = m_viewModel.MaxReachedScore.ToString();
                    break;
            }
        }

        private void OnMnuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnMnuStartNew_Click(object sender, EventArgs e)
        {
            m_viewModel.CommandNewGame.Execute();
        }

        /// <summary>
        /// Called when the ViewModel wants to switch to GameOver view.
        /// </summary>
        /// <param name="message">the message to be processed.</param>
        private void OnMessage_Received(MessageGameOverDialogRequest message)
        {
            using (GameOverForm dlgGameOver = new GameOverForm())
            {
                dlgGameOver.ViewModel = message.ViewModel;
                dlgGameOver.StartPosition = FormStartPosition.CenterParent;
                switch (dlgGameOver.ShowDialog(this))
                {
                        // Dialog confirmed
                    case DialogResult.OK:
                        break;

                        // Dialog canceled
                    default:
                        break;
                }
            }
        }
    }
}
