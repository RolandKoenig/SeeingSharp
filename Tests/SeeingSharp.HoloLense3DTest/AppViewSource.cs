using Windows.ApplicationModel.Core;

namespace SeeingSharp.HoloLense3DTest
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
