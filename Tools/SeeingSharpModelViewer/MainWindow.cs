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
using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.View;
using SeeingSharpModelViewer.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeeingSharpModelViewer
{
    public partial class MainWindow : SeeingSharpForm
    {
        private SceneManager m_sceneManager;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (SeeingSharpApplication.IsInitialized)
            {
                m_sceneManager = new SceneManager(m_panGraphics.Scene, m_panGraphics.Camera);

                this.Text = $"{SeeingSharpApplication.Current.ProductName} - {SeeingSharpApplication.Current.ProductVersion}";

                m_dlgOpenFile.Filter =
                    GraphicsCore.Current.ImportersAndExporters.GetOpenFileDialogFilter();
            }
        }

        private async void OnCmdOpen_Click(object sender, EventArgs e)
        {
            if(m_dlgOpenFile.ShowDialog(this) == DialogResult.OK)
            {
                await m_sceneManager.ImportFileAsync(m_dlgOpenFile.FileName);
            }
        }
    }
}
