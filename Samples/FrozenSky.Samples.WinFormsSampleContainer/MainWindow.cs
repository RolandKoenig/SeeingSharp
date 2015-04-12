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

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets a collection containing all render controls.
        /// </summary>
        private IEnumerable<RenderLoop> GetAllRenderLoops()
        {
            foreach(TabPage actTabPage in m_tabControl.TabPages)
            {
                FrozenSkyRendererControl actRenderControl = actTabPage.Controls
                    .Cast<Control>()
                    .Where((actControl) => actControl is FrozenSkyRendererControl)
                    .FirstOrDefault() as FrozenSkyRendererControl;
                if (actRenderControl == null) { continue; }

                yield return actRenderControl.RenderLoop;
            }
        }

        /// <summary>
        /// Loads the engine demo for the currently selected tab control.
        /// </summary>
        private void UpdateCurrentTabControl()
        {
            // Get current render control
            TabPage actTabPage = m_tabControl.SelectedTab;
            FrozenSkyRendererControl rendererControl = actTabPage.Controls
                .Cast<Control>()
                .Where((actControl) => actControl is FrozenSkyRendererControl)
                .FirstOrDefault() as FrozenSkyRendererControl;
            if (rendererControl == null) { return; }

            // Ensure that this one is the initial call
            if (rendererControl.RenderLoop.Scene.CountObjects > 0) { return; }

            // Initialize demo
            rendererControl.RenderLoop.ApplySample(actTabPage.Tag as string);
        }

        /// <summary>
        /// Gets the currently selected render control.
        /// </summary>
        private FrozenSkyRendererControl GetCurrentRenderControl()
        {
            if (m_tabControl.TabCount <= 0) { return null; }

            // Get current render control
            TabPage actTabPage = m_tabControl.SelectedTab;
            return actTabPage.Controls
                .Cast<Control>()
                .Where((actControl) => actControl is FrozenSkyRendererControl)
                .FirstOrDefault() as FrozenSkyRendererControl;
        }

        private void ChangeRenderResolution(int width, int height)
        {
            FrozenSkyRendererControl renderControl = GetCurrentRenderControl();
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
        /// Updates the current control state.
        /// </summary>
        private void UpdateControlState()
        {
            FrozenSkyRendererControl actRenderControl = GetCurrentRenderControl();

            // Update state values
            m_lblRenderResolutionValue.Text = actRenderControl != null ?
                actRenderControl.RenderLoop.CurrentViewSize.Width + " x " + actRenderControl.RenderLoop.CurrentViewSize.Height :
                "-";
            m_lblCountObjectsValue.Text = actRenderControl != null ?
                actRenderControl.RenderLoop.VisibleObjectCount.ToString() :
                "-";

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
   
            // Default initializations
            FrozenSkyApplication.InitializeAsync(
                Assembly.GetExecutingAssembly(),
                new Assembly[]{
                    typeof(GraphicsCore).Assembly
                },
                new string[0]).Wait();
            GraphicsCore.Initialize(TargetHardware.Direct3D11, false);

            // Add all sample pages
            foreach(string actSampleName in SampleManager.GetSampleNames())
            {
                var actRenderControl = new FrozenSkyRendererControl() { Dock = DockStyle.Fill };
                //actRenderControl.Camera = new FrozenSky.Multimedia.Drawing3D.OrthographicCamera3D(100, 100);

                TabPage actTabPage = new TabPage(actSampleName);
                actTabPage.Controls.Add(actRenderControl);
                actTabPage.Tag = actSampleName;
                m_tabControl.TabPages.Add(actTabPage);
            }

            // Create main object filter initially
            m_viewboxfilter = new SceneViewboxObjectFilter();
            m_viewboxfilter.EnableYFilter = false;
            foreach (RenderLoop actRenderLoop in GetAllRenderLoops())
            {
                actRenderLoop.ManipulateFilterList += OnRenderLoopManipulateFilterList;
            }

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
                        foreach(RenderLoop actRenderLoop in GetAllRenderLoops())
                        {
                            actRenderLoop.SetRenderingDevice(actDeviceInner);
                        }
                        m_cboDevice.Text = actDeviceInner.AdapterDescription;
                    });
            }

            this.UpdateCurrentTabControl();
            this.UpdateControlState();
        }

        /// <summary>
        /// Manipulates the current filter list of the given render panel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OnRenderLoopManipulateFilterList(object sender, ManipulateFilterListArgs e)
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

        /// <summary>
        /// Initialize all tab pages when we move to them.
        /// </summary>
        private void OnTabControlSelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.DesignMode) { return; }

            this.UpdateCurrentTabControl();
            this.UpdateControlState();
        }

        private void OnCmdShowPerformanceClick(object sender, EventArgs e)
        {
            this.ShowChildForm(new PerformanceMeasureForm());

            this.UpdateControlState();
        }

        private void OnRefreshTimerTick(object sender, EventArgs e)
        {
            this.UpdateControlState();
        }

        private void OnCmdChangeResolutionClick(object sender, EventArgs e)
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

        private void OnCmdChangeResolutionToBigWindow(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private async void OnCmdCopyClick(object sender, EventArgs e)
        {
            FrozenSkyRendererControl renderControl = GetCurrentRenderControl();
            if (renderControl == null) { return; }

            using(Bitmap screenshot = await renderControl.RenderLoop.GetScreenshotGdiAsync())
            using (Bitmap rescaledScreenshot = new Bitmap(screenshot, new Size(300, 225)))
            {
                Clipboard.SetImage(rescaledScreenshot);
            }
        }
    }
}
