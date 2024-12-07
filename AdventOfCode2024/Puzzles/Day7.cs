using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day7 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");
        var sumOfResults = 0L;

        while (stream.ReadLine() is string line)
        {
            var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray();
            var expected = long.Parse(split[0][..^1]);
            var nums = split.Skip(1).Select(int.Parse).ToArray();

            var stack = new Stack<(int n, long sum)>();

            stack.Push((1, nums[0]));

            while (stack.Count > 0)
            {
                var (n, sum) = stack.Pop();

                if (n == nums.Length)
                {
                    if (sum == expected)
                    {
                        sumOfResults += sum;
                        break;
                    }
                    continue;
                }

                stack.Push((n + 1, sum + nums[n]));
                stack.Push((n + 1, sum * nums[n]));
            }
        }

        return sumOfResults.ToString();
    }
}