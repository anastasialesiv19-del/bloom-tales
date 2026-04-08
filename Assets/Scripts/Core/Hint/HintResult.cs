using System;
using Core.Contracts;
using Core.Matching;
using JetBrains.Annotations;

namespace Core.Hint
{
    public readonly struct HintResult
    {
        public Move? Move { get; }
        public  HintStrength Strength { get; }
        [CanBeNull] public HintExplanation Explanation { get; }
        public BoardSnapshot BoardSnapshot { get; }

        public HintResult(Move? move, BoardSnapshot boardSnapshot)
        {
            Move = move;
            Strength = HintStrength.None;
            Explanation = null;
            BoardSnapshot = boardSnapshot ?? throw new ArgumentNullException(nameof(boardSnapshot));
        }
        
        public HintResult(Move? move,  HintStrength strength, HintExplanation explanation, BoardSnapshot boardSnapshot)
        {
            Move = move;
            Strength = move == null ? HintStrength.None : strength;
            if(move != null && explanation == null) throw new ArgumentNullException(nameof(explanation));
            Explanation = move == null ? null : explanation;
            BoardSnapshot = boardSnapshot ?? throw new ArgumentNullException(nameof(boardSnapshot));
        }
    }
}