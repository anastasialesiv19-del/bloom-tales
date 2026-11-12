using System;
using System.Linq;
using System.Collections.Generic;
using Core.Board;
using Core.Matching;

namespace Core.Specials
{
    public static class SpecialSpawnResolver
    {
        public static IReadOnlyList<SpecialSpawnDecision> BuildDecisions(BoardModel board, IReadOnlyList<Match>  matches, Move? sourceMove = null, 
            IReadOnlyList<PieceFall> fallsBeforeWave = null, IReadOnlyList<Pos> refillsBeforeWave = null)
        {
            var result = new List<SpecialSpawnDecision>();
            foreach (var match in matches)
            {
                var type = SpecialMatchDetector.ClassifyMatch(match);
                if (type == SpecialMatchType.None)
                {
                    continue;
                }
                
                var pieceColor = board.Get(match.Cells[0].Row, match.Cells[0].Col).Color;
                if (pieceColor == null)
                {
                    throw new ArgumentException("Special spawn requires colored match piece.");
                }
                var color = pieceColor.Value;

                var piece = SpecialPieceFactory.CreateFromMatchType(type, color, type == SpecialMatchType.Line4 && SpecialMatchDetector.IsHorizontalLine(match.Cells));

                var pos = ResolveSpawnPosition(match, sourceMove, fallsBeforeWave, refillsBeforeWave);
                foreach (var item in result)
                {
                    if (item.SpawnPosition.Equals(pos))
                    {
                        throw new InvalidOperationException("Two special spawns cannot target the same position.");
                    }
                }
                result.Add(new SpecialSpawnDecision(pos, piece, match.Cells));
            }
            return result;
        }

        private static Pos ResolveSpawnPosition(Match match, Move? sourceMove, IReadOnlyList<PieceFall> fallsBeforeWave, IReadOnlyList<Pos> refillsBeforeWave)
        {
            if (sourceMove.HasValue)
            {
                if (ContainsPosition(match, sourceMove.Value.To))
                {
                    return sourceMove.Value.To;
                }
                if (ContainsPosition(match, sourceMove.Value.From))
                {
                    return sourceMove.Value.From;
                }
            }

            var refillPieces = new List<Pos>();
            if (refillsBeforeWave != null)
            {
                foreach (var pos in refillsBeforeWave)
                {
                    if (ContainsPosition(match, pos))
                    {
                        refillPieces.Add(pos);
                    }
                }

                if (refillPieces.Count > 0)
                {
                    var posSpawnPiece = refillPieces.OrderBy(p => p.Row).ThenBy(p => p.Col).FirstOrDefault();
                    return posSpawnPiece;
                }
            }
            
            var fallPieceTo = new List<Pos>();
            var greatestDistance = -1;
            if (fallsBeforeWave != null)
            {
                foreach(var piece in fallsBeforeWave)
                {
                    if (ContainsPosition(match, piece.To))
                    {
                        var distance = piece.To.Row - piece.From.Row;
                        if (distance > greatestDistance)
                        {
                            fallPieceTo.Clear();
                            greatestDistance = distance;
                            fallPieceTo.Add(piece.To);
                        }
                        else if (distance == greatestDistance)
                        {
                            fallPieceTo.Add(piece.To);
                        }
                    }
                }

                if (fallPieceTo.Count == 1)
                {
                    return fallPieceTo[0];
                }
                
                if (fallPieceTo.Count > 1)
                {
                    var posSpawnPiece = fallPieceTo.OrderBy(p => p.Row).ThenBy(p => p.Col).FirstOrDefault();
                    return posSpawnPiece;
                }
            }
            return GetDeterministicSpawnPosition(match);
        }

        private static Pos GetDeterministicSpawnPosition(Match match)
        {
            var bestPos = match.Cells[0];
            foreach (var pos in match.Cells)
            {
                if (pos.Row < bestPos.Row)
                {
                    bestPos = pos;
                }
                else if (pos.Row == bestPos.Row && pos.Col < bestPos.Col)
                {
                    bestPos = pos;
                }
            }
            return bestPos;
        }

        private static bool ContainsPosition(Match match, Pos pos)
        {
            foreach (var cell in match.Cells)
            {
                if (cell.Equals(pos))
                {
                    return true;
                }
            }
            return false;
        }
    }
}