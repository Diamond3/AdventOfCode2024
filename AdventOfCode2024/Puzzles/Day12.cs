using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day12 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");
        var sum = 0L;
        var y = 0;
        var x = 0;

        var map = new Dictionary<(int y, int x), char>();
        var visited = new HashSet<(int y, int x)>();
        var stack = new Stack<(int y, int x)>();
        var directions = new (int y, int x)[]
        {
            (0, 1),
            (0, -1),
            (1, 0),
            (-1, 0),
        };

        while (stream.ReadLine() is string line)
        {
            for (x = 0; x < line.Length; x++)
            {
                map[(y, x)] = line[x];
            }
            y++;
        }

        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                var key = (i, j);
                if (!visited.Contains(key))
                {
                    var area = 1;
                    var perimeter = 0;
                    var id = map[key];

                    stack.Push(key);
                    visited.Add(key);

                    while (stack.Count > 0)
                    {
                        var pos = stack.Pop();
                        foreach (var dir in directions)
                        {
                            var nextPos = (y: pos.y + dir.y, x: pos.x + dir.x);
                            if (!map.TryGetValue(nextPos, out var sideId) || sideId != id)
                            {
                                perimeter++;
                            }

                            else if (!visited.Contains(nextPos) && map.TryGetValue(nextPos, out var nextId) && nextId == id)
                            {
                                stack.Push(nextPos);
                                visited.Add(nextPos);
                                area++;
                            }
                        }
                    }
                    sum += area * perimeter;
                }
            }
        }
       

        return sum.ToString();
    }
}