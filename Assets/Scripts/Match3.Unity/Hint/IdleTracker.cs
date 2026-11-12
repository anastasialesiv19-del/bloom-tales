namespace Match3.Unity.Hint
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