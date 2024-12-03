using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day3 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var wholeSum = 0L;

        var isEnabled = true;
        while (stream.ReadLine() is string line)
        {
            for (int i = 0; i < line.Length - 4; i++)
            {
                var j = i;
                int a = 0, b = 0;

                if (line.Substring(j, 4) == "do()")
                {
                    isEnabled = true;
                    i += 3;
                    continue;
                }

                if (j + 6 < line.Length && line.Substring(j, 7) == "don't()")
                {
                    isEnabled = false;
                    i += 6;
                    continue;
                }

                if (!isEnabled)
                {
                    continue;
                }
                
                if (line.Substring(j, 4) == "mul(")
                {
                    
                    j += 4;

                    var c = 0;
                    while(c < 3 && char.IsNumber(line[j]))
                    {
                        a = a * 10 + (line[j] - '0');
                        j++;
                        c++;

                    }

                    if (c == 0 || line[j++] != ',')
                    {
                        continue;
                    }

                    c = 0;
                    while (c < 3 && char.IsNumber(line[j]))
                    {
                        b = b * 10 + (line[j] - '0');
                        j++;
                        c++;
                    }

                    if (c == 0 || line[j] != ')')
                    {
                        continue;
                    }

                    wholeSum += a * b;
                    i = j;
                }
            }
        }

        return wholeSum.ToString();
    }
}