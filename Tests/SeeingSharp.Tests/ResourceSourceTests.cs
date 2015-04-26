#region License information (SeeingSharp and all based games/applications)
/*
    SeeingSharp and all games/applications based on it (more info at http://www.rolandk.de/wp)
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

using System;
using SeeingSharp.Util;
using Xunit;

namespace SeeingSharp.Tests
{
    public class ResourceLinkTests
    {
        public const string TEST_CATEGORY = "SeeingSharp ResourceLink";

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void GetFileExtension_StandardFile()
        {
            ResourceLink extCS = new DesktopFileSystemResourceLink("C:/Blub/Blub.cs");
            ResourceLink extC = new DesktopFileSystemResourceLink("C:/Blub/Blub.c");
            ResourceLink extNull = new DesktopFileSystemResourceLink("C:/Club/Blub");
            ResourceLink extVB = new DesktopFileSystemResourceLink("C:/Club/Blub.cs.vb");

            Assert.True(extCS.FileExtension == "cs");
            Assert.True(extC.FileExtension == "c");
            Assert.True(extNull.FileExtension == "");
            Assert.True(extVB.FileExtension == "vb");
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void GetFileExtension_WpfResourceUri()
        {
            // This is needed for the uri pack schema
            //  see: http://stackoverflow.com/questions/6005398/uriformatexception-invalid-uri-invalid-port-specified
            if (!UriParser.IsKnownScheme("pack"))
            {
                new System.Windows.Application();
            }

            ResourceLink extPNG = new Uri("/SeeingSharp.Samples.Base;component/Assets/Textures/LogoTexture.png", UriKind.Relative);
            ResourceLink extJPG = new Uri("pack://application:,,,/SeeingSharp.Tests;component/Resources/Textures/Background.jpg");

            Assert.True(extPNG.FileExtension == "png");
            Assert.True(extJPG.FileExtension == "jpg");
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void GetFileExtension_WinRTResourceUri()
        {
            ResourceLink extPNG = new Uri("ms-appx:///SeeingSharp.Samples.Base/Assets/Textures/LogoTexture.png");

            Assert.True(extPNG.FileExtension == "png");
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void GetFileExtension_AssemblyResourceLink()
        {
            ResourceLink extPNG = new AssemblyResourceLink(this.GetType(), "DummyNamespace.DummyFile.png");
            ResourceLink extJPG = extPNG.GetForAnotherFile("Dummy.jpg");

            Assert.True(extPNG.FileExtension == "png");
            Assert.True(extJPG.FileExtension == "jpg");
        }
    }
}
