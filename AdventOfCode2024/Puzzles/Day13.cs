using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day13 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");
        var sum = 0L;
        
        while (stream.ReadLine() is string line)
        {
            var a = line.Trim()
                .Split(", ", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.Parse(x.Split('+')[1]))
                .ToArray();

            var b = stream.ReadLine()!.Trim()
                .Split(", ", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.Parse(x.Split('+')[1]))
                .ToArray();

            var prize = stream.ReadLine()!.Trim()
                .Split(", ", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.Parse(x.Split('=')[1]))
                .ToArray();

            stream.ReadLine();

            var stack = new Stack<((int x, int y), int aCount, int bCount)>();
            stack.Push(((x: a[0], y: a[1]), 1, 0));
            stack.Push(((x: b[0], y: b[1]), 0, 1));

            var minPrice = int.MaxValue;
            var currentMin = (a: -1, b: -1);

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    var c = (x: i * a[0] + j * b[0], y: i * a[1] + j * b[1]);
                    var price = i * 3 + j * 1;

                    if (c.x == prize[0] && c.y == prize[1])
                    {
                        if (minPrice > price)
                        {
                            minPrice = price;
                            currentMin = (a: i, b: j);
                        }
                        continue;
                    }
                }
            }
            if (currentMin.a != -1 && currentMin.b != -1)
            {
                sum += minPrice;
            }
        }

        return sum.ToString();
    }
}