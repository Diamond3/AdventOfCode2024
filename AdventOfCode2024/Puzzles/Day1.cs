using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day1 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");
        var freq = new Dictionary<int, int>();
        var left = new List<int>();

        while (stream.ReadLine() is string line)
        {
            var nums = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            left.Add(nums[0]);

            var key = nums[1];
            freq[key] = freq.GetValueOrDefault(key, 0) + 1;
        }

        return left.Sum(x => freq.GetValueOrDefault(x, 0) * x).ToString();
    }
}