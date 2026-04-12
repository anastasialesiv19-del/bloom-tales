using System;
using System.Collections.Generic;
using System.Linq;
using Core.Contracts;
using Core.Turns;
using JetBrains.Annotations;

namespace Core.Presentation
{
    public sealed class TurnPresentationModel
    {
        public BoardSnapshot InitialBoard { get; }
        public BoardSnapshot FinalBoard { get; }
        public bool WasApplied { get; }
        public bool WasShuffled { get; }
        public bool CreatedMatch { get; }
        public int TotalClearedCells { get; }
        [CanBeNull] public BoardSnapshot SwapBoard { get; }
        [CanBeNull] public BoardSnapshot DirectActivationBoard { get; }
        [CanBeNull] public BoardSnapshot PrimaryResolveBoard { get; }
        [CanBeNull] public BoardSnapshot GravityBoard { get; }
        [CanBeNull] public BoardSnapshot RefillBoard { get; }
        [CanBeNull] public BoardSnapshot ShuffleBoard { get; }
        public IReadOnlyList<BoardSnapshot> CascadeBoards { get; }

        public TurnPresentationModel(BoardSnapshot initialBoard, BoardSnapshot finalBoard, bool wasApplied,
            bool wasShuffled,
            bool createdMatch, int totalClearedCells, BoardSnapshot swapBoard, BoardSnapshot directActivationBoard,
            BoardSnapshot primaryResolveBoard, BoardSnapshot gravityBoard, BoardSnapshot refillBoard,
            BoardSnapshot shuffleBoard, IReadOnlyList<BoardSnapshot> cascadeBoards)
        {
            InitialBoard = initialBoard ?? throw new ArgumentNullException("initialBoard", "InitialBoard is null");
            FinalBoard = finalBoard ?? throw new ArgumentNullException("finalBoard", "FinalBoard is null");
            WasApplied = wasApplied;
            WasShuffled = wasShuffled;
            CreatedMatch = createdMatch;
            TotalClearedCells = totalClearedCells;
            SwapBoard = swapBoard;
            DirectActivationBoard = directActivationBoard;
            PrimaryResolveBoard = primaryResolveBoard;
            GravityBoard = gravityBoard;
            RefillBoard = refillBoard;
            ShuffleBoard = shuffleBoard;
            CascadeBoards = cascadeBoards ?? throw new ArgumentNullException("cascadeBoards", "CascadeBoards is null");
        }
    }
}