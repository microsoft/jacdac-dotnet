using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SnippetExtractor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var input = Path.GetFullPath(args[0]);
            var output = Path.GetFullPath(args[1]);
            Console.WriteLine($"processing ${input}/*.cs -> ${output}.g.cs");
            if (!Directory.Exists(output))
                Directory.CreateDirectory(output);
            foreach (var file in Directory.GetFiles(input, "*.cs"))
            {
                ProcessFile(file, output);
            }
        }

        static void ProcessFile(string file, string output)
        {
            var ofile = Path.GetFullPath(Path.Combine(output, Path.ChangeExtension(Path.GetFileName(file).ToLowerInvariant(), ".g.cs")));
            Console.WriteLine($"  {file} -> ${ofile}");
            var csharp = File.ReadAllText(file);
            var lines = csharp.Split("\n");
            var namespaces = new List<string>();
            var sources = new List<string>();
            List<string> current = null;
            var currentStartIndex = 0;

            foreach (var line in lines)
            {
                var mstart = Regex.Match(line, @"^\s*#region\s+(namespaces|sources)?\s*");
                if (mstart.Success)
                {
                    var region = mstart.Groups[1].Value;
                    if (string.IsNullOrEmpty(region))
                    {
                        Console.Error.WriteLine("only namespaces and sources regions");
                        return;
                    }
                    if (current != null)
                    {
                        Console.Error.WriteLine("nested regions not allowed");
                        return;
                    }
                    current = region == "namespaces" ? namespaces : sources;
                    currentStartIndex = line.Length - line.TrimStart().Length;
                }
                else
                {
                    var mend = Regex.Match(line, @"^\s*#endregion\s*");
                    if (mend.Success)
                    {
                        current = null;
                        currentStartIndex = 0;
                    }
                    else if (current != null)
                        current.Add(String.IsNullOrWhiteSpace(line) ? "" : line.Substring(currentStartIndex).TrimEnd());
                }
            }

            if (sources.Count == 0)
            {
                Console.Error.WriteLine("missing sources");
                return;
            }

            namespaces.Sort();

            using var writer = new StreamWriter(File.OpenWrite(ofile));
            //writer.WriteLine("```cs");
            foreach (var ns in namespaces)
                writer.WriteLine(ns);
            writer.WriteLine();
            writer.WriteLine("...");
            writer.WriteLine();
            foreach (var line in sources)
                writer.WriteLine(line);
            //writer.WriteLine("```");

        }
    }
}
