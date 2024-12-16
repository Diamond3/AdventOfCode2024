using AdventOfCode2024.Utils;
using System.Collections.Generic;
using System.IO;
using System.Security;

namespace AdventOfCode2024.Puzzles;

internal class Day16 : ISolver
{
    public class Node
    {
        public Node Prev = null!;
        public HashSet<(int, int)> Neighbors = [];
        public (int y, int x) CurrentDirection;
        public long Distance;
        public (int y, int x) Pos;

        public Node((int y, int x) pos)
        {
            CurrentDirection = (-1, -1);
            Pos = pos;
            Distance = long.MaxValue;
        }
    }

    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");
        var charMap = new Dictionary<(int y, int x), char>();
        var map = new Dictionary<(int y, int x), Node>();

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

        CreateGraph(start, charMap, map);

        var visited = new HashSet<Node>();
        var queue = new PriorityQueue<Node, long>();

        queue.Enqueue(map[start], map[start].Distance);
        map[start].Distance = 0;
        map[start].CurrentDirection = (0, 1);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            foreach (var nPos in node.Neighbors)
            {
                var n = map[nPos];
                var nodeDir = GetDirToNode(node.Pos, n.Pos);
                var cost = nodeDir == node.CurrentDirection ? 0 : 1000;
                var dist = node.Distance + GetDistance(node.Pos, n.Pos) + cost;
                if (dist <= n.Distance)
                {
                    n.CurrentDirection = nodeDir;
                    n.Distance = dist;
                    n.Prev = node;
                }

                if (visited.Add(n))
                {
                    queue.Enqueue(n, dist);
                }
            }
        }

        //PrintMap(charMap, map, end, mapWidth, mapHeight);

        return map[end].Distance.ToString();
    }

    private void PrintMap(Dictionary<(int y, int x), char> charMap, Dictionary<(int y, int x), Node> map, (int, int) end, int mapWidth, int mapHeight)
    {
        var path = new HashSet<(int, int)>() { end };
        var currentNode = map[end];

        while (currentNode != null)
        {
            path.Add(currentNode.Pos);
            //Console.WriteLine(currentNode.Distance);
            currentNode = currentNode.Prev;
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

    private void CreateGraph((int, int) start, Dictionary<(int y, int x), char> charMap, Dictionary<(int y, int x), Node> map)
    {
        var visited = new HashSet<(int, int)>();
        var stack = new Stack<(int y, int x)>();
        stack.Push(start);

        map[start] = new Node(start)
        {
            Distance = 0
        };

        while (stack.Count > 0)
        {
            var pos = stack.Pop();
            foreach (var n in GetNeighbors(pos, charMap))
            {
                if (!map.ContainsKey(n))
                {
                    map[n] = new Node(n);
                }

                if (!visited.Contains(n))
                {
                    stack.Push(n);
                }

                map[pos].Neighbors.Add(n);
            }

            visited.Add(pos);
        }
    }

    private List<(int y, int x)> GetNeighbors((int y, int x) pos, Dictionary<(int y, int x), char> map)
    {
        var list = new List<(int y, int x)>();
        var directions = new (int y, int x)[]
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
        var directions = new (int y, int x)[]
        {
                    (0, 1),
                    (0, -1),
                    (-1, 0),
                    (1, 0),
        };

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

