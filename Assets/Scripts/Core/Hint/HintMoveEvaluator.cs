using System.Linq;
using Core.Matching;

namespace Core.Hint
{
    public static class HintMoveEvaluator
    {
        public static bool IsDirectSpecialActivation(BoardModel board, Move move)
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
                return pieceA.Color != null || pieceB.Color != null;
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

        public static bool HasAnySpecialPiece(BoardModel board)
        {
            return board.AllPositions().Any(pos => board.Get(pos.Row, pos.Col).IsSpecial);
        }
    }
}