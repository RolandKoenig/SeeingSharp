using SeeingSharp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.RKKinectLounge.Base
{
    public static class ModuleManager
    {
        private static List<IKinectLoungeModule> s_loadedModules;

        /// <summary>
        /// Initializes the <see cref="ViewFactory"/> class.
        /// </summary>
        static ModuleManager()
        {
            s_loadedModules = SeeingSharpApplication.Current.TypeQuery.GetAndInstanciateByContract<IKinectLoungeModule>();
        }

        public static List<IKinectLoungeModule> LoadedModules
        {
            get { return s_loadedModules; }
        }
    }
}
