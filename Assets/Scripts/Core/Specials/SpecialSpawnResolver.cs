using System;
using System.Collections.Generic;
using Core.Matching;

namespace Core.Specials
{
    public static class SpecialSpawnResolver
    {
        public static IReadOnlyList<SpecialSpawnDecision> BuildDecisions(BoardModel board, IReadOnlyList<Match>  matches, Move? sourceMove = null)
        {
            var result = new List<SpecialSpawnDecision>();
            foreach (var match in matches)
            {
                Piece piece;
                var type = SpecialMatchDetector.ClassifyMatch(match);
                if(type == SpecialMatchType.None) continue;
                
                var pieceColor = board.Get(match.Cells[0].Row, match.Cells[0].Col).Color;
                if (pieceColor == null) throw new ArgumentException("Special spawn requires colored match piece.");
                var color = pieceColor.Value;

                piece = SpecialPieceFactory.CreateFromMatchType(type, color, 
                    type == SpecialMatchType.Line4 && SpecialMatchDetector.IsHorizontalLine(match.Cells));

                var pos = ResolveSpawnPosition(match, sourceMove);
                foreach (var item in result)
                    if (item.SpawnPosition.Equals(pos))
                        throw new InvalidOperationException("Two special spawns cannot target the same position.");
                
                result.Add(new SpecialSpawnDecision(pos, piece, match.Cells));
            }
            return result;
        }

        private static Pos ResolveSpawnPosition(Match match, Move? sourceMove)
        {
            if (sourceMove.HasValue)
            {
                if(ContainsPosition(match, sourceMove.Value.To))
                    return sourceMove.Value.To;
                if(ContainsPosition(match, sourceMove.Value.From))
                    return sourceMove.Value.From;
            }
            return GetDeterministicSpawnPosition(match);
        }

        private static Pos GetDeterministicSpawnPosition(Match match)
        {
            var bestPos = match.Cells[0];
            foreach (var pos in match.Cells)
            {
                if (pos.Row < bestPos.Row) 
                    bestPos = pos;
                else if (pos.Row == bestPos.Row && pos.Col < bestPos.Col) 
                    bestPos = pos;
            }
            
            return bestPos;
        }

        private static bool ContainsPosition(Match match, Pos pos)
        {
            foreach (var cell in match.Cells)
            {
                if(cell.Equals(pos)) return true;
            }
            return false;
        }
    }
}