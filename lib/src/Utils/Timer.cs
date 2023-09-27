using UnityEngine;

namespace INUlib.Utils
{
    public class Timer
    {
        #region Fields
        private float m_startTime;
        #endregion

        #region Constructor
        public Timer() => Restart();
        public Timer(float elapsedTime) => m_startTime = Time.time - elapsedTime;
        #endregion

        #region Methods
        public void Restart() => m_startTime = Time.time;

        public float ElapsedTime() => Time.time - m_startTime;
        #endregion
    }
}