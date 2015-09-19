using RKRocket.ViewModel;
using SeeingSharp.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RKRocket
{
    public partial class MainWindow : Form
    {
        private MainUIViewModel m_viewModel;

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

            m_viewModel = new MainUIViewModel();
            m_renderPanel.Scene = m_viewModel.Game.GameScene;
            m_renderPanel.Camera = m_viewModel.Game.Camera;

            m_viewModel.PropertyChanged += OnViewModel_PropertyChanged;
        }

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
            }
        }
    }
}
