using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Worlds
{
    public class Timer
    {
        #region Data members
        private long mLastTick;
        private float mTimeElapsed;
        private float mTimeDelta;
        private float mTimeSpeed;
        #endregion

        #region Constructor
        public Timer()
        {
            Reset();
            mTimeSpeed = 1.0f;
        }
        #endregion

        #region Properties
        public float TimeElapsed
        {
            get { return mTimeElapsed; }
            set { mTimeElapsed = value; }
        }

        public float TimeDelta
        {
            get { return mTimeDelta; }
            set { mTimeDelta = value; }
        }

        public float TimeSpeed
        {
            get { return mTimeSpeed; }
            set { mTimeSpeed = value; }
        }
        #endregion

        #region Methods
        public void Reset()
        {
            mLastTick = DateTime.Now.Ticks;
            mTimeElapsed = 0.0f;
            mTimeDelta = 0.0f;
        }

        public void Tick()
        {
            long currentTick = DateTime.Now.Ticks;
            long tickDelta = currentTick - mLastTick;
            mTimeDelta = (float)tickDelta * 0.0000001f * mTimeSpeed;
            mTimeElapsed += mTimeDelta;
            mLastTick = currentTick;
        }

        public void PassiveTick()
        {
            long mLastTick = DateTime.Now.Ticks;
            mTimeDelta = 0.0f;
        }
        #endregion
    }
}
