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

	********************************************************************************
    Additional permission under GNU GPL version 3 section 7

    If you modify this Program, or any covered work, by linking or combining it with 
	DirectX (or a modified version of that library), containing parts covered by the 
	terms of [name of library's license], the licensors of this Program grant you additional 
	permission to convey the resulting work. {Corresponding Source for a non-source form of 
	such a combination shall include the source code for the parts of DirectX used 
	as well as that of the covered work.}
*/
#endregion
using FrozenSky.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Some namespace mappings
using GDI = System.Drawing;

namespace FrozenSky.Tests
{
    [TestClass]
    public class UtilityTests
    {
        public const string TEST_CATEGORY = "FrozenSky Utilities";

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void BitmapComparison_Positive()
        {
            using(GDI.Bitmap leftBitmap = Properties.Resources.FlatShadedObject)
            using(GDI.Bitmap rightBitmap = Properties.Resources.FlatShadedObject)
            {
                Assert.IsTrue(
                    BitmapComparison.CalculatePercentageDifference(leftBitmap, rightBitmap) == 0f);
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void BitmapComparison_Negative_Without_Model()
        {
            using (GDI.Bitmap leftBitmap = Properties.Resources.ClearedScreen)
            using (GDI.Bitmap rightBitmap = Properties.Resources.FlatShadedObject)
            {
                float comparisonResult = BitmapComparison.CalculatePercentageDifference(leftBitmap, rightBitmap);
                Assert.IsTrue(comparisonResult > 0.25f);
                Assert.IsTrue(comparisonResult < 0.6f);
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void BitmapComparison_Negative_Inversed_Image()
        {
            using (GDI.Bitmap leftBitmap = Properties.Resources.FlatShadedObject)
            using (GDI.Bitmap rightBitmap = Properties.Resources.FlatShadedObject_Negative)
            {
                float comparisonResult = BitmapComparison.CalculatePercentageDifference(leftBitmap, rightBitmap);
                Assert.IsTrue(comparisonResult > 0.9f);
                Assert.IsTrue(comparisonResult <= 1.0f);
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void BitmapComparison_Negative_BlackWhite()
        {
            using (GDI.Bitmap leftBitmap = Properties.Resources.WhiteScreen)
            using (GDI.Bitmap rightBitmap = Properties.Resources.BlackScreen)
            {
                float comparisonResult = BitmapComparison.CalculatePercentageDifference(leftBitmap, rightBitmap);
                Assert.IsTrue(comparisonResult == 1.0f);
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void BitmapComparison_Negative_Enlighted()
        {
            using (GDI.Bitmap leftBitmap = Properties.Resources.FlatShadedObject)
            using (GDI.Bitmap rightBitmap = Properties.Resources.FlatShadedObject_Enlighted)
            {
                float comparisonResult = BitmapComparison.CalculatePercentageDifference(leftBitmap, rightBitmap);
                Assert.IsTrue(comparisonResult > 0.1);
                Assert.IsTrue(comparisonResult < 0.4);
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void BitmapComparison_Negative_Smaller()
        {
            using (GDI.Bitmap leftBitmap = Properties.Resources.FlatShadedObject)
            using (GDI.Bitmap rightBitmap = Properties.Resources.FlatShadedObject_Smaller)
            {
                float comparisonResult = BitmapComparison.CalculatePercentageDifference(leftBitmap, rightBitmap);
                Assert.IsTrue(comparisonResult > 0.1);
                Assert.IsTrue(comparisonResult < 0.4);
            }
        }
    }
}
