using FrozenSky.Infrastructure;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrozenSky.RKKinectLounge.Base
{
    public class MainWindowViewModel : ViewModelBase
    {
        private KinectWelcomeViewModel m_welcomeViewModel;
        private MainFolderViewModel m_mainFolderViewModel;
        private bool m_isWelcomeViewVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            m_welcomeViewModel = new KinectWelcomeViewModel();
            m_mainFolderViewModel = new MainFolderViewModel(Properties.Settings.Default.DataPath);
            m_isWelcomeViewVisible = true;

            if (!FrozenSkyApplication.IsInitialized) { return; }

            // Register on messages 
            FrozenSkyMessageHandler uiMessageHandler = FrozenSkyApplication.Current.UIMessageHandler;
            uiMessageHandler.Subscribe<MessagePersonEngaged>(OnMessagePersonEngaged);
            uiMessageHandler.Subscribe<MessagePersonDisengaged>(OnMessagePersonDisengaged);
            uiMessageHandler.Subscribe<MessageManualEnter>(OnMessageManualEnter);
            uiMessageHandler.Subscribe<MessageManualExit>(OnMessageManualExit);
        }

        private void ActivateWelcomeView()
        {
            this.IsWelcomeViewVisible = true;
        }

        private void ActivateMainFolderView()
        {
            if (this.IsWelcomeViewVisible)
            {
                m_mainFolderViewModel = new MainFolderViewModel(Properties.Settings.Default.DataPath);
                this.IsWelcomeViewVisible = false;
            }
        }

        /// <summary>
        /// Called when we've engaged a person.
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnMessagePersonEngaged(MessagePersonEngaged message)
        {
            ActivateMainFolderView();
        }

        /// <summary>
        /// Called when we've disengaged a person.
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnMessagePersonDisengaged(MessagePersonDisengaged message)
        {
            ActivateWelcomeView();
        }

        private void OnMessageManualEnter(MessageManualEnter message)
        {
            ActivateMainFolderView();
        }

        private void OnMessageManualExit(MessageManualExit message)
        {
            ActivateWelcomeView();
        }

        /// <summary>
        /// Gets the ViewModel of the WelcomeView.
        /// </summary>
        public KinectWelcomeViewModel WelcomeViewModel
        {
            get { return m_welcomeViewModel; }
        }

        /// <summary>
        /// Gets the ViewModel of the MainFolderView.
        /// </summary>
        public MainFolderViewModel MainFolderViewModel
        {
            get { return m_mainFolderViewModel; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the WelcomeView is displayed currently.
        /// </summary>
        public bool IsWelcomeViewVisible
        {
            get { return m_isWelcomeViewVisible; }
            set
            {
                if (m_isWelcomeViewVisible != value)
                {
                    m_isWelcomeViewVisible = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(() => KinectViewerVisibility);
                    RaisePropertyChanged(() => DisplayedViewModel);
                }
            }
        }

        /// <summary>
        /// Gets the currently displayed viewmodel object.
        /// </summary>
        public ViewModelBase DisplayedViewModel
        {
            get
            {
                if (m_isWelcomeViewVisible) { return this.WelcomeViewModel; }
                else { return this.MainFolderViewModel; }
            }
        }

        /// <summary>
        /// Gets the visibility of the main KinectViewer.
        /// </summary>
        public Visibility KinectViewerVisibility
        {
            get { return m_isWelcomeViewVisible ? Visibility.Visible : Visibility.Collapsed; }
        }

        /// <summary>
        /// Gets the title of the current view.
        /// </summary>
        public string Title
        {
            get { return FrozenSkyApplication.Current.ProductName; }
        }
    }
}
