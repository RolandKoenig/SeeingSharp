using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.RKKinectLounge.Modules.Kinect
{
    public static class BodyChecks
    {
        /// <summary>
        /// Very simple check which determines whether the current body is inside the main region.
        /// </summary>
        /// <param name="body">The body to be checked.</param>
        public static bool IsBodyInsideRegion(Body body)
        {
            Joint neckJoint = body.Joints[JointType.Neck];

            if (body.Joints[JointType.Neck].Position.Z > 2.2f) { return false; }
            if (neckJoint.Position.X > 0.4f) { return false; }
            if (neckJoint.Position.X < -0.4f) { return false; }

            return true;
        }

        /// <summary>
        /// Simple check logic for person engagement.
        /// If the hand is over the head of a person, then this one gets enganged.
        /// </summary>
        /// <param name="jointType">The JointType to check for (left or right hand)</param>
        /// <param name="body">The body which is to be checked.</param>
        public static bool IsHandOverhead(JointType jointType, Body body)
        {
            return (body.Joints[jointType].Position.Y >
                    body.Joints[JointType.Head].Position.Y);
        }

        /// <summary>
        /// Simple check logic for person disengagement.
        /// If the hand is below the base of the spine, then this person gets disengaged.
        /// </summary>
        /// <param name="jointType">The JointType to check for (left or right hand)</param>
        /// <param name="body">The body which is to be checked.</param>
        public static bool IsHandBelowHip(JointType jointType, Body body)
        {
            return (body.Joints[jointType].Position.Y <
                    body.Joints[JointType.SpineBase].Position.Y);
        }
    }
}
