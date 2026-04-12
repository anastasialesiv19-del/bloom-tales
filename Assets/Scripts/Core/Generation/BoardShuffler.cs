using System;
using System.Collections.Generic;
using System.Linq;
using Core.Board;
using Core.Matching;
using Core.Randomness;

namespace Core.Generation
{
    public static class BoardShuffler
    {
        public static void Shuffle(BoardModel board, IRandomProvider provider = null)
        {
            if(BoardResolver.HasEmptyCells(board)) throw new InvalidOperationException("Shuffle only works on a full board without empty cells.");
            
            var allPieces = board.AllPositions().Select(pos => board.Get(pos.Row, pos.Col)).ToList();

            var rng = provider ?? new SystemRandomProvider();
            
            var maxAttempts = 100;
            while (maxAttempts > 0)
            {
                var shuffledPieces = new List<Piece>(allPieces);
                var n = shuffledPieces.Count;
                while (n > 1)
                {
                    n--;
                    var k = rng.Next(0, n + 1);
                    (shuffledPieces[k], shuffledPieces[n]) =  (shuffledPieces[n], shuffledPieces[k]);
                }

                var index = 0;
                foreach (var pos in board.AllPositions())
                {
                    board.Set(pos.Row, pos.Col, shuffledPieces[index]);
                    index++;
                }

                if (IsPlayableBoard(board)) return;
                maxAttempts--;
            }
            throw new InvalidOperationException("Max attempts reached");
        }
        
        public static bool IsPlayableBoard(BoardModel board)
        {
            bool noMatches = MatchFinder.FindMatches(board).Count == 0;
            bool hasAnyPossibleMove = MoveFinder.HasAnyPossibleMove(board);
            bool hasAnyEmptyPiece = BoardResolver.HasEmptyCells(board);
            return noMatches && hasAnyPossibleMove && !hasAnyEmptyPiece;
        }
    }
}