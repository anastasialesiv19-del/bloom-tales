using System;
using System.Collections.Generic;
using System.Linq;
using Core.Matching;
using Core.Specials;
using NUnit.Framework;

namespace Tests.EditMode.Specials
{
    public class SpecialEffectResolverTest
    {
        [Test]
        public void RowClear_AddsWholeRowToClearedCells()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.RowClear(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            List<Pos> initiallyClearedCells = new List<Pos>();
            initiallyClearedCells.Add(new Pos(2, 2));
            var result = SpecialEffectResolver.CollectTriggeredCells(board, initiallyClearedCells);
            
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Contains(new Pos(2,0)));
            Assert.IsTrue(result.Contains(new Pos(2,1)));
            Assert.IsTrue(result.Contains(new Pos(2,2)));
            Assert.IsTrue(result.Contains(new Pos(2,3)));
        }
        
        [Test]
        public void ColumnClear_AddsWholeColumnToClearedCells()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Green));
            board.Set(2, 2, Piece.ColumnClear(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Green));
            
            List<Pos> initiallyClearedCells = new List<Pos> { new Pos(2, 2) };
            var result = SpecialEffectResolver.CollectTriggeredCells(board, initiallyClearedCells);
            
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Contains(new Pos(0,2)));
            Assert.IsTrue(result.Contains(new Pos(1,2)));
            Assert.IsTrue(result.Contains(new Pos(2,2)));
        }
        
        [Test]
        public void AreaBomb_AddsNeighborAreaToClearedCells()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.AreaBomb(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            List<Pos> initiallyClearedCells = new List<Pos> { new Pos(1, 2) };
            var result = SpecialEffectResolver.CollectTriggeredCells(board, initiallyClearedCells);
            
            Assert.AreEqual(9, result.Count);
            Assert.IsTrue(result.Contains(new Pos(0,1)));
            Assert.IsTrue(result.Contains(new Pos(0,2)));
            Assert.IsTrue(result.Contains(new Pos(0,3)));
            Assert.IsTrue(result.Contains(new Pos(1,1)));
            Assert.IsTrue(result.Contains(new Pos(1,2)));
            Assert.IsTrue(result.Contains(new Pos(1,3)));
            Assert.IsTrue(result.Contains(new Pos(2,1)));
            Assert.IsTrue(result.Contains(new Pos(2,2)));
            Assert.IsTrue(result.Contains(new Pos(2,3)));
        }
        
        [Test]
        public void TriggeredSpecials_CanChainIntoOtherSpecials()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.AreaBomb(PieceColor.Green));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.RowClear(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.Normal(PieceColor.Green));
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            List<Pos> initiallyClearedCells = new List<Pos> { new Pos(1, 2) };
            var result = SpecialEffectResolver.CollectTriggeredCells(board, initiallyClearedCells);
            
            Assert.AreEqual(8, result.Count);
            Assert.IsTrue(result.Contains(new Pos(0,0)));
            Assert.IsTrue(result.Contains(new Pos(0,1)));
            Assert.IsTrue(result.Contains(new Pos(1,0)));
            Assert.IsTrue(result.Contains(new Pos(1,1)));
            Assert.IsTrue(result.Contains(new Pos(1,2)));
            Assert.IsTrue(result.Contains(new Pos(1,3)));
            Assert.IsTrue(result.Contains(new Pos(2,0)));
            Assert.IsTrue(result.Contains(new Pos(2,1)));
        }

        [Test]
        public void ColorBomb_AddsAllCellsOfTargetColor()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Blue));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.ColorBomb());
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            List<Pos> initiallyClearedCells = new List<Pos> { new Pos(2, 2) , new Pos(2,1)};
            var result = SpecialEffectResolver.CollectTriggeredCells(board, initiallyClearedCells);
            
            Assert.AreEqual(5, result.Count);
            Assert.IsTrue(result.Contains(new Pos(0,1)));
            Assert.IsTrue(result.Contains(new Pos(1,0)));
            Assert.IsTrue(result.Contains(new Pos(1,3)));
            Assert.IsTrue(result.Contains(new Pos(2,1)));
            Assert.IsTrue(result.Contains(new Pos(2,2)));
        }

        [Test]
        public void ColorBomb_CanTriggerColoredSpecialsOfThatColor()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.RowClear(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Blue));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Green));
            board.Set(1, 3, Piece.Normal(PieceColor.Blue));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.ColorBomb());
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            List<Pos> initiallyClearedCells = new List<Pos> { new Pos(2, 2) , new Pos(2,1)};
            var result = SpecialEffectResolver.CollectTriggeredCells(board, initiallyClearedCells);
            
            Assert.AreEqual(8, result.Count);
            Assert.IsTrue(result.Contains(new Pos(0,0)));
            Assert.IsTrue(result.Contains(new Pos(0,1)));
            Assert.IsTrue(result.Contains(new Pos(0,2)));
            Assert.IsTrue(result.Contains(new Pos(0,3)));
            Assert.IsTrue(result.Contains(new Pos(1,0)));
            Assert.IsTrue(result.Contains(new Pos(1,3)));
            Assert.IsTrue(result.Contains(new Pos(2,1)));
            Assert.IsTrue(result.Contains(new Pos(2,2)));
        }

        [Test]
        public void ColorBomb_Throws_WhenNoTargetColorCanBeResolved()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Yellow));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.ColorBomb());
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
            
            List<Pos> initiallyClearedCells = new List<Pos> { new Pos(0, 2) };
            Assert.Throws<InvalidOperationException>(() =>
            SpecialEffectResolver.CollectTriggeredCells(board, initiallyClearedCells));

        }

        [Test]
        public void ColorBomb_UsesAdjacentColoredPieceFromInitialClearSet()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Blue));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Red));
            board.Set(2, 1, Piece.Normal(PieceColor.Blue));
            board.Set(2, 2, Piece.ColorBomb());
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            List<Pos> initiallyClearedCells = new List<Pos> { new Pos(2, 2) , new Pos(2,3)};
            var result = SpecialEffectResolver.CollectTriggeredCells(board, initiallyClearedCells);

            Assert.AreEqual(5, result.Count);
            Assert.IsTrue(result.Contains(new Pos(0,0)));
            Assert.IsTrue(result.Contains(new Pos(0,3)));
            Assert.IsTrue(result.Contains(new Pos(2,0)));
            Assert.IsTrue(result.Contains(new Pos(2,2)));
            Assert.IsTrue(result.Contains(new Pos(2,3)));
            Assert.IsFalse(result.Contains(new Pos(0,1)));
            Assert.IsFalse(result.Contains(new Pos(1,0)));
        }

        [Test]
        public void ColorBomb_WithAdjacentRowClear_TargetsItsColorAndTriggersSameColorSpecials()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.RowClear(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Blue));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Red));
            board.Set(2, 1, Piece.RowClear(PieceColor.Blue));
            board.Set(2, 2, Piece.ColorBomb());
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            List<Pos> initiallyClearedCells = new List<Pos> { new Pos(2, 2) , new Pos(2,1)};
            var result = SpecialEffectResolver.CollectTriggeredCells(board, initiallyClearedCells);

            Assert.AreEqual(10, result.Count);
            Assert.IsTrue(result.Contains(new Pos(0,0)));
            Assert.IsTrue(result.Contains(new Pos(0,1)));
            Assert.IsTrue(result.Contains(new Pos(0,2)));
            Assert.IsTrue(result.Contains(new Pos(0,3)));
            Assert.IsTrue(result.Contains(new Pos(1,0)));
            Assert.IsTrue(result.Contains(new Pos(1,2)));
            Assert.IsTrue(result.Contains(new Pos(2,0)));
            Assert.IsTrue(result.Contains(new Pos(2,1)));
            Assert.IsTrue(result.Contains(new Pos(2,2)));
            Assert.IsTrue(result.Contains(new Pos(2,3)));
            
            Assert.IsFalse(result.Contains(new Pos(1,1)));
            Assert.IsFalse(result.Contains(new Pos(1,3)));
        }
        
        [Test]
        public void ColorBomb_WithAdjacentColumnClear_TargetsItsColorAndTriggersSameColorSpecials()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.ColumnClear(PieceColor.Blue));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Blue));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Red));
            board.Set(2, 1, Piece.ColumnClear(PieceColor.Blue));
            board.Set(2, 2, Piece.ColorBomb());
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            List<Pos> initiallyClearedCells = new List<Pos> { new Pos(2, 2) , new Pos(2,1)};
            var result = SpecialEffectResolver.CollectTriggeredCells(board, initiallyClearedCells);

            Assert.AreEqual(9, result.Count);
            Assert.IsTrue(result.Contains(new Pos(0,1)));
            Assert.IsTrue(result.Contains(new Pos(0,3)));
            Assert.IsTrue(result.Contains(new Pos(1,0)));
            Assert.IsTrue(result.Contains(new Pos(1,1)));
            Assert.IsTrue(result.Contains(new Pos(1,2)));
            Assert.IsTrue(result.Contains(new Pos(1,3)));
            Assert.IsTrue(result.Contains(new Pos(2,1)));
            Assert.IsTrue(result.Contains(new Pos(2,2)));
            Assert.IsTrue(result.Contains(new Pos(2,3)));
            
            Assert.IsFalse(result.Contains(new Pos(0,0)));
            Assert.IsFalse(result.Contains(new Pos(0,2)));
            Assert.IsFalse(result.Contains(new Pos(2,0)));
            
        }
        
        [Test]
        public void ColorBomb_WithAdjacentAreaBomb_TargetsItsColorAndTriggersSameColorSpecials()
        {
            var board = new BoardModel(3, 4);
            board.Set(0, 0, Piece.Normal(PieceColor.Red));
            board.Set(0, 1, Piece.Normal(PieceColor.Blue));
            board.Set(0, 2, Piece.Normal(PieceColor.Green));
            board.Set(0, 3, Piece.Normal(PieceColor.Red));
                            
            board.Set(1, 0, Piece.Normal(PieceColor.Blue));
            board.Set(1, 1, Piece.Normal(PieceColor.Yellow));
            board.Set(1, 2, Piece.Normal(PieceColor.Blue));
            board.Set(1, 3, Piece.Normal(PieceColor.Green));
                            
            board.Set(2, 0, Piece.Normal(PieceColor.Red));
            board.Set(2, 1, Piece.AreaBomb(PieceColor.Red));
            board.Set(2, 2, Piece.ColorBomb());
            board.Set(2, 3, Piece.Normal(PieceColor.Red));
            
            List<Pos> initiallyClearedCells = new List<Pos> { new Pos(2, 2) , new Pos(2,1)};
            var result = SpecialEffectResolver.CollectTriggeredCells(board, initiallyClearedCells);

            Assert.AreEqual(9, result.Count);
            Assert.IsTrue(result.Contains(new Pos(0,0)));
            Assert.IsTrue(result.Contains(new Pos(0,3)));
            Assert.IsTrue(result.Contains(new Pos(1,0)));
            Assert.IsTrue(result.Contains(new Pos(1,1)));
            Assert.IsTrue(result.Contains(new Pos(1,2)));
            Assert.IsTrue(result.Contains(new Pos(2,0)));
            Assert.IsTrue(result.Contains(new Pos(2,1)));
            Assert.IsTrue(result.Contains(new Pos(2,2)));
            Assert.IsTrue(result.Contains(new Pos(2,3)));
            
            Assert.IsFalse(result.Contains(new Pos(0,1)));
            Assert.IsFalse(result.Contains(new Pos(0,2)));
            Assert.IsFalse(result.Contains(new Pos(1,3)));
            
            
        }
    }
}