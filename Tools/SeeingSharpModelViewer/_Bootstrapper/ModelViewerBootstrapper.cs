#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# Tools. More info at
     - https://github.com/RolandKoenig/SeeingSharp/tree/master/Tools (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)

	This program is distributed under the terms of the Microsoft Public License (Ms-PL)-
	More info at https://msdn.microsoft.com/en-us/library/ff647676.aspx
*/
#endregion License information (SeeingSharp and all based games/applications)

using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Views;
using System.Threading.Tasks;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharpModelViewer.ModelViewerBootstrapper),
    contractType: typeof(SeeingSharp.Infrastructure.IBootstrapperItem))]

namespace SeeingSharpModelViewer
{
    public class ModelViewerBootstrapper : IBootstrapperItem
    {
        /// <summary>
        /// Executes the background action behind this item.
        /// </summary>
        /// <param name="app">The current application instance.</param>
        public async Task Execute(SeeingSharpApplication app)
        {
            MainWindowViewModel modelVM = null;
            using (MemoryRenderTarget dummyRenderTarget = new MemoryRenderTarget(128, 128))
            {
                modelVM = new MainWindowViewModel();

                // Assign main scene and camera to the dummy render target
                dummyRenderTarget.Scene = modelVM.Scene;
                dummyRenderTarget.Camera = modelVM.Camera;

                // Initialize the scen
                await modelVM.InitializeAsync();

                // Perform some dummy renderings
                await dummyRenderTarget.AwaitRenderAsync();
                await dummyRenderTarget.AwaitRenderAsync();
            }

            // All went well, so register the main viewmode globally
            app.RegisterSingleton(modelVM);
        }

        public string Description
        {
            get { return Localizables.BOOTSTRAPPER_NAME; }
        }

        public int OrderID
        {
            get { return 0; }
        }
    }
}