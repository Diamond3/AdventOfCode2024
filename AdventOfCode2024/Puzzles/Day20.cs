using AdventOfCode2024.Utils;
using System.Diagnostics;

namespace AdventOfCode2024.Puzzles;

internal class Day20 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var map = new List<char[]>();
        var dist = new List<int[]>();

        var h = new List<int[]>();
        var prev = new List<(int y, int x)[]>();
        var walls = new HashSet<(int, int)>();
        var empty = new HashSet<(int, int)>();

        var jumpPos = new Dictionary<(int, int), HashSet<(int, int)>>();

        var start = (y: 0, x: 0);
        var end = (y: 0, x: 0);

        var st = new Stopwatch();

        var y = 0;
        while (stream.ReadLine() is string line)
        {
            var distArr = new int[line.Length];
            var rowArr = new char[line.Length];
            for (int x = 0; x < line.Length; x++)
            {
                rowArr[x] = line[x];
                if (rowArr[x] == 'S')
                {
                    start = (y, x);
                    rowArr[x] = '.';
                }
                if (rowArr[x] == 'E')
                {
                    end = (y, x);
                    rowArr[x] = '.';
                }
                if (x != 0 && x != line.Length - 1 && y != 0 && rowArr[x] == '#')
                {
                    walls.Add((y, x));
                }
                if (rowArr[x] == '.')
                {
                    empty.Add((y, x));
                }
                distArr[x] = int.MaxValue;
            }
            map.Add(rowArr);
            dist.Add(distArr);
            y++;
        }

        for (y = 0; y < map.Count; y++)
        {
            for (int x = 0; x < map[0].Length; x++)
            {
                var nextPos = GetJumpPosAfter2s((y, x), map);
                if (nextPos.Count != 0)
                {
                    jumpPos[(y, x)] = new HashSet<(int, int)>(nextPos);
                }
            }
        }

        SetHeuristic(map, h, end);

        st.Start();

        var shortestTime = ShortestPath(map, dist, h, start, end, int.MaxValue, [(-1, -1), (-1, -1)]);

        var cheatCount = 0;
        var cheatDict = new Dictionary<int, int>();
        var cheked = new HashSet<((int, int), (int, int))>();

        PrintMap(map);
        // Add start positions to list and pass them to the path finding

        foreach (var startPos in jumpPos.Keys)
        {
            foreach (var endPos in jumpPos[startPos])
            {
                var a = ShortestPath(map, dist, h, start, end, shortestTime, [startPos, endPos]);

                if (a < shortestTime && shortestTime - a >= 100)
                {
                    if (!cheatDict.ContainsKey(shortestTime - a))
                    {
                        cheatDict[shortestTime - a] = 0;
                    }
                    cheatDict[shortestTime - a]++;
                    cheatCount++;
                }
            }
        }

        //Console.WriteLine(st.ElapsedMilliseconds);

        /*for (int y = 0; y < MapSize; y++)
        {
            for (int x = 0; x < MapSize; x++)
            {
                Console.Write(map[y][x] + " ");
            }
            Console.WriteLine();
        }*/

        return cheatCount.ToString();
    }

    private void SetHeuristic(List<char[]> map, List<int[]> h, (int y, int x) end)
    {
        for (int y = 0; y < map.Count; y++)
        {
            var hArr = new int[map[0].Length];
            for (int x = 0; x < map[0].Length; x++)
            {
                hArr[x] = Heuristic((y, x), end);
            }
            h.Add(hArr);
        }
    }

    private void PrintMap(List<char[]> map)
    {
        for (int y = 0; y < map.Count; y++)
        {
            for (int x = 0; x < map[0].Length; x++)
            {
                Console.Write(map[y][x]);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    private int ShortestPath(List<char[]> map, List<int[]> dist, List<int[]> h, (int y, int x) start, (int y, int x) end, int shortestTime, (int y, int x)[] jump)
    {
        ResetDist(dist);

        var queue = new PriorityQueue<(int y, int x), int>();
        var visited = new HashSet<(int, int)>();

        dist[start.y][start.x] = 0;
        queue.Enqueue(start, 0);

        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();

            foreach (var nTemp in GetNeighbors(pos, map))
            {
                var n = nTemp;
                var cost = 1;
                if (jump[0] == n)
                {
                    n = jump[1];
                    cost += 2;
                }
                var newDist = dist[pos.y][pos.x] + cost;

                if (newDist < shortestTime && newDist < dist[n.y][n.x])
                {
                    dist[n.y][n.x] = newDist;
                    queue.Enqueue(n, newDist + h[n.y][n.x]);
                }
            }

            visited.Add(pos);
        }

        return dist[end.y][end.x];
    }

    private static int Heuristic((int y, int x) current, (int y, int x) goal)
    {
        return Math.Abs(current.y - goal.y) + Math.Abs(current.x - goal.x);
    }

    private void ResetDist(List<int[]> dist)
    {
        for (int y = 0; y < dist.Count; y++)
        {
            for (int x = 0; x < dist[0].Length; x++)
            {
                dist[y][x] = int.MaxValue;
            }
        }
    }

    private List<(int y, int x)> GetNeighbors((int y, int x) pos, List<char[]> map)
    {
        var list = new List<(int y, int x)>();
        var directions = new (int y, int x)[4]
        {
            (0, 1),
            (0, -1),
            (-1, 0),
            (1, 0),
        };

        for (int i = 0; i < 4; i++)
        {
            var y = pos.y + directions[i].y;
            var x = pos.x + directions[i].x;

            if (x >= 0 && x < map[0].Length && y >= 0 && y < map.Count && map[y][x] != '#')
            {
                list.Add((y, x));
            }
        }

        return list;
    }

    private List<(int y, int x)> GetJumpPosAfter2s((int y, int x) pos, List<char[]> map)
    {
        var list = new List<(int y, int x)>();
        var directions = new (int y, int x)[4]
        {
            (0, 1),
            (0, -1),
            (-1, 0),
            (1, 0),
        };

        for (int i = 0; i < 4; i++)
        {
            var y = pos.y + directions[i].y;
            var x = pos.x + directions[i].x;

            if (IsLegal(map, y, x) && map[y][x] == '#')
            {
                var nextY = y + directions[i].y;
                var nextX = x + directions[i].x;

                if (IsLegal(map, nextY, nextX) && map[nextY][nextX] != '#')
                {
                    list.Add((nextY, nextX));
                }
            }
        }

        return list;
    }

    private static bool IsLegal(List<char[]> map, int y, int x)
    {
        return x >= 0 && x < map[0].Length && y >= 0 && y < map.Count;
    }
}