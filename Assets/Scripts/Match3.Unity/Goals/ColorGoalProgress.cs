using Core.Matching;

namespace Match3.Unity.Goals
{
    public class ColorGoalProgress
    {
        public PieceColor Color { get; }
        public int Current { get; private set; }
        public int Target { get; }

        public ColorGoalProgress(PieceColor color, int target)
        {
            Color = color;
            Target = target;
            Current = 0;
        }

        public void AddApprox(int clearedCells)
        {
            Current += clearedCells / 2;
        }
        
        public bool IsCompleted =>  Current >= Target;
    }
}