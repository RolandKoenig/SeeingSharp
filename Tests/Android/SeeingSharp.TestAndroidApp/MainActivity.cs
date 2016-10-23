using Android.App;
using Android.Widget;
using Android.OS;
using SeeingSharp.Infrastructure;
using System.Reflection;

namespace SeeingSharp.TestAndroidApp
{
    [Activity(Label = "SeeingSharp.TestAndroidApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Initialize main application object
            await SeeingSharpApplication.InitializeAsync(
                Assembly.GetExecutingAssembly(),
                null,
                new string[0]);

            // Initialize UI environment (sets the ui SynchronizationContext, etc.)
            SeeingSharpApplication.Current.InitializeUIEnvironment();

        }
    }
}

