using Core.Turns;
using Core.Matching;
using System;
using System.Collections.Generic;
using Core.Randomness;
using JetBrains.Annotations;

namespace Tests.EditMode.TestDoubles
{
    public class SequenceRandomProvider : IRandomProvider
    {
        private readonly int[] _sequence;
        private int _currentIndex = 0;
        public SequenceRandomProvider([CanBeNull] int[] sequence)
        {
            if(sequence == null || sequence.Length == 0) throw new InvalidOperationException("sequence is null or empty");
            _sequence = sequence;
        }

        public int Next(int maxExclusive)
        {
            if(_currentIndex >= _sequence.Length) throw new InvalidOperationException("Sequence overflow");
            var value = _sequence[_currentIndex];
            _currentIndex++;
            return value < 0 || value >= maxExclusive ? throw new InvalidOperationException("Sequence overflow") : value;
        }

        public int Next(int minInclusive, int maxExclusive)
        {
            if(_currentIndex >= _sequence.Length) throw new InvalidOperationException("Sequence overflow");
            var value = _sequence[_currentIndex];
            _currentIndex++;
            return value < minInclusive || value >= maxExclusive ? throw new InvalidOperationException("Sequence overflow") : value;
        }
    }
}
namespace Tests.EditMode.TestDoubles
{
    public static class CascadeTestScenario
    {
        public static TurnResult ResolveTwoWaves()
        {
            var board = new BoardModel(3, 3);
            var colors = new[,]
            {
                { PieceColor.Red, PieceColor.Blue, PieceColor.Red },
                { PieceColor.Green, PieceColor.Red, PieceColor.Yellow },
                { PieceColor.Blue, PieceColor.Green, PieceColor.Yellow }
            };
            foreach (var pos in board.AllPositions())
                board.Set(pos.Row, pos.Col, Piece.Normal(colors[pos.Row, pos.Col]));

            // Red primary row -> green cascade -> blue cascade -> stable row.
            var provider = new SequenceRandomProvider(new[] { 2, 2, 2, 4, 4, 4, 0, 4, 0 });
            return TurnProcessor.TryProcessMove(board, new Move(new Pos(0, 1), new Pos(1, 1)), provider);
        }
    }
}

namespace Tests.EditMode.TestDoubles
{
    public sealed class SeededRandomProvider : IRandomProvider
    {
        private readonly Random random = new Random(12345);
        public int Next(int maxExclusive) => random.Next(maxExclusive);
        public int Next(int minInclusive, int maxExclusive) => random.Next(minInclusive, maxExclusive);
    }
}
