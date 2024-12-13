using AdventOfCode2024.Utils;
using System;
using System.Diagnostics;

namespace AdventOfCode2024.Puzzles;

internal class Day11 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var lastMemory = new Dictionary<(long val, int depth), long>();

        var nums = stream.ReadLine()!.Split(' ').Select(long.Parse).ToArray();
        var sum = 0L;
        var DEPTH = 75;

        var queue = new Queue<(long current, long prevNum, int depth)>();
        var finalDepth = new HashSet<long>();

        for (int i = 0; i < nums.Length; i++)
        {
            queue.Enqueue((nums[i], -1L, 0));
        }

        while (queue.Count > 0)
        {
            var (num, prev, depth) = queue.Dequeue();
            var digitCount = (int)Math.Log10(num) + 1;

            if (lastMemory.ContainsKey((num, depth)))
            {
                if (lastMemory.ContainsKey((prev, depth - 1)))
                {
                    lastMemory[(num, depth)] += lastMemory[(prev, depth - 1)];
                }
                else
                {
                    lastMemory[(num, depth)]++;
                }
                continue;
            }

            if (!lastMemory.ContainsKey((num, depth)))
            {
                lastMemory[(num, depth)] = 1;
                if (lastMemory.ContainsKey((prev, depth - 1)))
                {
                    lastMemory[(num, depth)] = lastMemory[(prev, depth - 1)];
                }
            }

            if (depth == DEPTH)
            {
                finalDepth.Add(num);
                continue;
            }
            
            if (num == 0)
            {
                queue.Enqueue((1, num, depth + 1));
            }
            else if (digitCount % 2 == 0)
            {
                // 891233 -> 6
                // 891233 / 1000 => 891
                // 891233 % 1000 => 233
                var zeros = Math.Pow(10, digitCount / 2);
                queue.Enqueue(((long)(num / zeros), num, depth + 1));
                queue.Enqueue(((long)(num % zeros), num, depth + 1));
            }
            else
            {
                queue.Enqueue((num * 2024, num, depth + 1));
            }
        }

        foreach (var num in finalDepth)
        {
            sum += lastMemory[(num, DEPTH)];
        }

        return sum.ToString();
    }
}