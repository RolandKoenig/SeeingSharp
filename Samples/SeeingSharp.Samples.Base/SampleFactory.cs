#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Checking;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Samples.Base
{
    /// <summary>
    /// This singleton class holds all information of available 
    /// samples and can instanciate them.
    /// </summary>
    public class SampleFactory
    {
        private static SampleFactory s_current;

        private List<Tuple<SampleDescription, Type>> m_sampleTypes;

        /// <summary>
        /// Prevents a default instance of the <see cref="SampleFactory"/> class from being created.
        /// </summary>
        private SampleFactory()
        {
            // Create and register viewmodel for performance analyziation
            PerformanceAnalysisViewModel performanceAnalyzisViewModel = new PerformanceAnalysisViewModel();
            SeeingSharpApplication.Current.RegisterSingleton(performanceAnalyzisViewModel);

            // Query for all sample types
            m_sampleTypes = new List<Tuple<SampleDescription, Type>>();
            foreach (Type actSampleType in SeeingSharpApplication.Current.TypeQuery
                .GetTypesByContract(typeof(SampleBase)))
            {
                SampleInfoAttribute actInfoAttrib = actSampleType.GetTypeInfo().GetCustomAttribute<SampleInfoAttribute>();
                if (actInfoAttrib == null) { continue; }

                if (!DoesSampleSupportCurrentPlatform(actInfoAttrib))
                {
                    continue;
                }

                m_sampleTypes.Add(Tuple.Create(
                    new SampleDescription(actInfoAttrib, actSampleType),
                    actSampleType));
            }
            m_sampleTypes.Sort(new Comparison<Tuple<SampleDescription, Type>>(
                (left, right) => left.Item1.OrderID.CompareTo(right.Item1.OrderID)));
        }

        /// <summary>
        /// Gets a collection containing all sample infomration objects.
        /// </summary>
        public IEnumerable<SampleDescription> GetSampleInfos()
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

            throw new SeeingSharpException(
                string.Format(
                "Sample {0} in category {1} not found!", 
                sampleName, categoryName));
        }

        /// <summary>
        /// Creates the given sample.
        /// </summary>
        /// <param name="sampleInfo">The sample information.</param>
        public SampleBase CreateSample(SampleDescription sampleInfo)
        {
            sampleInfo.EnsureNotNull("sampleInfo");

            var actSample = m_sampleTypes.First((actTuple) => actTuple.Item1 == sampleInfo);
            return Activator.CreateInstance(actSample.Item2) as SampleBase;
        }

        /// <summary>
        /// Applies the given sample to the given RenderLoop.
        /// </summary>
        /// <param name="renderLoop">The render loop.</param>
        /// <param name="sampleDesc">The sample to be applied.</param>
        public SampleBase ApplySample(RenderLoop renderLoop, SampleDescription sampleDesc)
        {
            renderLoop.EnsureNotNull("renderLoop");
            sampleDesc.EnsureNotNull("sampleDesc");

            SampleBase sample = Activator.CreateInstance(sampleDesc.SampleClass) as SampleBase;
            sample.OnStartupAsync(renderLoop);
            return sample;
        }

        private bool DoesSampleSupportCurrentPlatform(SampleInfoAttribute sampleAttrib)
        {
            switch(GraphicsCore.Current.CurrentPlatform)
            {
                case SeeingSharpPlatform.Desktop:
                    return sampleAttrib.TargetPlatform.HasFlag(SampleTargetPlatform.Desktop);

                case SeeingSharpPlatform.ModernPCOrTabletApp:
                    return sampleAttrib.TargetPlatform.HasFlag(SampleTargetPlatform.ModernPCOrTabletApp);

                case SeeingSharpPlatform.WindowsPhone:
                    return sampleAttrib.TargetPlatform.HasFlag(SampleTargetPlatform.WindowsPhone);

                default:
                    throw new SeeingSharpException("Unable to handle platform " + GraphicsCore.Current.CurrentPlatform);
            }
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
