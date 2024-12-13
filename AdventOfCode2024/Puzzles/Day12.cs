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
            (0, 1),     // 0
            (0, -1),    // 1
            (1, 0),     // 2
            (-1, 0),    // 3
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
                    var sidesDict = new Dictionary<(int y, int x), HashSet<(int y, int x)>>()
                    {
                        { (0, 1), new HashSet<(int, int)>() },
                        { (0, -1), new HashSet<(int, int)>() },
                        { (1, 0), new HashSet<(int, int)>() },
                        { (-1, 0), new HashSet<(int, int)>() },
                    };

                    var area = 1;
                    var id = map[key];

                    stack.Push(key);
                    visited.Add(key);

                    while (stack.Count > 0)
                    {
                        var pos = stack.Pop();
                        var dirIndx = 0;
                        foreach (var dir in directions)
                        {
                            var nextPos = (y: pos.y + dir.y, x: pos.x + dir.x);
                            if (!map.TryGetValue(nextPos, out var sideId) || sideId != id)
                            {
                                sidesDict[dir].Add(nextPos);
                            }

                            else if (!visited.Contains(nextPos) && map.TryGetValue(nextPos, out var nextId) && nextId == id)
                            {
                                stack.Push(nextPos);
                                visited.Add(nextPos);
                                area++;
                            }
                            dirIndx++;
                        }
                    }

                    var sides = 0;
                    foreach(var dir in directions)
                    {
                        var chekedSides = new HashSet<(int, int)>();
                        var checkDir = GetCheckDirection(dir);

                        foreach (var pos in sidesDict[dir])
                        {
                            if (chekedSides.Contains(pos))
                            {
                                continue;
                            }

                            sides++;

                            AddSameSidePosToChekedSet(sidesDict[dir], chekedSides, checkDir, pos);
                            AddSameSidePosToChekedSet(sidesDict[dir], chekedSides, (y: -checkDir.y, x: -checkDir.x), pos);
                        }
                    }

                    sum += area * sides;
                }
            }
        }

        return sum.ToString();
    }

    private static void AddSameSidePosToChekedSet(HashSet<(int y, int x)> set, HashSet<(int, int)> chekedSides, (int y, int x) checkDir, (int y, int x) pos)
    {
        while (true)
        {
            pos = (y: pos.y + checkDir.y, x: pos.x + checkDir.x);
            if (!set.Contains(pos))
            {
                break;
            }
            chekedSides.Add(pos);
        }
    }

    /*
        (0, 1) -> (1, 0),
        (0, -1) -> (1, 0),
        (1, 0) -> (0, 1),
        (-1, 0) -> (0, 1)
    */
    private (int y, int x) GetCheckDirection((int y, int x) dir)
    {
        return (y: Math.Abs(dir.x), x: Math.Abs(dir.y));
    }
}