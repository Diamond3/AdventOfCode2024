using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day14 : ISolver
{
    private const int Height = 103;
    private const int Width = 101;
    private const int Seconds = 1000;

    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var set = new HashSet<(int y, int x)>();
        var robots = new List<((int y, int x) p, (int y, int x) v)>();

        var lines = new Dictionary<int, bool[]>();

        var yPos = new List<(int x1, int x2)>();
        
        while (stream.ReadLine() is string line)
        {
            var nums = line.Split(new char[] { ' ', '=', ',', 'v', 'p' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToArray();

            var p = (y: nums[1], x: nums[0]);
            var v = (y: nums[3], x: nums[2]);

            robots.Add((p, v));
        }

        for (int s = 0; s < 20000; s++)
        {
            set.Clear();
            ClearDictionary(lines);
            for (int i = 0; i < robots.Count; i++)
            {
                var pos = GetPosInTime(robots[i].p, robots[i].v, s);
                lines[pos.y][pos.x] = true;
                set.Add(pos);
            }

            if (HasConsecutiveLine(lines, 7))
            {
                Console.WriteLine($"Seconds - {s}");
                PrintMap(set);
            }
        }

        return "";
    }

    private bool HasConsecutiveLine(Dictionary<int, bool[]> lines, int neededLength) // Check if current view has more than 10 robots in one line
    {
        var longestLine = int.MinValue;
        var c = 0;

        foreach (var line in lines.Values)
        {
            var start = 0;
            for (int i = 0; i < line.Length - 1; i++)
            {
                if (line[i])
                {
                    start = i;
                    c = 1;
                    for (int j = i + 1; j < line.Length; j++, i++, c++)
                    {
                        if (!line[j])
                        {
                            if (c > longestLine)
                            {
                                longestLine = c;
                            }
                            break;
                        }
                    }
                }
            }
        }

        return longestLine > neededLength;
    }

    private void ClearDictionary(Dictionary<int, bool[]> dict)
    {
        for (int i = 0; i < Height; i++)
        {
            dict[i] = new bool[Width];
        }
    }

    private static (int y, int x) GetPosInTime((int y, int x) p, (int y, int x) v, int time)
    {
        var pos = (y: (p.y + v.y * time) % Height, x: (p.x + v.x * time) % Width);

        if (pos.y < 0)
        {
            pos.y = Height + pos.y;
        }

        if (pos.x < 0)
        {
            pos.x = Width + pos.x;
        }

        return pos;
    }

    private int GetQuadrantIndex((int y, int x) pos, int halfHeight, int halfWidth)
    {
        if (pos.x < halfWidth && pos.y < halfHeight)
        {
            return 0;
        }

        if (pos.x > halfWidth && pos.y < halfHeight)
        {
            return 1;
        }

        if (pos.x < halfWidth && pos.y > halfHeight)
        {
            return 2;
        }

        if (pos.x > halfWidth && pos.y > halfHeight)
        {
            return 3;
        }

        return -1;
    }

    private void PrintMap(HashSet<(int y, int x)> pos)
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                if (pos.Contains((i, j)))
                {
                    Console.Write('O');
                }
                else
                {
                    Console.Write('.');
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}