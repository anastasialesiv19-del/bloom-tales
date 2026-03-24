using System.Collections.Generic;
using Core.Matching;
using Core.Specials;

namespace Core.Hint
{
    public static class HintFinder
    {
        public static HintResult FindBestHint(BoardModel board)
        {
            var possibleMoves = MoveFinder.FindAllPossibleMoves(board);
            if (possibleMoves.Count == 0) return new HintResult(null);

            var bestMove = possibleMoves[0];
            int bestScore = HintScorer.ScoreMove(board, bestMove);

            for (int i = 1; i < possibleMoves.Count; i++)
            {
                var move = possibleMoves[i];
                int score = HintScorer.ScoreMove(board, move);

                if (score <= bestScore)
                    continue;

                bestScore = score;
                bestMove = move;
            }
            
            var explanation = BuildExplanation(board, bestMove, bestScore);
            return new HintResult(bestMove, HintStrength.Strong, explanation);
        }

        public static HintResult FindAdaptiveHint(BoardModel board, HintContext context)
        {
            var strength = HintPolicy.GetStrength(context);
            if(strength ==  HintStrength.None) return new HintResult(null);
            var result = FindBestHint(board);
            return result.Move == null ? new HintResult(null) : new HintResult(result.Move, strength, result.Explanation);
        }

        private static HintExplanation BuildExplanation(BoardModel board, Move move, int bestScore)
        {
            var reasons = new List<HintReasonType>();
            if(IsDirectSpecialActivation(board, move))
                reasons.Add(HintReasonType.ActivatesSpecialDirectly);
            if (MoveFinder.CreatesMatch(board, move.From, move.To))
                reasons.Add(HintReasonType.CreatesMatch);
            if(CreatesSpecial(board, move))
                reasons.Add(HintReasonType.CreatesSpecial);
            return new HintExplanation(reasons, bestScore);
        }

        private static bool IsDirectSpecialActivation(BoardModel board, Move move)
        {
            var pieceA =  board.Get(move.From.Row, move.From.Col);
            var pieceB =  board.Get(move.To.Row, move.To.Col);


            
            var aIsLineOrArea = IsDirectLineOrArea(pieceA.Kind);
            var bIsLineOrArea = IsDirectLineOrArea(pieceB.Kind);
            var aIsColorBomb = IsDirectColorBomb(pieceA.Kind);
            var bIsColorBomb = IsDirectColorBomb(pieceB.Kind);

            if (aIsColorBomb || bIsColorBomb)
            {
                if (aIsColorBomb && bIsColorBomb)
                    return true;
                return pieceA.Color != null && pieceB.Color != null;
            }
            if (aIsLineOrArea && bIsLineOrArea)
                return true;
            if (aIsLineOrArea && !pieceB.IsSpecial)
                return true;
            if (bIsLineOrArea && !pieceA.IsSpecial)
                return true;
            return false;
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

        private static bool CreatesSpecial(BoardModel board, Move move)
        {
            if (!MoveFinder.CreatesMatch(board, move.From, move.To))
                return false;
            
            board.Swap(move.From, move.To);
            var matches = MatchFinder.FindMatches(board);
            var spawns = SpecialSpawnResolver.BuildDecisions(board, matches, move);
            board.Swap(move.From, move.To);
            
            return spawns.Count > 0;
        }
    }
}