using SeeingSharp.Multimedia.DrawingVideo;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RKWebcamCapture
{
    public partial class MainWindow : Form
    {
        private CaptureDeviceChooser m_deviceChooser;
        private FrameByFrameVideoReader m_videoReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Common update method which refresh all UI states.
        /// </summary>
        private void UpdateDialogState()
        {
            CaptureDeviceInfo selectedDevice = m_cboDevice.SelectedItem as CaptureDeviceInfo;

            m_cmdCapture.Enabled = selectedDevice != null;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Initialize device selection
            m_deviceChooser = new CaptureDeviceChooser();
            m_cboDevice.Items.Clear();
            m_cboDevice.Items.AddRange(m_deviceChooser.DeviceInfos.ToArray());
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            CommonTools.SafeDispose(ref m_videoReader);
            CommonTools.SafeDispose(ref m_deviceChooser);
        }

        private void OnCmdCapture_Click(object sender, EventArgs e)
        {

            UpdateDialogState();
        }

        private void OnCboDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDialogState();
        }
    }
}
