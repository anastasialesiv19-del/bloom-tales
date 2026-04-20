using UnityEngine;

namespace Match3.Unity.GameMode
{
    public class MoveLimitController
    {
        public int RemainingMoves { get; private set; }
        public bool IsExpired { get; private set; }

        public void StartMoves(int totalMoves)
        {
            RemainingMoves = Mathf.Max(0,totalMoves);
            IsExpired = RemainingMoves <= 0;
        }

        public void ConsumeMove()
        {
            if (IsExpired) return;
            RemainingMoves--;
            if (RemainingMoves > 0) return;
            RemainingMoves = 0;
            IsExpired = true;
        }
        
        public void ResetMoves(int totalMoves)
        {
            StartMoves(totalMoves);
        }
    }
}