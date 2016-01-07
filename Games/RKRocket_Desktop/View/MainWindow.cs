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
using RKRocket.ViewModel;
using SeeingSharp.Infrastructure;
using SeeingSharp.View;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RKRocket.View
{
    public partial class MainWindow : SeeingSharpForm
    {
        #region State
        private MainUIViewModel m_viewModel;
        private int m_dropDownOpenCount;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Updates the state of the dialog.
        /// </summary>
        private void UpdateDialogState()
        {
            // Update status bar
            int minXPos =
                m_lblLevel.Width + m_lblLevelValue.Width +
                m_lblHealth.Width + m_lblHealthValue.Width +
                20;
            int targetX = m_barStatus.Width - (
                m_lblScore.Width + m_lblScoreValue.Width +
                m_lblMaxReached.Width + m_lblMaxReachedValue.Width);
            if (targetX > minXPos)
            {
                m_lblScore.Margin = new Padding(targetX - minXPos, 3, 0, 3);
            }
            else
            {
                m_lblScore.Margin = new Padding(0, 3, 0, 3);
            }
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

            this.UpdateDialogState();
        }

        private void OnMenu_DropDownOpened(object sender, EventArgs e)
        {
            m_dropDownOpenCount++;
            base.Messenger.Publish<MessageMenuOpened>();
        }

        private void OnMenu_DropDownClosed(object sender, EventArgs e)
        {
            m_dropDownOpenCount--;
            CommonTools.InvokeDelayed(() =>
                {
                    if (this.IsDisposed) { return; }
                    if(m_dropDownOpenCount <= 0)
                    {
                        base.Messenger.Publish<MessageMenuClosed>();
                    }
                },
                TimeSpan.FromMilliseconds(300.0));
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

            this.UpdateDialogState();
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

                dlgGameOver.ShowDialog(this);
            }
        }

        private void OnBarStatus_SizeChanged(object sender, EventArgs e)
        {
            this.UpdateDialogState();
        }

        private void OnCmdPerformanceInfo_Click(object sender, EventArgs e)
        {
            PerformanceMeasureForm perfForm = new PerformanceMeasureForm();
            perfForm.Show();
        }
    }
}
