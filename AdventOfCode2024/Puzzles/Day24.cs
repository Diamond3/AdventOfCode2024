using AdventOfCode2024.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.Intrinsics.Arm;

namespace AdventOfCode2024.Puzzles;

internal class Day24 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var inputX = new List<string>();
        var inputY = new List<string>();
        var output = new List<string>();

        var zBitsQueue = new PriorityQueue<string, int>();
        var wires = new Dictionary<string, int>();
        var gates = new Dictionary<string, (string left, string right, string operation)>();

        var possibleSwaps = new (int a, int b, int c, int d)[]{
            (1, 0, 3, 2),
            (2, 3, 0, 1),
            (3, 2, 1, 0)
        };

        while (stream.ReadLine() is string line && !string.IsNullOrEmpty(line))
        {
            if (line.Split(": ", 2) is [var node, var value])
            {
                wires[node] = int.Parse(value);
                if (node[0] == 'x')
                {
                    inputX.Insert(0, node);
                }

                if (node[0] == 'y')
                {
                    inputY.Insert(0, node);
                }
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
                    zBitsQueue.Enqueue(result, 100 - int.Parse(result[1..]));
                }
            }
        }

        while (zBitsQueue.Count > 0)
        {
            output.Add(zBitsQueue.Dequeue());
        }

        var xNum = Convert.ToInt64(string.Join("", inputX.Select(x => wires[x])), 2);
        var yNum = Convert.ToInt64(string.Join("", inputY.Select(y => wires[y])), 2);
        var expectedOutput = Convert.ToString(xNum + yNum, 2);

        for (int i = 0; i < output.Count; i++)
        {
            var tempWires = new Dictionary<string, int>(wires);
            var nodes = new HashSet<string>();
            var a = GetValue(wires, gates, output[i], nodes);
            if (a != expectedOutput[i])
            {
                Console.WriteLine(output[i]);
            }
        }

        /*var ans = "";
        for (int i = 0; i < output.Count; i++)
        {
            var key = output[i];
            var a = GetValue(wires, gates, key);
            wires[key] = a;

            ans += a;
        }
        var b = ans.Length;*/

        //return Convert.ToInt64(ans, 2).ToString();
        return "";
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

    private int GetValue(Dictionary<string, int> wires, Dictionary<string, (string left, string right, string operation)> gates, string node, HashSet<string> nodes)
    {
        nodes.Add(node);
        if (wires.ContainsKey(node))
        {
            return wires[node];
        }

        var (l, r, operation) = gates[node];
        var left = GetValue(wires, gates, l, nodes);
        var right = GetValue(wires, gates, r, nodes);

        wires[node] = GateOperationResult(left, right, operation);

        return wires[node];
    }
}