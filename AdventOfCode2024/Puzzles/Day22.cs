using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day22 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");
        var sum = 0L;
        var dict = new Dictionary<int, Dictionary<(int, int, int, int), int>>();

        var buyerIndx = 0;
        while (stream.ReadLine() is string line)
        {
            var secret = long.Parse(line);
            var prev = 0;

            var changes = new int[2000];
            var prices = new int[2000];

            dict[buyerIndx] = [];

            for (int i = 0; i < 2000; i++)
            {
                prev = (int)(secret % 10);
                secret = Prune(Mix(secret, secret * 64));
                secret = Prune(Mix(secret, (int)(secret / 32f)));
                secret = Prune(Mix(secret, secret * 2048));
                prices[i] = (int)(secret % 10);
                changes[i] = prices[i] - prev;
            }

            for (int i = 3; i < 2000; i++)
            {
                var key = (changes[i - 3], changes[i - 2], changes[i - 1], changes[i]);
                if (!dict[buyerIndx].ContainsKey(key))
                {
                    dict[buyerIndx].Add(key, prices[i]);
                }
            }

            buyerIndx++;
            sum += secret;
        }

        var maxSum = -1L;
        var minKey = (0, 0, 0, 0);

        for (int a = -9; a < 10; a++)
        {
            for (int b = -9; b < 10; b++)
            {
                for (int c = -9; c < 10; c++)
                {
                    for (int d = -9; d < 10; d++)
                    {
                        var tempSum = 0L;
                        var key  = (a, b, c, d);

                        for (int i = 0; i < dict.Count; i++)
                        {
                            if (dict[i].TryGetValue(key, out var val))
                            {
                                tempSum += val;
                            }
                        }

                        if (tempSum > maxSum)
                        {
                            maxSum = tempSum;
                            minKey = key;
                        }
                    }
                }
            }
        }
        //Console.WriteLine(minKey);
        return maxSum.ToString();
    }

    private long Mix(long secret, long mixValue)
    {
        return secret ^ mixValue;
    }

    private long Prune(long secret)
    {
        return secret % 16777216;
    }
}