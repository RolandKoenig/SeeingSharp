﻿using RKRocket.Game;
using RKRocket.View;
using RKRocket.ViewModel;
using SeeingSharp.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RKRocket
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : SeeingSharpPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            CtrlNavFrame.Navigate(typeof(GameStatusView));
        }

        private async void OnCmdShowProjectHomepage_Click(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.Game.Scene.IsPaused)
            {
                await Windows.System.Launcher.LaunchUriAsync(
                    new Uri("https://github.com/RolandKoenig/SeeingSharp"));
            }
        }

        private void OnMessage_Received(MessageGameOverDialogRequest message)
        {
            CtrlNavFrame.Navigate(
                typeof(GameOverView),
                message.ViewModel);
        }

        public MainUIViewModel ViewModel
        {
            get { return this.DataContext as MainUIViewModel; }
        }
    }
}
