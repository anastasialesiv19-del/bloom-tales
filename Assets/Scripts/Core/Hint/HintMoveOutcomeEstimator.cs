using System.Collections.Generic;
using System.Linq;
using Core.Board;
using Core.Matching;
using Core.Specials;

namespace Core.Hint
{
    public static class HintMoveOutcomeEstimator
    {
        public static HintMoveOutcome Estimate(BoardModel board, Move move)
        {
            if (!MoveValidator.IsValidSwap(board, move.From, move.To))
            {
                return HintMoveOutcome.Empty;
            }

            var directType = HintMoveEvaluator.GetDirectSpecialHintType(board, move);
            if (directType != DirectSpecialHintType.None)
            {
                return EstimateDirectSpecialMove(board, move, directType);
            }

            if (MoveFinder.CreatesMatch(board, move.From, move.To))
            {
                return EstimateMatchMove(board, move);
            }
            
            return HintMoveOutcome.Empty;
        }

        private static HintMoveOutcome EstimateMatchMove(BoardModel board, Move move)
        {
            return BoardSimulation.WithTemporarySwap(board, move, () =>
            {
                var matches = MatchFinder.FindMatches(board);
                var matchedCells = new HashSet<Pos>();

                foreach (var match in matches)
                {
                    foreach (var cell in match.Cells)
                    {
                        matchedCells.Add(cell);
                    }
                }
                var finalCells = SpecialEffectResolver.CollectTriggeredCells(board, matchedCells);
                return BuildOutcome(board, finalCells);
            });
        }

        private static HintMoveOutcome EstimateDirectSpecialMove(BoardModel board, Move move, DirectSpecialHintType type)
        {
            if (type == DirectSpecialHintType.ColorBombWithColorBomb)
            {
                var allNonEmptyCells = board.AllPositions().Where(pos => !board.Get(pos.Row, pos.Col).IsEmpty).ToArray();
                return BuildOutcome(board, allNonEmptyCells);
            }

            if (type == DirectSpecialHintType.ColorBombWithNormal || type == DirectSpecialHintType.ColorBombWithSpecial)
            {
                var initialCells = new HashSet<Pos> { move.From, move.To };
                var finalCells = SpecialEffectResolver.CollectTriggeredCells(board, initialCells);
                return BuildOutcome(board, finalCells);
            }

            if (type == DirectSpecialHintType.LineOrAreaWithNormal)
            {
                return EstimateLineOrAreaWithNormal(board, move);
            }
            return EstimateSpecialCombo(board, move);
        }

        private static HintMoveOutcome EstimateLineOrAreaWithNormal(BoardModel board, Move move)
        {
            return BoardSimulation.WithTemporarySwap(board, move, () =>
            {
                var pieceAtFrom = board.Get(move.From.Row, move.From.Col);
                var pieceAtTo = board.Get(move.To.Row, move.To.Col);
                Pos specialFinalCell;

                if (IsLineOrArea(pieceAtFrom.Kind))
                {
                    specialFinalCell = move.From;
                }
                else if (IsLineOrArea(pieceAtTo.Kind))
                {
                    specialFinalCell = move.To;
                }
                else
                {
                    return HintMoveOutcome.Empty;
                }

                var specialPiece = board.Get(specialFinalCell.Row, specialFinalCell.Col);
                var effectCells = GetDirectSpecialEffectCells(board, specialFinalCell, specialPiece.Kind).ToHashSet();
                var finalCells = SpecialEffectResolver.CollectTriggeredCells(board, effectCells);

                return BuildOutcome(board, finalCells);
            });
        }

        private static HintMoveOutcome EstimateSpecialCombo(BoardModel board, Move move)
        {
            var pieceA = board.Get(move.From.Row, move.From.Col);
            var pieceB = board.Get(move.To.Row, move.To.Col);
            var cells = new HashSet<Pos>();

            if (IsLineOrArea(pieceA.Kind))
            {
                cells.UnionWith(GetDirectSpecialEffectCells(board, move.From, pieceA.Kind));
            }
            if (IsLineOrArea(pieceB.Kind))
            {
                cells.UnionWith(GetDirectSpecialEffectCells(board, move.To, pieceB.Kind));
            }

            var finalCells = SpecialEffectResolver.CollectTriggeredCells(board, cells);
            return BuildOutcome(board, finalCells);
        }

        private static HintMoveOutcome BuildOutcome(BoardModel board, IReadOnlyCollection<Pos> clearedCells)
        {
            var clearedByColor = new Dictionary<PieceColor, int>();
            var clearedPieceCount = 0;

            foreach (var cell in clearedCells)
            {
                if (!board.InBounds(cell.Row, cell.Col))
                {
                    continue;
                }
                if (board.HasObstacle(cell.Row, cell.Col))
                {
                    continue;
                }

                var piece = board.Get(cell.Row, cell.Col);
                if (piece.IsEmpty)
                {
                    continue;
                }
    
                clearedPieceCount++;
                
                if (piece.Color == null)
                {
                    continue;
                }
                
                var color = piece.Color.Value;
                if (clearedByColor.ContainsKey(color))
                {
                    clearedByColor[color]++;
                }
                else
                {
                    clearedByColor[color] = 1;
                }
            }
            return new HintMoveOutcome(clearedByColor, CountAffectedObstacles(board, clearedCells), clearedPieceCount);
        }

        private static int CountAffectedObstacles(BoardModel board, IReadOnlyCollection<Pos> clearedCells)
        {
            var affectedObstacles = new HashSet<Pos>();
            foreach (var cell in clearedCells)
            {
                AddObstacleIfPresent(board, cell, affectedObstacles);
                AddObstacleIfPresent(board, new Pos(cell.Row + 1, cell.Col), affectedObstacles);
                AddObstacleIfPresent(board, new Pos(cell.Row - 1, cell.Col), affectedObstacles);
                AddObstacleIfPresent(board, new Pos(cell.Row, cell.Col + 1), affectedObstacles);
                AddObstacleIfPresent(board, new Pos(cell.Row, cell.Col - 1), affectedObstacles);
            }
            return affectedObstacles.Count;
        }

        private static void AddObstacleIfPresent(BoardModel board, Pos pos, HashSet<Pos> affectedObstacles)
        {
            if (!board.InBounds(pos.Row, pos.Col))
            {
                return;
            }
            
            if (board.HasObstacle(pos.Row, pos.Col))
            {
                affectedObstacles.Add(pos);
            }
        }

        private static IEnumerable<Pos> GetDirectSpecialEffectCells(BoardModel board, Pos cell, PieceKind kind)
        {
            if (kind == PieceKind.RowClear)
            {
                for (int col = 0; col < board.Width; col++)
                {
                    yield return new Pos(cell.Row, col);
                }
                yield break;
            }
            
            if (kind == PieceKind.ColumnClear)
            {
                for (int row = 0; row < board.Height; row++)
                {
                    yield return new Pos(row, cell.Col);
                }
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
            }
        }

        private static bool IsLineOrArea(PieceKind kind)
        {
            return kind is PieceKind.RowClear or PieceKind.ColumnClear or PieceKind.AreaBomb;
        }
    }
}