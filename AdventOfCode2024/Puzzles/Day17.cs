using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Puzzles;

internal class Day17 : ISolver
{
    public string Solve()
    {
        using var stream = new StreamReader($"Inputs/{GetType().Name}.txt");

        var registers = new Dictionary<int, int>();

        while (stream.ReadLine() is string line && !string.IsNullOrEmpty(line))
        {
            var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            registers.Add(split[1][0] - 'A', int.Parse(split[^1]));
        }

        var program = stream.ReadLine()!
            .Split(' ')[1]
            .Split(',')
            .Select(int.Parse)
            .ToArray();

        var pointer = 0;
        while (true)
        {
            if (pointer + 1 >= program.Length)
            {
                break;
            }

            pointer = ApplyOperation(pointer, program, registers);
        }


        return "".ToString();
    }

    private int ComboOp(int n, Dictionary<int, int> reg)
    {
        if (n < 4)
        {
            return n;
        }

        return reg[n - 4];
    }

    private int ApplyOperation(int indx, int[] program, Dictionary<int, int> reg)
    {
        var op = program[indx + 1];
        var jumped = false;

        switch (program[indx])
        {
            case 0:
                reg[0] = reg[0] / (int)Math.Pow(2, ComboOp(op, reg));
                break;

            case 1:
                reg[1] = reg[1] ^ op;
                break;

            case 2:
                reg[1] = ComboOp(op, reg) % 8;
                break;

            case 3:
                if (reg[0] != 0)
                {
                    indx = op;
                    jumped = true;
                }
                break;

            case 4:
                reg[1] = reg[1] ^ reg[2];
                break;

            case 5:
                Console.Write($"{ComboOp(op, reg) % 8},");
                break;

            case 6:
                reg[1] = reg[0] / (int)Math.Pow(2, ComboOp(op, reg));
                break;

            case 7:
                reg[2] = reg[0] / (int)Math.Pow(2, ComboOp(op, reg));
                break;
        }

        if (!jumped)
        {
            indx += 2;
        }
        return indx;
    }
}

