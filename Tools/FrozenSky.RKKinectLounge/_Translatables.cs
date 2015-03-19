using FrozenSky.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: AssemblyQueryableType(typeof(FrozenSky.RKKinectLounge.Translatables))]

namespace FrozenSky.RKKinectLounge
{
    [TranslateableClass("RKKinectLounge")]
    internal static class Translatables
    {
        public static string KINECT_BOOT_DESC = "Initializing Kinect-Hanndler";
    }
}
