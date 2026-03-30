using System;
using System.Collections.Generic;
using Core.Board;
using Core.Matching;

namespace Core.Generation
{
    public static class BoardShuffler
    {
        private static readonly Random _random = new Random();
        
        public static void Shuffle(BoardModel board)
        {
            if(BoardResolver.HasEmptyCells(board)) throw new InvalidOperationException("Shuffle only works on a full board without empty cells.");
            
            var allPieces = new List<Piece>();
            foreach (var pos in board.AllPositions())
                allPieces.Add(board.Get(pos.Row, pos.Col));
            
            int maxAttempts = 100;
            while (maxAttempts > 0)
            {
                var shuffledPieces = new List<Piece>(allPieces);
                int n = shuffledPieces.Count;
                while (n > 1)
                {
                    n--;
                    int k = _random.Next(0, n + 1);
                    (shuffledPieces[k], shuffledPieces[n]) =  (shuffledPieces[n], shuffledPieces[k]);
                }

                int index = 0;
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