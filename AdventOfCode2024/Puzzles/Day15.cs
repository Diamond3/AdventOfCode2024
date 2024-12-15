using AdventOfCode2024.Utils;
using System.Runtime.ExceptionServices;

namespace AdventOfCode2024.Puzzles;

internal class Day15 : ISolver
{
    private const int Height = 103;
    private const int Width = 101;
    private const int Seconds = 1000;

    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var sum = 0L;
        var pos = (y: 0, x: 0);
        var map = new List<char[]>();
        var commands = new List<char>();
        var directions = new Dictionary<char, (int y, int x)>()
        {
            { '>', (0, 1) },
            { '<', (0, -1) },
            { 'v', (1, 0) },
            { '^', (-1, 0) },
        };

        while (stream.ReadLine() is string line && !string.IsNullOrEmpty(line))
        {
            var arr = line.ToCharArray();
            for (int i = 0; i < arr.Length; i++)
            {
                if (line[i] == '@')
                {
                    pos = (y: map.Count, x: i);
                    arr[i] = '.';
                    break;
                }
            }
            map.Add(arr);
        }

        while (stream.ReadLine() is string line)
        {
            commands.AddRange(line);
        }

        for (int c = 0; c < commands.Count; c++)
        {
            var dir = directions[commands[c]];
            pos = Move(pos, dir, map);
        }

        for (int y = 0; y < map.Count; y++)
        {
            for (int x = 0; x < map[0].Length; x++)
            {
                if (map[y][x] == 'O')
                {
                    sum += 100 * y + x;
                }
            }
        }

        //PrintMap(pos, map);

        return sum.ToString();
    }

    private void PrintMap((int y, int x) pos, List<char[]> map)
    {
        for (int y = 0; y < map.Count; y++)
        {
            for (int x = 0; x < map[0].Length; x++)
            {
                if (pos == (y, x))
                {
                    Console.Write('@');
                }
                else
                {
                    Console.Write(map[y][x]);
                }
            }
            Console.WriteLine();
        }
    }

    private (int y, int x) Move((int y, int x) pos, (int y, int x) dir, List<char[]> map)
    {
        var y = pos.y + dir.y;
        var x = pos.x + dir.x;

        if (map[y][x] == '.')
        {
            return (y, x);
        }

        var boxes = GetMovedBoxes((y, x), dir, map);
        if (boxes.Count == 0)
        {
            return pos;
        }

        for (int i = boxes.Count - 1; i > 0; i--)
        {
            var b = boxes[i];
            map[b.y][b.x] = 'O';
        }

        map[boxes[0].y][boxes[0].x] = '.';
        return (y, x);
    }

    private List<(int y, int x)> GetMovedBoxes((int y, int x) pos, (int y, int x) dir, List<char[]> map)
    {
        var movedBoxes = new List<(int y, int x)>(); // Last is empty space
        while (map[pos.y][pos.x] != '.')
        {
            if (map[pos.y][pos.x] == '#')
            {
                return [];
            }

            movedBoxes.Add(pos);
            pos = (y: pos.y + dir.y, x: pos.x + dir.x);
        }
        movedBoxes.Add(pos);

        return movedBoxes;
    }
}