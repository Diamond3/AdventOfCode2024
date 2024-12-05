using AdventOfCode2024.Utils;
using System.Globalization;

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

            var list = new List<int>();

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

            if (!isValid)
            {
                var hashSet = new HashSet<int>(nums);
                var neededRules = new Dictionary<int, List<int>>(); // Get only rules that are needed

                var newLineSet = new HashSet<int>(); // Newly created line set
                var midNum = 0;

                foreach (var num in hashSet)
                {
                    neededRules[num] = new List<int>();

                    foreach (var r in rules[num])
                    {
                        if (hashSet.Contains(r))
                        {
                            neededRules[num].Add(r);
                        }
                    }

                    if (neededRules[num].Count == 0)
                    {
                        newLineSet.Add(num); // No Rules need for these so they are added to the front
                    }
                }

                for (int i = 0; i < neededRules.Count && newLineSet.Count != neededRules.Count; i++)
                {
                    foreach (var r in neededRules.Keys)
                    {
                        if (!newLineSet.Contains(r) && ContainsAllNeededValues(newLineSet, neededRules[r]))
                        {
                            newLineSet.Add(r);

                            if (newLineSet.Count - 1 == nums.Length / 2)
                            {
                                midNum = r;
                            }
                        }
                    }
                }

                if (newLineSet.Count == nums.Length) // Only if all rules were satisfied
                {
                    sum += midNum;
                }
            }
        }

        return sum.ToString();
    }

    private bool ContainsAllNeededValues(HashSet<int> newLineSet, List<int> list)
    {
        foreach(var num in list)
        {
            if (!newLineSet.Contains(num))
            {
                return false;
            }
        }
        return true;
    }
}