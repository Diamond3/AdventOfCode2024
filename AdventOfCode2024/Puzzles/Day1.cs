using AdventOfCode2024.Utils;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode2024.Puzzles;

internal class Day1 : ISolver
{
    public string Solve()
    {
        var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var left = new List<int>();
        var right = new List<int>();

        while (stream.ReadLine() is string line)
        {
            var nums = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            left.Add(nums[0]);
            right.Add(nums[1]);
        }

        left.Sort();
        right.Sort();

        var sum = 0;
        for (int i = 0; i < left.Count; i++)
        {
            sum += Math.Abs(left[i] - right[i]);
        }

        return sum.ToString();
    }
}