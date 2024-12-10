using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day10 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var map = new Dictionary<(int y, int x), int>();
        var stack = new Stack<((int y, int x) pos, HashSet<(int x, int y)> visited)>();
        var directions = new (int y, int x)[]
        {
            (0, 1),
            (0, -1),
            (1, 0),
            (-1, 0)
        };

        var y = 0;
        var sum = 0;

        while (stream.ReadLine() is string str)
        {
            for (int x = 0; x < str.Length; x++)
            {
                map[(y, x)] = str[x] - '0';
                if (map[(y, x)] == 0)
                {
                    stack.Push(((y, x), new HashSet<(int x, int y)>() { (y, x) }));
                }
            }
            y++;
        }

        while (stack.Count > 0)
        {
            var (pos, visited) = stack.Pop();
            if (map[pos] == 9)
            {
                sum++;
                continue;
            }

            foreach (var dir in directions)
            {
                var newPos = (pos.y + dir.y, pos.x + dir.x);

                if (map.TryGetValue(newPos, out var cost)
                    && map[pos] + 1 == cost
                    && !visited.Contains(newPos))
                {
                    var newVisited = new HashSet<(int, int)>(visited)
                    {
                        newPos
                    };

                    stack.Push((newPos, newVisited));
                }
            }
        }
        return sum.ToString();
    }
}