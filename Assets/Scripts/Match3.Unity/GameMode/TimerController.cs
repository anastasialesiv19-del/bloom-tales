using UnityEngine;

namespace Match3.Unity.GameMode
{
    public class TimerController
    {
        public float RemainingSeconds { get; private set; }
        public bool IsExpired { get; private set; }

        public void StartTimer(float totalSeconds)
        {
            RemainingSeconds = Mathf.Max(0f, totalSeconds);
            IsExpired = RemainingSeconds <= 0f;
        }

        public void Tick(float deltaTime)
        {
            if (IsExpired)
            {
                return;
            }
            
            RemainingSeconds -= deltaTime;
            if (!(RemainingSeconds <= 0f))
            {
                return;
            }
            
            RemainingSeconds = 0f;
            IsExpired = true;
        }

        public void StopTimer()
        {
            RemainingSeconds = 0f;
            IsExpired = true;
        }
    }
}