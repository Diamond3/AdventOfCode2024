using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day24 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var zBits = new PriorityQueue<string, int>();
        var wires = new Dictionary<string, int>();
        var gates = new Dictionary<string, (string left, string right, string operation)>();

        while (stream.ReadLine() is string line && !string.IsNullOrEmpty(line))
        {
            if (line.Split(": ", 2) is [var node, var value])
            {
                wires[node] = int.Parse(value);
            }
        }

        while (stream.ReadLine() is string line && !string.IsNullOrEmpty(line))
        {
            if (line.Split([' ', '-', '>'], 4, StringSplitOptions.RemoveEmptyEntries) 
                is [var left, var operation, var right, var result])
            {
                gates[result] = (left, right, operation);
                if(result[0] == 'z')
                {
                    zBits.Enqueue(result, 100 - int.Parse(result[1..]));
                }
            }
        }

        var ans = "";
        while (zBits.Count > 0)
        {
            var key = zBits.Dequeue();
            var a = GetValue(wires, gates, key);
            wires[key] = a;

            ans += a;
        }

        return Convert.ToInt64(ans, 2).ToString();
    }

    private int GateOperationResult(int l, int r, string operation)
    {
        return operation switch
        {
            "XOR" => l ^ r,
            "AND" => l & r,
            _ => l | r
        };
    }

    private int GetValue(Dictionary<string, int> wires, Dictionary<string, (string left, string right, string operation)> gates, string node)
    {
        if (wires.ContainsKey(node))
        {
            return wires[node];
        }

        var (l, r, operation) = gates[node];
        var left = GetValue(wires, gates, l);
        var right = GetValue(wires, gates, r);

        wires[node] = GateOperationResult(left, right, operation);

        return wires[node];
    }
}