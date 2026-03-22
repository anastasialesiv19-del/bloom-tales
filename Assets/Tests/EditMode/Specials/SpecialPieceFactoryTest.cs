using System;
using Core.Matching;
using Core.Specials;
using NUnit.Framework;

namespace Tests.EditMode.Specials
{
    public class SpecialPieceFactoryTest
    {
        [Test]
        public void CreateFromMatchType_Line4Horizontal_ReturnsRowClear()
        {
            var piece = SpecialPieceFactory.CreateFromMatchType(SpecialMatchType.Line4, PieceColor.Red, true);
            Assert.AreEqual(piece.Kind, PieceKind.RowClear);
            Assert.AreEqual(piece.Color, PieceColor.Red);
            Assert.IsTrue(piece.IsSpecial);
            Assert.IsFalse(piece.IsEmpty);
        }

        [Test]
        public void CreateFromMatchType_Line4Vertical_ReturnsColumnClear()
        {
            var piece = SpecialPieceFactory.CreateFromMatchType(SpecialMatchType.Line4, PieceColor.Blue, false);
            Assert.AreEqual(piece.Kind, PieceKind.ColumnClear);
            Assert.AreEqual(piece.Color, PieceColor.Blue);
            Assert.IsTrue(piece.IsSpecial);
            Assert.IsFalse(piece.IsEmpty);
        }

        [Test]
        public void CreateFromMatchType_Line5_ReturnsColorBomb()
        {
            var piece = SpecialPieceFactory.CreateFromMatchType(SpecialMatchType.Line5, PieceColor.Blue, false);
            Assert.AreEqual(piece.Kind, PieceKind.ColorBomb);
            Assert.IsNull(piece.Color);
            Assert.IsTrue(piece.IsSpecial);
            Assert.IsFalse(piece.IsEmpty);
        }

        [Test]
        public void CreateFromMatchType_TShape_ReturnsAreaBomb()
        {
            var piece = SpecialPieceFactory.CreateFromMatchType(SpecialMatchType.TShape, PieceColor.Green, false);
            Assert.AreEqual(piece.Kind, PieceKind.AreaBomb);
            Assert.AreEqual(piece.Color, PieceColor.Green);
            Assert.IsTrue(piece.IsSpecial);
            Assert.IsFalse(piece.IsEmpty);
        }

        [Test]
        public void CreateFromMatchType_LShape_ReturnsAreaBomb()
        {
            var piece = SpecialPieceFactory.CreateFromMatchType(SpecialMatchType.LShape, PieceColor.Green, false);
            Assert.AreEqual(piece.Kind, PieceKind.AreaBomb);
            Assert.AreEqual(piece.Color, PieceColor.Green);
            Assert.IsTrue(piece.IsSpecial);
            Assert.IsFalse(piece.IsEmpty);
        }

        [Test]
        public void CreateFromMatchType_None_Throws()
        {
            Assert.Throws<ArgumentException>(() => 
                SpecialPieceFactory.CreateFromMatchType(SpecialMatchType.None, PieceColor.Green, true));
            
        }
    }
}