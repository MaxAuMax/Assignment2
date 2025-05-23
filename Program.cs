﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

class Program
{
    public static IEnumerable<string> EnumerateFilesRecursively(string path)
    {
        foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
        {
            yield return file;
        }
    }

    public static string FormatByteSize(long byteSize)
    {
        string[] units = { "B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB" };
        double size = byteSize;
        int unitIndex = 0;

        while (size >= 1000 && unitIndex < units.Length - 1)
        {
            size /= 1000;
            unitIndex++;
        }

        return $"{size:F2} {units[unitIndex]}";
    }

    public static XDocument CreateReport(IEnumerable<string> files)
    {
        var fileStats = files
            .Select(file => new FileInfo(file))
            .GroupBy(f => f.Extension.ToLower())
            .Select(g => new
            {
                Type = string.IsNullOrWhiteSpace(g.Key) ? "[no extension]" : g.Key,
                Count = g.Count(),
                Size = g.Sum(f => f.Length)
            })
            .OrderByDescending(entry => entry.Size);

        var rows = fileStats.Select(stat =>
            new XElement("tr",
                new XElement("td", stat.Type),
                new XElement("td", stat.Count),
                new XElement("td", FormatByteSize(stat.Size))
            )
        );

        return new XDocument(
            new XElement("html",
                new XElement("head",
                    new XElement("title", "File Report")
                ),
                new XElement("body",
                    new XElement("h1", "File Report"),
                    new XElement("table",
                        new XElement("tr",
                            new XElement("th", "Type"),
                            new XElement("th", "Count"),
                            new XElement("th", "Size")
                        ),
                        rows
                    )
                )
            )
        );
    }

    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: <input-folder> <output-report.html>");
            return;
        }

        string inputPath = args[0];
        string outputPath = args[1];

        if (!Directory.Exists(inputPath))
        {
            Console.WriteLine($"Directory does not exist: {inputPath}");
            return;
        }

        var files = EnumerateFilesRecursively(inputPath);
        var report = CreateReport(files);
        report.Save(outputPath);

        Console.WriteLine($"Report generated: {outputPath}");
    }
}
