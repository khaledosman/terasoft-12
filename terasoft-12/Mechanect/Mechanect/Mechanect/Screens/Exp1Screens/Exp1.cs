﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Mechanect.Common;
using Microsoft.Xna.Framework.Content;
using Mechanect.Exp1;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace Mechanect.Screens.Exp1Screens
{
    class Exp1 : Mechanect.Common.GameScreen
    {
        public static readonly List<String> gCommands = new List<string>() { "constantDisplacement", "constantAcceleration", "constantVelocity"};//, "increasingAcceleration", "decreasingAcceleration"};
        SpriteFont spritefont1;
        string currentcommand = "";
        List<int> cumtimeslice = new List<int>();
        List<string> racecommands = new List<string>();
        List<int> timeslice = new List<int>();
        int avatarconst;
        #region KeeVariables
        float firstframe = 5000;
        float speed = 5000;
        float speed2 = 50000;
        float speedr = 50000;
        float speedr2 = 50000;
        float[] speedlist = new float[2];
        float[] speedlistr = new float[2];
        float[] speedlist2 = new float[2];
        float[] speedlistr2 = new float[2];
        bool calculatespeedbool = false;
        bool calculatespeedbool2 = false;
        bool calculatespeedboolr = false;
        bool calculatespeedboolr2 = false;
        float[] min = new float[2];
        float[] minr = new float[2];
        float[] min2 = new float[2];
        float[] minr2 = new float[2];
        float[] max = new float[2];
        float[] maxr = new float[2];
        float[] max2 = new float[2];
        float[] maxr2 = new float[2];
        Joint left, right;
        int timecounter;
        #endregion
        MKinect kinect;
        Viewport ViewPort
        {
            get
            {
                return ScreenManager.GraphicsDevice.Viewport;
            }
        }
        ContentManager Content
        {
            get
            {
                return ScreenManager.Game.Content;
            }
        }
        SpriteBatch SpriteBatch
        {
            get
            {
                return ScreenManager.SpriteBatch;
            }
        }
        Environ1 environ1;
        GraphicsDevice graphics;
        User1 user1, user2;
        float timer = -4;
        CountDown countdown;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="user1">Player 1 </param>
        /// <param name="user2">Player 2</param>
        /// <remarks>
        /// <para>AUTHOR: Safty</para>
        /// <para>DATE WRITTEN: 15/5/12 </para>
        /// <para>DATE MODIFIED: 24/5/12 </para>
        /// </remarks>
        public Exp1(User1 user1, User1 user2)
        {
            this.user1 = user1;
            this.user2 = user2;
        }
        /// <summary>
        /// Initializes the Kinect.
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Safty</para>
        /// <para>DATE WRITTEN: 15/5/12 </para>
        /// <para>DATE MODIFIED: 24/5/12 </para>
        /// </remarks>
        public override void Initialize()
        {
            kinect = new MKinect();
            isTwoPlayers = true;
            base.Initialize();
        }
        /// <summary>
        /// Loads the textures needed for this Screen from the content manager.
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Safty</para>
        /// <para>DATE WRITTEN: 15/5/12 </para>
        /// <para>DATE MODIFIED: 24/5/12 </para>
        /// </remarks>
        public override void LoadContent()
        {
            spritefont1 = Content.Load<SpriteFont>("SpriteFont1");
            graphics = this.ScreenManager.GraphicsDevice;
            environ1 = new Environ1(ScreenManager.Game.Content, ScreenManager.Game.GraphicsDevice,this.SpriteBatch);
            environ1.LoadContent();
            loadcountdown();
            avatarconst = (int)(graphics.DisplayMode.Height * 0.01);
            this.Initialize_Timeslices();
            base.LoadContent();
        }




        /// <summary>
        /// Updates the logic of the game, including Skeleton selection, game commands representation, Countdown and Disqualification.
        /// </summary>
        /// <param name="gameTime">Game time for Synchronization issues</param>
        /// <remarks>
        /// <para>AUTHOR: Safty</para>
        /// <para>DATE WRITTEN: 15/5/12 </para>
        /// <para>DATE MODIFIED: 24/5/12 </para>
        /// </remarks>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {          
            environ1.Update();
            if (user1.skeleton == null || user2.skeleton == null)
            {           
                kinect.requestSkeleton();
                user1.skeleton = kinect.globalSkeleton;
                kinect.request2ndSkeleton();
                user2.skeleton = kinect.globalSkeleton2;
                user1.Kneepos.Clear();
                user2.Kneepos.Clear();
                user1.Kneeposr.Clear();
                user2.Kneeposr.Clear();
                user1.Velocitylist.Clear();
                user2.Velocitylist.Clear();
                user1.Disqualified = user2.Disqualified = user1.Winner = user2.Winner = false;
                
            }
            else
            {
                if (user1.skeleton.Position.X > user2.skeleton.Position.X)
                {
                    Skeleton temp = new Skeleton();
                    temp = user2.skeleton;
                    user2.skeleton = user1.skeleton;
                    user1.skeleton = temp;
                }

                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(timer < 0)
                {
                     countdown.Update();
                }
                if (timer > 0)
                {              
                   if (firstframe != (user1.skeleton.Joints[JointType.KneeLeft].Position.Y))
                    {
                        fill_Knee_pos();
                        getspeedleft();
                        getspeedleft2();
                        getspeedright();
                        getspeedright2();
                        firstframe = (user1.skeleton.Joints[JointType.KneeLeft].Position.Y);
                    }

                   Tools1.CheckTheCommand(timer, user1, user2, timeslice, racecommands,3);
                   Tools1.SetWinner(user1,user2, (float)(graphics.DisplayMode.Height * 0.91)); // with respect to the track

                   for (int i = 0; i < timeslice.Count(); i++)
                   {
                       if (timer >= cumtimeslice[i] && timer < cumtimeslice[i + 1])
                       {
                           currentcommand = racecommands[i];
                           user1.ActiveCommand = i;
                           user2.ActiveCommand = i;
                           
                       }
                   }                  
                }
            }
            if (user1.Winner || user2.Winner || (user1.Disqualified && user2.Disqualified))
            {
                ScreenManager.AddScreen(new Winnerscreen(user1,user2,graphics,this.racecommands,this.timeslice));
                Remove();
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the Countdown, Race environment and the Commands.
        /// </summary>
        /// <param name="gameTime">Game time for Synchronization issues</param>
        /// <remarks>
        /// <para>AUTHOR: Safty</para>
        /// <para>DATE WRITTEN: 15/5/12 </para>
        /// <para>DATE MODIFIED: 24/5/12 </para>
        /// </remarks>
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            
            environ1.Draw();
            if (user1.skeleton == null || user2.skeleton == null)
            {
                SpriteBatch.Begin();
                SpriteBatch.DrawString(spritefont1, "Please stand in range", new Microsoft.Xna.Framework.Vector2((int)(graphics.DisplayMode.Width * 0.02), (int)(graphics.DisplayMode.Height * 0.5)), Color.White);
                SpriteBatch.DrawString(spritefont1, "Please stand in range", new Microsoft.Xna.Framework.Vector2((int)(graphics.DisplayMode.Width * 0.63), (int)(graphics.DisplayMode.Height * 0.5)), Color.White);
                
                SpriteBatch.End();
            }
            else
            {
                if (timer < 0)
                {
                    SpriteBatch.Begin();
                    countdown.DrawCountdown(SpriteBatch, (int)(graphics.DisplayMode.Width * 0.4), (int)(graphics.DisplayMode.Height * 0.4));
                    SpriteBatch.End();
                    countdown.PlaySoundEffects();
                }
                if (timer > 0)
                {
                    SpriteBatch.Begin();
                    SpriteBatch.DrawString(spritefont1, this.currentcommand, new Microsoft.Xna.Framework.Vector2((int)(graphics.DisplayMode.Width * 0.02), (int)(graphics.DisplayMode.Height * 0.5)), Color.White);
                    SpriteBatch.DrawString(spritefont1, this.currentcommand, new Microsoft.Xna.Framework.Vector2((int)(graphics.DisplayMode.Width * 0.63), (int)(graphics.DisplayMode.Height * 0.5)), Color.White);
                    SpriteBatch.End();
                }
            }
            base.Draw(gameTime);

        }
        /// <summary>
        /// Load the countdown textures from the Content Manager.
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Safty</para>
        /// <para>DATE WRITTEN: 15/5/12 </para>
        /// <para>DATE MODIFIED: 24/5/12 </para>
        /// </remarks>
        public void loadcountdown()
        {
            Texture2D Texthree = Content.Load<Texture2D>("3");
            Texture2D Textwo = Content.Load<Texture2D>("2");
            Texture2D Texone = Content.Load<Texture2D>("1");
            Texture2D Texgo = Content.Load<Texture2D>("go");
            Texture2D Texback = Content.Load<Texture2D>("track2");
            SoundEffect Seffect1 = Content.Load<SoundEffect>("BEEP1B");
            SoundEffect Seffect2 = Content.Load<SoundEffect>("StartBeep");
            countdown = new CountDown();
            countdown.InitializeCountDown(Texthree, Textwo, Texone, Texgo, Seffect1, Seffect2);//initializes the Countdown 
        }
        #region kneespeedcalc
        public void fill_Knee_pos()
        {
                user1.Kneepos.Add((float)Math.Round((user1.skeleton.Joints[JointType.KneeLeft].Position.Y), 1));
                user2.Kneepos.Add((float)Math.Round((user2.skeleton.Joints[JointType.KneeLeft].Position.Y), 1));
                user1.Kneeposr.Add((float)Math.Round((user1.skeleton.Joints[JointType.KneeRight].Position.Y), 1));
                user2.Kneeposr.Add((float)Math.Round((user2.skeleton.Joints[JointType.KneeRight].Position.Y), 1));     
        }

       
        /// <summary>
        /// converts left knee positions to velocity list.
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Safty</para>
        /// <para>DATE WRITTEN: 15/5/12 </para>
        /// <para>DATE MODIFIED: 24/5/12 </para>
        /// </remarks>
        public void getspeedleft()
        {
            if (user1.Kneepos.Count() != 0)
            {
                if (user1.Kneepos.Count() == 1)
                {
                    min[0] = user1.Kneepos[0]; //set the first input to be minimum
                    min[1] = timer;
                }
                else
                {
                    if (user1.Kneepos[user1.Kneepos.Count() - 1] == user1.Kneepos[user1.Kneepos.Count() - 2]) // if the next input inlist equal the one b4 .. discard the one b4
                    {

                        if (user1.Kneepos[user1.Kneepos.Count() - 1] == max[0])
                        {
                            max[0] = user1.Kneepos[user1.Kneepos.Count() - 1];
                            max[1] = timer;
                        }
                        if (user1.Kneepos[user1.Kneepos.Count() - 1] == min[0])
                        {
                            min[0] = user1.Kneepos[user1.Kneepos.Count() - 1];
                            min[1] = timer;
                        }
                        user1.Kneepos.RemoveAt(user1.Kneepos.Count() - 2);
                    }
                    else
                    {
                        if (user1.Kneepos[user1.Kneepos.Count() - 1] > user1.Kneepos[user1.Kneepos.Count() - 2]) // if the next input greater than the one b4 .. set it to max
                        {
                            calculatespeedbool = true;
                            max[0] = user1.Kneepos[user1.Kneepos.Count() - 1];
                            max[1] = timer;
                            user1.Kneepos.RemoveAt(user1.Kneepos.Count() - 2);
                        }
                        else // the next input is smaller than the one b4 .. 
                        {
                            if (calculatespeedbool)
                            {
                                if ((max[0] - min[0]) >= 0.3)
                                {
                                    speed = Math.Abs(((max[0] - min[0]) / (max[1] - min[1])));
                                    if (speed <= 4)
                                    {
                                        float[] speedlist = new float[2];
                                        speedlist[0] = speed;
                                        speedlist[1] = timer;//calculate the speed of the oscillation from min to max
                                        user1.Velocitylist.Add(speedlist);
                                        user1.Positions.Add(speed * this.avatarconst);                                     
                                        this.environ1.bike1.Move(speed * avatarconst);                                    
                                        calculatespeedbool = false;
                                    }
                                    else
                                    {
                                        calculatespeedbool = false;
                                    }
                                }
                                else
                                {
                                    calculatespeedbool = false;
                                }
                            }
                            min[0] = user1.Kneepos[user1.Kneepos.Count() - 1];
                            min[1] = timer;
                            user1.Kneepos.RemoveAt(user1.Kneepos.Count() - 2);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// converts Right knee positions to velocity list.
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Safty</para>
        /// <para>DATE WRITTEN: 15/5/12 </para>
        /// <para>DATE MODIFIED: 24/5/12 </para>
        /// </remarks>
        public void getspeedright()
        {
            if (user1.Kneeposr.Count() != 0)
            {
                if (user1.Kneeposr.Count() == 1)
                {
                    minr[0] = user1.Kneeposr[0]; //set the first input to be minimum
                    minr[1] = timer;
                }
                else
                {
                   if (user1.Kneeposr[user1.Kneeposr.Count() - 1] == user1.Kneeposr[user1.Kneeposr.Count() - 2]) // if the next input inlist equal the one b4 .. discard the one b4
                    {
                        if (user1.Kneeposr[user1.Kneeposr.Count() - 1] == maxr[0])
                        {
                            maxr[0] = user1.Kneeposr[user1.Kneeposr.Count() - 1];
                            maxr[1] = timer;
                        }
                        if (user1.Kneeposr[user1.Kneeposr.Count() - 1] == minr[0])
                        {
                            minr[0] = user1.Kneeposr[user1.Kneeposr.Count() - 1];
                            minr[1] = timer;
                        }
                        user1.Kneeposr.RemoveAt(user1.Kneeposr.Count() - 2);
                    }
                    else
                    {
                        if (user1.Kneeposr[user1.Kneeposr.Count() - 1] > user1.Kneeposr[user1.Kneeposr.Count() - 2]) // if the next input greater than the one b4 .. set it to max
                        {
                            calculatespeedboolr = true;
                            maxr[0] = user1.Kneeposr[user1.Kneeposr.Count() - 1];
                            maxr[1] = timer;
                            user1.Kneeposr.RemoveAt(user1.Kneeposr.Count() - 2);
                        }
                        else // the next input is smaller than the one b4 .. 
                        {
                            if (calculatespeedboolr)
                            {
                                if ((maxr[0] - minr[0]) >= 0.3)
                                {
                                    speedr = Math.Abs(((maxr[0] - minr[0]) / (maxr[1] - minr[1])));
                                    if (speedr <= 4)
                                    {
                                        float[] speedlistr = new float[2];
                                        speedlistr[0] = speedr;
                                        speedlistr[1] = timer;//calculate the speed of the oscillation from min to max
                                        user1.Velocitylist.Add(speedlistr);
                                        user1.Positions.Add(speedr * this.avatarconst);
                                        // this.Environment1.MoveAvatar(1, (int)speedr * 15);
                                        this.environ1.bike1.Move(speedr * avatarconst);
                                        calculatespeedboolr = false;
                                    }
                                    else
                                    {
                                        calculatespeedboolr = false;
                                    }
                                }
                                else
                                {
                                    calculatespeedboolr = false;
                                }
                            }
                            minr[0] = user1.Kneeposr[user1.Kneeposr.Count() - 1];
                            minr[1] = timer;
                            user1.Kneeposr.RemoveAt(user1.Kneeposr.Count() - 2);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// converts left knee positions to velocity list.
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Safty</para>
        /// <para>DATE WRITTEN: 15/5/12 </para>
        /// <para>DATE MODIFIED: 24/5/12 </para>
        /// </remarks>
        public void getspeedleft2()
        {
            if (user2.Kneepos.Count() != 0)
            {
                if (user2.Kneepos.Count() == 1)
                {
                    min2[0] = user2.Kneepos[0]; //set the first input to be minimum
                    min2[1] = timer;
                }
                else
                {
                    if (user2.Kneepos[user2.Kneepos.Count() - 1] == user2.Kneepos[user2.Kneepos.Count() - 2]) // if the next input inlist equal the one b4 .. discard the one b4
                    {
                        if (user2.Kneepos[user2.Kneepos.Count() - 1] == max2[0])
                        {
                            max2[0] = user2.Kneepos[user2.Kneepos.Count() - 1];
                            max2[1] = timer;
                        }
                        if (user2.Kneepos[user2.Kneepos.Count() - 1] == min2[0])
                        {
                            min2[0] = user2.Kneepos[user2.Kneepos.Count() - 1];
                            min2[1] = timer;
                        }
                        user2.Kneepos.RemoveAt(user2.Kneepos.Count() - 2);
                    }
                    else
                    {
                        if (user2.Kneepos[user2.Kneepos.Count() - 1] > user2.Kneepos[user2.Kneepos.Count() - 2]) // if the next input greater than the one b4 .. set it to max
                        {
                            calculatespeedbool2 = true;
                            max2[0] = user2.Kneepos[user2.Kneepos.Count() - 1];
                            max2[1] = timer;
                            user2.Kneepos.RemoveAt(user2.Kneepos.Count() - 2);
                        }
                        else // the next input is smaller than the one b4 .. 
                        {
                            if (calculatespeedbool2)
                            {
                                if ((max2[0] - min2[0]) >= 0.3)
                                {
                                    speed2 = Math.Abs(((max2[0] - min2[0]) / (max2[1] - min2[1])));
                                    if (speed2 <= 4)
                                    {
                                        float[] speedlist2 = new float[2];
                                        speedlist2[0] = speed2;
                                        speedlist2[1] = timer;//calculate the speed of the oscillation from min to max
                                        user2.Velocitylist.Add(speedlist2);
                                        user2.Positions.Add(speed2 * this.avatarconst);
                                        this.environ1.bike2.Move(speed2 * avatarconst);
                                        calculatespeedbool2 = false;
                                    }
                                    else
                                    {
                                        calculatespeedbool2 = false;
                                    }
                                }
                                else
                                {
                                    calculatespeedbool2 = false;
                                }
                            }
                            min2[0] = user2.Kneepos[user2.Kneepos.Count() - 1];
                            min2[1] = timer;
                            user2.Kneepos.RemoveAt(user2.Kneepos.Count() - 2);
                        }
                    }
                }
            }
        }
       
        /// <summary>
        /// converts Right knee positions to velocity list.
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Safty</para>
        /// <para>DATE WRITTEN: 15/5/12 </para>
        /// <para>DATE MODIFIED: 24/5/12 </para>
        /// </remarks>
        public void getspeedright2()
        {

            if (user2.Kneeposr.Count() != 0)
            {
                if (user2.Kneeposr.Count() == 1)
                {
                    minr2[0] = user2.Kneeposr[0]; //set the first input to be minimum
                    minr2[1] = timer;
                }
                else
                {
                    if (user2.Kneeposr[user2.Kneeposr.Count() - 1] == user2.Kneeposr[user2.Kneeposr.Count() - 2]) // if the next input inlist equal the one b4 .. discard the one b4
                    {
                        if (user2.Kneeposr[user2.Kneeposr.Count() - 1] == maxr2[0])
                        {
                            maxr2[0] = user2.Kneeposr[user2.Kneeposr.Count() - 1];
                            maxr2[1] = timer;
                        }
                        if (user2.Kneeposr[user2.Kneeposr.Count() - 1] == minr2[0])
                        {
                            minr2[0] = user2.Kneeposr[user2.Kneeposr.Count() - 1];
                            minr2[1] = timer;
                        }
                        user2.Kneeposr.RemoveAt(user2.Kneeposr.Count() - 2);                       
                    }
                    else
                    {
                        if (user2.Kneeposr[user2.Kneeposr.Count() - 1] > user2.Kneeposr[user2.Kneeposr.Count() - 2]) // if the next input greater than the one b4 .. set it to max
                        {
                            calculatespeedboolr2 = true;
                            maxr2[0] = user2.Kneeposr[user2.Kneeposr.Count() - 1];
                            maxr2[1] = timer;
                            user2.Kneeposr.RemoveAt(user2.Kneeposr.Count() - 2);
                        }
                        else // the next input is smaller than the one b4 .. 
                        {
                            if (calculatespeedboolr2)
                            {
                                if ((maxr2[0] - minr2[0]) >= 0.3)
                                {
                                    speedr2 = Math.Abs(((maxr2[0] - minr2[0]) / (maxr2[1] - minr2[1])));
                                    if (speedr2 <= 4)
                                    {
                                        float[] speedlistr2 = new float[2];
                                        speedlistr2[0] = speedr2;
                                        speedlistr2[1] = timer;//calculate the speed of the oscillation from min to max
                                        user2.Velocitylist.Add(speedlistr2);
                                        user2.Positions.Add(speedr2 * this.avatarconst);
                                        this.environ1.bike2.Move(speedr2 * avatarconst);
                                        calculatespeedboolr2 = false;
                                    }
                                    else
                                    {
                                        calculatespeedboolr2 = false;
                                    }
                                }
                                else
                                {
                                    calculatespeedboolr2 = false;
                                }
                            }
                            minr2[0] = user2.Kneeposr[user2.Kneeposr.Count() - 1];
                            minr2[1] = timer;
                            user2.Kneeposr.RemoveAt(user2.Kneeposr.Count() - 2);
                        }
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Initializes the commulative time array and the shuffled race commands to be used.
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Safty</para>
        /// <para>DATE WRITTEN: 15/5/12 </para>
        /// <para>DATE MODIFIED: 24/5/12 </para>
        /// </remarks>
        public void Initialize_Timeslices()
        {
            for (int i = 0; i < 30; i++) //make commandlist
            {
                List<string> racecommandshuf = Exp1.gCommands;
                Tools1.commandshuffler<string>(racecommandshuf);
                racecommands = racecommands.Concat<string>(racecommandshuf).ToList<string>();
            }
            Random rand = new Random();
            foreach (string command in racecommands)
            {
                switch (command)
                {
                    case "constantDisplacement":
                        timeslice.Add(rand.Next(4, 6));
                        break;
                    case "constantAcceleration":
                        timeslice.Add(rand.Next(3, 5));
                        break;
                    case "increasingAcceleration":
                        timeslice.Add(rand.Next(3, 5));
                        break;
                    case "decreasingAcceleration":
                        timeslice.Add(rand.Next(4, 5));
                        break;
                    case "constantVelocity":
                        timeslice.Add(rand.Next(6, 9));
                        break;
                }
            }
            cumtimeslice.Add(0);
            cumtimeslice.Add(timeslice[0]);
            for (int i = 1; i < timeslice.Count(); i++)
            {
                cumtimeslice.Add(timeslice[i] + cumtimeslice[i]);
            }
        }

    }
}
