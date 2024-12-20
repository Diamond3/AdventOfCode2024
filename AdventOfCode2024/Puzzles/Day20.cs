using AdventOfCode2024.Utils;
using System.Diagnostics;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode2024.Puzzles;

internal class Day20 : ISolver
{
    private (int y, int x)[] Directions =
        [
            (0, 1),
            (0, -1),
            (-1, 0),
            (1, 0),
        ];

    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var map = new List<char[]>();

        var path = new HashSet<(int, int)>();

        var start = (y: 0, x: 0);
        var end = (y: 0, x: 0);

        var st = new Stopwatch();

        var y = 0;
        while (stream.ReadLine() is string line)
        {
            for (int x = 0; x < line.Length; x++)
            {
                if (line[x] == 'S')
                {
                    start = (y, x);
                }

                if (line[x] == 'E')
                {
                    end = (y, x);
                }

                if (line[x] == 'E' || line[x] == 'S' || line[x] == '.')
                {
                    path.Add((y, x));
                }
            }
            y++;
        }

        var cheatCount = 0;
        var cheatDict = new Dictionary<int, int>();
        var cheked = new HashSet<((int, int), (int, int))>();

        var mainPathLen = GetPathLen(path, start, end);

        var pos = (y: start.y + 1, x: start.x);
        var visited = new HashSet<(int, int)>();

        while (true)
        {
            for (int i = 0; i < 4; i++)
            {
                var newPos = (pos.y + Directions[i].y, pos.x + Directions[i].x);

                if (!visited.Contains(newPos) && path.Contains(newPos))
                {
                    pos = newPos;
                    visited.Add(newPos);

                    var stack = new Stack<((int y, int x) current, HashSet<(int, int)> visited)>();
                    for (int k = 0; k < 4; k++)
                    {
                        var nextPos = (pos.y + Directions[k].y, pos.x + Directions[k].x);
                        newPos = (pos.y + Directions[k].y * 2, pos.x + Directions[k].x * 2);

                        if (!path.Contains(nextPos) && !visited.Contains(newPos) && path.Contains(newPos))
                        {
                            stack.Push((newPos, new HashSet<(int, int)>(visited) { nextPos, newPos }));
                        }
                    }

                    while (stack.Count > 0)
                    {
                        var (current, visitedSet) = stack.Pop();

                        var length = visitedSet.Count - 1;
                        if (current == end && length < mainPathLen)
                        {
                            var key = mainPathLen - length;

                            if (!cheatDict.ContainsKey(key))
                            {
                                cheatDict[key] = 0;
                            }

                            cheatDict[key]++;
                            cheatCount++;
                        }

                        for (int l = 0; l < 4; l++)
                        {
                            newPos = (current.y + Directions[l].y, current.x + Directions[l].x);

                            if (!visitedSet.Contains(newPos) && path.Contains(newPos))
                            {
                                stack.Push((newPos, new HashSet<(int, int)>(visitedSet) { newPos }));
                            }
                        }
                    }

                    break;
                }
            }

            if (visited.Contains(end))
            {
                break;
            }
        }

        return cheatCount.ToString();
    }

    private int GetPathLen(HashSet<(int, int)> path, (int y, int x) start, (int y, int x) end)
    {
        var pos = start;
        var prev = start;
        var len = 0;

        while (true)
        {
            for (int i = 0; i < 4; i++)
            {
                var newPos = (pos.y + Directions[i].y, pos.x + Directions[i].x);

                if (prev != newPos && path.Contains(newPos))
                {
                    prev = pos;
                    pos = newPos;
                    len++;
                    break;
                }
            }

            if (pos == end)
            {
                return len;
            }
        }
    }

    private void SetHeuristic(List<char[]> map, List<int[]> h, (int y, int x) end)
    {
        for (int y = 0; y < map.Count; y++)
        {
            var hArr = new int[map[0].Length];
            for (int x = 0; x < map[0].Length; x++)
            {
                hArr[x] = Heuristic((y, x), end);
            }
            h.Add(hArr);
        }
    }

    private void PrintMap(HashSet<(int, int)> map, HashSet<(int, int)> visited)
    {
        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 16; x++)
            {
                if (visited.Contains((y, x)) && !map.Contains((y, x)))
                {
                    Console.Write('x');
                }
                else
                {
                    Console.Write(map.Contains((y, x)) ? '.' : '#');
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    private int ShortestPath(List<char[]> map, List<int[]> dist, List<int[]> h, (int y, int x) start, (int y, int x) end, int shortestTime, (int y, int x)[] jump)
    {
        ResetDist(dist);

        var queue = new PriorityQueue<(int y, int x), int>();
        var visited = new HashSet<(int, int)>();

        dist[start.y][start.x] = 0;
        queue.Enqueue(start, 0);

        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();

            foreach (var nTemp in GetNeighbors(pos, map))
            {
                var n = nTemp;
                var cost = 1;
                if (jump[0] == n)
                {
                    n = jump[1];
                    cost += 2;
                }
                var newDist = dist[pos.y][pos.x] + cost;

                if (newDist < shortestTime && newDist < dist[n.y][n.x])
                {
                    dist[n.y][n.x] = newDist;
                    queue.Enqueue(n, newDist + h[n.y][n.x]);
                }
            }

            visited.Add(pos);
        }

        return dist[end.y][end.x];
    }

    private static int Heuristic((int y, int x) current, (int y, int x) goal)
    {
        return Math.Abs(current.y - goal.y) + Math.Abs(current.x - goal.x);
    }

    private void ResetDist(List<int[]> dist)
    {
        for (int y = 0; y < dist.Count; y++)
        {
            for (int x = 0; x < dist[0].Length; x++)
            {
                dist[y][x] = int.MaxValue;
            }
        }
    }

    private List<(int y, int x)> GetNeighbors((int y, int x) pos, List<char[]> map)
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

            if (x >= 0 && x < map[0].Length && y >= 0 && y < map.Count && map[y][x] != '#')
            {
                list.Add((y, x));
            }
        }

        return list;
    }

    private List<(int y, int x)> GetNext((int y, int x) pos, List<char[]> map)
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

            if (IsLegal(map, y, x))
            {
                list.Add((y, x));
            }
        }

        return list;
    }

    private static bool IsLegal(List<char[]> map, int y, int x)
    {
        return x >= 0 && x < map[0].Length && y >= 0 && y < map.Count;
    }
}