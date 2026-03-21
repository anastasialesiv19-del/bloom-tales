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
        {
            clearedCount = 0;
            var finalCellsToClear = new HashSet<Pos>();
            var pieceA = board.Get(move.From.Row, move.From.Col);
            var pieceB = board.Get(move.To.Row, move.To.Col);
            var aSupported = IsDirectlyActivatableSpecial(pieceA.Kind);
            var bSupported = IsDirectlyActivatableSpecial(pieceB.Kind);
            if ((aSupported && bSupported) || (!aSupported && !bSupported)) return false;
            Pos specialCell, otherCell;
            if (aSupported)
            {
                specialCell = move.From;
                otherCell = move.To;
            }
            else
            {
                specialCell = move.To;
                otherCell = move.From;
            }

            finalCellsToClear.Add(specialCell);
            finalCellsToClear.Add(otherCell);
            var effectCells = SpecialEffectResolver.CollectTriggeredCells(board, finalCellsToClear);
            finalCellsToClear.UnionWith(effectCells);
            clearedCount = BoardResolver.ClearCells(board, finalCellsToClear);
            return true;   

        }

        private static bool IsDirectlyActivatableSpecial(PieceKind kind)
        {
            switch (kind)
            {
                case PieceKind.RowClear:
                case PieceKind.ColumnClear:
                case PieceKind.AreaBomb:
                    return true;
                default:
                    return false;
            }
        }
    }
}