﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;
using Mechanect.Common;
using Common.Classes;
using Mechanect.Classes;

namespace Mechanect
{
    public class Button
    {

        private Vector2 position;
        private GifAnimation.GifAnimation texture, animation, stopped;
        private int screenW, ScreenH;

        private Vector2 Position;
        private User user;
        private Texture2D hand;
        private Vector2 handPosition;

        private Timer1 timer;

        private bool status;

        ///<remarks>
        ///<para>
        ///Author: HegazY
        ///</para>
        ///</remarks>
        /// <summary>
        /// The constructor used to initialize the button
        /// </summary>
        /// <param name="t">the gif texture of the button not moving</param>
        /// <param name="tt">the gif texture of the button moving</param>
        /// <param name="p">the position of the button, where the center is top left corner</param>
        /// <param name="sw">screen width</param>
        /// <param name="sh">screen height</param>
        /// <param name="h">the picture of the hand</param>
        /// <param name="u">the user instance</param>
        public Button(GifAnimation.GifAnimation t, GifAnimation.GifAnimation tt, Vector2 p, int sw, int sh, Texture2D h, User u)
        {
            position = p;
            texture = t;
            stopped = t;
            animation = tt;
            screenW = sw;
            ScreenH = sh;
            hand = h;
            user = u;
            timer = new Timer1();
        }

        ///<remarks>
        ///<para>
        ///Author: HegazY
        ///</para>
        ///</remarks>
        /// <summary>
        /// drawing the button and the hand
        /// </summary>
        /// <param name="spriteBatch">used to draw the texture</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(texture.GetTexture(), position, Color.White);
            spriteBatch.End();
        }

        ///<remarks>
        ///<para>
        ///Author: HegazY
        ///</para>
        ///</remarks>
        /// <summary>
        /// checks if the hand of the user has been hovered on the button and starts
        /// the timer and activating the pointer to be moved.
        /// </summary>
        /// <param name="gameTime">takes the object gametime which tracks the time of the game</param>
        public void Update(GameTime gameTime)
        {
            user.setSkeleton();
            MoveHand();
            texture.Update(gameTime.ElapsedGameTime.Ticks);

            if (CheckColission())
            {
                if (!timer.IsRunning())
                    timer.Start(gameTime);
                else
                {
                    Animate();
                    if (timer.GetDuration(gameTime) >= (2000))
                    {
                        status = true;
                        timer.Stop();
                    }

                }
            }
            else
            {
                timer.Stop();
                Stop();
            }


        }

        ///<remarks>
        ///<para>
        ///Author: HegazY
        ///</para>
        ///</remarks>
        /// <summary>
        /// used to track the user's hand
        /// </summary>
        private void MoveHand()
        {
            Skeleton skeleton = user.USER;
            if (skeleton != null)
            {
                handPosition.X = user.Kinect.GetJointPoint(skeleton.Joints[JointType.HandRight], screenW, ScreenH).X;
                handPosition.Y = user.Kinect.GetJointPoint(skeleton.Joints[JointType.HandRight], screenW, ScreenH).Y;
            }
        }

        ///<remarks>
        ///<para>
        ///Author: HegazY
        ///</para>
        ///</remarks>
        /// <summary>
        /// changing the button to the animated picture
        /// </summary>
        private void Animate()
        {
            texture = animation;
        }

        ///<remarks>
        ///<para>
        ///Author: HegazY
        ///</para>
        ///</remarks>
        /// <summary>
        /// changing the button to the stopped picture
        /// </summary>
        private void Stop()
        {
            texture = stopped;
        }

        ///<remarks>
        ///<para>
        ///Author: HegazY
        ///</para>
        ///</remarks>
        /// <summary>
        /// checks if the hand of the user is over the button or not
        /// </summary>
        /// <returns>returns true if the user is hovering the button</returns>
        private bool CheckColission()
        {
            Skeleton skeleton = user.Kinect.requestSkeleton();
            if (skeleton != null)
            {
                Point hand = user.Kinect.GetJointPoint(skeleton.Joints[JointType.HandRight], screenW, ScreenH);
                Rectangle r1 = new Rectangle(hand.X, hand.Y, 50, 50);
                Rectangle r2 = new Rectangle((int)position.X, (int)position.Y, texture.GetTexture().Width, texture.GetTexture().Height);

                return r1.Intersects(r2);
            }
            return false;
        }

        ///<remarks>
        ///<para>
        ///Author: HegazY
        ///</para>
        ///</remarks>
        /// <summary>
        /// used to check if the button has been clicked or not
        /// </summary>
        /// <returns>returns true if the user clicked the button</returns>
        public bool IsClicked()
        {
            return status;
        }


        ///<remarks>
        ///<para>
        ///Author: HegazY
        ///</para>
        ///</remarks>
        /// <summary>
        /// used to reset the status variable to false, which means the button is not clicked
        /// </summary>
        public void Reset()
        {
            status = false;
        }

        ///<remarks>
        ///<para>
        ///Author: HegazY
        ///</para>
        ///</remarks>
        /// <summary>
        /// draw the hand on the screen. It must be called after beginning the SpriteBatch.
        /// </summary>
        public void DrawHand(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(hand, handPosition, Color.White);
        }
    }
}
