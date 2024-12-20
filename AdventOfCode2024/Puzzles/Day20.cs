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

        SetHeuristic(map, h, end);

        st.Start();

        var shortestTime = ShortestPath(map, dist, h, start, end, int.MaxValue);
        var cheatCount = 0;
        var cheatDict = new Dictionary<int, int>();
        var cheked = new HashSet<((int, int), (int, int))>();

        // PrintMap(map);
        // Add start positions to list and pass them to the path finding
        foreach (var wall in walls)
        {
            foreach (var n in GetNeighbors(wall))
            {
                if ((empty.Contains(n) || (walls.Contains(n)) && !cheked.Contains((wall, n)) && !cheked.Contains((n, wall))))
                {
                    cheked.Add((n, wall));
                    RemoveWallMap(map, wall, n);
                    PrintMap(map);
                    var a = ShortestPath(map, dist, h, start, end, shortestTime);

                    if (a < shortestTime)
                    {
                        if (!cheatDict.ContainsKey(shortestTime - a))
                        {
                            cheatDict[shortestTime - a] = 0;
                        }
                        cheatDict[shortestTime - a]++;
                        cheatCount++;
                    }
                    ResetMap(map, wall, n, walls);
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

    private int ShortestPath(List<char[]> map, List<int[]> dist, List<int[]> h, (int y, int x) start, (int y, int x) end, int shortestTime)
    {
        ResetDist(dist);

        var queue = new PriorityQueue<(int y, int x), int>();
        var visited = new HashSet<(int, int)>();

        dist[start.y][start.x] = 0;
        queue.Enqueue(start, 0);

        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();

            foreach (var n in GetNeighbors(pos, map))
            {
                var cost = 1;
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

    private void ResetMap(List<char[]> map, (int y, int x) p1, (int y, int x) p2, HashSet<(int, int)> walls)
    {
        map[p1.y][p1.x] = walls.Contains(p1) ? '#' : '.';
        map[p2.y][p2.x] = walls.Contains(p2) ? '#' : '.';
    }

    private void RemoveWallMap(List<char[]> map, (int y, int x) p1, (int y, int x) p2)
    {
        map[p1.y][p1.x] = '1';
        map[p2.y][p2.x] = '2';
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

    private (int y, int x)[] GetNeighbors((int y, int x) pos)
    {
        return
        [
            (pos.y, pos.x + 1),
            (pos.y, pos.x - 1),
            (pos.y - 1, pos.x),
            (pos.y + 1, pos.x),
        ];
    }
}
