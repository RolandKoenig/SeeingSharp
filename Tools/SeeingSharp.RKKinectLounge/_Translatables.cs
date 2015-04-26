using SeeingSharp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: AssemblyQueryableType(typeof(SeeingSharp.RKKinectLounge.Translatables))]

namespace SeeingSharp.RKKinectLounge
{
    [TranslateableClass("RKKinectLounge")]
    internal static class Translatables
    {
        public static string KINECT_BOOT_DESC = "Initializing Kinect-Hanndler";
    }
}
