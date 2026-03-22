using System;
using Core.Matching;
using NUnit.Framework;

namespace Tests.EditMode.Pieces
{
    public class PieceTest
    {
        [Test]
        public void Piece_Normal_HasKindNormal_AndColor()
        {
            var piece = Piece.Normal(PieceColor.Blue);
            Assert.AreEqual(PieceKind.Normal, piece.Kind);
            Assert.AreEqual(PieceColor.Blue, piece.Color);
            Assert.IsFalse(piece.IsEmpty);
            Assert.IsFalse(piece.IsSpecial);
        }

        [Test]
        public void Piece_Empty_HasKindEmpty_AndNoColor()
        {
            var piece = Piece.Empty();
            Assert.AreEqual(PieceKind.Empty, piece.Kind);
            Assert.IsNull(piece.Color);
            Assert.IsTrue(piece.IsEmpty);
            Assert.IsFalse(piece.IsSpecial);

        }

        [Test]
        public void Piece_ColorBomb_HasNoColor()
        {
            var piece = Piece.ColorBomb();
            Assert.AreEqual(PieceKind.ColorBomb, piece.Kind);
            Assert.IsNull(piece.Color);
            Assert.IsFalse(piece.IsEmpty);
            Assert.IsTrue(piece.IsSpecial);
        }

        [Test]
        public void Piece_EmptyWithColor_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new Piece(PieceKind.Empty, PieceColor.Red));
        }
        
        [Test]
        public void Piece_NormalWithoutColor_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new Piece(PieceKind.Normal, null));
        }
    }
}