using Core.Matching;

namespace Core.Hint
{
    public static class HintMoveEvaluator
    {
        public static bool IsDirectSpecialActivation(BoardModel board, Move move)
        {
           return GetDirectSpecialHintType(board, move) != DirectSpecialHintType.None;
        }
        
        public static DirectSpecialHintType GetDirectSpecialHintType(BoardModel board, Move move)
        {
            var pieceA =  board.Get(move.From.Row, move.From.Col);
            var pieceB =  board.Get(move.To.Row, move.To.Col);
            
            if (pieceA.IsEmpty || pieceB.IsEmpty) return DirectSpecialHintType.None;
            if(!IsSpecial(pieceA) &&  !IsSpecial(pieceB)) return DirectSpecialHintType.None;

            if (IsColorBomb(pieceA.Kind) || IsColorBomb(pieceB.Kind))
            {
                if (IsColorBomb(pieceA.Kind) && IsColorBomb(pieceB.Kind))
                    return DirectSpecialHintType.ColorBombWithColorBomb;
                if ((IsSpecial(pieceA) && !IsColorBomb(pieceA.Kind)) || (IsSpecial(pieceB) && !IsColorBomb(pieceB.Kind)))
                    return DirectSpecialHintType.ColorBombWithSpecial;
                return DirectSpecialHintType.ColorBombWithNormal;
            }

            if (IsArea(pieceA.Kind) && IsArea(pieceB.Kind))
                return DirectSpecialHintType.AreaWithArea;

            if (IsLine(pieceA.Kind) && IsLine(pieceB.Kind))
                return DirectSpecialHintType.LineWithLine;
            
            if ((IsLine(pieceA.Kind) && IsArea(pieceB.Kind)) || (IsArea(pieceA.Kind) && IsLine(pieceB.Kind)))
                return DirectSpecialHintType.LineWithArea;
            
            if ((IsLineOrArea(pieceA.Kind) && !IsSpecial(pieceB)) || (IsLineOrArea(pieceB.Kind) && !IsSpecial(pieceA)))
                return DirectSpecialHintType.LineOrAreaWithNormal;
            
            return DirectSpecialHintType.None;
        }
        
        private static bool IsColorBomb(PieceKind kind)
        {
            return kind == PieceKind.ColorBomb;
        } 
        
        private static bool IsLine(PieceKind kind)
        {
            return kind is PieceKind.RowClear or PieceKind.ColumnClear;
        }
        
        private static bool IsLineOrArea(PieceKind kind)
        {
            return kind is PieceKind.RowClear or PieceKind.ColumnClear or PieceKind.AreaBomb;
        } 
        
        private static bool IsArea(PieceKind kind)
        {
            return kind == PieceKind.AreaBomb;
        }
        
        private static bool IsSpecial(Piece piece)
        {
            return piece.IsSpecial;
        }
    }
}