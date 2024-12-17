using AdventOfCode2024.Utils;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode2024.Puzzles;

internal class Day17 : ISolver
{
    public class Operation
    {
        public char Ans;
        public char Left;
        public Op Op;
        public char Right;
        public bool IsOutput = false;
        public bool Is2Pow = false;
        public bool IsJump = false;

        public Operation(char ans, char left, Op op, char right)
        {
            Ans = ans;
            Left = left;
            Op = op;
            Right = right;
        }
        public Operation()
        {

        }
    }

    public enum Op
    {
        Div, Mod, Xor
    }
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
        var output = new List<int>();

        while (true)
        {
            if (pointer + 1 >= program.Length)
            {
                break;
            }

            pointer = ApplyOperation(pointer, program, registers, output);
        }

        for (int i = 0; i < output.Count; i++)
        {
            Console.Write(output[i] + " ");
        }
        Console.WriteLine();

        /*var ans = 0;

        var l = new int[]{ 30886132, 3860766, 482595, 60324, 7540, 942, 117, 14, 1 };

        var A = 14;
        for (int i = 0; i < output.Count; i++)
        {
            A = l[i];
            ans = ((A % 8) ^ 1 ^ 4 ^ (A / (int)Math.Pow(2, ((A % 8) ^ 1)))) % 8;
            //A /= 8;
            Console.WriteLine($"{A} -> {ans}");
        }*/


        // Full program patter => ((A % 8) ^ 1 ^ 4 ^ (A / (int)Math.Pow(2, ((A % 8) ^ 1)))) % 8
        // next_a is in [current_a * 8; current_a * 8 + 7]

        var stack = new Stack<(long start, int i)>();
        stack.Push((1, program.Length - 1));

        var min = long.MaxValue;

        while (stack.Count > 0)
        {
            var (num, i) = stack.Pop();

            for (int j = 0; j < 8; j++)
            {
                var a = num + j;
                var div = (long)Math.Pow(2, (a % 8) ^ 1);

                if (div == 0L)
                {
                    continue;
                }

                var result = ((a % 8) ^ 1 ^ 4 ^ (a / div)) % 8;
                if (result == program[i])
                {
                    if (i == 0)
                    {
                        min = Math.Min(min, a);
                        //Console.WriteLine(a);
                        break;
                    }
                    stack.Push((a * 8, i - 1));
                }
            }
        }

        return min.ToString();
    }

    private int ComboOp(int n, Dictionary<int, int> reg)
    {
        if (n < 4)
        {
            return n;
        }

        return reg[n - 4];
    }

    private int ApplyOperation(int indx, int[] program, Dictionary<int, int> reg, List<int> output)
    {
        var op = program[indx + 1];
        var jumped = false;
        Console.WriteLine($"Opcode: {program[indx]}");
        Console.WriteLine($"Indx: {indx}");
        Console.WriteLine($"Operand: {op}");
        Console.WriteLine($"A: {reg[0]} | B: {reg[1]} | C: {reg[2]}");
        Console.WriteLine();

        switch (program[indx])
        {
            case 0:
                var a = ComboOp(op, reg);
                var b = reg[0];
                Console.WriteLine($"Combo OP: {a}");
                Console.WriteLine($"A = A {reg[0]} / 2^{a} = {b / (int)Math.Pow(2, a)}");

                Console.WriteLine($"A = A / 2^{GetCombOpAsChar(op)}");
                reg[0] = b / (int)Math.Pow(2, a);

                break;

            case 1:
                Console.WriteLine($"B = B {reg[1]} XOR {op} = {reg[1] ^ op}");
                Console.WriteLine($"B = B XOR {op}");
                reg[1] = reg[1] ^ op;
                break;

            case 2:
                Console.WriteLine($"Combo OP: {ComboOp(op, reg)}");
                Console.WriteLine($"B = {ComboOp(op, reg)} % 8 = {ComboOp(op, reg) % 8}");
                Console.WriteLine($"B = {GetCombOpAsChar(op)} % 8");
                reg[1] = ComboOp(op, reg) % 8;
                break;

            case 3:
                if (reg[0] != 0)
                {
                    indx = op;
                    Console.WriteLine($" A != 0 -> New indx: {indx}");
                    jumped = true;
                }
                else
                {
                    Console.WriteLine($"No Jump");
                }
                break;

            case 4:
                Console.WriteLine($"B = B {reg[1]} XOR C {reg[2]} = {reg[1] ^ reg[2]}");
                Console.WriteLine($"B = B XOR C");
                reg[1] = reg[1] ^ reg[2];
                break;

            case 5:
                Console.WriteLine($"Combo OP: {ComboOp(op, reg)}");
                Console.WriteLine($"    -> OUTPUT = {ComboOp(op, reg)} % 8 = {ComboOp(op, reg) % 8}");
                Console.WriteLine($"After: A: {reg[0]} | B: {reg[1]} | C: {reg[2]}");

                Console.WriteLine($"OUTPUT => {GetCombOpAsChar(op)} % 8");
                Console.Write($"{ComboOp(op, reg) % 8},");
                output.Add(ComboOp(op, reg) % 8);
                break;

            case 6:
                Console.WriteLine($"Combo OP: {ComboOp(op, reg)}");
                Console.WriteLine($"B = A {reg[1]} / 2^{ComboOp(op, reg)} = {reg[0] / (int)Math.Pow(2, ComboOp(op, reg))}");

                Console.WriteLine($"B = A / 2^{GetCombOpAsChar(op)}");
                reg[1] = reg[0] / (int)Math.Pow(2, ComboOp(op, reg));
                break;

            case 7:
                Console.WriteLine($"Combo OP: {ComboOp(op, reg)}");
                Console.WriteLine($"C = A {reg[1]} / 2^{ComboOp(op, reg)} = {reg[0] / (int)Math.Pow(2, ComboOp(op, reg))}");

                Console.WriteLine($"C = A / 2^{GetCombOpAsChar(op)}");
                reg[2] = reg[0] / (int)Math.Pow(2, ComboOp(op, reg));
                break;
        }

        if (!jumped)
        {
            indx += 2;
        }

        Console.WriteLine($"After: A: {reg[0]} | B: {reg[1]} | C: {reg[2]}");
        Console.WriteLine("----------------\n");
        return indx;
    }

    private char GetCombOpAsChar(int op)
    {
        if (op < 4)
        {
            return (char)(op + '0');
        }

        return (char)((op - 4) + 'A');
    }
}
