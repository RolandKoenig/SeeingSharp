#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it.
    More info at
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
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
#endregion License information (SeeingSharp and all based games/applications)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

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