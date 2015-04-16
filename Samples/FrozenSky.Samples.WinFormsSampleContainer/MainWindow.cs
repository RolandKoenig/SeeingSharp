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
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using FrozenSky;
using FrozenSky.Util;
using FrozenSky.Checking;
using FrozenSky.Samples.Base;
using FrozenSky.Multimedia.Views;
using FrozenSky.Multimedia.Core;
using FrozenSky.Infrastructure;
using FrozenSky.Samples.WinFormsSampleContainer.Views;

namespace FrozenSky.Samples.WinFormsSampleContainer
{
    public partial class MainWindow : Form
    {
        private SceneViewboxObjectFilter m_viewboxfilter;

        private bool m_isChangingSample;
        private SampleInfoAttribute m_actSampleInfo;
        private SampleBase m_actSample;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Changes the render resolution to given width and height.
        /// </summary>
        /// <param name="width">The width in pixels.</param>
        /// <param name="height">The height in pixels.</param>
        private void ChangeRenderResolution(int width, int height)
        {
            FrozenSkyRendererControl renderControl = m_ctrlRenderer;
            if (renderControl == null) { return; }

            Size2 currentViewSize = renderControl.RenderLoop.CurrentViewSize;
            Size2 currentWindowSize = new Size2(this.Width, this.Height);
            Size2 difference = new Size2(
                currentWindowSize.Width - currentViewSize.Width,
                currentWindowSize.Height - currentViewSize.Height);
            Size2 newWindowSize = new Size2(width + difference.Width, height + difference.Height);

            this.WindowState = FormWindowState.Normal;
            this.Width = newWindowSize.Width;
            this.Height = newWindowSize.Height;
        }

        /// <summary>
        /// Applies the given sample.
        /// </summary>
        /// <param name="sampleInfo">The sample to be applied.</param>
        private async void ApplySample(SampleInfoAttribute sampleInfo)
        {
            m_isChangingSample.EnsureFalse("m_isChangingSample");

            m_isChangingSample = true;
            try
            {
                this.UpdateControlState();

                if (m_actSampleInfo == sampleInfo) { return; }

                // Clear previous sample 
                if (m_actSample != null)
                {
                    m_actSample.SetClosed();

                    await m_ctrlRenderer.RenderLoop.Scene.ManipulateSceneAsync((manipulator) =>
                    {
                        manipulator.Clear(true);
                    });
                }

                // Reset members
                m_actSample = null;
                m_actSampleInfo = null;

                // Apply new sample
                if (sampleInfo != null)
                {
                    SampleBase sampleObject = SampleFactory.Current.CreateSample(sampleInfo);
                    await sampleObject.OnStartupAsync(m_ctrlRenderer.RenderLoop);

                    m_actSample = sampleObject;
                    m_actSampleInfo = sampleInfo;
                }
            }
            finally
            {
                m_isChangingSample = false;
            }

            this.UpdateControlState();
        }

        /// <summary>
        /// Updates the current control state.
        /// </summary>
        private void UpdateControlState()
        {
            FrozenSkyRendererControl actRenderControl = m_ctrlRenderer;

            // Update state values
            m_lblRenderResolutionValue.Text = actRenderControl != null ?
                actRenderControl.RenderLoop.CurrentViewSize.Width + " x " + actRenderControl.RenderLoop.CurrentViewSize.Height :
                "-";
            m_lblCountObjectsValue.Text = actRenderControl != null ?
                actRenderControl.RenderLoop.VisibleObjectCount.ToString() :
                "-";

            // Update sample tab
            m_tabControlSamples.Enabled = !m_isChangingSample;

            // Update enabled states
            m_cmdShowPerformance.Enabled = actRenderControl != null;
        }

        /// <summary>
        /// Default load event.. trigger first initialization here
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.DesignMode) { return; }

            // Just a dummy call to trigger initialization
            Assembly test = System.Windows.Application.ResourceAssembly;

            // Add all sample pages
            Dictionary<string, ListView> generatedTabs = new Dictionary<string, ListView>();
            foreach(var actSampleInfo in SampleFactory.Current.GetSampleInfos())
            {
                // Find the ListView of the current category
                ListView actListView = null;
                if(!generatedTabs.ContainsKey(actSampleInfo.Category))
                {
                    TabPage actTabPage = new TabPage(actSampleInfo.Category);
                    m_tabControlSamples.TabPages.Add(actTabPage);

                    actListView = new ListView();
                    actListView.Dock = DockStyle.Fill;
                    actListView.Activation = ItemActivation.OneClick;
                    actListView.ItemActivate += OnListView_ItemActivate;
                    actListView.MultiSelect = false;
                    actTabPage.Controls.Add(actListView);

                    generatedTabs.Add(actSampleInfo.Category, actListView);
                }
                else
                {
                    actListView = generatedTabs[actSampleInfo.Category];
                }

                // Generate the new list entry for the current sample
                ListViewItem newListItem = new ListViewItem();
                newListItem.Text = actSampleInfo.Name;
                newListItem.Tag = actSampleInfo;
                actListView.Items.Add(newListItem);
            }

            // Create main object filter initially
            m_viewboxfilter = new SceneViewboxObjectFilter();
            m_viewboxfilter.EnableYFilter = false;
            m_ctrlRenderer.RenderLoop.ManipulateFilterList += OnRenderLoop_ManipulateFilterList;

            // Handle device combobox
            m_cboDevice.Text = GraphicsCore.Current.DefaultDevice.AdapterDescription;
            foreach(EngineDevice actDevice in GraphicsCore.Current.LoadedDevices)
            {
                EngineDevice actDeviceInner = actDevice;
                m_cboDevice.DropDownItems.Add(
                    actDevice.AdapterDescription,
                    null,
                    (sender, eArgs) =>
                    {
                        m_ctrlRenderer.RenderLoop.SetRenderingDevice(actDeviceInner);
                        m_cboDevice.Text = actDeviceInner.AdapterDescription;
                    });
            }

            this.UpdateControlState();
        }

        /// <summary>
        /// Called when the user activates a link in the top ListView object.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnListView_ItemActivate(object sender, EventArgs e)
        {
            ListView actListView = sender as ListView;
            actListView.EnsureNotNull("actListView");
            actListView.FocusedItem.EnsureNotNull("actListView.FocusedItem");

            SampleInfoAttribute sampleInfo = actListView.FocusedItem.Tag as SampleInfoAttribute;
            sampleInfo.EnsureNotNull("sampleInfo");

            this.ApplySample(sampleInfo);
        }

        /// <summary>
        /// Manipulates the current filter list of the given render panel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OnRenderLoop_ManipulateFilterList(object sender, ManipulateFilterListArgs e)
        {
            if (!e.FilterList.Contains(m_viewboxfilter))
            {
                e.FilterList.Add(m_viewboxfilter);
            }
        }

        /// <summary>
        /// Called when the control changed its size.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.UpdateControlState();
        }

        private void OnCmdShowPerformance_Click(object sender, EventArgs e)
        {
            this.ShowChildForm(new PerformanceMeasureForm());

            this.UpdateControlState();
        }

        private void OnRefreshTimer_Tick(object sender, EventArgs e)
        {
            this.UpdateControlState();
        }

        private void OnCmdChangeResolution_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem callerItem = sender as ToolStripMenuItem;
            if (callerItem == null) { return; }

            string parameter = callerItem.Tag as string;
            if (string.IsNullOrWhiteSpace(parameter)) { return; }
            if (!parameter.Contains('x')) { return; }

            string[] parameterParts = parameter.Split('x');
            if (parameterParts.Length != 2) { return; }

            int width = 0;
            int height = 0;
            if (!Int32.TryParse(parameterParts[0], out width)) { return; }
            if (!Int32.TryParse(parameterParts[1], out height)) { return; }

            ChangeRenderResolution(width, height);
        }

        private void OnCmdChangeResolutionToBigWindow_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private async void OnCmdCopy_Click(object sender, EventArgs e)
        {
            FrozenSkyRendererControl renderControl = m_ctrlRenderer;
            if (renderControl == null) { return; }

            using(Bitmap screenshot = await renderControl.RenderLoop.GetScreenshotGdiAsync())
            using(Bitmap rescaledScreenshot = new Bitmap(screenshot, new Size(300, 225)))
            {
                Clipboard.SetImage(rescaledScreenshot);
            }
        }
    }
}
