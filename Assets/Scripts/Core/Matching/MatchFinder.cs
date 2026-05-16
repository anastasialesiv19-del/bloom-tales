using System;
using System.Collections.Generic;
using System.Linq;
using Core.Matching;

namespace Core.Matching
{
    public static class MatchFinder
    {
        public static List<Match> FindMatches(BoardModel board)
        {
            var matches = new List<List<Pos>>();
            for (int row = 0; row < board.Height; row++)
            {
                PieceColor? currentColor = null;
                int matchStart = 0, matchLength = 0;
                
                for (int col = 0; col < board.Width; col++)
                {
                    if (board.HasObstacle(row, col))
                    {
                        FlushHoriz(matches, row, matchStart, matchLength);
                        currentColor = null;
                        matchLength = 0;
                        continue;
                    }

                    var cell = board.Get(row, col);
                    if (IsMatchCandidate(cell))
                    {
                        if(currentColor != null && cell.Color != null && currentColor == cell.Color)
                            matchLength++;
                        else
                        {
                            FlushHoriz(matches, row, matchStart, matchLength);
                            currentColor = cell.Color;
                            matchStart = col;
                            matchLength = 1;
                        }
                    }
                    else
                    {
                        FlushHoriz(matches, row, matchStart, matchLength);
                        currentColor = null;
                        matchLength = 0;
                    }
                }
                FlushHoriz(matches, row, matchStart, matchLength);
            }
            //vertical matches
            for (int col = 0; col < board.Width; col++)
            {
                PieceColor? currentColor = null;
                int matchStart = 0, matchLength = 0;
                
                for (int row = 0; row < board.Height; row++)
                {
                    if (board.HasObstacle(row, col))
                    {
                        FlushVert(matches, col, matchStart, matchLength);
                        currentColor = null;
                        matchLength = 0;
                        continue;
                    }

                    var cell = board.Get(row, col);
                    if (IsMatchCandidate(cell))
                    {
                        if(currentColor != null && cell.Color != null && currentColor == cell.Color)
                            matchLength++;
                        else
                        {
                            FlushVert(matches, col, matchStart, matchLength);
                            currentColor = cell.Color;
                            matchStart = row;
                            matchLength = 1;
                        }
                    }
                    else
                    {
                        FlushVert(matches, col, matchStart, matchLength);
                        currentColor = null;
                        matchLength = 0;
                    }
                }
                FlushVert(matches, col, matchStart, matchLength);
            }
            return MergeOverlaps(matches);
        }

        static bool IsMatchCandidate(Piece piece)
        {
            if (piece.Kind == PieceKind.Empty) return false;
            if (piece.Kind == PieceKind.ColorBomb) return false;
            if (piece.Color == null) throw new ArgumentException("Piece kind requires color.");
            return true;
        }

        static void FlushHoriz(List<List<Pos>> horiz, int row, int startCol, int len)
        {
            if (len >= 3)
            {
                var line = new List<Pos>(len);
                for (int i = 0; i < len; i++)
                    line.Add(new Pos(row, startCol + i));
                horiz.Add(line);
            }
        }

        static void FlushVert(List<List<Pos>> vert, int col, int startRow, int len)
        {
            if (len >= 3)
            {
                var line = new List<Pos>(len);
                for (int i = 0; i < len; i++)
                    line.Add(new Pos(startRow + i, col));
                vert.Add(line);
            }
        }

        static List<Match> MergeOverlaps(List<List<Pos>> matches)
        {
            if (matches.Count <= 1) return matches.Select(line => new Match(line)).ToList();
            var index = new Dictionary<Pos, List<int>>(); //які матчі проходять через клітинку?
            for (int i = 0; i < matches.Count; i++)
                foreach (var p in matches[i])
                    (index.TryGetValue(p, out var list) ? list : index[p] = new List<int>()).Add(i);
            var adj = Enumerable.Range(0, matches.Count)
                .Select(_ => new List<int>()).ToArray(); //послідовність List<int> для всіх матчів для збереження матчів-сусіді
            foreach(var linesAtPos in index.Values) //створення унікальних пар матчів
                for(int a = 0; a < linesAtPos.Count; a++)
                for (int b = a + 1; b < linesAtPos.Count; b++)
                {
                    int i = linesAtPos[a], j = linesAtPos[b];
                    if(!adj[i].Contains(j))
                        adj[i].Add(j);
                    if(!adj[j].Contains(i))
                        adj[j].Add(i);
                }

            var result = new List<List<Pos>>();
            var visited = new  bool [matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                if (visited[i]) continue;
                var stack = new Stack<int>();
                stack.Push(i);
                visited[i] = true;
                var set = new HashSet<Pos>();
                while (stack.Count > 0)
                {
                    var v = stack.Pop();
                    foreach(var pos in matches[v])
                        set.Add(pos);
                    foreach (var to in adj[v])
                    {
                        if (!visited[to])
                        {
                            visited[to] = true;
                            stack.Push(to);
                        }
                    }
                }
                result.Add(set.OrderBy(p => p.Row).ThenBy(p => p.Col).ToList());
            }
            return result.Select(line => new Match(line)).ToList();
        }
    }
}