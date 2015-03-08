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
using FrozenSky.Multimedia;
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Views;
using FrozenSky.Infrastructure;
using FrozenSky.Multimedia.Objects;
using FrozenSky;
using FrozenSky.Multimedia.Drawing3D;
using RK2048.Graphics;
using RK2048.Logic;
using Windows.System;
using Windows.UI.Input;
using FrozenSky.Util;
using System.Threading.Tasks;

namespace RK2048
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainGamePage : SwapChainBackgroundPanel
    {
        public MainGamePage()
        {
            this.InitializeComponent();
        }
    }
}
