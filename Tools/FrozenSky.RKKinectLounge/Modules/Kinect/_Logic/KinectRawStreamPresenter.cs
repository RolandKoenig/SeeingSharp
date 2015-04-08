using FrozenSky.Util;
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace FrozenSky.RKKinectLounge.Modules.Kinect
{
    public class KinectRawStreamPresenter : IDisposable
    {
        private ImageDataFlowHelper m_dataFlowColor;
        private ImageDataFlowHelper m_dataFlowDepth;
        private ImageDataFlowHelper m_dataFlowInfrared;
        private ImageDataFlowHelper m_dataFlowLongExposureInfrared;
        private ImageDataFlowHelper m_dataFlowBodyIndex;

        private List<MessageSubscription> m_messageSubscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectRawStreamPresenter"/> class.
        /// </summary>
        public KinectRawStreamPresenter()
        {
            m_dataFlowColor = new ImageDataFlowHelper("Color");
            m_dataFlowDepth = new ImageDataFlowHelper("Depth");
            m_dataFlowInfrared = new ImageDataFlowHelper("Infrared");
            m_dataFlowLongExposureInfrared = new ImageDataFlowHelper("L. Exp. Infrared");
            m_dataFlowBodyIndex = new ImageDataFlowHelper("Body Index");

            // Get the MessageHandler of the KinectThread
            FrozenSkyMessageHandler kinectMessageHandler =
                FrozenSkyMessageHandler.GetByName(Constants.KINECT_THREAD_NAME);

            // Subscribe to messages from KinectHandler
            m_messageSubscriptions = new List<MessageSubscription>();
            m_messageSubscriptions.Add(
                kinectMessageHandler.Subscribe<MessageColorFrameArrived>(OnMessage_WpfColorFrameArrived));
            m_messageSubscriptions.Add(
                kinectMessageHandler.Subscribe<MessageDepthFrameArrived>(OnMessage_DepthFrameArrived));
            m_messageSubscriptions.Add(
                kinectMessageHandler.Subscribe<MessageInfraredFrameArrived>(OnMessage_InfraredFrameArrived));
            m_messageSubscriptions.Add(
                kinectMessageHandler.Subscribe<MessageLongExposureInfraredFrameArrived>(OnMessage_LongExposureInfraredFrameArrived));
            m_messageSubscriptions.Add(
                kinectMessageHandler.Subscribe<MessageBodyIndexFrameArrived>(OnMessage_BodyIndexFrameArrived));
        }

        /// <summary>
        /// Führt anwendungsspezifische Aufgaben aus, die mit dem Freigeben, Zurückgeben oder Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
        /// </summary>
        public void Dispose()
        {
            m_messageSubscriptions.ForEach((actSubscription) => actSubscription.Unsubscribe());
            m_messageSubscriptions.Clear();

            CommonTools.SafeDispose(ref m_dataFlowColor);
            CommonTools.SafeDispose(ref m_dataFlowDepth);
            CommonTools.SafeDispose(ref m_dataFlowInfrared);
            CommonTools.SafeDispose(ref m_dataFlowLongExposureInfrared);
            CommonTools.SafeDispose(ref m_dataFlowBodyIndex);
        }

        /// <summary>
        /// Gets a collection of scenes which presents raw streams from kinect.
        /// </summary>
        /// <param name="streamSubset">A value describing what streams to be presented..</param>
        public IEnumerable<Scene> GetRawStreamPresenterScenes(
            RawStreamSubset streamSubset = RawStreamSubset.All)
        {
            switch (streamSubset)
            {
                case RawStreamSubset.All:
                    yield return m_dataFlowColor.Scene;
                    yield return m_dataFlowDepth.Scene;
                    yield return m_dataFlowInfrared.Scene;
                    yield return m_dataFlowLongExposureInfrared.Scene;
                    yield return m_dataFlowBodyIndex.Scene;
                    break;

                case RawStreamSubset.OnlyColorAndDepth:
                    yield return m_dataFlowColor.Scene;
                    yield return m_dataFlowDepth.Scene;
                    break;

                case RawStreamSubset.OnlyBodyIndex:
                    yield return m_dataFlowBodyIndex.Scene;
                    break;

                default:
                    throw new ArgumentException("Unknown member of enum RawStreamSubset: " + streamSubset);
            }
        }

        /// <summary>
        /// Called when a new color frame is received.
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnMessage_WpfColorFrameArrived(MessageColorFrameArrived message)
        {
            if (m_dataFlowColor.Scene.CountViews <= 0) { return; }

            using (ColorFrame colorFrame = message.ColorFrameArgs.FrameReference.AcquireFrame())
            {
                if (colorFrame == null) { return; }

                using (KinectBuffer colorFrameBuffer = colorFrame.LockRawImageBuffer())
                {
                    lock (m_dataFlowColor.SyncBufferLock)
                    {
                        // Resize buffers if needed
                        m_dataFlowColor.ResizeSyncBufferIfNeeded(new Size2(
                            colorFrame.FrameDescription.Width,
                            colorFrame.FrameDescription.Height));

                        // Store pixel data in SyncBuffer
                        if (colorFrame.RawColorImageFormat == ColorImageFormat.Bgra)
                        {
                            colorFrame.CopyRawFrameDataToIntPtr(
                                m_dataFlowColor.SyncBuffer.Pointer,
                                m_dataFlowColor.SyncBuffer.SizeInBytes);
                        }
                        else
                        {
                            colorFrame.CopyConvertedFrameDataToIntPtr(
                                m_dataFlowColor.SyncBuffer.Pointer,
                                m_dataFlowColor.SyncBuffer.SizeInBytes,
                                ColorImageFormat.Bgra);
                        }
                    }
                    m_dataFlowColor.SyncBufferChanged = true;
                }
            }
        }

        /// <summary>
        /// Called when a new depth frame is received.
        /// </summary>
        /// <param name="message">The message.</param>
        private unsafe void OnMessage_DepthFrameArrived(MessageDepthFrameArrived message)
        {
            if (m_dataFlowDepth.Scene.CountViews <= 0) { return; }

            using (DepthFrame depthFrame = message.DepthFrameArgs.FrameReference.AcquireFrame())
            {
                if (depthFrame == null) { return; }

                using (KinectBuffer depthFrameBuffer = depthFrame.LockImageBuffer())
                {
                    ushort minReliableDistance = depthFrame.DepthMinReliableDistance;
                    ushort maxReliableDistance = depthFrame.DepthMaxReliableDistance;

                    lock (m_dataFlowDepth.SyncBufferLock)
                    {
                        // Resize buffers if needed
                        m_dataFlowDepth.ResizeSyncBufferIfNeeded(new Size2(
                            depthFrame.FrameDescription.Width,
                            depthFrame.FrameDescription.Height));

                        // Store pixel data in SyncBuffer
                        int frameWidth = depthFrame.FrameDescription.Width;
                        int frameHeight = depthFrame.FrameDescription.Height;
                        ushort* depthBufferP = (ushort*)depthFrameBuffer.UnderlyingBuffer.ToPointer();
                        int* syncBufferP = (int*)m_dataFlowDepth.SyncBuffer.Pointer.ToPointer();
                        for (int loopX = 0; loopX < frameWidth; loopX++)
                        {
                            for (int loopY = 0; loopY < frameHeight; loopY++)
                            {
                                // Read depth value (ushort)
                                ushort currentValue = depthBufferP[(loopY * frameWidth) + loopX];

                                // Convert depth value to a factor (white=min distance, black=max distance)
                                float currentColor = (float)(currentValue - minReliableDistance) / (float)maxReliableDistance;
                                currentColor = 1f - EngineMath.Clamp(currentColor, 0f, 1f);

                                syncBufferP[(loopY * frameWidth) + loopX] =
                                    (new Color4(currentColor, currentColor, currentColor, 1f)).ToArgb();
                            }
                        }

                        m_dataFlowDepth.SyncBufferChanged = true;
                    }
                }
            }
        }

        /// <summary>
        /// Called when a new infrared frame is received.
        /// </summary>
        /// <param name="message">The message.</param>
        private unsafe void OnMessage_InfraredFrameArrived(MessageInfraredFrameArrived message)
        {
            if (m_dataFlowInfrared.Scene.CountViews <= 0) { return; }

            using (InfraredFrame infraredFrame = message.InfraredFrameArgs.FrameReference.AcquireFrame())
            {
                if (infraredFrame == null) { return; }

                using (KinectBuffer infraredBuffer = infraredFrame.LockImageBuffer())
                {
                    ushort minReliableDistance = 0;
                    ushort maxReliableDistance = ushort.MaxValue;
                    lock (m_dataFlowInfrared.SyncBufferLock)
                    {
                        // Resize buffers if needed
                        m_dataFlowInfrared.ResizeSyncBufferIfNeeded(new Size2(
                            infraredFrame.FrameDescription.Width,
                            infraredFrame.FrameDescription.Height));

                        // Store pixel data in SyncBuffer
                        int frameWidth = infraredFrame.FrameDescription.Width;
                        int frameHeight = infraredFrame.FrameDescription.Height;
                        ushort* infraredBufferP = (ushort*)infraredBuffer.UnderlyingBuffer.ToPointer();
                        int* syncBufferP = (int*)m_dataFlowInfrared.SyncBuffer.Pointer.ToPointer();
                        for (int loopX = 0; loopX < frameWidth; loopX++)
                        {
                            for (int loopY = 0; loopY < frameHeight; loopY++)
                            {
                                // Read depth value (ushort)
                                ushort currentValue = infraredBufferP[(loopY * frameWidth) + loopX];

                                // Convert infrared value to a factor (white=min distance, black=max distance)
                                float currentColor = (float)(currentValue - minReliableDistance) / (float)maxReliableDistance;
                                currentColor = EngineMath.Clamp(currentColor, 0f, 1f);

                                syncBufferP[(loopY * frameWidth) + loopX] =
                                    (new Color4(currentColor, currentColor, currentColor, 1f)).ToArgb();
                            }
                        }

                        m_dataFlowInfrared.SyncBufferChanged = true;
                    }
                }
            }
        }

        /// <summary>
        /// Called when a new long exposure infrared frame is received.
        /// </summary>
        /// <param name="message">The message.</param>
        private unsafe void OnMessage_LongExposureInfraredFrameArrived(MessageLongExposureInfraredFrameArrived message)
        {
            if (m_dataFlowLongExposureInfrared.Scene.CountViews <= 0) { return; }

            using (LongExposureInfraredFrame infraredFrame = message.LongExposureInfraredFrameArgs.FrameReference.AcquireFrame())
            {
                if (infraredFrame == null) { return; }

                using (KinectBuffer infraredBuffer = infraredFrame.LockImageBuffer())
                {
                    ushort minReliableValue = 0;
                    ushort maxReliableValue = ushort.MaxValue;
                    lock (m_dataFlowLongExposureInfrared.SyncBufferLock)
                    {
                        // Resize buffers if needed
                        m_dataFlowLongExposureInfrared.ResizeSyncBufferIfNeeded(new Size2(
                            infraredFrame.FrameDescription.Width,
                            infraredFrame.FrameDescription.Height));

                        // Store pixel data in SyncBuffer
                        int frameWidth = infraredFrame.FrameDescription.Width;
                        int frameHeight = infraredFrame.FrameDescription.Height;
                        ushort* infraredBufferP = (ushort*)infraredBuffer.UnderlyingBuffer.ToPointer();
                        int* syncBufferP = (int*)m_dataFlowLongExposureInfrared.SyncBuffer.Pointer.ToPointer();
                        for (int loopX = 0; loopX < frameWidth; loopX++)
                        {
                            for (int loopY = 0; loopY < frameHeight; loopY++)
                            {
                                // Read depth value (ushort)
                                ushort currentValue = infraredBufferP[(loopY * frameWidth) + loopX];

                                // Convert infrared value to a factor (white=min distance, black=max distance)
                                float currentColor = (float)(currentValue - minReliableValue) / (float)maxReliableValue;
                                currentColor = EngineMath.Clamp(currentColor, 0f, 1f);

                                syncBufferP[(loopY * frameWidth) + loopX] =
                                    (new Color4(currentColor, currentColor, currentColor, 1f)).ToArgb();
                            }
                        }

                        m_dataFlowLongExposureInfrared.SyncBufferChanged = true;
                    }
                }
            }
        }

        /// <summary>
        /// Called when a new body index frame is received.
        /// </summary>
        /// <param name="message">The message.</param>
        private unsafe void OnMessage_BodyIndexFrameArrived(MessageBodyIndexFrameArrived message)
        {
            if (m_dataFlowBodyIndex.Scene.CountViews <= 0) { return; }

            using (BodyIndexFrame bodyIndexFrame = message.BodyIndexFrameArgs.FrameReference.AcquireFrame())
            {
                if (bodyIndexFrame == null) { return; }

                using (KinectBuffer bodyIndexBuffer = bodyIndexFrame.LockImageBuffer())
                {
                    lock (m_dataFlowBodyIndex.SyncBufferLock)
                    {
                        // Resize buffers if needed
                        m_dataFlowBodyIndex.ResizeSyncBufferIfNeeded(new Size2(
                            bodyIndexFrame.FrameDescription.Width,
                            bodyIndexFrame.FrameDescription.Height));

                        // Store pixel data in SyncBuffer
                        int frameWidth = bodyIndexFrame.FrameDescription.Width;
                        int frameHeight = bodyIndexFrame.FrameDescription.Height;
                        byte* infraredBufferP = (byte*)bodyIndexBuffer.UnderlyingBuffer.ToPointer();
                        int* syncBufferP = (int*)m_dataFlowBodyIndex.SyncBuffer.Pointer.ToPointer();
                        for (int loopX = 0; loopX < frameWidth; loopX++)
                        {
                            for (int loopY = 0; loopY < frameHeight; loopY++)
                            {
                                // Read depth value (ushort)
                                byte currentValue = infraredBufferP[(loopY * frameWidth) + loopX];

                                // get color based on read index
                                //  index 255 means nothing recognized
                                Color4 currentColor = Color4.Transparent;
                                if (currentValue != 255)
                                {
                                    int colorIndex = currentValue % Constants.KINECT_BODY_INDEX_COLORS.Length;
                                    currentColor = Constants.KINECT_BODY_INDEX_COLORS[colorIndex];
                                }

                                syncBufferP[(loopY * frameWidth) + loopX] = currentColor.ToArgb();
                            }
                        }

                        m_dataFlowBodyIndex.SyncBufferChanged = true;
                    }
                }
            }
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
            public string StreamName;
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
            /// <param name="width">The width of the image.</param>
            /// <param name="height">The height of the image.</param>
            public ImageDataFlowHelper(string streamName)
            {
                this.StreamName = streamName;
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

                    // Draw the stream's name
                    RectangleF titleBounds = new RectangleF(
                        viewBounds.Width / 2f - 100f,
                        10f,
                        200f, 30f);
                    graphics.FillRoundedRectangle(titleBounds, 5f, 5f, this.TextBackground);
                    graphics.DrawText(
                        this.StreamName, this.TextFormat,
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
    }
}
