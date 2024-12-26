using AdventOfCode2024.Utils;
using System.Runtime.CompilerServices;

namespace AdventOfCode2024.Puzzles;

internal class Day25 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");
        var keys = new List<int[]>();
        var locks = new List<int[]>();

        while (stream.ReadLine() is string line)
        {
            var isLock = line.Contains('#');
            var l = new int[5] { -1, -1, -1, -1, -1 };

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    l[j] += line[j] == '#' ? 1 : 0;
                }
                line = stream.ReadLine()!;
            }

            if (isLock)
            {
                locks.Add(l);
            }
            else
            {
                keys.Add(l);
            }
        }

        var dict = new HashSet<int[]>();
        var count = 0;
        foreach (var key in keys)
        {
            foreach (var lo in locks)
            {
                var fits = true;
                for (int i = 0; i < 5; i++)
                {
                    if (lo[i] + key[i] > 5)
                    {
                        fits = false;
                        break;
                    }
                }
                if (fits)
                {
                    count++;
                }
            }
        }
        

        return count.ToString();
    }
}