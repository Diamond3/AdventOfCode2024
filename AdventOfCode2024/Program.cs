﻿using AdventOfCode2024.Puzzles;
using AdventOfCode2024.Utils;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace AdventOfCode2024;

public class Program
{
    private const string Year = "2024";

    private static async Task Main(string[] args)
    {
        try
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();
            var cookie = configuration["AoC:SessionCookie"]!;
            var aocClient = new AocClient(cookie, Year);

            var solversClasses = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(ISolver).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToList();

            if (!solversClasses.Any())
            {
                Console.WriteLine("No puzzle solvers found.");
                return;
            }

            var latestSolver = solversClasses
                .OrderByDescending(x => int.Parse(x.Name.ToLowerInvariant().Replace("day", "")))
                .FirstOrDefault();

            if (latestSolver == null)
            {
                Console.WriteLine("No valid puzzle solvers found with proper naming.");
                return;
            }

            var day = int.Parse(latestSolver.Name.ToLowerInvariant().Replace("day", ""));
            var inputDirectory = "Inputs";
            var inputPath = $"{inputDirectory}/day{day}.txt";

            Directory.CreateDirectory(inputDirectory);

            if (!File.Exists(inputPath))
            {
                var input = await aocClient.GetDayInputAsJsonAsync(day);
                File.WriteAllText(inputPath, input);

                Console.WriteLine("Input file created!");
            }

            latestSolver = typeof(Day23);

            Console.WriteLine($"Executing {latestSolver}");

            var answer = (Activator.CreateInstance(latestSolver) as ISolver)?.Solve();
            Console.WriteLine(answer);

            /*var response = await aocClient.PostAnswerAsync(day, answer!, 1);
            Console.WriteLine(response);*/
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}