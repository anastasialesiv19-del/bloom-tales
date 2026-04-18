using UnityEngine;

namespace Match3.Unity
{
    public class IdleTracker
    {
        public float IdleSeconds { get; private set; }

        public void Tick(float deltaTime)
        {
            IdleSeconds += deltaTime;
        }
        
        public void ResetIdle()
        {
            IdleSeconds = 0f;
        }
    }
}