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
            var totalClearStats = ClearResolutionStats.Empty;
            bool isPlayable = BoardShuffler.IsPlayableBoard(board);
            if (!MoveValidator.IsValidSwap(board, move.From, move.To))
            {
                tracePhases.Add(new TurnPhaseTrace(TurnPhaseType.InvalidSwap));
                return CreateFailureResult(tracePhases, move, isPlayable, FailureReason.InvalidSwap, false,false, initialBoardSnapshot, initialBoardSnapshot);
            }
            
            var totalCleared = 0;
            var specialApplied = TryResolveDirectSpecialActivation(board, move, out var directClearStats, out var directSwapSnapshot, out var directActivations);
            
            if (specialApplied)
            {
                if (directSwapSnapshot != null)
                {
                    tracePhases.Add(new TurnPhaseTrace(
                        TurnPhaseType.SwapApplied,
                        boardAfterPhase: directSwapSnapshot));
                }

                totalClearStats = totalClearStats.Add(directClearStats);
                totalCleared = totalClearStats.TotalClearedCells;

                var directSnapshot = BoardSnapshotFactory.Capture(board);

                tracePhases.Add(new TurnPhaseTrace(
                    TurnPhaseType.DirectSpecialActivation,
                    directClearStats.TotalClearedCells,
                    boardAfterPhase: directSnapshot, specialActivations: directActivations));
            }
            
            bool createdMatch = false;
            if (!specialApplied)
            {
                if (!MoveFinder.CreatesMatch(board, move.From, move.To))
                {
                    tracePhases.Add(new TurnPhaseTrace(TurnPhaseType.NoMatchCreated));
                    return  CreateFailureResult(tracePhases, move, isPlayable, FailureReason.NoMatchCreated, true,
                        false, initialBoardSnapshot,  initialBoardSnapshot);
                }
                
                createdMatch = true;
                board.Swap(move.From, move.To);
                var directSnapshot = BoardSnapshotFactory.Capture(board);
                tracePhases.Add(new TurnPhaseTrace(TurnPhaseType.SwapApplied,  boardAfterPhase: directSnapshot));
                
                var primaryMatches = MatchFinder.FindMatches(board);
                var spawns = SpecialSpawnResolver.BuildDecisions(board, primaryMatches, move);
                var primaryStats = BoardResolver.ClearMatchesAndSpawnSpecials(board, primaryMatches, spawns, out var primaryActivations);
                totalClearStats = totalClearStats.Add(primaryStats);
                totalCleared = totalClearStats.TotalClearedCells;
                directSnapshot = BoardSnapshotFactory.Capture(board);
                tracePhases.Add(new TurnPhaseTrace(TurnPhaseType.PrimaryMatchResolved, primaryStats.TotalClearedCells, 
                    spawns.Count, boardAfterPhase: directSnapshot, specialSpawns: spawns, specialActivations: primaryActivations));
            }
            
            var initialFalls = BoardResolver.ApplyGravity(board);
            var gravitySnapshot = BoardSnapshotFactory.Capture(board);
            tracePhases.Add(new TurnPhaseTrace(TurnPhaseType.GravityApplied, boardAfterPhase: gravitySnapshot, falls: initialFalls));
            
            var initialRefills = BoardResolver.Refill(board, provider);
            var refillSnapshot = BoardSnapshotFactory.Capture(board);
            tracePhases.Add(new TurnPhaseTrace(TurnPhaseType.RefillApplied, boardAfterPhase: refillSnapshot));
            
            var cascadeResult = CascadeResolver.Resolve(board, provider,
                (cascadeIndex, clearedCells, waveSnapshot, waveSpawns, waveActivations, waveActivation) =>
                {
                    tracePhases.Add(
                        new TurnPhaseTrace(TurnPhaseType.CascadeWaveResolved, clearedCells: clearedCells, cascadeIndex: cascadeIndex,
                            boardAfterPhase: waveSnapshot, spawnedSpecials: waveSpawns.Count, specialSpawns: waveSpawns, specialActivations: waveActivations, cascadeAnimation: waveActivation));
                }, initialFalls, initialRefills);
            totalClearStats = totalClearStats.Add(cascadeResult.ClearStats);
            totalCleared = totalClearStats.TotalClearedCells;

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
                cascadeResult.CascadeCount, totalCleared, move, FailureReason.None, new TurnTrace(tracePhases), 
                initialBoardSnapshot, finalBoardSnapshot, totalClearStats);
        }
        
        private static TurnResult CreateFailureResult(List<TurnPhaseTrace> tracePhases, Move move, bool boardWasPlayableBeforeMove, 
            FailureReason failureReason, bool wasSwapValid, bool wasApplied, BoardSnapshot initialBoardSnapshot, BoardSnapshot finalBoardSnapshot)
        {
            return new TurnResult(wasSwapValid, wasApplied, false, false, boardWasPlayableBeforeMove,
                0, 0, move, failureReason, new TurnTrace(tracePhases), initialBoardSnapshot, finalBoardSnapshot);
        }

        private static bool TryResolveDirectSpecialActivation(BoardModel board, Move move, out ClearResolutionStats clearStats, out BoardSnapshot swapSnapshot, out IReadOnlyList<SpecialActivationEvent> activations)
        {
            activations = new List<SpecialActivationEvent>();
            swapSnapshot = null;
            clearStats = ClearResolutionStats.Empty;
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
                    var directActivations = new List<SpecialActivationEvent>()
                    {
                        new (move.From, pieceA, 0, allNonEmptyCells),
                        new (move.To, pieceB, 0, allNonEmptyCells)
                    };
                    activations = new List<SpecialActivationEvent>(directActivations).AsReadOnly();
                    clearStats = BoardResolver.ClearCells(board, allNonEmptyCells);
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
                
                return ClearDirectActivation(board, specialCell, otherCell, out clearStats, out activations);
            }

            if (aIsLineOrArea && bIsLineOrArea)
                return ClearDirectSpecialCombo(board, move, out clearStats, out activations);
            
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

            if (otherLineOrAreaPiece.IsEmpty)
                return false;

            if (otherLineOrAreaPiece.IsSpecial)
                return false;

            return ClearLineOrAreaWithNormalAfterSwap(board, move, lineOrAreaCell, out clearStats, out swapSnapshot, out activations);
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

        private static bool ClearDirectActivation(BoardModel board, Pos specialCell, Pos otherCell, out ClearResolutionStats clearStats, out IReadOnlyList<SpecialActivationEvent> activations)
        {
            var initialCells = new HashSet<Pos> { specialCell, otherCell };
            var finalCells = SpecialEffectResolver.CollectTriggeredCells(board, initialCells, out activations);
            clearStats = BoardResolver.ClearCells(board, finalCells);
            return true;
        }
        
        private static bool ClearLineOrAreaWithNormalAfterSwap(BoardModel board, Move move, Pos specialCellBeforeSwap,
            out ClearResolutionStats clearStats, out BoardSnapshot swapSnapshot, out IReadOnlyList<SpecialActivationEvent> activations)
        {
            clearStats = ClearResolutionStats.Empty;

            board.Swap(move.From, move.To);
            swapSnapshot = BoardSnapshotFactory.Capture(board);

            var specialFinalCell = specialCellBeforeSwap.Equals(move.From) ? move.To : move.From;
            var initialCells = new HashSet<Pos> { specialFinalCell };
            var finalCells = SpecialEffectResolver.CollectTriggeredCells(board, initialCells, out activations);

            clearStats = BoardResolver.ClearCells(board, finalCells);
            return true;
        }

        private static bool ClearDirectSpecialCombo(BoardModel board, Move move, out ClearResolutionStats clearStats, out IReadOnlyList<SpecialActivationEvent> activations)
        {
            var initialCellsToClear = new HashSet<Pos>{move.From,  move.To};
            var finalCellsToClear = SpecialEffectResolver.CollectTriggeredCells(board, initialCellsToClear, out activations);
            clearStats = BoardResolver.ClearCells(board, finalCellsToClear);
            return true;
        }
    }
}