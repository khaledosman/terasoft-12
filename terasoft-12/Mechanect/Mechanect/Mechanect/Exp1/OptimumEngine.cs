﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mechanect.Exp1
{
    public class OptimumEngine
    {
        /// <remarks>
        /// <para>Author: Ahmed Shirin</para>
        /// <para>Date Written 19/4/2012</para>
        /// <para>Date Modified 14/5/2012</para>
        /// </remarks>
        /// <summary>
        /// The function OptimumConstantVelocity derives the optimum values for the "constantVelocity" command.
        /// </summary>
        /// <param name="size">The number of frames assigned to the current command.</param> 
        /// <param name="g">An instance of the PerformanceGraph.</param>
        /// <returns>void</returns>
        public static void OptimumConstantVelocity(int size, PerformanceGraph g)
        {
            float velocity = g.getPreviousV();
            float x = g.getPreviousD() - velocity;
            for (int i = 0; i <= size - 1; i++)
            {
                if (x >= 0)
                {
                    g.getOptV().Add(velocity);
                    g.getOptD().Add(x);
                }
                else
                {
                    g.getOptV().Add(0);
                    g.getOptD().Add(0);
                }
                g.getOptA().Add(0);
                x = x - velocity;
            }
        }

        /// <remarks>
        /// <para>Author: Ahmed Shirin</para>
        /// <para>Date Written 19/4/2012</para>
        /// <para>Date Modified 22/5/2012</para>
        /// </remarks>
        /// <summary>
        /// The function OptimumConstantAcceleration derives the optimum values for the "constantAcceleration" command.
        /// </summary>
        /// <param name="size">The number of frames assigned to the current command.</param>  
        /// <param name="g">An instance of the PerformanceGraph.</param>
        /// <returns>void</returns>
        public static void OptimumConstantAcceleration(int round, int size, PerformanceGraph g)
        {
            float acceleration = g.getPreviousA();
            if (acceleration == 0)
            {
                acceleration = round * 0.5F;
            }
            float accumulator = g.getPreviousV();
            float z = g.getPreviousV() + acceleration;
            float x = g.getPreviousD() - z;
            for (int i = 0; i <= size - 1; i++)
            {
                if (x >= 0)
                {
                    g.getOptA().Add(acceleration);
                    g.getOptV().Add(z);
                    g.getOptD().Add(x);
                }
                else
                {
                    g.getOptA().Add(0);
                    g.getOptV().Add(0);
                    g.getOptD().Add(0);
                }
                z = z + acceleration;
                x = x - z;
            }
        }

        /// <remarks>
        /// <para>Author: Ahmed Shirin</para>
        /// <para>Date Written 19/4/2012</para>
        /// <para>Date Modified 22/5/2012</para>
        /// </remarks>
        /// <summary>
        /// The function OptimumIncreasingAcceleration derives the optimum values for the "increasingAcceleration" command.
        /// </summary>
        /// <param name="disq1">Player 1's disqualification time.</param>
        /// <param name="disq2">Player 2's disqualification time.</param>
        /// <param name="start">The command's initial frame number.</param>
        /// <param name="end">The command's final frame number.</param>
        /// <param name="accelerationTest1">A list representing Player 1's acceleration during the race.</param>
        /// <param name="accelerationTest2">A list representing Player 2's acceleration during the race.</param>
        /// <param name="g">An instance of the PerformanceGraph.</param>
        /// <returns>void</returns>
        public static void OptimumIncreasingAcceleration(int round, int size, PerformanceGraph g)
        {
            double value = round * 0.5;
            List<float> accelerationTrial = new List<float>();
            List<float> velocityTest = new List<float>();
            float accumulatorAcc = g.getPreviousA();
            float accumulatorVel = g.getPreviousV();
            float accumulatorDis = g.getPreviousD();
            float adder = (float)value;
            for (int i = 0; i <= size - 1; i++)
            {
                float z0 = accumulatorAcc + (float)value;
                accelerationTrial.Add(z0);
                float z1 = accumulatorVel + accelerationTrial[i];
                velocityTest.Add(z1);
                float z2 = accumulatorDis - (float)velocityTest[i];
                if (z2 >= 0)
                {
                    g.getOptA().Add((float)adder);
                    g.getOptD().Add(z2);
                    g.getOptV().Add(velocityTest[i]);
                }
                else
                {
                    g.getOptA().Add(0);
                    g.getOptD().Add(0);
                    g.getOptV().Add(0);
                }
                adder += (float)value;
                accumulatorAcc = z0;
                accumulatorVel = z1;
                accumulatorDis = z2;
            }
        }

        /// <remarks>
        /// <para>Author: Ahmed Shirin</para>
        /// <para>Date Written 19/4/2012</para>
        /// <para>Date Modified 14/5/2012</para>
        /// </remarks>
        /// <summary>
        /// The function OptimumDeccreasingAcceleration derives the optimum values for the "decreasingAcceleration" command.
        /// </summary>
        /// <param name="disq1">Player 1's disqualification time.</param>
        /// <param name="disq2">Player 2's disqualification time.</param>
        /// <param name="start">The command's initial frame number.</param>
        /// <param name="end">The command's final frame number.</param>
        /// <param name="accelerationTest1">A list representing Player 1's acceleration during the race.</param>
        /// <param name="accelerationTest2">A list representing Player 2's acceleration during the race.</param>
        /// <param name="g">An instance of the PerformanceGraph.</param>
        /// <returns>void</returns>
        public static void OptimumDecreasingAcceleration(int size, PerformanceGraph g)
        {
            double value = 0.1;
            List<float> accelerationTrial = new List<float>();
            List<float> velocityTest = new List<float>();
            float accumulatorAcc = g.getPreviousA();
            float accumulatorVel = g.getPreviousV();
            float accumulatorDis = g.getPreviousD();
            for (int i = 0; i <= size - 1; i++)
            {
                float z0 = accumulatorAcc - (float)value;
                if (z0 >= 0)
                {
                    accelerationTrial.Add(z0);
                }
                else
                {
                    accelerationTrial.Add(0);
                }
                float z1 = accumulatorVel + accelerationTrial[i];
                velocityTest.Add(z1);
                float z2 = accumulatorDis - (float)velocityTest[i];
                if (z2 >= 0)
                {
                    g.getOptA().Add(accelerationTrial[i]);
                    g.getOptD().Add(z2);
                    g.getOptV().Add(velocityTest[i]);
                }
                else
                {
                    g.getOptA().Add(0);
                    g.getOptD().Add(0);
                    g.getOptV().Add(0);
                }
                accumulatorAcc = z0;
                accumulatorVel = z1;
                accumulatorDis = z2;
            }
        }

        /// <remarks>
        /// <para>Author: Ahmed Shirin</para>
        /// <para>Date Written 19/4/2012</para>
        /// <para>Date Modified 14/5/2012</para>
        /// </remarks>
        /// <summary>
        /// The function OptimumConstantDisplacement derives the optimum values for the "constantDisplacement" command.
        /// </summary>
        /// <param name="size">The number of frames assigned to the current command.</param>
        /// <param name="g">An instance of the PerformanceGraph.</param>
        /// <returns>void</returns>
        public static void OptimumConstantDisplacement(int size, PerformanceGraph g)
        {
            for (int k = 0; k <= size - 1; k++)
            {
                g.getOptD().Add(g.getPreviousD());
                g.getOptV().Add(0);
                g.getOptA().Add(0);
            }
        }

        /// <remarks>
        /// <para>Author: Ahmed Shirin</para>
        /// <para>Date Created: 19/4/2012</para>
        /// <para>Date Modified: 14/5/2012</para>
        /// </remarks>
        /// <summary>
        /// The GetOptimum funciton is used to derive the optimum accelerations/velocities/displacements during the race by calling the necessary functions.
        /// </summary>
        /// <param name="player1disq">The instance when player 1 was disqualified.</param>
        /// <param name="player2disq">The instance when player 2 was disqualified.</param>
        /// <param name="g">An instance of the PerformanceGraph.</param>
        /// <returns>void</returns>
        public static void GetOptimum(int round, PerformanceGraph g)
        {
            int start = 0;
            int end = 0;
            g.setPreviousD(g.getTrackLength());
            g.setPreviousV(0);
            g.setPreviousA(0);
            g.getOptD().Add(g.getTrackLength());
            g.getOptV().Add(0);
            g.getOptA().Add(0);
            for (int i = 0; i <= g.getTimeSpaces().Count - 1; i++)
            {
                List<float> tempList = new List<float>();
                end = start + (int)(g.getTimeSpaces()[i]);
                int size = 0;
                for (int j = start; j <= end - 1; j++)
                {
                    size++;
                }
                if (g.getCommands()[i].Equals("constantVelocity"))
                {
                    OptimumConstantVelocity(size, g);
                }
                if (g.getCommands()[i].Equals("constantAcceleration"))
                {
                    OptimumConstantAcceleration(round, size, g);
                }
                if (g.getCommands()[i].Equals("increasingAcceleration"))
                {
                    OptimumIncreasingAcceleration(round, size, g);
                }
                if (g.getCommands()[i].Equals("decreasingAcceleration"))
                {
                    OptimumDecreasingAcceleration(size, g);
                }
                if (g.getCommands()[i].Equals("constantDisplacement"))
                {
                    OptimumConstantDisplacement(size, g);
                }
                g.setPreviousA(g.getOptA()[g.getOptA().Count - 1]);
                g.setPreviousV(g.getOptV()[g.getOptV().Count - 1]);
                g.setPreviousD(g.getOptD()[g.getOptD().Count - 1]);
                start = end;
                tempList.Clear();
            }      
            Choose(g);
            if (!CheckOptimum(g))
            {
                g.getOptD().Clear();
                g.getOptV().Clear();
                g.getOptA().Clear();
                GetOptimum(round + 1, g);
            }
        }

        /// <remarks>
        /// <para>Author: Ahmed Shirin</para>
        /// <para>Date Created: 24/5/2012</para>
        /// <para>Date Modified: 24/5/2012</para>
        /// </remarks>
        /// <summary>
        /// The function Choose is used to guarantee that the curve does not cover more/less than 25% of the screen width.
        /// </summary>
        /// <param name="g">Aninstance of the PerformanceGraph.</param>
        /// <returns>void.</returns>
        public static void Choose(PerformanceGraph g)
        {
            int size=g.getOptD().Count;
            if (size < 32)
            {
                float finalD = g.getOptD()[size - 1];
                float finalV = g.getOptV()[size - 1];
                float finalA = g.getOptA()[size - 1];
                int difference = 32 - size;
                for (int i = 0; i <= difference - 1; i++)
                {
                    g.getOptD().Add(finalD);
                    g.getOptV().Add(finalV);
                    g.getOptA().Add(finalA);
                }
            }
            if (size > 32)
            {
                int [] chosenTimings = new int[32];
                int timeCounter = 0;
                for (int i = 0; i <= chosenTimings.Length - 1; i++)
                {
                    chosenTimings[i] = (int)(32* ((double)timeCounter / (double)32));
                    timeCounter++;
                }
                List<float> temp1 = new List<float>();
                List<float> temp2 = new List<float>();
                List<float> temp3 = new List<float>();
                for (int i = 0; i <= chosenTimings.Length - 1; i++)
                {
                    temp1.Add(g.getOptD()[chosenTimings[i]]);
                    temp2.Add(g.getOptV()[chosenTimings[i]]);
                    temp3.Add(g.getOptA()[chosenTimings[i]]);
                }
                g.setOptD(temp1);
                g.setOptV(temp2);
                g.setOptA(temp3);
            }
        }

        /// <remarks>
        /// <para>Author: Ahmed Shirin</para>
        /// <para>Date Created: 22/5/2012</para>
        /// <para>Date Modified: 22/5/2012</para>
        /// </remarks>
        /// <summary>
        /// The function CheckOptimum is used to check whether the optimum player has reached the end line or not.
        /// </summary>
        /// <param name="g">An instance of the PerformanceGraph.</param>
        /// <returns>Boolean: A boolean representing the state of the optimum player.</returns>
        public static Boolean CheckOptimum(PerformanceGraph g)
        {
            Boolean x = false;
            if (g.getOptD()[g.getOptD().Count - 1] == 0)
            {
                x = true;
            }
            return x;
        }
    }
}
