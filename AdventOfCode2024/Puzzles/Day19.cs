using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day19 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var dict = new Dictionary<char, Dictionary<int, HashSet<string>>>();
        var count = 0;

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
            var stack = new Stack<int>();

            stack.Push(0);

            var found = false;

            while (stack.Count > 0)
            {
                var i = stack.Pop();

                if (i >= line.Length)
                {
                    Console.WriteLine(line);
                    found = true;
                    break;
                }

                foreach (var index in NextIndexes(i, line, dict))
                {
                    stack.Push(index);
                }
            }

            if (found)
            {
                count++;
            }
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
