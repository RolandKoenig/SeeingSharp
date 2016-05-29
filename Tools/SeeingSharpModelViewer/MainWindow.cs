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
using SeeingSharp;
using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;
using SeeingSharp.View;
using SeeingSharpModelViewer.Data;
using SeeingSharpModelViewer.View;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Updates the state of the dialog.
        /// </summary>
        private void UpdateDialogState()
        {
            // Information about currently opened file
            DesktopFileSystemResourceLink fileResourceLink = m_sceneManager.CurrentFile as DesktopFileSystemResourceLink;
            string fileName = string.Empty;
            if (fileResourceLink != null)
            {
                fileName = fileResourceLink.FileName;
                m_lblCurrentFile.Text = fileName;
            }
            else
            {
                m_lblCurrentFile.Text = "-";
            }

            // Title
            string titleString = $"{SeeingSharpApplication.Current.ProductName} - {SeeingSharpApplication.Current.ProductVersion}";
            if(!string.IsNullOrEmpty(fileName))
            {
                titleString += $" ({fileName})";
            }
            this.Text = titleString;

            // Handle import options 
            m_propertiesImporter.SelectedObject = m_sceneManager.CurrentImportOptions;
            m_cmdReloadObject.Enabled = m_sceneManager.CurrentImportOptions != null;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!SeeingSharpApplication.IsInitialized) { return; }

            // Create main ViewModel
            m_sceneManager = new SceneManager(m_panGraphics.RenderLoop);
            m_sceneManager.PropertyChanged += OnSceneManager_PropertyChanged;

            // Update Input/Export formats for file dialog
            m_dlgOpenFile.Filter =
                GraphicsCore.Current.ImportersAndExporters.GetOpenFileDialogFilter();

            // Handle device combobox
            m_cboDevice.Text = GraphicsCore.Current.DefaultDevice.AdapterDescription;
            foreach (EngineDevice actDevice in GraphicsCore.Current.LoadedDevices)
            {
                EngineDevice actDeviceInner = actDevice;
                m_cboDevice.DropDownItems.Add(
                    actDevice.AdapterDescription,
                    null,
                    (sender, eArgs) =>
                    {
                        m_panGraphics.RenderLoop.SetRenderingDevice(actDeviceInner);
                        m_cboDevice.Text = actDeviceInner.AdapterDescription;
                    });
            }

            // Peform common updates finally
            this.UpdateDialogState();
        }

        private void OnSceneManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(SceneManager.CurrentImportOptions):
                case nameof(SceneManager.CurrentFile):
                    this.UpdateDialogState();
                    break;
            }
        }

        private async void OnCmdOpen_Click(object sender, EventArgs e)
        {
            if (m_dlgOpenFile.ShowDialog(this) == DialogResult.OK)
            {
                await m_sceneManager.ImportNewFileAsync(m_dlgOpenFile.FileName);
            }

            this.UpdateDialogState();
        }

        private async void OnCmdClose_Click(object sender, EventArgs e)
        {
            await m_sceneManager.CloseAsync();

            this.UpdateDialogState();
        }

        private async void OnCmdReloadObject_Click(object sender, EventArgs e)
        {
            await m_sceneManager.ReloadCurrentFileAsync();

            this.UpdateDialogState();
        }
    }
}
