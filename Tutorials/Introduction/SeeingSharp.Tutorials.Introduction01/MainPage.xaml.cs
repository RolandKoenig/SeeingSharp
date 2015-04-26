using SeeingSharp.Multimedia.Views;
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

namespace SeeingSharp.Tutorials.Introduction01
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SeeingSharpPanelPainter m_panelPainter;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += OnMainPage_Loaded;
        }

        private void OnMainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_panelPainter != null) { return; }

            // Attach the painter to the target render panel
            m_panelPainter = new SeeingSharpPanelPainter(this.RenderTargetPanel);
            m_panelPainter.RenderLoop.ClearColor = Color4.CornflowerBlue;
        }
    }
}
