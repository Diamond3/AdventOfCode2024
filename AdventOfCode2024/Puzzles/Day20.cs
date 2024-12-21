using AdventOfCode2024.Utils;
using System.Diagnostics;

namespace AdventOfCode2024.Puzzles;

internal class Day20 : ISolver
{
    private (int y, int x)[] Directions =
        [
            (0, 1),
            (0, -1),
            (-1, 0),
            (1, 0),
        ];

    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var map = new List<char[]>();

        var path = new HashSet<(int, int)>();

        var start = (y: 0, x: 0);
        var end = (y: 0, x: 0);

        var st = new Stopwatch();

        var y = 0;
        while (stream.ReadLine() is string line)
        {
            for (int x = 0; x < line.Length; x++)
            {
                if (line[x] == 'S')
                {
                    start = (y, x);
                }

                if (line[x] == 'E')
                {
                    end = (y, x);
                }

                if (line[x] == 'E' || line[x] == 'S' || line[x] == '.')
                {
                    path.Add((y, x));
                }
            }
            y++;
        }

        var pos = start;
        var visited = new HashSet<(int, int)>();
        var dict = new Dictionary<(int y, int x), int>()
        {
            { start, 0 }
        };

        while (pos != end)
        {
            for (int i = 0; i < 4; i++)
            {
                var nextPos = (y: pos.y + Directions[i].y, x: pos.x + Directions[i].x);

                if (path.Contains(nextPos) && !visited.Contains(nextPos))
                {
                    visited.Add(nextPos);
                    pos = nextPos;
                    dict[pos] = visited.Count;
                    break;
                }
            }
        }

        var len = visited.Count;
        var cheatCount = 0;
        visited.Clear();
        var cheatDict = new Dictionary<int, int>();

        pos = start;

        while (pos != end)
        {
            foreach (var p in PosInRadius(pos, 2, path))
            {
                if (!visited.Contains(p))
                {
                    var newLen = visited.Count + (len - dict[p]) + 2;
                    if (newLen < len && newLen <= len - 100)
                    {
                        if (!cheatDict.ContainsKey(len - newLen))
                        {
                            cheatDict[len - newLen] = 0;
                        }
                        cheatDict[len - newLen]++;
                        cheatCount++;
                    }
                }
            }

            for (int i = 0; i < 4; i++)
            {
                var nextPos = (y: pos.y + Directions[i].y, x: pos.x + Directions[i].x);

                if (path.Contains(nextPos) && !visited.Contains(nextPos))
                {
                    visited.Add(nextPos);
                    pos = nextPos;

                    break;
                }
            }
        }


        return cheatCount.ToString();
    }

    private List<(int y, int x)> PosInRadius((int y, int x) center, int radius, HashSet<(int y, int x)> path)
    {
        var list = new List<(int y, int x)>();
        foreach (var pos in path)
        {
            if ((int)Math.Pow(pos.y - center.y, 2) == radius * radius - (int)Math.Pow(pos.x - center.x, 2))
            {
                list.Add(pos);
            }
        }
        return list;
    }

    private void PrintMap(HashSet<(int, int)> map, HashSet<(int, int)> visited)
    {
        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 16; x++)
            {
                if (visited.Contains((y, x)) && !map.Contains((y, x)))
                {
                    Console.Write('x');
                }
                else
                {
                    Console.Write(map.Contains((y, x)) ? '.' : '#');
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}