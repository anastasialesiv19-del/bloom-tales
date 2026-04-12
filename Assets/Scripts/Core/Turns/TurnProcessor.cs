using System;
using System.Collections.Generic;
using System.Linq;
using Core.Board;
using Core.Contracts;
using Core.Generation;
using Core.Matching;
using Core.Randomness;
using Core.Specials;

namespace Core.Turns
{
    public static class TurnProcessor
    {
        public static TurnResult TryProcessMove(BoardModel board, Move move)
        {
            return TryProcessMove(board, move, null);
        }

        public static TurnResult TryProcessMove(BoardModel board, Move move, IRandomProvider provider)
        {
            var initialBoardSnapshot = BoardSnapshotFactory.Capture(board);
            
            var tracePhases = new List<TurnPhaseTrace>();
            bool isPlayable = BoardShuffler.IsPlayableBoard(board);
            if (!MoveValidator.IsValidSwap(board, move.From, move.To))
            {
                tracePhases.Add(new TurnPhaseTrace(TurnPhaseType.InvalidSwap));
                return CreateFailureResult(tracePhases, move, isPlayable, FailureReason.InvalidSwap, false,false, initialBoardSnapshot, initialBoardSnapshot);
            }
            
            var specialApplied = TryResolveDirectSpecialActivation(board, move, out var totalCleared);
            if (specialApplied)
            {
                var directSnapshot = BoardSnapshotFactory.Capture(board);
                tracePhases.Add(new TurnPhaseTrace(TurnPhaseType.DirectSpecialActivation, totalCleared, boardAfterPhase: directSnapshot));
            }
            
            bool createdMatch = false;
            if (!specialApplied)
            {
                if (!MoveFinder.CreatesMatch(board, move.From, move.To))
                {
                    tracePhases.Add(new TurnPhaseTrace(TurnPhaseType.NoMatchCreated));
                    return  CreateFailureResult(tracePhases, move, isPlayable, FailureReason.NoMatchCreated, true,false, initialBoardSnapshot,  initialBoardSnapshot);
                }
                
                createdMatch = true;
                board.Swap(move.From, move.To);
                var directSnapshot = BoardSnapshotFactory.Capture(board);
                tracePhases.Add(new TurnPhaseTrace(TurnPhaseType.SwapApplied,  boardAfterPhase: directSnapshot));
                
                var primaryMatches = MatchFinder.FindMatches(board);
                var spawns = SpecialSpawnResolver.BuildDecisions(board, primaryMatches, move);
                var primaryCleared = BoardResolver.ClearMatchesAndSpawnSpecials(board, primaryMatches, spawns);
                totalCleared += primaryCleared;
                directSnapshot = BoardSnapshotFactory.Capture(board);
                tracePhases.Add(new TurnPhaseTrace(TurnPhaseType.PrimaryMatchResolved, primaryCleared, spawns.Count, boardAfterPhase: directSnapshot));
            }
            
            BoardResolver.ApplyGravity(board);
            var gravitySnapshot = BoardSnapshotFactory.Capture(board);
            tracePhases.Add(new TurnPhaseTrace(TurnPhaseType.GravityApplied, boardAfterPhase: gravitySnapshot));
            
            BoardResolver.Refill(board, provider);
            var refillSnapshot = BoardSnapshotFactory.Capture(board);
            tracePhases.Add(new TurnPhaseTrace(TurnPhaseType.RefillApplied, boardAfterPhase: refillSnapshot));
            
            var cascadeResult = CascadeResolver.Resolve(
                board, provider,
                (cascadeIndex, clearedCells, waveSnapshot) =>
                {
                    tracePhases.Add(
                        new TurnPhaseTrace(
                            TurnPhaseType.CascadeWaveResolved,
                            clearedCells: clearedCells,
                            cascadeIndex: cascadeIndex,
                            boardAfterPhase: waveSnapshot));
                });
            totalCleared += cascadeResult.TotalClearedCells;

            bool wasShuffled = false;
            if (!BoardShuffler.IsPlayableBoard(board))
            {
                BoardShuffler.Shuffle(board, provider);
                var shuffleSnapshot = BoardSnapshotFactory.Capture(board);
                wasShuffled = true;
                tracePhases.Add(new TurnPhaseTrace(TurnPhaseType.ShuffleApplied, boardShuffled: true, boardAfterPhase: shuffleSnapshot));
            }
            
            var finalBoardSnapshot = BoardSnapshotFactory.Capture(board);
            tracePhases.Add(new TurnPhaseTrace(TurnPhaseType.TurnCompleted));
                        return new TurnResult(true, true, wasShuffled, createdMatch, isPlayable,
                cascadeResult.CascadeCount, totalCleared, move, FailureReason.None, new TurnTrace(tracePhases), initialBoardSnapshot, finalBoardSnapshot);

        }
        
        private static TurnResult CreateFailureResult(List<TurnPhaseTrace> tracePhases, Move move, bool boardWasPlayableBeforeMove, FailureReason failureReason, bool wasSwapValid, bool wasApplied, BoardSnapshot initialBoardSnapshot, BoardSnapshot finalBoardSnapshot)
        {
            return new TurnResult(wasSwapValid, wasApplied, false, false, boardWasPlayableBeforeMove,
                0, 0, move, failureReason, new TurnTrace(tracePhases), initialBoardSnapshot, finalBoardSnapshot);
        }

        private static bool TryResolveDirectSpecialActivation(BoardModel board, Move move, out int clearedCount)
        { 
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
                if (aIsColorBomb && bIsColorBomb)
                {
                    var allNonEmptyCells = board.AllPositions().Where(pos => !board.Get(pos.Row, pos.Col).IsEmpty).ToArray();
                    clearedCount = BoardResolver.ClearCells(board, allNonEmptyCells);
                    return true;
                }
                
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

            if (aIsLineOrArea && bIsLineOrArea)
            {
                return ClearDirectSpecialCombo(board, move, out clearedCount);
            }
            
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

        private static bool ClearDirectSpecialCombo(BoardModel board, Move move, out int clearedCount)
        {
            clearedCount = 0;
            var pieceA = board.Get(move.From.Row, move.From.Col);
            var pieceB = board.Get(move.To.Row, move.To.Col);
            var cellsToClear = new HashSet<Pos>{move.From,  move.To};
            
            var cellsEffectA = GetDirectSpecialEffectCells(board, move.From, pieceA.Kind);
            var cellsEffectB = GetDirectSpecialEffectCells(board, move.To, pieceB.Kind);
            
            cellsToClear.UnionWith(cellsEffectA);
            cellsToClear.UnionWith(cellsEffectB);
            
            clearedCount = BoardResolver.ClearCells(board, cellsToClear);
            return true;
        }

        private static IEnumerable<Pos> GetDirectSpecialEffectCells(BoardModel board, Pos cell, PieceKind kind)
        {
            if (kind == PieceKind.RowClear)
            {
                for (int col = 0; col < board.Width; col++)
                    yield return new Pos(cell.Row, col);
                yield break;
            }
            if (kind == PieceKind.ColumnClear)
            {
                for(int row = 0;  row < board.Height; row++)
                    yield return new Pos(row, cell.Col);
                yield break;
            }
            if (kind == PieceKind.AreaBomb)
            {
                for (int row = cell.Row - 1; row <= cell.Row + 1; row++)
                {
                    for (int col = cell.Col - 1; col <= cell.Col + 1; col++)
                    {
                        if (board.InBounds(row, col))
                        {
                            yield return new Pos(row, col);
                        }
                    }
                }
                yield break;
            }
            throw new InvalidOperationException("Direct special effect is not supported for this piece kind.");
                    
            
        }
       
    }
}