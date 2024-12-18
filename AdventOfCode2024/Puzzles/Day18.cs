using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day18 : ISolver
{
    int MapSize = 71;

    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var map = new char[MapSize][];
        var dist = new int[MapSize][];
        var prev = new (int y, int x)[MapSize][];

        for (int y = 0; y < MapSize; y++)
        {
            var arr = new char[MapSize];
            var distArr = new int[MapSize];
            var prevArr = new int[MapSize];
            for (int x = 0; x < MapSize; x++)
            {
                arr[x] = '.';
                distArr[x] = int.MaxValue;
            }
            map[y] = arr;
            dist[y] = distArr;
        }

        for (int i = 0; i < 1024; i++)
        {
            var split = stream.ReadLine()!.Split(',').Select(int.Parse).ToArray();
            map[split[1]][split[0]] = '#';
        }

        var queue = new PriorityQueue<(int y, int x), int>();
        var visited = new HashSet<(int, int)>();

        dist[0][0] = 0;
        queue.Enqueue((0, 0), 0);

        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();

            foreach (var n in GetNeighbors(pos, map))
            {
                var cost = 1;
                var d = dist[pos.y][pos.x] + cost;

                if (d < dist[n.y][n.x])
                {
                    dist[n.y][n.x] = d;
                    queue.Enqueue(n, dist[n.y][n.x]);
                }
            }

            visited.Add(pos);
        }

        for (int y = 0; y < MapSize; y++)
        {
            for (int x = 0; x < MapSize; x++)
            {
                Console.Write(map[y][x] + " ");
            }
            Console.WriteLine();
        }

        return dist[MapSize - 1][MapSize - 1].ToString();
    }

    private List<(int y, int x)> GetNeighbors((int y, int x) pos, char[][] map)
    {
        var list = new List<(int y, int x)>();
        var directions = new (int y, int x)[4]
        {
            (0, 1),
            (0, -1),
            (-1, 0),
            (1, 0),
        };

        for (int i = 0; i < 4; i++)
        {
            var y = pos.y + directions[i].y;
            var x = pos.x + directions[i].x;

            if (x >= 0 && x < MapSize && y >= 0 && y < MapSize && map[y][x] != '#')
            {
                list.Add((y, x));
            }
        }

        return list;
    }
}
