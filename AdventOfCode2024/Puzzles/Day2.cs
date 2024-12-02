using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day2 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");
        var safeReportsCount = 0;

        while (stream.ReadLine() is string line)
        {
            var arr = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            if (IsValidReport(arr, -1)
                || Enumerable.Range(0, arr.Length).Any(i => IsValidReport(arr, i)))
            {
                safeReportsCount++;
            }
        }

        return safeReportsCount.ToString();
    }

    // skip => 0 -> l1 r2, 1 -> l0 r2, 2 -> l0, r1, 3 -> l0 ...

    /* skip == 0 => x 1 2 3 4
     * skip == 1 => 0 x 2 3 4
     * skip == 2 => 0 1 x 3 4
     */

    private static bool IsValidReport(int[] arr, int skipInd)
    {
        var left = skipInd != 0 ? 0 : 1;
        var right = skipInd != 1 ? left + 1 : left + 2;
        var firstDirection = Math.Sign(arr[right] - arr[left]);

        while (right < arr.Length)
        {
            if (right != skipInd)
            {
                var diff = arr[right] - arr[left];

                if (Math.Abs(diff) > 3 || firstDirection != Math.Sign(diff))
                {
                    return false;
                }

                left = right;
            }
            right++;
        }

        return true;
    }
}