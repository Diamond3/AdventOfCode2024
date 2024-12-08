using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day8 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");
        var antennaPairs = new Dictionary<char, List<(int y, int x)>>();
        var y = 0;
        var mapWidth = 0;

        while (stream.ReadLine() is string line)
        {
            mapWidth = line.Length;
            for (int x = 0; x < line.Length; x++)
            {
                if (line[x] != '.')
                {
                    if (!antennaPairs.ContainsKey(line[x]))
                    {
                        antennaPairs[line[x]] = [];
                    }

                    antennaPairs[line[x]].Add((y, x));
                }
            }
            y++;
        }

        var mapHeight = y;
        var antinodeSet = new HashSet<(int, int)>();

        foreach (var list in antennaPairs.Values)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    var dir = (y: list[i].y - list[j].y, x: list[i].x - list[j].x); // Dir from j to i

                    var antinode = (y: list[i].y + dir.y, x: list[i].x + dir.x);
                    while (AddAntinode(mapWidth, mapHeight, antinodeSet, antinode))
                    {
                        antinode = (y: antinode.y + dir.y, x: antinode.x + dir.x);
                    }

                    antinode = (y: list[j].y - dir.y, x: list[j].x - dir.x);
                    while (AddAntinode(mapWidth, mapHeight, antinodeSet, antinode))
                    {
                        antinode = (y: antinode.y - dir.y, x: antinode.x - dir.x);
                    }

                    antinodeSet.Add(list[i]);
                    antinodeSet.Add(list[j]);
                }
            }
        }

        return antinodeSet.Count.ToString();
    }

    private bool AddAntinode(int mapWidth, int mapHeight, HashSet<(int, int)> antinodeSet, (int y, int x) antinode)
    {
        if (antinode.x >= 0 && antinode.x < mapWidth && antinode.y >= 0 && antinode.y < mapHeight)
        {
            antinodeSet.Add(antinode);
            return true;
        }

        return false;
    }
}