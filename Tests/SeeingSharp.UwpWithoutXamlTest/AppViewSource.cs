﻿using Windows.ApplicationModel.Core;

namespace SeeingSharp.UwpWithoutXamlTest
{
    // The entry point for the app.
    internal class AppViewSource : IFrameworkViewSource
    {
        public IFrameworkView CreateView()
        {
            return new AppView();
        }
    }
}
