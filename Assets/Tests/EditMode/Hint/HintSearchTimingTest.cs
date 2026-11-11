using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Core.Board;
using Core.Hint;
using Core.Matching;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode.Hint
{
    public class HintSearchTimingTest
    {
        [TestCase("Normal")]
        [TestCase("Specials")]
        [TestCase("ObstaclesAndGoals")]
        [Category("Timing")]
        public void MeasureFindBestHint(string scenario)
        {
            const int warmupCount = 10;
            const int sampleCount = 100;
            var board = CreateBoard();
            var context = HintGoalContext.Empty;

            if (scenario == "Specials")
            {
                board.Set(3, 3, Piece.RowClear(board.Get(3, 3).Color.Value));
                board.Set(5, 5, Piece.ColumnClear(board.Get(5, 5).Color.Value));
                board.Set(6, 2, Piece.AreaBomb(board.Get(6, 2).Color.Value));
            }
            else if (scenario == "ObstaclesAndGoals")
            {
                AddBox(board, 2, 2);
                AddBox(board, 4, 4);
                AddBox(board, 6, 6);
                context = new HintGoalContext(
                    new Dictionary<PieceColor, int> { { PieceColor.Red, 5 } },
                    remainingObstacles: 3, remainingScore: 10);
            }

            Assert.AreEqual(0, MatchFinder.FindMatches(board).Count,
                "The initial board must contain no matches.");
            var candidates = HintMoveCollector.Collect(board).Count;
            Assert.Greater(candidates, 0);

            for (int i = 0; i < warmupCount; i++)
                Assert.IsTrue(HintFinder.FindBestHint(board, context).Move.HasValue);

            var samples = new double[sampleCount];
            var stopwatch = new Stopwatch();
            for (int i = 0; i < sampleCount; i++)
            {
                stopwatch.Restart();
                var result = HintFinder.FindBestHint(board, context);
                stopwatch.Stop();
                samples[i] = stopwatch.ElapsedTicks * 1000.0 / Stopwatch.Frequency;
                Assert.IsTrue(result.Move.HasValue);
            }

            double total = 0;
            double maximum = 0;
            foreach (var sample in samples)
            {
                total += sample;
                if (sample > maximum) maximum = sample;
            }

            TestContext.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "Scenario={0}; Board=8x8; Candidates={1}; Warmup={2}; Samples={3}; MeanMs={4:F4}; MaxMs={5:F4}; AllAtMost100Ms={6}",
                scenario, candidates, warmupCount, sampleCount,
                total / sampleCount, maximum, maximum <= 100.0));
            TestContext.WriteLine("Unity=" + Application.unityVersion);
            TestContext.WriteLine("OS=" + SystemInfo.operatingSystem);
            TestContext.WriteLine("CPU=" + SystemInfo.processorType);
            TestContext.WriteLine("RAM_MB=" + SystemInfo.systemMemorySize);
        }

        private static BoardModel CreateBoard()
        {
            var colors = new[]
            {
                PieceColor.Red, PieceColor.Blue, PieceColor.Green, PieceColor.Yellow
            };
            var board = new BoardModel(8, 8);
            for (int row = 0; row < 8; row++)
                for (int col = 0; col < 8; col++)
                    board.Set(row, col, Piece.Normal(colors[(row + col) % colors.Length]));

            // Swapping (0,1) and (1,1) creates a red match on row 0.
            board.Set(0, 2, Piece.Normal(PieceColor.Red));
            board.Set(1, 1, Piece.Normal(PieceColor.Red));
            return board;
        }

        private static void AddBox(BoardModel board, int row, int col)
        {
            board.Set(row, col, Piece.Empty());
            board.SetObstacle(row, col,
                new ObstacleModel { Kind = ObstacleKind.Box, Health = 1 });
        }
    }
}
