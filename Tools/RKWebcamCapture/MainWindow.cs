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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.DrawingVideo;
using SeeingSharp.Multimedia.Views;
using SeeingSharp.Util;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace RKWebcamCapture
{
    public partial class MainWindow : Form
    {
        private CaptureDeviceChooser m_deviceChooser;
        private CaptureDeviceInfo m_currentlyPlayingDevice;

        private QRCodeReader m_qrReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            m_qrReader = new QRCodeReader();
        }

        /// <summary>
        /// Common update method which refresh all UI states.
        /// </summary>
        private void UpdateDialogState()
        {
            CaptureDeviceInfo selectedDevice = m_cboDevice.SelectedItem as CaptureDeviceInfo;
            m_cmdPlay.Enabled = selectedDevice != null;
            m_cboDevice.Enabled = m_cboDevice.Items.Count > 0;

            m_cmdStop.Enabled = m_currentlyPlayingDevice != null;

            m_cmdReadQRCode.Enabled =
                (selectedDevice != null) &&
                (m_currentlyPlayingDevice == null);
            m_cmdSaveImage.Enabled =
                (selectedDevice != null) &&
                (m_currentlyPlayingDevice == null);

            m_lblCurrentDeviceLabel.Text = m_currentlyPlayingDevice != null ? selectedDevice.ToString() : "<None>";
        }

        /// <summary>
        /// Called when the window opens.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Initialize device selection
            m_deviceChooser = new CaptureDeviceChooser();

            m_cboDevice.Items.Clear();
            m_cboDevice.Items.AddRange(m_deviceChooser.DeviceInfos.ToArray());
            if (m_cboDevice.Items.Count > 0)
            {
                m_cboDevice.SelectedIndex = 0;
            }

            // Initial update call
            UpdateDialogState();
        }

        /// <summary>
        /// Called when the window closes.
        /// </summary>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            CommonTools.SafeDispose(ref m_deviceChooser);
        }

        /// <summary>
        /// Called when user wants to display a camera stream.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void OnCmdPlay_Click(object sender, EventArgs e)
        {
            // Close previous stream first
            if (m_mediaPlayer.State == MediaPlayerState.Playing)
            {
                await m_mediaPlayer.CloseVideoAsync();
            }

            // Open the new stream
            var captureDevice = m_cboDevice.SelectedItem as CaptureDeviceInfo;
            if (captureDevice != null)
            {
                await m_mediaPlayer.ShowCaptureDeviceAsync(captureDevice);
                m_currentlyPlayingDevice = captureDevice;
            }

            UpdateDialogState();
        }

        private void OnCboDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDialogState();
        }

        private async void OnCmdStop_Click(object sender, EventArgs e)
        {
            // Close previous stream first
            if (m_mediaPlayer.State == MediaPlayerState.Playing)
            {
                await m_mediaPlayer.CloseVideoAsync();
            }
            m_currentlyPlayingDevice = null;

            m_panVideoArea.Invalidate(true);

            UpdateDialogState();
        }

        private void OnCmdReadQRCode_Click(object sender, EventArgs e)
        {
            CaptureDeviceInfo selectedDevice = m_cboDevice.SelectedItem as CaptureDeviceInfo;
            if (selectedDevice == null) { return; }

            using (FrameByFrameVideoReader frameByFrameReader = new FrameByFrameVideoReader(selectedDevice))
            using (MemoryMappedTexture32bpp capturedFrame1 = frameByFrameReader.ReadFrame())
            using (MemoryMappedTexture32bpp capturedFrame2 = frameByFrameReader.ReadFrame())
            {
                capturedFrame2.SetAllAlphaValuesToOne_ARGB();

                // Change current background image
                Bitmap newBGImage = GraphicsHelper.LoadBitmapFromMappedTexture(capturedFrame2);
                Bitmap prevBGImage = m_panVideoArea.BackgroundImage as Bitmap;
                m_panVideoArea.BackgroundImage = newBGImage;
                if (prevBGImage != null) { prevBGImage.Dispose(); }

                // Load binary data to ZXing format
                RGBLuminanceSource luminanceSource = new RGBLuminanceSource(
                    capturedFrame2.ToArray(), capturedFrame2.Width, capturedFrame2.Height,
                    RGBLuminanceSource.BitmapFormat.BGRA32);
                BinaryBitmap binBitmap = new BinaryBitmap(new HybridBinarizer(luminanceSource));

                // Perform QR-Code reading
                Result result = m_qrReader.decode(binBitmap);

                // Store the result
                if ((result == null) ||
                    (string.IsNullOrEmpty(result.Text)))
                {
                    MessageBox.Show(this, "Nothing found..", "QR-Code Reader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(this, "Content: " + result.Text, "QR-Code Reader", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void OnCmdSaveImage_Click(object sender, EventArgs e)
        {
            CaptureDeviceInfo selectedDevice = m_cboDevice.SelectedItem as CaptureDeviceInfo;
            if (selectedDevice == null) { return; }

            using (FrameByFrameVideoReader frameByFrameReader = new FrameByFrameVideoReader(selectedDevice))
            using (MemoryMappedTexture32bpp capturedFrame1 = frameByFrameReader.ReadFrame())
            using (MemoryMappedTexture32bpp capturedFrame2 = frameByFrameReader.ReadFrame())
            {
                capturedFrame2.SetAllAlphaValuesToOne_ARGB();
                using (Bitmap bitmap = GraphicsHelper.LoadBitmapFromMappedTexture(capturedFrame2))
                {
                    if (m_dlgSaveImageFile.ShowDialog() == DialogResult.OK)
                    {
                        bitmap.Save(m_dlgSaveImageFile.FileName);
                    }
                }
            }
        }
    }
}