using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day5 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var rules = new Dictionary<int, List<int>>();
        var sum = 0;

        while (stream.ReadLine() is string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                break;
            }

            var nums = line.Split('|').Select(int.Parse).ToArray();

            if (!rules.ContainsKey(nums[0]))
            {
                rules[nums[0]] = new List<int>();
            }

            rules[nums[0]].Add(nums[1]);
        }

        while (stream.ReadLine() is string line)
        {
            var chekedNums = new HashSet<int>();
            var nums = line.Split(',').Select(int.Parse).ToArray();
            var isValid = true;

            for (int i = 0; i < nums.Length; i++)
            {
                if (rules.ContainsKey(nums[i]))
                {
                    foreach (var r in rules[nums[i]])
                    {
                        if (chekedNums.Contains(r))
                        {
                            isValid = false;
                            break;
                        }
                    }
                }

                if (!isValid)
                {
                    break;
                }

                chekedNums.Add(nums[i]);
            }

            if (isValid)
            {
                sum += nums[nums.Length / 2];
            }
        }

        return sum.ToString();
    }
}