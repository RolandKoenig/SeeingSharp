#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2014 Roland König (RolandK)

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
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FrozenSky.Util;

namespace FrozenSky.Tests
{
    [TestClass]
    public class ResourceSourceTests
    {
        public const string TEST_CATEGORY = "FrozenSky ResourceSource";

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void GetFileExtension_StandardFile()
        {
            ResourceSource extCS = new ResourceSource("C:/Blub/Blub.cs");
            ResourceSource extC = new ResourceSource("C:/Blub/Blub.c");
            ResourceSource extNull = new ResourceSource("C:/Club/Blub");
            ResourceSource extVB = new ResourceSource("C:/Club/Blub.cs.vb");

            Assert.IsTrue(extCS.FileExtension == "cs");
            Assert.IsTrue(extC.FileExtension == "c");
            Assert.IsTrue(extNull.FileExtension == "");
            Assert.IsTrue(extVB.FileExtension == "vb");
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void GetFileExtension_WpfResourceUri()
        {
            // This is needed for the uri pack schema
            //  see: http://stackoverflow.com/questions/6005398/uriformatexception-invalid-uri-invalid-port-specified
            if (!UriParser.IsKnownScheme("pack"))
            {
                new System.Windows.Application();
            }

            ResourceSource extPNG = new Uri("/FrozenSky.Samples.Base;component/Assets/Textures/LogoTexture.png", UriKind.Relative);
            ResourceSource extJPG = new Uri("pack://application:,,,/FrozenSky.Tests;component/Resources/Textures/Background.jpg");

            Assert.IsTrue(extPNG.FileExtension == "png");
            Assert.IsTrue(extJPG.FileExtension == "jpg");
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void GetFileExtension_WinRTResourceUri()
        {
            ResourceSource extPNG = new Uri("ms-appx:///FrozenSky.Samples.Base/Assets/Textures/LogoTexture.png");

            Assert.IsTrue(extPNG.FileExtension == "png");
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void GetFileExtension_AssemblyResourceLink()
        {
            ResourceSource extPNG = new AssemblyResourceLink(this.GetType(), "DummyNamespace.DummyFile.png");
            ResourceSource extJPG = extPNG.GetForAnotherFile("Dummy.jpg");

            Assert.IsTrue(extPNG.FileExtension == "png");
            Assert.IsTrue(extJPG.FileExtension == "jpg");
        }
    }
}
