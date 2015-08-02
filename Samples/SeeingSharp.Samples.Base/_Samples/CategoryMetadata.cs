using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Samples.Base
{
    public static class CategoryMetadata
    {
        public static string GetCategoryIconSymbol(string categoryName)
        {
            switch(categoryName)
            {
                case Constants.SAMPLEGROUP_BASIC:
                    return "";

                case Constants.SAMPLEGROUP_DIRECT2D:
                    return "";

                case Constants.SAMPLEGROUP_MF:
                    return "";

                default:
                    return "";
            }
        }
    }
}
