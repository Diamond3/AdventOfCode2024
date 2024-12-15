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
            var arr = new char[line.Length * 2];

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '@')
                {
                    pos = (y: map.Count, x: i * 2);
                    arr[i * 2] = '.';
                    arr[i * 2 + 1] = '.';
                }

                if (line[i] == 'O')
                {
                    arr[i * 2] = '[';
                    arr[i * 2 + 1] = ']';
                }

                if (line[i] == '#')
                {
                    arr[i * 2] = '#';
                    arr[i * 2 + 1] = '#';
                }

                if (line[i] == '.')
                {
                    arr[i * 2] = '.';
                    arr[i * 2 + 1] = '.';
                }
            }
            map.Add(arr);
        }

        PrintMap(pos, map);

        while (stream.ReadLine() is string line)
        {
            commands.AddRange(line);
        }

        for (int c = 0; c < commands.Count; c++)
        {
            var dir = directions[commands[c]];
            pos = Move(pos, dir, map);

        }

        PrintMap(pos, map);

        for (int y = 0; y < map.Count; y++)
        {
            for (int x = 0; x < map[0].Length; x++)
            {
                if (map[y][x] == '[')
                {
                    sum += 100 * y + x;
                }
            }
        }


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
        Console.WriteLine();
    }

    private (int y, int x) Move((int y, int x) pos, (int y, int x) dir, List<char[]> map)
    {
        var y = pos.y + dir.y;
        var x = pos.x + dir.x;

        if (map[y][x] == '.')
        {
            return (y, x);
        }

        if (map[y][x] == '#')
        {
            return pos;
        }

        if (dir.y == 0)
        {
            return MoveHorizontaly(pos, dir, map, y, x);
        }
        
        return MoveVerticaly(pos, dir, map, y, x);
    }

    private (int y, int x) MoveVerticaly((int y, int x) pos, (int y, int x) dir, List<char[]> map, int y, int x)
    {
        var boxes = GetMovedBoxesVerticaly((y, x), dir, map);
        if (boxes.Count == 0)
        {
            return pos;
        }

        for (int i = boxes.Count - 1; i >= 0; i--)
        {
            var b = boxes[i];
            var nextY = b.y + dir.y;
            var nextX = b.x + dir.x;

            map[b.y][b.x] = '.';
            map[b.y][b.x + 1] = '.';

            map[nextY][nextX] = '[';
            map[nextY][nextX + 1] = ']';

        }

        return (y, x);
    }

    private (int y, int x) MoveHorizontaly((int y, int x) pos, (int y, int x) dir, List<char[]> map, int y, int x)
    {
        var boxes = GetMovedBoxesHorizontaly((y, x), dir, map);
        if (boxes.Count == 0)
        {
            return pos;
        }

        var boxChars = new char[2] { ']', '[' };
        if (dir.x == -1)
        {
            boxChars = ['[', ']'];
        }

        for (int i = boxes.Count - 1; i > 0; i--)
        {
            var b = boxes[i];
            map[b.y][b.x] = boxChars[i % 2];
        }

        map[boxes[0].y][boxes[0].x] = '.';
        return (y, x);
    }

    private List<(int y, int x)> GetMovedBoxesHorizontaly((int y, int x) pos, (int y, int x) dir, List<char[]> map)
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

    private List<(int y, int x)> GetMovedBoxesVerticaly((int y, int x) pos, (int y, int x) dir, List<char[]> map)
    {
        var firstBox = (y: pos.y, x: pos.x + (map[pos.y][pos.x] == '[' ? 0 : -1)); // Only left pos of box

        var movedBoxes = new List<(int y, int x)>()
        {
            firstBox
        };

        var checkPos = new HashSet<(int y, int x)>
        {
            firstBox
        };

        while (checkPos.Count > 0)
        {
            var nextCheckPos = new HashSet<(int y, int x)>();

            foreach (var p in checkPos)
            {
                var y = p.y + dir.y;
                var x = p.x + dir.x;

                if (map[y][x] == '#' || map[y][x + 1] == '#')
                {
                    return [];
                }

                if (map[y][x] == '[')
                {
                    nextCheckPos.Add((y, x));
                    movedBoxes.Add((y, x));
                    continue;
                }

                if (map[y][x] == ']')
                {
                    nextCheckPos.Add((y, x - 1));
                    movedBoxes.Add((y, x - 1));
                }

                if (map[y][x + 1] == '[')
                {
                    nextCheckPos.Add((y, x + 1));
                    movedBoxes.Add((y, x + 1));
                }
            }

            checkPos = nextCheckPos;
        }

        return movedBoxes;
    }
}