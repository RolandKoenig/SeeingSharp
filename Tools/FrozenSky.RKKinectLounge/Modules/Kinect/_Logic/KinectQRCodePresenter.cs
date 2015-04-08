using FrozenSky.Util;
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using ZXing;
using ZXing.QrCode;
using ZXing.Common;

namespace FrozenSky.RKKinectLounge.Modules.Kinect
{
    public class KinectQRCodePresenter : IDisposable
    {
        private const string RESULT_NOTHING = "-Nothing-";

        private ImageDataFlowHelper m_dataFlowQR;

        private byte[] m_arReaderRawBitmap;
        private QRCodeReader m_qrReader;
        private Task m_qrReaderTask;

        private List<MessageSubscription> m_messageSubscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectRawStreamPresenter"/> class.
        /// </summary>
        public KinectQRCodePresenter()
        {
            m_dataFlowQR = new ImageDataFlowHelper(RESULT_NOTHING);

            m_arReaderRawBitmap = new byte[0];
            m_qrReader = new QRCodeReader();

            // Get the Messenger of the KinectThread
            FrozenSkyMessenger kinectMessenger =
                FrozenSkyMessenger.GetByName(Constants.KINECT_THREAD_NAME);

            // Subscribe to messages from KinectHandler
            m_messageSubscriptions = new List<MessageSubscription>();
            m_messageSubscriptions.Add(
                kinectMessenger.Subscribe<MessageColorFrameArrived>(OnMessage_WpfColorFrameArrived));
        }

        /// <summary>
        /// Führt anwendungsspezifische Aufgaben aus, die mit dem Freigeben, Zurückgeben oder Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
        /// </summary>
        public void Dispose()
        {
            m_messageSubscriptions.ForEach((actSubscription) => actSubscription.Unsubscribe());
            m_messageSubscriptions.Clear();

            CommonTools.SafeDispose(ref m_dataFlowQR);
        }

        /// <summary>
        /// Called when a new color frame is received.
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnMessage_WpfColorFrameArrived(MessageColorFrameArrived message)
        {
            if (m_dataFlowQR.Scene.CountViews <= 0) { return; }

            using (ColorFrame colorFrame = message.ColorFrameArgs.FrameReference.AcquireFrame())
            {
                if (colorFrame == null) { return; }

                using (KinectBuffer colorFrameBuffer = colorFrame.LockRawImageBuffer())
                {
                    lock (m_dataFlowQR.SyncBufferLock)
                    {
                        int frameWidth = colorFrame.FrameDescription.Width;
                        int frameHeight = colorFrame.FrameDescription.Height;

                        // Resize buffers if needed
                        m_dataFlowQR.ResizeSyncBufferIfNeeded(new Size2(frameWidth, frameHeight));

                        // Do first check whehter we have to load the buffer for the QR-Scanner
                        //  => If scanning is running currently, then wo should not copy into this buffer!
                        bool loadQRBuffer = m_qrReaderTask == null;

                        // Prepare buffer for qr reader
                        int bufferSize = frameWidth * frameHeight * 4;
                        if((loadQRBuffer) &&
                           (m_arReaderRawBitmap.Length < bufferSize))
                        {
                            m_arReaderRawBitmap = new byte[frameWidth * frameHeight * 4];
                        }

                        // Store pixel data in SyncBuffer and qr reader buffer
                        if (colorFrame.RawColorImageFormat == ColorImageFormat.Bgra)
                        {
                            colorFrame.CopyRawFrameDataToIntPtr(
                                m_dataFlowQR.SyncBuffer.Pointer,
                                m_dataFlowQR.SyncBuffer.SizeInBytes);
                            if (loadQRBuffer) { colorFrame.CopyRawFrameDataToArray(m_arReaderRawBitmap); }
                        }
                        else
                        {
                            colorFrame.CopyConvertedFrameDataToIntPtr(
                                m_dataFlowQR.SyncBuffer.Pointer,
                                m_dataFlowQR.SyncBuffer.SizeInBytes,
                                ColorImageFormat.Bgra);
                            if (loadQRBuffer) { colorFrame.CopyConvertedFrameDataToArray(m_arReaderRawBitmap, ColorImageFormat.Bgra); }
                        }

                        // Try to read the QR code
                        if (loadQRBuffer)
                        {
                            BeginScanQRCode(frameWidth, frameHeight);
                        }
                        
                    }
                    m_dataFlowQR.SyncBufferChanged = true;
                }
            }
        }

        /// <summary>
        /// Helpermethod: Starts to scan the QR code.
        /// This task gets synchronized by local m_qrReaderTask variable.
        /// </summary>
        /// <param name="frameWidth">The current width of the frame (in pixel).</param>
        /// <param name="frameHeight">The current height of the frame (in pixel).</param>
        private void BeginScanQRCode(int frameWidth, int frameHeight)
        {
            m_qrReaderTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    // Load binary data to ZXing format
                    RGBLuminanceSource luminanceSource = new RGBLuminanceSource(
                        m_arReaderRawBitmap, frameWidth, frameHeight,
                        RGBLuminanceSource.BitmapFormat.BGRA32);
                    BinaryBitmap binBitmap = new BinaryBitmap(new HybridBinarizer(luminanceSource));
                    
                    // Perform QR-Code reading
                    Result result = m_qrReader.decode(binBitmap);

                    // Store the result
                    if ((result == null) ||
                        (string.IsNullOrEmpty(result.Text)))
                    {
                        m_dataFlowQR.QRText = RESULT_NOTHING;
                    }
                    else
                    {
                        m_dataFlowQR.QRText = result.Text;
                    }
                }
                catch (Exception ex)
                {
                    m_dataFlowQR.QRText = "Error: " + ex.Message;
                }
                finally
                {
                    m_qrReaderTask = null;
                }
            });
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************        
        /// <summary>
        /// Helper object for transfering image data from KinectThread to RenderThread.
        /// </summary>
        private class ImageDataFlowHelper : IDisposable
        {
            // Resources for title rendering
            public string QRText;
            public TextFormatResource TextFormat;
            public BrushResource TextBackground;
            public BrushResource TextForeground;

            public Scene Scene;
            public WriteableBitmapResource Bitmap;
            public Size2 BitmapSize;

            public MemoryMappedTexture32bpp SyncBuffer;
            public Size2 SyncBufferSize;
            public object SyncBufferLock;
            public bool SyncBufferChanged;

            public bool IsDisposed;

            /// <summary>
            /// Initializes a new instance of the <see cref="ImageDataFlowHelper"/> class.
            /// </summary>
            public ImageDataFlowHelper(string qrText)
            {
                this.QRText = qrText;
                this.TextFormat = new TextFormatResource("Arial", 18f, FontWeight.Bold);
                this.TextFormat.TextAlignment = TextAlignment.Center;
                this.TextFormat.ParagraphAlignment = ParagraphAlignment.Center;
                this.TextBackground = new SolidBrushResource(Color4.LightGray.ChangeAlphaTo(0.7f));
                this.TextForeground = new SolidBrushResource(Color4.DarkBlue);

                // Initialize scene(s)
                this.Scene = new Scene();
                this.Scene.ManipulateSceneAsync((manipulator) =>
                {
                    manipulator.AddDrawingLayer(OnDraw);
                }).FireAndForget();

                // Initialize members for Direct2D rendering
                this.Bitmap = null;
                this.BitmapSize = new Size2(0, 0);

                // Initialize members for synchronization
                this.SyncBufferSize = new Size2(0, 0);
                this.SyncBuffer = null;
                this.SyncBufferLock = new object();
                this.SyncBufferChanged = false;
            }

            /// <summary>
            /// Resizes the sync buffer when the given size is different to the current one.
            /// </summary>
            /// <param name="newSize">The new size to be set.</param>
            public void ResizeSyncBufferIfNeeded(Size2 newSize)
            {
                if (IsDisposed) { throw new ObjectDisposedException("ImageDataFlowHelper"); }

                if ((newSize != this.SyncBufferSize) ||
                   (this.SyncBuffer == null))
                {
                    lock (this.SyncBufferLock)
                    {
                        CommonTools.SafeDispose(ref this.SyncBuffer);
                        this.SyncBuffer = new MemoryMappedTexture32bpp(
                            new Size2(newSize.Width, newSize.Height));
                        this.SyncBufferSize = newSize;
                    }
                }
            }

            /// <summary>
            /// Called when we've to draw the image.
            /// </summary>
            /// <param name="graphics">The graphics.</param>
            private void OnDraw(Graphics2D graphics)
            {
                if (IsDisposed) { throw new ObjectDisposedException("ImageDataFlowHelper"); }

                if (this.SyncBuffer == null) { return; }

                // Change bitmap contents if needed
                if (this.SyncBufferChanged)
                {
                    lock (this.SyncBufferLock)
                    {
                        // Recreate bitmap on need
                        if ((this.BitmapSize != this.SyncBufferSize) ||
                           (this.Bitmap == null))
                        {
                            CommonTools.SafeDispose(ref this.Bitmap);
                            this.Bitmap = new WriteableBitmapResource(
                                new Size2(this.SyncBufferSize.Width, this.SyncBufferSize.Height),
                                BitmapFormat.Bgra, AlphaMode.Ignore);
                            this.BitmapSize = this.SyncBufferSize;
                        }

                        // Write data from SyncBuffer to bitmap
                        this.Bitmap.SetBitmapContent(
                            graphics,
                            this.SyncBuffer.Pointer,
                            this.SyncBuffer.Pitch);
                    }
                }

                // Draw the bitmap on the screen
                if ((this.Bitmap != null) &&
                    (this.BitmapSize.Width > 0) &&
                    (this.BitmapSize.Height > 0))
                {
                    graphics.Clear(Color4.Transparent);

                    // Draw the current contents of the stream
                    RectangleF viewBounds = new RectangleF(
                        0f, 0f,
                        graphics.ScreenSize.Width, graphics.ScreenSize.Height);
                    graphics.DrawBitmap(this.Bitmap, viewBounds);

                    // Draw the text from the QR code
                    RectangleF titleBounds = new RectangleF(
                        viewBounds.Width / 2f - 200f,
                        10f,
                        400f, 100f);
                    graphics.FillRoundedRectangle(titleBounds, 5f, 5f, this.TextBackground);
                    graphics.DrawText(
                        this.QRText, this.TextFormat,
                        titleBounds,
                        this.TextForeground);
                }
            }

            /// <summary>
            /// Führt anwendungsspezifische Aufgaben aus, die mit dem Freigeben, Zurückgeben oder Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
            /// </summary>
            public void Dispose()
            {
                this.IsDisposed = true;

                // Dispose the Bitmap and the native SyncBuffer.
                CommonTools.SafeDispose(ref this.SyncBuffer);
                CommonTools.SafeDispose(ref this.Bitmap);

                // Dispose graphics resources
                CommonTools.SafeDispose(ref this.TextFormat);
                CommonTools.SafeDispose(ref this.TextBackground);
                CommonTools.SafeDispose(ref this.TextForeground);

                // The scene obejct disposes all resource automatically
                this.Scene = null;

                this.SyncBufferLock = null;
            }
        }

        /// <summary>
        /// Gets the scene to which the QR-Result is rendered.
        /// </summary>
        public Scene Scene { get { return m_dataFlowQR.Scene; } }
    }
}
