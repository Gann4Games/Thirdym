using UnityEngine;

namespace Gann4Games.Thirdym.Utility
{
    public class TimerTool
    {
        public float CurrentTime { get; private set; }
        public float MaxTime { get; private set; }

        public TimerTool(float maxTime)
        {
            MaxTime = maxTime;
        }
        public void Count() => CurrentTime += Time.deltaTime;
        public void Reset() => CurrentTime = 0; 
        public void SetMaxTime(float newMaxTime) => MaxTime = newMaxTime;
        public bool HasFinished() => CurrentTime > MaxTime;
    }
}
