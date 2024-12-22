using AdventOfCode2024.Utils;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Security;
using System.Xml.Linq;

namespace AdventOfCode2024.Puzzles;

internal class Day21 : ISolver
{
    Dictionary<(int, int), char> dirToChar = new ()
        {
            { (0, 1), '>' },
            { (0, -1), '<' },
            { (1, 0), 'v' },
            { (-1, 0), '^' }
        };

    Dictionary<char, (int, int)> charToDir = new()
        {
            { '>', (0, 1) },
            { '<', (0, -1)},
            { 'v', (1, 0) },
            { '^', (-1, 0) }
        };

    Dictionary<(int, int), char> keyPosToChar = new ()
        {
                             { (0, 1), '^' }, { (0, 2), 'A' },
            { (1, 0), '<' }, { (1, 1), 'v' }, { (1, 2), '>' }
        };

    Dictionary<char, (int, int)> keyToPos = new ()
        {
                             { '^', (0, 1) }, { 'A', (0, 2) },
            { '<', (1, 0) }, { 'v', (1, 1) }, { '>', (1, 2) }
        };

    Dictionary<(int, int), char> numPosToChar = new ()
        {
            { (0, 0), '7' }, { (0, 1), '8' }, { (0, 2), '9' },
            { (1, 0), '4' }, { (1, 1), '5' }, { (1, 2), '6' },
            { (2, 0), '1' }, { (2, 1), '2' }, { (2, 2), '3' },
                             { (3, 1), '0' }, { (3, 2), 'A' }
        };

    Dictionary<char, (int, int)> numToPos = new ()
        {
            { '7' , (0, 0) }, { '8' , (0, 1) }, { '9' , (0, 2) },
            { '4' , (1, 0) }, { '5' , (1, 1) }, { '6' , (1, 2) },
            { '1' , (2, 0) }, { '2' , (2, 1) }, { '3' , (2, 2) },
                              { '0' , (3, 1) }, { 'A' , (3, 2) }
        };

    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var keyPaths = BuildShortestPaths(dirToChar, keyPosToChar);
        var numPaths = BuildShortestPaths(dirToChar, numPosToChar);

        var sum = 0L;

        while (stream.ReadLine() is string line)
        {
            var num = int.Parse(line[..^1]);
            var paths = GetPossibleShortestCombinations((3, 2), numPaths, line, numToPos, int.MaxValue);

            var currentShortest = int.MaxValue;
            for (int i = 0; i < 2; i++)
            {
                currentShortest = int.MaxValue;
                var nextPathsList = new List<string>();
                foreach (var p in paths)
                {
                    var next = GetPossibleShortestCombinations((0, 2), keyPaths, p, keyToPos, currentShortest);
                    if (next.Count == 0)
                    {
                        continue;
                    }
                    if (next[0].Length < currentShortest)
                    {
                        currentShortest = next[0].Length;
                        nextPathsList.Clear();
                    }
                    if (next[0].Length == currentShortest)
                    {
                        nextPathsList.AddRange(next);
                    }
                }
                paths = nextPathsList;
            }
            sum += num * currentShortest;
        }

        return sum.ToString();
    }

    private List<string> GetPossibleShortestCombinations((int y, int x) robot, Dictionary<((int, int), (int, int)), List<string>> keypadPaths, string line, Dictionary<char, (int, int)> charToPos, int maxLen)
    {
        var shortestPathLen = int.MaxValue;
        var possiblePaths = new List<string>();

        var stack = new Stack<(string code, (int y, int x) pos, string path)>();
        // (3, 2), (0, 2), (0, 2)
        stack.Push((line, robot, ""));

        while (stack.Count > 0)
        {
            var (code, pos, path) = stack.Pop();

            if (path.Length > maxLen)
            {
                return [];
            }
            if (code.Length == 0)
            {
                if (shortestPathLen > path.Length)
                {
                    shortestPathLen = path.Length;
                    possiblePaths.Clear();
                }
                if (shortestPathLen == path.Length)
                {
                    possiblePaths.Add(path);
                }
                continue;
            }

            var neededPos = charToPos[code[0]];

            if (neededPos != pos)
            {
                foreach (var p in keypadPaths[(pos, neededPos)])
                {
                    stack.Push((code[1..], neededPos, $"{path}{p}{'A'}"));
                }
            }
            else
            {
                stack.Push((code[1..], neededPos, $"{path}{'A'}"));
            }

        }

        return possiblePaths;
    }

    private Dictionary<((int, int), (int, int)), List<string>> BuildShortestPaths(Dictionary<(int, int), char> charDirections, Dictionary<(int, int), char> keypad)
    {
        var path = new Dictionary<((int, int), (int, int)), List<string>>();
        foreach (var source in keypad.Keys)
        {
            foreach (var dest in keypad.Keys)
            {
                if (source != dest && !path.ContainsKey((source, dest)))
                {
                    path[(source, dest)] = GetShortestPaths(charDirections, keypad, source, dest);
                }
            }
        }
        return path;
    }

    private List<string> GetShortestPaths(Dictionary<(int, int), char> charDirections, Dictionary<(int, int), char> map, (int, int) source, (int, int) dest)
    {
        var distance = new Dictionary<(int, int), int>();
        distance[source] = 0;

        var queue = new Queue<((int y, int x) pos, string path)>();
        queue.Enqueue((source, ""));

        var shortestPaths = new List<string>();
        var minDist = int.MaxValue;

        while (queue.Count > 0)
        {
            var (pos, path) = queue.Dequeue();
            var distHere = distance[pos];

            if (pos == dest)
            {
                if (distHere < minDist)
                {
                    minDist = distHere;
                    shortestPaths.Clear();
                }
                if (distHere == minDist)
                {
                    shortestPaths.Add(path);
                }
                continue;
            }

            if (distHere >= minDist)
            {
                continue;
            }

            foreach (var (n, dir) in GetNeighbors(pos, map))
            {
                var newDist = distHere + 1;
                var newPath = path + charDirections[dir];

                if (!distance.ContainsKey(n) || newDist < distance[n])
                {
                    distance[n] = newDist;
                    queue.Enqueue((n, newPath));
                }
                else if (newDist == distance[n])
                {
                    queue.Enqueue((n, newPath));
                }
            }
        }

        return shortestPaths;
    }

    private List<((int y, int x) n,  (int y, int x) dir)> GetNeighbors((int y, int x) pos, Dictionary<(int, int), char> map)
    {
        var list = new List<((int y, int x) n, (int y, int x) dir)>();
        var directions = new (int y, int x)[]
        {
            (0, 1),
            (0, -1),
            (-1, 0),
            (1, 0),
        };

        for (int i = 0; i < 4; i++)
        {
            var next = (y: pos.y + directions[i].y, x: pos.x + directions[i].x);
            if (map.ContainsKey(next))
            {
                list.Add((next, directions[i]));
            }
        }

        return list;
    }
}