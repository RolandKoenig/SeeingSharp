#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# Tools. More info at 
     - https://github.com/RolandKoenig/SeeingSharp/tree/master/Tools (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
	This program is distributed under the terms of the Microsoft Public License (Ms-PL)-
	More info at https://msdn.microsoft.com/en-us/library/ff647676.aspx
*/
#endregion
using SeeingSharp;
using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;
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
        private Task m_loadingTask;

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

            // Set enables / disables states
            m_propertiesImporter.Enabled = m_loadingTask == null;
            m_cmdReloadObject.Enabled = (m_sceneManager.CurrentImportOptions != null) && (m_loadingTask == null);
            m_cmdOpen.Enabled = m_loadingTask == null;
            m_cmdClose.Enabled = m_loadingTask == null;
            m_lblProgress.Visible = m_loadingTask != null;
            m_barProgress.Visible = m_loadingTask != null;
        }

        private void HandleLoadingTask(Task loadingTask)
        {
            m_loadingTask = loadingTask;
            m_loadingTask.ContinueWith(
                (lastTask) =>
                {
                    m_loadingTask = null;
                    this.UpdateDialogState();
                },
                TaskScheduler.FromCurrentSynchronizationContext());
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

        private void OnCmdOpen_Click(object sender, EventArgs e)
        {
            if(m_loadingTask != null) { return; }

            if (m_dlgOpenFile.ShowDialog(this) == DialogResult.OK)
            {
                this.HandleLoadingTask(m_sceneManager.ImportNewFileAsync(m_dlgOpenFile.FileName));
            }

            this.UpdateDialogState();
        }

        private void OnCmdClose_Click(object sender, EventArgs e)
        {
            this.HandleLoadingTask(m_sceneManager.CloseAsync());

            this.UpdateDialogState();
        }

        private void OnCmdReloadObject_Click(object sender, EventArgs e)
        {
            this.HandleLoadingTask(m_sceneManager.ReloadCurrentFileAsync());

            this.UpdateDialogState();
        }

        private void OnChkWireFrame_Click(object sender, EventArgs e)
        {
            m_panGraphics.RenderLoop.ViewConfiguration.WireframeEnabled =
                m_toolSeparator_03.Checked;

            this.UpdateDialogState();
        }

        private async void OnCmdCopyScreenshot_Click(object sender, EventArgs e)
        {
            using (Bitmap bitmap = await m_panGraphics.RenderLoop.GetScreenshotGdiAsync())
            {
                Clipboard.SetImage(bitmap);
            }
        }

        private async void OnChkShowGround_CheckedChanged(object sender, EventArgs e)
        {
            await m_sceneManager.SetBackgroundVisibility(m_chkShowGround.Checked);
        }
    }
}
