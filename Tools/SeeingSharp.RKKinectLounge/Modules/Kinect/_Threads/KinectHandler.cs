using SeeingSharp.Infrastructure;
using SeeingSharp.Util;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SeeingSharp.RKKinectLounge.Modules.Kinect
{
    public class KinectHandler : PropertyChangedBase
    {
        private SeeingSharpMessenger m_Messenger;
        private PerformanceAnalyzer m_performanceAnalyzer;

        private KinectSensor m_sensor;
        private InfraredFrameReader m_readerInfraredFrame;
        private LongExposureInfraredFrameReader m_readerLongExposureInfraredFrame;
        private ColorFrameReader m_readerColorFrame;
        private DepthFrameReader m_readerDepthFrame;
        private BodyFrameReader m_readerBodyFrame;
        private BodyIndexFrameReader m_readerBodyIndexFrame;

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectHandler"/> class.
        /// </summary>
        /// <param name="Messenger">The Messenger which is used for thread- und component-synchronization.</param>
        /// <param name="performanceAnalyzer">The PerformanceAnalyzer which tracks the kinect's performance values.</param>
        public KinectHandler(
            SeeingSharpMessenger Messenger,
            PerformanceAnalyzer performanceAnalyzer)
        {
            if (!SeeingSharpApplication.IsInitialized) { return; }

            m_Messenger = Messenger;
            m_performanceAnalyzer = performanceAnalyzer;


            m_sensor = KinectSensor.GetDefault();
            if (m_sensor == null) { return; }

            m_sensor.IsAvailableChanged += OnSensor_IsAvailableChanged;
            m_sensor.Open();
        }

        /// <summary>
        /// Called when the kinect sensor is available.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="IsAvailableChangedEventArgs"/> instance containing the event data.</param>
        private void OnSensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // This call is automatically dispatched to the main thread, so no
            // synchronization is needed
            base.RaisePropertyChanged(() => UniqueKinectId);
            base.RaisePropertyChanged(() => IsSensorOpen);
            base.RaisePropertyChanged(() => IsSensorAvailable);

            if(e.IsAvailable)
            {
                m_readerInfraredFrame = m_sensor.InfraredFrameSource.OpenReader();
                m_readerInfraredFrame.FrameArrived += OnReaderInfraredFrame_FrameArrived;

                m_readerLongExposureInfraredFrame = m_sensor.LongExposureInfraredFrameSource.OpenReader();
                m_readerLongExposureInfraredFrame.FrameArrived += OnReaderLongExposureInfrared_FrameArrived;

                m_readerColorFrame = m_sensor.ColorFrameSource.OpenReader();
                m_readerColorFrame.FrameArrived += OnReaderColorFrame_FrameArrived;

                m_readerDepthFrame = m_sensor.DepthFrameSource.OpenReader();
                m_readerDepthFrame.FrameArrived += OnReaderDepthFrame_FrameArrived;

                m_readerBodyFrame = m_sensor.BodyFrameSource.OpenReader();
                m_readerBodyFrame.FrameArrived += OnReaderBodyFrame_FrameArrived;

                m_readerBodyIndexFrame = m_sensor.BodyIndexFrameSource.OpenReader();
                m_readerBodyIndexFrame.FrameArrived += OnReaderBodyIndexFrame_FrameArrived;
            }
            else
            {
                CommonTools.SafeDispose(ref m_readerInfraredFrame);
                CommonTools.SafeDispose(ref m_readerColorFrame);
                CommonTools.SafeDispose(ref m_readerDepthFrame);
                CommonTools.SafeDispose(ref m_readerBodyFrame);
                CommonTools.SafeDispose(ref m_readerBodyIndexFrame);
            }
        }

        /// <summary>
        /// Called when an infrared fame has arrived from the sensor.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="InfraredFrameArrivedEventArgs"/> instance containing the event data.</param>
        private void OnReaderInfraredFrame_FrameArrived(object sender, InfraredFrameArrivedEventArgs e)
        {
            // Notify flowrate occurrence
            m_performanceAnalyzer.NotifyFlowRateOccurrence(
                Constants.KINECT_PERF_FLOWRATE_INFRARED_FRAME);

            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame == null) { return; }

                this.AquiredInfraredFrames = this.AquiredInfraredFrames + 1;

                // Publich event to whole application
                m_Messenger.Publish(
                    new MessageInfraredFrameArrived(e));
            }
        }

        /// <summary>
        /// Called when an long exposure infrared fame has arrived from the sensor.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="InfraredFrameArrivedEventArgs"/> instance containing the event data.</param>
        private void OnReaderLongExposureInfrared_FrameArrived(object sender, LongExposureInfraredFrameArrivedEventArgs e)
        {
            // Notify flowrate occurrence
            m_performanceAnalyzer.NotifyFlowRateOccurrence(
                Constants.KINECT_PERF_FLOWRATE_LONG_EXPOSURE_INFRARED_FRAME);

            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame == null) { return; }

                this.AquiredLongExposureInfraredFrames = this.AquiredLongExposureInfraredFrames + 1;

                // Publich event to whole application
                m_Messenger.Publish(
                    new MessageLongExposureInfraredFrameArrived(e));
            }
        }

        /// <summary>
        /// Called when a color frame has arrived from the sensor.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ColorFrameArrivedEventArgs"/> instance containing the event data.</param>
        private void OnReaderColorFrame_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            // Notify flowrate occurrence
            m_performanceAnalyzer.NotifyFlowRateOccurrence(
                Constants.KINECT_PERF_FLOWRATE_COLOR_FRAME);

            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame == null) { return; }

                this.AquiredColorFrames = this.AquiredColorFrames + 1;

                // Publich event to whole application
                m_Messenger.Publish(
                    new MessageColorFrameArrived(e));
            }
        }

        /// <summary>
        /// Called when a depth frame arrived from the sensor.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DepthFrameArrivedEventArgs"/> instance containing the event data.</param>
        private void OnReaderDepthFrame_FrameArrived(object sender, DepthFrameArrivedEventArgs e)
        {
            // Notify flowrate occurrence
            m_performanceAnalyzer.NotifyFlowRateOccurrence(
                Constants.KINECT_PERF_FLOWRATE_DEPTH_FRAME);

            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame == null) { return; }

                this.AquiredDepthFrames = this.AquiredDepthFrames + 1;

                // Publich event to whole application
                m_Messenger.Publish(
                    new MessageDepthFrameArrived(e));
            }
        }

        /// <summary>
        /// Called when a body frame arrived from the sensor.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DepthFrameArrivedEventArgs"/> instance containing the event data.</param>
        private void OnReaderBodyFrame_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            // Notify flowrate occurrence
            m_performanceAnalyzer.NotifyFlowRateOccurrence(
                Constants.KINECT_PERF_FLOWRATE_BODY_FRAME);

            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame == null) { return; }

                this.AquiredBodyFrames = this.AquiredBodyFrames + 1;

                // Publich event to whole application
                m_Messenger.Publish(
                    new MessageBodyFrameArrived(e));
            }
        }

        /// <summary>
        /// Called when a body index frame arrived from the sensor.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DepthFrameArrivedEventArgs"/> instance containing the event data.</param>
        private void OnReaderBodyIndexFrame_FrameArrived(object sender, BodyIndexFrameArrivedEventArgs e)
        {
            // Notify flowrate occurrence
            m_performanceAnalyzer.NotifyFlowRateOccurrence(
                Constants.KINECT_PERF_FLOWRATE_BODYINDEX_FRAME);

            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame == null) { return; }

                this.AquiredBodyFrames = this.AquiredBodyFrames + 1;

                // Publich event to whole application
                m_Messenger.Publish(
                    new MessageBodyIndexFrameArrived(e));
            }
        }

        public int AquiredInfraredFrames
        {
            get;
            set;
        }

        public int AquiredLongExposureInfraredFrames
        {
            get;
            set;
        }

        public int AquiredInfraredLongFrames
        {
            get;
            set;
        }

        public int AquiredColorFrames
        {
            get;
            set;
        }

        public int AquiredDepthFrames
        {
            get;
            set;
        }

        public int AquiredBodyFrames
        {
            get;
            set;
        }

        public string UniqueKinectId
        {
            get
            {
                if (m_sensor == null) { return string.Empty; }
                return m_sensor.UniqueKinectId;
            }
        }

        public bool IsSensorOpen
        {
            get
            {
                if (m_sensor == null) { return false; }
                return m_sensor.IsOpen;
            }
        }

        public bool IsSensorAvailable
        {
            get
            {
                if (m_sensor == null) { return false; }
                return m_sensor.IsAvailable;
            }
        }

        public KinectCapabilities Capabilities
        {
            get
            {
                if (m_sensor == null) { return KinectCapabilities.None; }
                return m_sensor.KinectCapabilities;
            }
        }
    }
}
