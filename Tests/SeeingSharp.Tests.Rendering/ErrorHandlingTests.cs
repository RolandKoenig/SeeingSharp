﻿#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
	Exception are projects where it is noted otherwhise.
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
using SeeingSharp.Checking;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Views;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xunit;

namespace SeeingSharp.Tests.Rendering
{
    [Collection("Rendering_SeeingSharp")]
    public class ErrorHandlingTests
    {
        public const string TEST_CATEGORY = "SeeingSharp Multimedia ErrorHandling";

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public async Task WinForms_Parent_Child_Switch()
        {
            await UnitTestHelper.InitializeWithGrahicsAsync();

            Panel hostPanel1 = null;
            Panel hostPanel2 = null;
            SeeingSharpRendererControl renderControl = null;
            int stepID = 0;
            Exception fakeUIThreadException = null;

            ObjectThread fakeUIThread = new ObjectThread("Fake-UI", 100);
            fakeUIThread.ThreadException += (sender, eArgs) =>
            {
                fakeUIThreadException = eArgs.Exception;
            };
            fakeUIThread.Starting += (sender, eArgs) =>
            {
                hostPanel1 = new System.Windows.Forms.Panel();
                hostPanel1.Size = new Size(500, 500);
                hostPanel2 = new System.Windows.Forms.Panel();
                hostPanel2.Size = new Size(500, 500);

                renderControl = new SeeingSharpRendererControl();
                renderControl.Dock = System.Windows.Forms.DockStyle.Fill;

                hostPanel1.CreateControl();
                hostPanel2.CreateControl();
                hostPanel1.Controls.Add(renderControl);
            };
            fakeUIThread.Tick += (sender, eArgs) =>
            {
                Application.DoEvents();
                stepID++;
                
                switch(stepID)
                {
                    case 2:
                        hostPanel1.Controls.Remove(renderControl);
                        break;

                    case 4:
                        hostPanel2.Controls.Add(renderControl);
                        break;

                    case 8:
                        hostPanel2.Controls.Remove(renderControl);
                        break;

                    case 10:
                        renderControl.Dispose();
                        hostPanel2.Dispose();
                        hostPanel1.Dispose();
                        break;

                    case 11:
                        fakeUIThread.Stop();
                        break;
                }
            };
            fakeUIThread.Start();

            await fakeUIThread.WaitUntilSoppedAsync();

            // Some checks after rendering
            Assert.True(GraphicsCore.Current.MainLoop.IsRunning);
            Assert.True(GraphicsCore.Current.MainLoop.RegisteredRenderLoopCount == 0);
            Assert.Null(fakeUIThreadException);
            Assert.True(renderControl.IsDisposed);
        }
    }
}