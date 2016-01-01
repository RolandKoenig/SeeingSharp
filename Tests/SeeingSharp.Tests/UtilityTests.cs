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
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Some namespace mappings
using GDI = System.Drawing;
using Xunit;

namespace SeeingSharp.Tests
{
    public class UtilityTests
    {
        public const string TEST_CATEGORY = "SeeingSharp Utilities";

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void BitmapComparison_Positive()
        {
            using(GDI.Bitmap leftBitmap = Properties.Resources.FlatShadedObject)
            using(GDI.Bitmap rightBitmap = Properties.Resources.FlatShadedObject)
            {
                Assert.True(
                    BitmapComparison.CalculatePercentageDifference(leftBitmap, rightBitmap) == 0f);
            }
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void BitmapComparison_Negative_Without_Model()
        {
            using (GDI.Bitmap leftBitmap = Properties.Resources.ClearedScreen)
            using (GDI.Bitmap rightBitmap = Properties.Resources.FlatShadedObject)
            {
                float comparisonResult = BitmapComparison.CalculatePercentageDifference(leftBitmap, rightBitmap);
                Assert.True(comparisonResult > 0.25f);
                Assert.True(comparisonResult < 0.6f);
            }
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void BitmapComparison_Negative_Inversed_Image()
        {
            using (GDI.Bitmap leftBitmap = Properties.Resources.FlatShadedObject)
            using (GDI.Bitmap rightBitmap = Properties.Resources.FlatShadedObject_Negative)
            {
                float comparisonResult = BitmapComparison.CalculatePercentageDifference(leftBitmap, rightBitmap);
                Assert.True(comparisonResult > 0.9f);
                Assert.True(comparisonResult <= 1.0f);
            }
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void BitmapComparison_Negative_BlackWhite()
        {
            using (GDI.Bitmap leftBitmap = Properties.Resources.WhiteScreen)
            using (GDI.Bitmap rightBitmap = Properties.Resources.BlackScreen)
            {
                float comparisonResult = BitmapComparison.CalculatePercentageDifference(leftBitmap, rightBitmap);
                Assert.True(comparisonResult == 1.0f);
            }
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void BitmapComparison_Negative_Enlighted()
        {
            using (GDI.Bitmap leftBitmap = Properties.Resources.FlatShadedObject)
            using (GDI.Bitmap rightBitmap = Properties.Resources.FlatShadedObject_Enlighted)
            {
                float comparisonResult = BitmapComparison.CalculatePercentageDifference(leftBitmap, rightBitmap);
                Assert.True(comparisonResult > 0.1);
                Assert.True(comparisonResult < 0.4);
            }
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void BitmapComparison_Negative_Smaller()
        {
            using (GDI.Bitmap leftBitmap = Properties.Resources.FlatShadedObject)
            using (GDI.Bitmap rightBitmap = Properties.Resources.FlatShadedObject_Smaller)
            {
                float comparisonResult = BitmapComparison.CalculatePercentageDifference(leftBitmap, rightBitmap);
                Assert.True(comparisonResult > 0.1);
                Assert.True(comparisonResult < 0.4);
            }
        }
    }
}
