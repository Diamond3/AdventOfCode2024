using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day19 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var dict = new Dictionary<char, Dictionary<int, HashSet<string>>>();
        var count = 0L;

        while (stream.ReadLine() is string line && !string.IsNullOrEmpty(line))
        {
            var split = line.Split(", ", StringSplitOptions.RemoveEmptyEntries);
            foreach (var str in split)
            {
                var key = str[0];
                if (!dict.ContainsKey(key))
                {
                    dict[str[0]] = new Dictionary<int, HashSet<string>>();
                }

                if (!dict[key].ContainsKey(str.Length))
                {
                    dict[key][str.Length] = new HashSet<string>();
                }

                dict[key][str.Length].Add(str);
            }
        }

        while (stream.ReadLine() is string line)
        {
            var memory = new Dictionary<(int ind, int depth), long>();
            var queue = new Queue<(int ind, int depth, int prevInd)>();
            var lastDepth = new HashSet<(int, int)>();

            queue.Enqueue((0, 0, -1));

            while (queue.Count > 0)
            {
                var (ind, depth, prevInd) = queue.Dequeue();
                var key = (ind, depth);
                var prevKey = (prevInd, depth - 1);

                if (memory.ContainsKey(key))
                {
                    if (memory.ContainsKey(prevKey))
                    {
                        memory[key] += memory[prevKey];
                    }
                    else
                    {
                        memory[key]++;
                    }
                    continue;
                }

                memory[key] = 1;
                if (memory.ContainsKey(prevKey))
                {
                    memory[key] = memory[prevKey];
                }

                if (ind >= line.Length)
                {
                    lastDepth.Add(key);
                    continue;
                }

                foreach (var index in NextIndexes(ind, line, dict))
                {
                    queue.Enqueue((index, depth + 1, ind));
                }
            }

            count += lastDepth.Select(x => memory[x]).Sum();
        }

        return count.ToString();
    }

    private List<int> NextIndexes(int c, string str, Dictionary<char, Dictionary<int, HashSet<string>>> dict)
    {
        var list = new List<int>();
        var key = str[c];
        if (dict.ContainsKey(key))
        {
            for (int i = 1; c + i <= str.Length; i++)
            {
                if (dict[key].TryGetValue(i, out var set) && set.Contains(str.Substring(c, i)))
                {
                    list.Add(c + i);
                }
            }
        }

        return list;
    }
}
