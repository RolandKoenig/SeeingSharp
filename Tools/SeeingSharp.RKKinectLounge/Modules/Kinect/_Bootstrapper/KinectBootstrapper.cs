#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it.
    More info at
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2015 Roland König (RolandK)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion License information (SeeingSharp and all based games/applications)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kinect.Toolkit.Input;
using SeeingSharp.Infrastructure;
using SeeingSharp.Util;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.RKKinectLounge.Modules.Kinect.KinectBootstrapper),
    contractType: typeof(SeeingSharp.Infrastructure.IBootstrapperItem))]

namespace SeeingSharp.RKKinectLounge.Modules.Kinect
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
            SeeingSharpApplication.Current.RegisterSingleton(kinectThread);
            SeeingSharpApplication.Current.Singletons.RegisterSingleton(performanceAnalyzer, Constants.KINECT_PERF_ANALYZER_NAME);

            // Create other processing objects
            KinectRawStreamPresenter rawStreamProcessor = new KinectRawStreamPresenter();
            SeeingSharpApplication.Current.Singletons.RegisterSingleton(rawStreamProcessor);

            KinectQRCodePresenter qrStreamProcessor = new KinectQRCodePresenter();
            SeeingSharpApplication.Current.Singletons.RegisterSingleton(qrStreamProcessor);

            // Creates the main engagement model for this application
            HandOverheadEngagementModel customEngagementModel = new HandOverheadEngagementModel();
            SeeingSharpApplication.Current.RegisterService<IKinectEngagementManager>(customEngagementModel);
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