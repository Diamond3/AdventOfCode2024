using AdventOfCode2024.Utils;
using System;

namespace AdventOfCode2024.Puzzles;

internal class Day11 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var memory = new Dictionary<long, int>();

        var nums = stream.ReadLine()!.Split(' ').Select(long.Parse).ToArray();
        var sum = 0L;

        for (int i = 0; i < nums.Length; i++)
        {
            var n = nums[i];

            var stack = new Stack<(long current, int count, int depth)>();
            stack.Push((n, 1, 0));

            while (stack.Count > 0)
            {
                var (num, count, depth) = stack.Pop();
                var digitCount = (int)Math.Log10(num) + 1;

                if (depth == 25)
                {
                    sum++;
                    continue;
                }

                if (num == 0)
                {
                    stack.Push((1, count + 1, depth + 1));
                }
                else if (digitCount % 2 == 0)
                {
                    // 891233 -> 6
                    // 891233 / 1000 => 891
                    // 891233 % 1000 => 233
                    var zeros = Math.Pow(10, digitCount / 2);
                    stack.Push(((long)(num / zeros), count + 1, depth + 1));
                    stack.Push(((long)(num % zeros), count + 1, depth + 1));
                }
                else
                {
                    stack.Push((num * 2024, count + 1, depth + 1));
                }
            }
        }

        return sum.ToString();
    }
}