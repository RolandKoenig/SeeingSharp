using RKRocket.Game;
using RKRocket.ViewModel;
using SeeingSharp.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RKRocket.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameOverView : SeeingSharpPage
    {
        public GameOverView()
        {
            this.InitializeComponent();

            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TxtName.Focus(FocusState.Keyboard);
        }

        private void OnMessage_Received(MessageNewGame message)
        {
            base.Frame.GoBack();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.ViewModel = e.Parameter as GameOverViewModel;
        }

        public GameOverViewModel ViewModel
        {
            get { return base.DataContext as GameOverViewModel; }
            set { this.DataContext = value; }
        }
    }
}
