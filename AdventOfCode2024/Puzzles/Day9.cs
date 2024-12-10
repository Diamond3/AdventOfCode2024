using AdventOfCode2024.Utils;
using System.Buffers;

namespace AdventOfCode2024.Puzzles;

internal class Day9 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var arr = stream.ReadLine()!.Select(x => x - '0').ToArray();

        var maxRight = arr.Length % 2 != 0 ? arr.Length - 1 : arr.Length - 2;
        var globalIndex = 0L;
        var sum = 0L;

        var alreadyMoved = new bool[arr.Length];

        for (int left = 0; left < arr.Length; left++)
        {

            if (left % 2 != 0)
            {
                var rightIndex = GetPossibleRightIndex(left, maxRight, arr, alreadyMoved);
                if (rightIndex == -1)
                {
                    globalIndex += arr[left];
                    continue;
                }

                for (int j = 0; j < arr[rightIndex]; j++, globalIndex++)
                {
                    arr[left]--;
                    sum += globalIndex * (rightIndex / 2);
                }

                if (arr[left] > 0)
                {
                    left--; // Try insert more
                }
            }
            else
            {
                if (alreadyMoved[left])
                {
                    globalIndex += arr[left]; // Since it is moved this is now just a space
                    continue;
                }

                for (int j = 0; j < arr[left]; j++, globalIndex++)
                {
                    sum += globalIndex * (left / 2);
                }
            }

            alreadyMoved[left] = true;
        }

        return sum.ToString();
    }

    private int GetPossibleRightIndex(int left, int maxRight, int[] arr, bool[] alreadyMoved)
    {
        for (int i = maxRight; i >= 0; i -= 2)
        {
            if (!alreadyMoved[i] && arr[left] >= arr[i] && left != i)
            {
                alreadyMoved[i] = true;
                return i;
            }
        }

        return -1;
    }
}