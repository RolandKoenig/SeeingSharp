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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SeeingSharp.Infrastructure;
using SeeingSharp.Util;

namespace SeeingSharp.RKKinectLounge.Base
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Status variables for the main window
        private KinectWelcomeViewModel m_welcomeViewModel;
        private MainFolderViewModel m_mainFolderViewModel;
        private bool m_isWelcomeViewVisible;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            m_welcomeViewModel = new KinectWelcomeViewModel();
            m_mainFolderViewModel = new MainFolderViewModel(Properties.Settings.Default.DataPath);
            m_isWelcomeViewVisible = true;

            if (!SeeingSharpApplication.IsInitialized) { return; }

            // Register on messages
            SeeingSharpMessenger uiMessenger = SeeingSharpApplication.Current.UIMessenger;
            uiMessenger.Subscribe<MessagePersonEngaged>(OnMessagePersonEngaged);
            uiMessenger.Subscribe<MessagePersonDisengaged>(OnMessagePersonDisengaged);
            uiMessenger.Subscribe<MessageManualEnter>(OnMessageManualEnter);
            uiMessenger.Subscribe<MessageManualExit>(OnMessageManualExit);
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
            // RK: Don't do this anymore, because we may want to show pictures also when we leave the area
            // ActivateWelcomeView();
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
            get { return SeeingSharpApplication.Current.ProductName; }
        }
    }
}