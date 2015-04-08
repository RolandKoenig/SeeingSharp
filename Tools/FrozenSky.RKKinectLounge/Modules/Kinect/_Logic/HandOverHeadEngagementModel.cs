using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FrozenSky.Infrastructure;
using FrozenSky.Util;
using Microsoft.Kinect;
using Microsoft.Kinect.Input;
using Microsoft.Kinect.Toolkit.Input;
using FrozenSky.RKKinectLounge.Base;

namespace FrozenSky.RKKinectLounge.Modules.Kinect
{
    // Taken from KinectControls-WPF Sample and modified
    // Differences:
    //  - We do only engage one person
    //  - Body data is received on a background thread

    /// <summary>
    /// A kinect engagement handler which will engage 1 person.
    /// Engagement signal: putting a hand over the head
    /// Disengagement signal: putting you hand down to your side
    /// </summary>
    public class HandOverheadEngagementModel : IKinectEngagementManager
    {
        private List<MessageSubscription> m_messageSubscriptions;

        private bool m_isStopped = true;
        private bool m_isProcessing;
        private List<Body> m_bodies;
        private bool m_engagementPeopleHaveChanged;
        private List<BodyHandPair> m_handsToEngage;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandOverheadEngagementModel"/> class.
        /// </summary>
        public HandOverheadEngagementModel()
        {
            m_bodies = new List<Body>();
            m_handsToEngage = new List<BodyHandPair>();

            // Get the Messenger of the KinectThread
            FrozenSkyMessenger kinectMessenger =
                FrozenSkyMessenger.GetByName(Constants.KINECT_THREAD_NAME);

            // Subscribe to messages from kinect
            m_messageSubscriptions = new List<MessageSubscription>();
            m_messageSubscriptions.Add(
                kinectMessenger.Subscribe<MessageBodyFrameArrived>(OnMessage_BodyFrameArrived));
        }

        /// <summary>
        /// Per frame, have the set of engaged body hand pairs changed.
        /// </summary>
        public bool EngagedBodyHandPairsChanged()
        {
            return this.m_engagementPeopleHaveChanged;
        }

        /// <summary>
        /// Called when the manager should start managing which BodyHandPairs are engaged.
        /// </summary>
        public void StartManaging()
        {
            this.m_isStopped = false;
        }

        /// <summary>
        /// Called when the manager should stop managing which BodyHandPairs are engaged.
        /// </summary>
        public void StopManaging()
        {
            this.m_isStopped = true;
        }

        /// <summary>
        /// Called when a body frame is received.
        /// </summary>
        /// <param name="message">The message to be processed.</param>
        private async void OnMessage_BodyFrameArrived(MessageBodyFrameArrived message)
        {
            if (m_isStopped) { return; }
            if (m_isProcessing) { return; }
            try
            {
                m_isProcessing = true;

                // Try to get the BodyFrame from the Kinect
                bool gotData = false;
                using (var frame = message.BodyFrameArgs.FrameReference.AcquireFrame())
                {
                    if (frame != null)
                    {
                        m_bodies.CreateDummyObjectUntilCapacityReached(frame.BodyCount);

                        frame.GetAndRefreshBodyData(m_bodies);
                        gotData = true;
                    }
                }

                // Process data if we've got one
                if (gotData && !m_isStopped)
                {
                    await FrozenSkyApplication.Current.UISyncContext.PostAsync(() =>
                    {
                        this.TrackEngagedPlayersViaHandOverHead();
                    });
                }
            }
            finally
            {
                m_isProcessing = false;
            }
        }

        /// <summary>
        /// Main logic for detecting whehter the user has his hands over his head.
        /// </summary>
        private void TrackEngagedPlayersViaHandOverHead()
        {
            this.m_engagementPeopleHaveChanged = false;
            var currentlyEngagedHands = KinectCoreWindow.KinectManualEngagedHands;
            
            this.m_handsToEngage.Clear();

            // Check to see if anybody who is currently engaged should be disengaged
            foreach (var bodyHandPair in currentlyEngagedHands)
            {
                var bodyTrackingId = bodyHandPair.BodyTrackingId;
                foreach (var body in this.m_bodies)
                {
                    if (body.TrackingId != bodyTrackingId) { continue; }

                    // Check for disengagement
                    JointType engagedHandJoint =
                        (bodyHandPair.HandType == HandType.LEFT) ? JointType.HandLeft : JointType.HandRight;
                    bool toBeDisengaged = false;

                    // Disengage because the body moved outside the main area
                    if (!BodyChecks.IsBodyInsideRegion(body))
                    {
                        toBeDisengaged = true;
                    }

                    // Perform disengagement if needed
                    if (toBeDisengaged){ this.m_engagementPeopleHaveChanged = true; }
                    else{ this.m_handsToEngage.Add(bodyHandPair); }
                }
            }

            // Check to see if anybody should be engaged, if not already engaged
            if (this.m_handsToEngage.Count == 0)
            {
                foreach (var body in this.m_bodies)
                {
                    if (!body.IsTracked) { continue; }
                    if (body.IsRestricted) { continue; }

                    if (!BodyChecks.IsBodyInsideRegion(body)) { continue; }

                    // Check for engagement
                    if (BodyChecks.IsHandOverhead(JointType.HandLeft, body))
                    {
                        // Engage the left hand
                        m_handsToEngage.Add(
                            new BodyHandPair(body.TrackingId, HandType.LEFT));
                        m_engagementPeopleHaveChanged = true;

                    }
                    else if (BodyChecks.IsHandOverhead(JointType.HandRight, body))
                    {

                        // Engage the right hand
                        m_handsToEngage.Add(
                            new BodyHandPair(body.TrackingId, HandType.RIGHT));
                        m_engagementPeopleHaveChanged = true;
                    }
                }
            }

            // Handle engagement and disengagement
            if(m_engagementPeopleHaveChanged)
            {
                if(m_handsToEngage.Count > 0)
                {
                    // Set current hand engagement
                    BodyHandPair firstPersonToEngage = this.m_handsToEngage[0]; ;
                    KinectCoreWindow.SetKinectOnePersonManualEngagement(firstPersonToEngage);

                    // We've engaged a new person
                    FrozenSkyApplication.Current.UIMessenger.Publish<MessagePersonEngaged>();
                }
                else
                {
                    // Reset current hand engagement
                    KinectCoreWindow.SetKinectOnePersonManualEngagement(null);

                    // We've disengaged
                    FrozenSkyApplication.Current.UIMessenger.Publish<MessagePersonDisengaged>();
                }
            }
        }

        /// <summary>
        /// If manual engagement is on, which hands are currently engaged.
        /// </summary>
        public IReadOnlyList<BodyHandPair> KinectManualEngagedHands
        {
            get
            {
                return KinectCoreWindow.KinectManualEngagedHands;
            }
        }
    }
}
