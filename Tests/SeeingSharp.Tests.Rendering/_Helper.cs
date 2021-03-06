﻿#region License information (SeeingSharp and all based games/applications)
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
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Util;

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
                GraphicsCore.Initialize();
                GraphicsCore.Current.SetDefaultDeviceToSoftware();
                GraphicsCore.Current.DefaultDevice.ForceDetailLevel(DetailLevel.High);
            }

            Assert.True(GraphicsCore.IsInitialized, "GraphicsCore could not be initialized!");

            
        }

        public static IDisposable FailTestOnInternalExceptions()
        {
            Exception internalEx = null;
            InternalExceptionLocation location = InternalExceptionLocation.DisposeGraphicsObject;

            EventHandler<InternalCatchedExceptionEventArgs> eventHandler = (object sender, InternalCatchedExceptionEventArgs e) =>
            {
                if(internalEx == null)
                {
                    internalEx = e.Exception;
                    location = e.Location;
                }
            };

            GraphicsCore.InternalCachedException += eventHandler;
            return new DummyDisposable(() =>
            {
                GraphicsCore.InternalCachedException -= eventHandler;

                Assert.True(internalEx == null, $"Internal exception at {location}: {internalEx}");
            });
        }

        private static void OnGraphicsCore_InternalCachedException(object sender, InternalCatchedExceptionEventArgs e)
        {
           
        }
    }
}
