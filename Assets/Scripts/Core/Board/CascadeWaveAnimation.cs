using System;
using System.Collections.Generic;
using Core.Contracts;

namespace Core.Board
{
    public sealed class CascadeWaveAnimation
    {
        public BoardSnapshot AfterClear { get; }
        public BoardSnapshot AfterGravity { get; }
        public IReadOnlyList<PieceFall> Falls { get; }

        public CascadeWaveAnimation(BoardSnapshot afterClear, BoardSnapshot afterGravity, IReadOnlyList<PieceFall> falls)
        {
            AfterClear = afterClear ?? throw new ArgumentNullException(nameof(afterClear));
            AfterGravity = afterGravity ?? throw new ArgumentNullException(nameof(afterGravity));
            Falls = falls == null ? Array.Empty<PieceFall>() : new List<PieceFall>(falls).AsReadOnly();
        }
    }
}