using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day23 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");
        var graph = new Dictionary<string, List<string>>();
        var tNodes = new HashSet<string>();

        while (stream.ReadLine() is string line)
        {
            if (line.Split('-', 2) is [var left, var right])
            {
                graph.TryAdd(left, []);
                graph[left].Add(right);

                graph.TryAdd(right, []);
                graph[right].Add(left);

                if (left[0] == 't')
                {
                    tNodes.Add(left);
                }
                else if (right[0] == 't')
                {
                    tNodes.Add(right);
                }
            }
        }

        var existingSet = new HashSet<string>();
        var count = 0;

        foreach (var tNode in tNodes)
        {
            var path = new Dictionary<string, List<string>>();
            var nextNodes = new HashSet<string>();
            var depth1 = graph[tNode];

            foreach (var node in depth1)
            {
                var depth2 = graph[node];
                foreach (var node2 in depth2)
                {
                    if (graph[node2].Contains(tNode))
                    {
                        var key  = string.Join(',', [.. new[] { tNode, node, node2 }.OrderBy(x => x)]);

                        if (existingSet.Add(key))
                        {
                            //Console.WriteLine(key);
                            count++;
                        }
                    }
                }
            }
        }

        return count.ToString();
    }
}