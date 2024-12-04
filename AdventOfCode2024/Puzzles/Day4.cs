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
        var directions = new (int aDir, int bDir)[]
        {
            (0, 0),     // 0 - normal, 1 - reversed
            (0, 1),
            (1, 0),
            (1, 1)
        };

        for (int y = 0; y < map.Count; y++)
        {
            for (int x = 0; x < map[0].Length; x++)
            {
                var a = GetStringFromDir(map, y, x, (1, 1));
                var b = GetStringFromDir(map, y + 2, x, (-1, +1));

                foreach (var (aDir, bDir) in directions)
                {
                    if (IsMatch(a, aDir) && IsMatch(b, bDir))
                    {
                        c++;
                    }
                }
            }
        }
        return c;
    }

    private bool IsMatch(string input, int dir)
    {
        var normal = "MAS";
        var reversed = "SAM";

        if (input.Length != 3)
        {
            return false;
        }

        if (dir == 0)
        {
            return normal == input;
        }

        return reversed == input;
    }

    private string GetStringFromDir(List<char[]> map, int y, int x, (int y, int x) dir)
    {
        var arr = new char[3];
        for (int i = 0; i < 3; i++)
        {
            var newX = x + dir.x * i;
            var newY = y + dir.y * i;

            if (newX < 0 || newX >= map[0].Length || newY < 0 || newY >= map.Count)
            {
                return "";
            }
            else
            {
                arr[i] = map[newY][newX];
            }
        }

        return new string(arr);
    }
}