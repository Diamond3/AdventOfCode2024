using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day14 : ISolver
{
    private const int Height = 103;
    private const int Width = 101;
    private const int Seconds = 100;

    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");
        var sum = 1L;
        var halfWidth = Width / 2;
        var halfHeight = Height / 2;

        var quadrants = new int[4];
        var set = new HashSet<(int y, int x)>();
        
        while (stream.ReadLine() is string line)
        {

            var nums = line.Split(new char[] { ' ', '=', ',', 'v', 'p' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToArray();

            var p = (y: nums[1], x: nums[0]);
            var v = (y: nums[3], x: nums[2]);

            var pos = (y: (p.y + v.y * Seconds) % Height, x: (p.x + v.x * Seconds) % Width);

            if (pos.y < 0)
            {
                pos.y = Height + pos.y;
            }

            if (pos.x < 0)
            {
                pos.x = Width + pos.x;
            }

            set.Add(pos);

            var quadrantIndx = GetQuadrantIndex(pos, halfHeight, halfWidth);
            if (quadrantIndx != -1)
            {
                quadrants[quadrantIndx]++;
            }
        }

        for (int i = 0; i < quadrants.Length; i++)
        {
            sum *= quadrants[i];
        }

        return sum.ToString();
    }

    private int GetQuadrantIndex((int y, int x) pos, int halfHeight, int halfWidth)
    {
        if (pos.x < halfWidth && pos.y < halfHeight)
        {
            return 0;
        }

        if (pos.x > halfWidth && pos.y < halfHeight)
        {
            return 1;
        }

        if (pos.x < halfWidth && pos.y > halfHeight)
        {
            return 2;
        }

        if (pos.x > halfWidth && pos.y > halfHeight)
        {
            return 3;
        }

        return -1;
    }
}