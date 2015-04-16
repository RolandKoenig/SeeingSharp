#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
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
#endregion

using FrozenSky.Infrastructure;
using FrozenSky.Multimedia.Core;
using FrozenSky.Checking;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Samples.Base
{
    public class SampleFactory
    {
        private static SampleFactory s_current;

        private List<Tuple<SampleInfoAttribute, Type>> m_sampleTypes;

        /// <summary>
        /// Prevents a default instance of the <see cref="SampleFactory"/> class from being created.
        /// </summary>
        private SampleFactory()
        {
            // Create and register viewmodel for performance analyziation
            PerformanceAnalysisViewModel performanceAnalyzisViewModel = new PerformanceAnalysisViewModel();
            FrozenSkyApplication.Current.RegisterSingleton(performanceAnalyzisViewModel);

            // Query for all sample types
            m_sampleTypes = new List<Tuple<SampleInfoAttribute, Type>>();
            foreach(Type actSampleType in FrozenSkyApplication.Current.TypeQuery
                .GetTypesByContract(typeof(SampleBase)))
            {
                SampleInfoAttribute actInfoAttrib = actSampleType.GetTypeInfo().GetCustomAttribute<SampleInfoAttribute>();
                if (actInfoAttrib == null) { continue; }

                m_sampleTypes.Add(Tuple.Create(
                    actInfoAttrib,
                    actSampleType));
            }
        }

        /// <summary>
        /// Gets a collection containing all sample infomration objects.
        /// </summary>
        public IEnumerable<SampleInfoAttribute> GetSampleInfos()
        {
            return m_sampleTypes.Select((actSampleType) => actSampleType.Item1);
        }

        /// <summary>
        /// Creates the given sample.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="sampleName">Name of the sample.</param>
        public SampleBase CreateSample(string categoryName, string sampleName)
        {
            foreach(var actSample in m_sampleTypes)
            {
                if((actSample.Item1.Category == categoryName) &&
                   (actSample.Item1.Name == sampleName))
                {
                    return Activator.CreateInstance(actSample.Item2) as SampleBase;
                }
            }

            throw new FrozenSkyException(
                string.Format(
                "Sample {0} in category {1} not found!", 
                sampleName, categoryName));
        }

        /// <summary>
        /// Creates the given sample.
        /// </summary>
        /// <param name="sampleInfo">The sample information.</param>
        public SampleBase CreateSample(SampleInfoAttribute sampleInfo)
        {
            sampleInfo.EnsureNotNull("sampleInfo");

            var actSample = m_sampleTypes.First((actTuple) => actTuple.Item1 == sampleInfo);
            return Activator.CreateInstance(actSample.Item2) as SampleBase;
        }

        /// <summary>
        /// Applies the sample with the given name to the given RenderLoop.
        /// </summary>
        /// <param name="renderLoop">The render loop.</param>
        /// <param name="sampleName">Name of the sample.</param>
        public void ApplySample(RenderLoop renderLoop, string sampleName)
        {
            var sampleType = m_sampleTypes
                .Where((actSampleType) => actSampleType.Item1.Name == sampleName)
                .Select((actTuple) => actTuple.Item2)
                .FirstOrDefault();
            if (sampleType == null) { throw new FrozenSkyException(string.Format("Unable to find sample {0}!", sampleName)); }

            SampleBase sample = Activator.CreateInstance(sampleType) as SampleBase;
            sample.OnStartupAsync(renderLoop);
        }

        public static SampleFactory Current
        {
            get
            {
                if (s_current == null) { s_current = new SampleFactory(); }
                return s_current;
            }
        }
    }
}
