using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day4 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");
        var map = new List<char[]>();

        var y = 0;
        while (stream.ReadLine() is string line)
        {
            map.Add(line.ToArray());
            y++;
        }

        var count = CountMatches(map);

        return count.ToString();
    }

    private int CountMatches(List<char[]> map)
    {
        var c = 0;
        var directions = new (int dirY, int dirX)[]
        {
            (0, 1),     // Right
            (0, -1),    // Left
            (1, 0),     // Down
            (-1, 0),    // Up
            
            (1, 1),
            (-1, -1),
            (-1, 1),  
            (1, -1)
        };

        for (int y = 0; y < map.Count; y++)
        {
            for (int x = 0; x < map[0].Length; x++)
            {
                foreach (var dir in directions)
                {
                    if (IsMatch(map, y, x, dir, "XMAS"))
                    {
                        c++;
                    }
                }
            }
        }
        return c;
    }

    private bool IsMatch(List<char[]> map, int y, int x, (int y, int x) dir, string word)
    {
        for (int i = 0; i < word.Length; i++)
        {
            var newX = x + dir.x * i;
            var newY = y + dir.y * i;

            if (newX < 0 || newX >= map[0].Length || newY < 0 || newY >= map.Count || map[newY][newX] != word[i])
            {
                return false;
            }
        }

        return true;
    }
}