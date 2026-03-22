using System.Collections.Generic;
using System.Linq;
using System.Net;
using Core.Board;
using Core.Generation;

namespace Core.Matching
{
    public static class TurnProcessor
    {
        public static TurnResult TryProcessMove(BoardModel board, Move move)
        {
            bool isPlayable = BoardShuffler.IsPlayableBoard(board);
            if (!MoveValidator.IsValidSwap(board, move.From, move.To))
                return new TurnResult(false, false, false, false, isPlayable,
                    0, 0, move, FailureReason.InvalidSwap);

            var specialApplied = TryResolveDirectSpecialActivation(board, move, out var totalCleared);
            bool createdMatch = false;
            if (!specialApplied)
            {
                if (!MoveFinder.CreatesMatch(board, move.From, move.To))
                    return new TurnResult(true, false, false, false, isPlayable,
                        0, 0, move, FailureReason.NoMatchCreated);

                createdMatch = true;
                board.Swap(move.From, move.To);

                var primaryMatches = MatchFinder.FindMatches(board);
                var spawns = SpecialSpawnResolver.BuildDecisions(board, primaryMatches, move);
                totalCleared += BoardResolver.ClearMatchesAndSpawnSpecials(board, primaryMatches, spawns);
            }

            BoardResolver.ApplyGravity(board);
            BoardResolver.Refill(board);

            var cascadeResult = CascadeResolver.Resolve(board);
            totalCleared += cascadeResult.TotalClearedCells;

            bool wasShuffled = false;
            if (!BoardShuffler.IsPlayableBoard(board))
            {
                BoardShuffler.Shuffle(board);
                wasShuffled = true;
            }

            return new TurnResult(true, true, wasShuffled, createdMatch, isPlayable,
                cascadeResult.CascadeCount, totalCleared, move, FailureReason.None);
        }

        private static bool TryResolveDirectSpecialActivation(BoardModel board, Move move, out int clearedCount)
        { // написать хелпер, шоб винести дубльовану логіку
            clearedCount = 0;
            var pieceA = board.Get(move.From.Row, move.From.Col);
            var pieceB = board.Get(move.To.Row, move.To.Col);

            var aIsLineOrArea = IsDirectLineOrArea(pieceA.Kind);
            var bIsLineOrArea = IsDirectLineOrArea(pieceB.Kind);
            var aIsColorBomb = IsDirectColorBomb(pieceA.Kind);
            var bIsColorBomb = IsDirectColorBomb(pieceB.Kind);

            var hasAnySupportedDirectSpecial = aIsLineOrArea || bIsLineOrArea || aIsColorBomb || bIsColorBomb;
            if (!hasAnySupportedDirectSpecial) return false;
            
            if (aIsColorBomb || bIsColorBomb)
            {
                if (aIsColorBomb && bIsColorBomb) return false;
                
                Pos specialCell, otherCell;
                Piece otherPiece;
                if (aIsColorBomb)
                {
                    specialCell = move.From;
                    otherCell = move.To; 
                    otherPiece = pieceB;
                }
                else
                {
                    specialCell = move.To;
                    otherCell = move.From;
                    otherPiece = pieceA;
                }

                if (otherPiece.IsEmpty) return false;
                if (otherPiece.Color == null) return false;
                
                return ClearDirectActivation(board, specialCell, otherCell, out clearedCount);
            }
            
            if (aIsLineOrArea && bIsLineOrArea) return false;
            Pos lineOrAreaCell, otherLineOrAreaCell;
            Piece otherLineOrAreaPiece;

            if (aIsLineOrArea)
            {
                lineOrAreaCell = move.From;
                otherLineOrAreaCell = move.To;
                otherLineOrAreaPiece = pieceB;
            }
            else
            {
                lineOrAreaCell = move.To;
                otherLineOrAreaCell = move.From;
                otherLineOrAreaPiece = pieceA;
            }

            if (otherLineOrAreaPiece.IsSpecial)
                return false;
            return ClearDirectActivation(board, lineOrAreaCell, otherLineOrAreaCell, out clearedCount);
        }

        private static bool IsDirectColorBomb(PieceKind kind)
        {
            return kind == PieceKind.ColorBomb;
        }

        private static bool IsDirectLineOrArea(PieceKind kind)
        {
            return kind switch
            {
                PieceKind.RowClear or PieceKind.ColumnClear or PieceKind.AreaBomb => true,
                _ => false
            };
        }

        private static bool ClearDirectActivation(BoardModel board, Pos specialCell, Pos otherCell,
            out int clearedCount)
        {
            var initialCells = new HashSet<Pos> { specialCell, otherCell };
            var finalCells = SpecialEffectResolver.CollectTriggeredCells(board, initialCells);
            clearedCount = BoardResolver.ClearCells(board, finalCells);
            return true;
        }
       
    }
}