using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day2 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");
        var safeReportsCount = 0;
        
        while(stream.ReadLine() is string line)
        {
            var arr = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            var direction = Math.Sign(arr[1] - arr[0]);
            var isSafe = false;

            for (int i = 1; i < arr.Length; i++)
            {
                var diff = arr[i] - arr[i - 1];
                if (Math.Abs(diff) > 3 || direction != Math.Sign(diff))
                {
                    isSafe = false;
                    break;
                }
                isSafe = true;
            }

            _ = isSafe ? safeReportsCount++ : 0;
        }

        return safeReportsCount.ToString();
    }
}