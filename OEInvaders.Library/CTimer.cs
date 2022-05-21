// <copyright file="CTimer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable SA1401
namespace OEInvaders.Library
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Sorted list of timers.
    /// </summary>
    public class CTimer
    {
        /// <summary>
        /// Returns how many executes are left for the timer. Use 0 for infinite.
        /// </summary>
        public uint ExecutesLeft;

        /// <summary>
        /// If the Timer should handle exceptions with a try-catch-finally. Can be changed dynamically.
        /// </summary>
        public bool HandleException;

        /// <summary>
        /// The Action getting called by the Timer. Can be changed dynamically.
        /// </summary>
        public Action Func;

        /// <summary>
        /// After how many milliseconds (after the last execution) the timer should get called. Can be changed dynamically.
        /// </summary>
        public uint ExecuteAfterMs;

        private static readonly List<CTimer> TimerStorage = new List<CTimer>();
        private static readonly List<CTimer> InsertAfterList = new List<CTimer>();
        private static readonly object InsertListLocker = new object();
        private static Stopwatch Stopwatch = new Stopwatch();
        private ulong executeAtMs;
        private bool willberemoved = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CTimer"/> class.
        /// Constructor used to create the Timer.
        /// </summary>
        /// <param name="thefunc">The Action which you want to get called.</param>
        /// <param name="executeafterms">Execute the Action after milliseconds. If executes is more than one, this gets added to executeatms.</param>
        /// <param name="executeatms">Execute at milliseconds.</param>
        /// <param name="executes">How many times to execute. 0 for infinitely.</param>
        /// <param name="handleexception">If try-catch-finally should be used when calling the Action</param>
        private CTimer(Action thefunc, uint executeafterms, ulong executeatms, uint executes, bool handleexception)
        {
            this.Func = thefunc;
            this.ExecuteAfterMs = executeafterms;
            this.executeAtMs = executeatms;
            this.ExecutesLeft = executes;
            this.HandleException = handleexception;
        }

        /// <summary>
        /// Starts the tick counter.
        /// </summary>
        public static void StartWatching()
        {
            Stopwatch.Start();
        }

        /// <summary>
        /// Runs on every frame.
        /// </summary>
        public static void OnUpdate()
        {
            ulong tick = GetTick();
            for (int i = TimerStorage.Count - 1; i >= 0; i--)
            {
                if (!TimerStorage[i].willberemoved)
                {
                    if (TimerStorage[i].executeAtMs <= tick)
                    {
                        CTimer thetimer = TimerStorage[i];
                        TimerStorage.RemoveAt(i);
                        if (thetimer.HandleException)
                        {
                            thetimer.ExecuteMeSafe();
                        }
                        else
                        {
                            thetimer.ExecuteMe();
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    TimerStorage.RemoveAt(i);
                }
            }

            lock (InsertListLocker)
            {
                if (InsertAfterList.Count > 0)
                {
                    bool newv = false;

                    // It would take a lot of time to reach long's length, but let's make sure we are infinite. =)
                    if (GetTick() > 120000000)
                    {
                        Stopwatch.Stop();
                        Stopwatch = new Stopwatch();
                        Stopwatch.Start();
                        newv = true;
                    }

                    foreach (CTimer timer in InsertAfterList)
                    {
                        if (newv)
                        {
                            timer.executeAtMs = timer.ExecuteAfterMs;
                        }

                        timer.InsertSorted();
                    }

                    InsertAfterList.Clear();
                }
            }
        }

        /// <summary>
        /// Use this method to create the Timer.
        /// </summary>
        /// <param name="thefunc">The Action which you want to get called.</param>
        /// <param name="executeafterms">Execute after milliseconds.</param>
        /// <param name="executes">Amount of executes. Use 0 for infinitely.</param>
        /// <param name="handleexception">If try-catch-finally should be used when calling the Action</param>
        /// <returns>CTimer</returns>
        public static CTimer SetTimer(Action thefunc, uint executeafterms, uint executes = 1, bool handleexception = true)
        {
            ulong executeatms = executeafterms + GetTick();
            CTimer thetimer = new CTimer(thefunc, executeafterms, executeatms, executes, handleexception);
            lock (InsertListLocker)
            {
                InsertAfterList.Add(thetimer);
            }

            return thetimer;
        }

        /// <summary>
        /// Returns the current tickcount.
        /// </summary>
        /// <returns>tickcount.</returns>
        private static ulong GetTick()
        {
            return (ulong)Stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Gets a value indicating whether if the timer is running.
        /// </summary>
        public bool IsRunning
        {
            get { return !this.willberemoved; }
        }

        /// <summary>
        /// Kills the timer.
        /// </summary>
        public void Kill()
        {
            this.willberemoved = true;
        }

        /// <summary>
        /// Executes the action in safe block, or unsafe block.
        /// </summary>
        /// <param name="changeexecutems">Changes the interval.</param>
        public void Execute(bool changeexecutems = true)
        {
            if (changeexecutems)
            {
                this.executeAtMs = GetTick();
            }

            if (this.HandleException)
            {
                this.ExecuteMeSafe();
            }
            else
            {
                this.ExecuteMe();
            }
        }

        /// <summary>
        /// Runs when the action is executed.
        /// </summary>
        private void ExecuteMe()
        {
            this.Func();
            if (this.ExecutesLeft == 1)
            {
                this.ExecutesLeft = 0;
                this.willberemoved = true;
            }
            else
            {
                if (this.ExecutesLeft != 0)
                {
                    this.ExecutesLeft--;
                }

                this.executeAtMs += this.ExecuteAfterMs;
                lock (InsertListLocker)
                {
                    InsertAfterList.Add(this);
                }
            }
        }

        /// <summary>
        /// Runs when the action is executed. (In a safe block)
        /// </summary>
        private void ExecuteMeSafe()
        {
            try
            {
                this.Func();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[CTimer] Failed at calling " + this.Func.Method.Name + " Error: " + ex);
            }
            finally
            {
                if (this.ExecutesLeft == 1)
                {
                    this.ExecutesLeft = 0;
                    this.willberemoved = true;
                }
                else
                {
                    if (this.ExecutesLeft != 0)
                    {
                        this.ExecutesLeft--;
                    }

                    this.executeAtMs += this.ExecuteAfterMs;
                    lock (InsertListLocker)
                    {
                        InsertAfterList.Add(this);
                    }
                }
            }
        }

        /// <summary>
        /// Inserts the data sorted.
        /// </summary>
        private void InsertSorted()
        {
            bool putin = false;
            for (int i = TimerStorage.Count - 1; i >= 0 && !putin; i--)
            {
                if (this.executeAtMs <= TimerStorage[i].executeAtMs)
                {
                    TimerStorage.Insert(i + 1, this);
                    putin = true;
                }
            }

            if (!putin)
            {
                TimerStorage.Insert(0, this);
            }
        }
    }
}