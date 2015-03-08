using FrozenSky.Infrastructure;
using RK2048.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace RK2048.DesktopWpf
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameCore m_gameCore;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += OnLoad;
            this.KeyDown += OnKeyDown;
        }

        /// <summary>
        /// Called when the user presses any key.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private async void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (m_gameCore == null) { return; }
            if (m_gameCore.IsAnyTaskRunning()) { return; }

            // Trigger movement of displayed tiles
            switch (e.Key)
            {
                case Key.Down:
                case Key.NumPad2:
                case Key.S:
                    await m_gameCore.TryMoveDownAsync();
                    break;

                case Key.Left:
                case Key.NumPad4:
                case Key.A:
                    await m_gameCore.TryMoveLeftAsync();
                    break;

                case Key.Up:
                case Key.NumPad8:
                case Key.W:
                    await m_gameCore.TryMoveUpAsync();
                    break;

                case Key.Right:
                case Key.NumPad6:
                case Key.D:
                    await m_gameCore.TryMoveRightAsync();
                    break;
            }
        }

        /// <summary>
        /// Default initialization event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void OnLoad(object sender, RoutedEventArgs e)
        {
            if (!FrozenSkyApplication.IsInitialized) { return; }

            // Apply correct window title
            string windowTitle = this.Title;
            this.Title = string.Format(Translatables.TITLE_VERSION, windowTitle, FrozenSkyApplication.Current.ProductVersion);

            // Set this window to maximized mode
            this.WindowState = System.Windows.WindowState.Maximized;

            m_gameCore = new GameCore();
            this.DataContext = m_gameCore;

            await m_gameCore.InitializeAsync(m_rendererElement);

            // Update the location of the camera finally
            m_gameCore.UpdateCameraLocation();
        }

        /// <summary>
        /// Called when the window is about to close.
        /// </summary>
        protected override async void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            if (m_gameCore != null)
            {
                e.Cancel = true;

                await m_gameCore.UpdateScoreDataAsync();
                m_gameCore = null;

                this.Close();
            }
        }

        /// <summary>
        /// Called when the user wants to restart the game.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void OnCmdRestart_Click(object sender, RoutedEventArgs e)
        {
            if (m_gameCore != null)
            {
                await m_gameCore.RestartGameAsync();
            }
        }

        private void OnCmdWeb_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.rolandk.de/wp");
        }
    }
}
