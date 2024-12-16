using AdventOfCode2024.Utils;
using System.Collections.Generic;
using System.IO;
using System.Security;
using static AdventOfCode2024.Puzzles.Day16;

namespace AdventOfCode2024.Puzzles;

internal class Day16 : ISolver
{
    public class Node
    {
        public (int y, int x) Pos;
        public HashSet<Node> PossiblePrev = [];
        public Node Prev = null!;
        public HashSet<Node> Neighbors = [];
        public (int y, int x) CurrentDirection;
        public long Distance;
        public int Tiles = 0;

        public Node((int y, int x) pos)
        {
            CurrentDirection = (-1, -1);
            Pos = pos;
            Distance = long.MaxValue;
        }
    }

    private (int y, int x)[] directions =
    [
        (0, 1),
        (0, -1),
        (-1, 0),
        (1, 0),
    ];

    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");
        var charMap = new Dictionary<(int y, int x), char>();

        var y = 0;
        var start = (0, 0);
        var end = (0, 0);

        var mapWidth = 0;
        var mapHeight = 0;

        while (stream.ReadLine() is string line)
        {
            mapWidth = line.Length;

            for (int x = 0; x < line.Length; x++)
            {
                var key = (y, x);
                charMap[key] = line[x];

                if (line[x] == 'S')
                {
                    start = key;
                    charMap[key] = '.';
                }
                else if (line[x] == 'E')
                {
                    end = key;
                    charMap[key] = '.';
                }
            }
            y++;
        }

        mapHeight = y;
        var map = new Dictionary<((int y, int x) pos, (int y, int x) dir), Node>();
        CreateGraph(start, charMap, map);

        var visited = new HashSet<Node>();
        var queue = new PriorityQueue<Node, long>();

        queue.Enqueue(map[(start,(0, -1))], map[(start, (0, -1))].Distance);
        queue.Enqueue(map[(start,(-1, 0))], map[(start, (-1, 0))].Distance);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            foreach (var n in node.Neighbors)
            {
                var cost = n.CurrentDirection == node.CurrentDirection ? 0 : 1000;
                var dist = node.Distance + GetDistance(node.Pos, n.Pos) + cost;

                if (dist < n.Distance)
                {
                    n.PossiblePrev.Clear();
                    n.PossiblePrev.Add(node);
                    n.Distance = dist;
                    queue.Enqueue(n, dist);
                }
                else if (dist == n.Distance)
                {
                    n.PossiblePrev.Add(node);
                    queue.Enqueue(n, dist);
                }

            }
        }

        var min = long.MaxValue;
        var dirIndx = new List<int>();

        for (var i = 0; i < 4; i++)
        {
            if (map.TryGetValue((end, directions[i]), out var val))
            {
                if (val.Distance < min)
                {
                    min = val.Distance;
                    dirIndx.Clear();
                    dirIndx.Add(i);
                }
                else if (val.Distance == min)
                {
                    dirIndx.Add(i);
                }
            }
        }

        //Console.WriteLine(min);

        var tiles = new HashSet<(int, int)>()
        {
            start
        };

        foreach (var i in dirIndx)
        {
            BuildPath(map[(end, directions[i])], tiles);
        }

        //PrintMap(charMap, map, end, mapWidth, mapHeight, tiles);

        return tiles.Count.ToString();
    }

    private void BuildPath(Node current, HashSet<(int, int)> path)
    {
        if (current != null)
        {
            foreach (var node in current.PossiblePrev)
            {
                var (y, x) = GetDirToNode(current.Pos, node.Pos);
                var temp = current.Pos;
                while (temp != node.Pos)
                {
                    path.Add(temp);
                    temp = (temp.y + y, temp.x + x);
                }

                //Console.WriteLine($"{node.Pos} -> {node.PossiblePrev.Count}");
                BuildPath(node, path);
            }
        }
    }

    private void PrintMap(Dictionary<(int y, int x), char> charMap, Dictionary<(int y, int x), Node> map, (int, int) end, int mapWidth, int mapHeight, HashSet<(int, int)> path)
    {
        foreach (var node in map.Values)
        {
            Console.WriteLine($"{node.Pos} -> {node.PossiblePrev.Count}");
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (path.Contains((y, x)))
                {
                    Console.Write('O');
                }
                else
                {
                    Console.Write(charMap[(y, x)]);
                }
            }
            Console.WriteLine();
        }
    }

    private (int y, int x) GetDirToNode((int y, int x) pos, (int y, int x) nPos)
    {
        return (y: Math.Sign(nPos.y - pos.y), x: Math.Sign(nPos.x - pos.x));
    }

    private long GetDistance((int y, int x) pos, (int y, int x) nPos)
    {
        return Math.Abs(nPos.y - pos.y) + Math.Abs(nPos.x - pos.x);
    }

    private void CreateGraph((int, int) start, Dictionary<(int y, int x), char> charMap, Dictionary<((int y, int x) pos, (int y, int x) dir), Node> map)
    {
        var visited = new HashSet<Node>();
        var stack = new Stack<Node>();

        var s1 = new Node(start)
        {
            CurrentDirection = (0, -1),
            Distance = 0
        };

        var s2 = new Node(start)
        {
            CurrentDirection = (-1, 0),
            Distance = 1000
        };

        map[(s1.Pos, s1.CurrentDirection)] = s1;
        map[(s2.Pos, s2.CurrentDirection)] = s2;

        stack.Push(s1);
        stack.Push(s2);

        while (stack.Count > 0)
        {
            var parent = stack.Pop();
            foreach (var n in GetNeighbors(parent.Pos, charMap))
            {
                var dir = GetDirToNode(parent.Pos, n);

                if (!map.ContainsKey((n, dir)))
                {
                    map[(n, dir)] = new Node(n)
                    {
                        CurrentDirection = dir
                    };
                }

                var node = map[(n, dir)];

                if (!visited.Contains(node))
                {
                    stack.Push(node);
                }

                parent.Neighbors.Add(node);
            }

            visited.Add(parent);
        }
    }

    private List<(int y, int x)> GetNeighbors((int y, int x) pos, Dictionary<(int y, int x), char> map)
    {
        var list = new List<(int y, int x)>();

        for (int i = 0; i < 4; i++)
        {
            var y = pos.y + directions[i].y;
            var x = pos.x + directions[i].x;

            if (map[(y, x)] == '.')
            {
                list.Add(GetLastOrIntersectionPos((y, x), directions[i], map));
            }
        }

        return list;
    }

    private (int y, int x) GetLastOrIntersectionPos((int y, int x) pos, (int y, int x) dir, Dictionary<(int y, int x), char> map)
    {
        var dist = 0;
        var nextPos = pos;

        do
        {
            nextPos = (y: pos.y + dir.y * dist, x: pos.x + dir.x * dist);
            dist++;
            if (map[nextPos] == '#')
            {
                dist--;
                break;
            }
        }
        while (NoIntersection(dir, map, nextPos));
        dist--;
        return (y: pos.y + dir.y * dist, x: pos.x + dir.x * dist);
    }

    private bool NoIntersection((int y, int x) dir, Dictionary<(int y, int x), char> map, (int y, int x) key)
    {
        for (int i = 0; i < 4; i++)
        {
            if ((-dir.y, -dir.x) == directions[i]
                || dir == directions[i])
            {
                continue;
            }

            if (map[(key.y + directions[i].y, key.x + directions[i].x)] == '.')
            {
                return false;
            }
        }

        return true;
    }
}

