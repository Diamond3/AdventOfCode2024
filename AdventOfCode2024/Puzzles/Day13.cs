using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day13 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");
        var sum = 0L;
        var add = 10000000000000L;

        while (stream.ReadLine() is string line)
        {
            var a = line.Trim()
                .Split(", ", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => long.Parse(x.Split('+')[1]))
                .ToArray();

            var b = stream.ReadLine()!.Trim()
                .Split(", ", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => long.Parse(x.Split('+')[1]))
                .ToArray();

            var prize = stream.ReadLine()!.Trim()
                .Split(", ", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => long.Parse(x.Split('=')[1]) + add)
                .ToArray();

            stream.ReadLine();

            var numerator = prize[0] * a[1] - prize[1] * a[0];
            var denominator = a[1] * b[0] - a[0] * b[1];
            var bCount = numerator / denominator;

            if (bCount * denominator != numerator)
            {
                continue;
            }

            numerator = prize[0] - b[0] * bCount;
            denominator = a[0];
            var aCount = numerator / denominator;

            if (aCount * denominator != numerator)
            {
                continue;
            }

            if (bCount > 0 && aCount > 0)
            {
                sum += aCount * 3 + bCount;
            }
        }
        return sum.ToString();
    }
}