using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day23 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");
        var graph = new Dictionary<string, List<string>>();

        while (stream.ReadLine() is string line)
        {
            if (line.Split('-', 2) is [var left, var right])
            {
                graph.TryAdd(left, []);
                graph[left].Add(right);

                graph.TryAdd(right, []);
                graph[right].Add(left);
            }
        }

        var longestSeq = new List<string>();
        BronKerbosch(graph, [], graph.Keys.ToList(), [], longestSeq);

        return string.Join(",", longestSeq.OrderBy(x => x));
    }

    private void BronKerbosch(Dictionary<string, List<string>> graph, List<string> current, List<string> candidates, List<string> exclued, List<string> longestSeq)
    {
        if (candidates.Count == 0 && exclued.Count == 0)
        {
            if (longestSeq.Count < current.Count)
            {
                longestSeq.Clear();
                longestSeq.AddRange(current);
            }
            return;
        }

        var pivot = candidates.Union(exclued).OrderBy(x => graph[x].Count).Last();
        foreach (var candidate in candidates.Except(graph[pivot]))
        {
            BronKerbosch(graph, new List<string>(current) { candidate },
                candidates.Intersect(graph[candidate]).ToList(),
                exclued.Intersect(graph[candidate]).ToList(),
                longestSeq);

            exclued.Add(candidate);
        }
    }
}