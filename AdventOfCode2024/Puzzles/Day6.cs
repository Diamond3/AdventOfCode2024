using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day6 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");
        var map = new Dictionary<(int y, int x), char>();
        var y = 0;

        var start = (y: 0, x: 0);
        var direction = (y: 0, x: 0);

        while (stream.ReadLine() is string line)
        {
            for (int x = 0; x < line.Length; x++)
            {
                map[(y, x)] = line[x];

                if (line[x] != '.' && line[x] != '#')
                {
                    start = (y, x);
                    direction = GetDirection(line[x]);
                }
            }

            y++;
        }

        var visited = new HashSet<(int, int)>() { start };
        var pos = start;

        while (true)
        {
            var newPos = (pos.y + direction.y, pos.x + direction.x);

            if (!map.ContainsKey(newPos))
            {
                break;
            }

            if (map[newPos] == '#')
            {
                direction = GetRotatedDirection(direction);
                continue;
            }

            visited.Add(newPos);
            pos = newPos;
        }

        return visited.Count.ToString();
    }

    private (int y, int x) GetRotatedDirection((int y, int x) direction)
    {
        return direction switch
        {
            (0, 1) => (1, 0),
            (0, -1) => (-1, 0),
            (-1, 0) => (0, 1),
            _ => (0, -1)
        };
    }

    private (int y, int x) GetDirection(char v)
    {
        return v switch
        {
            '>' => (0, 1),
            '<' => (0, -1),
            '^' => (-1, 0),
            _ => (1, 0)
        };
    }
}