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
using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Tests.Rendering
{
    internal static class UnitTestHelper
    {
        public static async Task InitializeWithGrahicsAsync()
        {
            // Initialize main application singleton
            if (!SeeingSharpApplication.IsInitialized)
            {
                await SeeingSharpApplication.InitializeAsync(
                    Assembly.GetExecutingAssembly(),
                    new Assembly[]{ typeof(GraphicsCore).Assembly },
                    new string[0]);
            }

            // Initialize the graphics engine
            if (!GraphicsCore.IsInitialized) 
            { 
                GraphicsCore.Initialize(TargetHardware.Direct3D11, false);
                GraphicsCore.Current.SetDefaultDeviceToSoftware();
                GraphicsCore.Current.DefaultDevice.ForceDetailLevel(DetailLevel.High);
            }

            Assert.True(GraphicsCore.IsInitialized, "GraphicsCore could not be initialized!");
        }
    }
}
