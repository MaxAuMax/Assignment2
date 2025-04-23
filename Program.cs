using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: Assignment2 <input-folder> <output-report.html>");
            return;
        }

        string inputPath = args[0];
        string outputPath = args[1];

        if (!Directory.Exists(inputPath))
        {
            Console.WriteLine($"Directory does not exist: {inputPath}");
            return;
        }

        var files = FileAnalyzer.EnumerateFilesRecursively(inputPath);
        var report = FileAnalyzer.CreateReport(files);
        report.Save(outputPath);

        Console.WriteLine($"Report generated: {outputPath}");
    }
}
