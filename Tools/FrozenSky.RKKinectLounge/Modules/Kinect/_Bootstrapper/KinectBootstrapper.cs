using FrozenSky.Infrastructure;
using FrozenSky.Util;
using Microsoft.Kinect.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(FrozenSky.RKKinectLounge.Modules.Kinect.KinectBootstrapper),
    contractType: typeof(FrozenSky.Infrastructure.IBootstrapperItem))]

namespace FrozenSky.RKKinectLounge.Modules.Kinect
{
    internal class KinectBootstrapper : IBootstrapperItem
    {
        /// <summary>
        /// Executes the background action behind this item.
        /// </summary>
        public async Task Execute()
        {
            // Create PerformanceAnalyzer for kinect logic
            PerformanceAnalyzer performanceAnalyzer = new PerformanceAnalyzer(
                TimeSpan.FromMilliseconds(Constants.KINECT_PERF_VALUE_INTERVAL_MS),
                TimeSpan.FromMilliseconds(Constants.KINECT_PERF_CALC_INTERVAL_MS));
            performanceAnalyzer.GenerateCurrentValueCollection = true;
            performanceAnalyzer.GenerateHistoricalCollection = Constants.KINECT_PERF_HISTORICAL_VALUE_COUNT > 0;
            performanceAnalyzer.MaxCountHistoricalEntries = Constants.KINECT_PERF_HISTORICAL_VALUE_COUNT;
            performanceAnalyzer.RunAsync(CancellationToken.None)
                .FireAndForget();

            // Create and start the KinectHandler
            KinectThread kinectThread = new KinectThread(performanceAnalyzer);
            await kinectThread.StartAsync();

            // Register created objects
            FrozenSkyApplication.Current.RegisterSingleton(kinectThread);
            FrozenSkyApplication.Current.Singletons.RegisterSingleton(performanceAnalyzer, Constants.KINECT_PERF_ANALYZER_NAME);
        
            // Create other processing objects
            KinectRawStreamPresenter rawStreamProcessor = new KinectRawStreamPresenter();
            FrozenSkyApplication.Current.Singletons.RegisterSingleton(rawStreamProcessor);

            KinectQRCodePresenter qrStreamProcessor = new KinectQRCodePresenter();
            FrozenSkyApplication.Current.Singletons.RegisterSingleton(qrStreamProcessor);

            // Creates the main engagement model for this application
            HandOverheadEngagementModel customEngagementModel = new HandOverheadEngagementModel();
            FrozenSkyApplication.Current.RegisterService<IKinectEngagementManager>(customEngagementModel);
        }

        /// <summary>
        /// Gets a simple order id. The list of bootstrapper items will be sorted by this ID.
        /// </summary>
        public int OrderID
        {
            get { return Constants.KINECT_BOOT_ORDERID; }
        }

        /// <summary>
        /// Gets a short description of this item for the UI (e. g. for splash screens).
        /// </summary>
        public string Description
        {
            get { return Translatables.KINECT_BOOT_DESC; }
        }
    }
}
