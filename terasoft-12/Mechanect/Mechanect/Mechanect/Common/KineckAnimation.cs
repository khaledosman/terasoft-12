﻿using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using UI.Components;

namespace Mechanect.Common
{
    /// <summary>
    /// Animates a 3D model according to the movement of the user from the Kinect.
    /// </summary>
    /// <remarks>
    /// <para>AUTHOR: AhmeD HegazY</para>
    /// </remarks>
    public class KineckAnimation
    {
        #region Fields

        private SkinnedCustomModel model;
        private User user;

        #endregion

        #region Initialization

        /// <summary>
        /// Used to animate the SkinnedCustomModel according to the movement of the player.
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: AhmeD HegazY</para>
        /// </remarks>
        /// <param name="model">The model with skeleton and bones.</param>
        /// <param name="user">The instance of the user.</param>
        public KineckAnimation(SkinnedCustomModel model, User user)
        {
            this.model = model;
            this.user = user;
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the movement of the model.
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: AhmeD HegazY</para>
        /// </remarks>
        public void Update()
        {
            MoveRightLeg();
            MoveLeftLeg();
        }

        #endregion

        #region Animating Bones

        /// <summary>
        /// Animates the the right leg.
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: AhmeD HegazY</para>
        /// </remarks>
        private void MoveRightLeg()
        {
            Skeleton skeleton = user.Kinect.requestSkeleton();
            if (skeleton != null)
            {
                Joint foot = skeleton.Joints[JointType.FootRight];
                Joint hip = skeleton.Joints[JointType.HipRight];
                float value = (hip.Position.Z - foot.Position.Z) * 3;
                if (value < 0)
                    model.MoveBone("R_Knee", -2, new Vector3(0, 0, value));
                else
                    model.MoveBone("R_Thigh", -2, new Vector3(0, 0, value));

            }
        }


        /// <summary>
        /// Animates the left leg.
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: AhmeD HegazY</para>
        /// </remarks>
        private void MoveLeftLeg()
        {
            Skeleton skeleton = user.Kinect.requestSkeleton();
            if (skeleton != null)
            {
                Joint foot = skeleton.Joints[JointType.FootLeft];
                Joint hip = skeleton.Joints[JointType.HipLeft];
                float value = (hip.Position.Z - foot.Position.Z) * 3;
                if (value < 0)
                    model.MoveBone("L_Knee2", -2, new Vector3(0, 0, value));
                else
                    model.MoveBone("L_Thigh1", -2, new Vector3(0, 0, value));

            }
        }

        #endregion
    }
}
