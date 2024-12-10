using AdventOfCode2024.Utils;
using System.Buffers;

namespace AdventOfCode2024.Puzzles;

internal class Day9 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var arr = stream.ReadLine()!.Select(x => x - '0').ToArray();

        var right = arr.Length % 2 != 0 ? arr.Length - 1 : arr.Length - 2;
        var globalIndex = 0L;
        var sum = 0L;

        right = GetNextRightIndex(right, arr);

        for (int left = 0; left < arr.Length; left++)
        {
            for (int j = 0; j < arr[left] && left <= right; j++, globalIndex++)
            {
                var currentInx = (left / 2);

                if (left % 2 != 0)  // Empty spaces
                {
                    currentInx = (right / 2);

                    arr[right]--;
                    right = GetNextRightIndex(right, arr);

                }

                sum += globalIndex * currentInx;
            }
        }

        return sum.ToString();
    }

    private int GetNextRightIndex(int right, int[] arr)
    {
        while (arr[right] == 0 && right > 1)
        {
            right -= 2;
        }

        return right;
    }
}