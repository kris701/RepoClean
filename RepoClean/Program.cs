using CommandLine;
using CommandLine.Text;
using RepoClean.Helpers;
using System;

namespace RepoClean
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<Options>(args);
            parserResult.WithNotParsed(errs => DisplayHelp(parserResult, errs));
            parserResult.WithParsed(Run);
        }

        public static void Run(Options opts)
        {
            opts.TargetPath = RootPath(opts.TargetPath);
            if (!Directory.Exists(opts.TargetPath))
            {
                Console.WriteLine($"Target path not found: {opts.TargetPath}");
                return;
            }
            var option = SearchOption.TopDirectoryOnly;
            if (opts.IsRecursive)
                option = SearchOption.AllDirectories;

            WriteColor("Finding directories... ", ConsoleColor.Blue);
            var directories = SearchDirectories(opts.TargetPath, option);
            WriteLineColor($"{directories.Count} found!", ConsoleColor.Green);
            if (directories.Count > 0)
                WriteLineColor("To delete:", ConsoleColor.Blue);

            foreach (var directory in directories)
            {
                WriteColor($"\t{directory} ", ConsoleColor.DarkGray);
                if (opts.IsShow)
                {
                    WriteLineColor("", ConsoleColor.DarkGray);
                    continue;
                }
                if (opts.IsForced)
                {
                    Directory.Delete(directory, true);
                    WriteLineColor("Removed", ConsoleColor.Green);
                    continue;
                }
                WriteColor($"Remove? [y/n] ", ConsoleColor.Yellow);
                var answer = Confirm();
                if (answer)
                {
                    Directory.Delete(directory, true);
                    WriteLineColor("Removed", ConsoleColor.Green);
                }
                else
                    WriteLineColor("Skipped", ConsoleColor.Yellow);
            }

            if (!opts.IsShow)
                WriteLineColor("Repositories successfully cleaned!", ConsoleColor.Green);
        }

        public static bool Confirm()
        {
            ConsoleKey response;
            do
            {
                response = Console.ReadKey(false).Key;
            } while (response != ConsoleKey.Y && response != ConsoleKey.N);

            return (response == ConsoleKey.Y);
        }

        private static List<string> SearchDirectories(string target, SearchOption option)
        {
            var folders = new HashSet<string>();
            var projs = Directory.GetFiles(target, "*.csproj", option);
            foreach(var proj in projs)
            {
                var dir = new DirectoryInfo(proj).Parent;
                if (dir == null)
                    continue;
                if (Directory.Exists(Path.Combine(dir.FullName, "bin")))
                    folders.Add(Path.Combine(dir.FullName, "bin"));
                if (Directory.Exists(Path.Combine(dir.FullName, "obj")))
                    folders.Add(Path.Combine(dir.FullName, "obj"));
            }

            var sols = Directory.GetFiles(target, "*.sln", option);
            foreach (var sol in sols)
            {
                var dir = new DirectoryInfo(sol).Parent;
                if (dir == null)
                    continue;
                if (Directory.Exists(Path.Combine(dir.FullName, ".vs")))
                    folders.Add(Path.Combine(dir.FullName, ".vs"));
            }
            var list = new List<string>();
            foreach(var folder in folders)
                if (!folders.Any(x => folder.StartsWith(x) && folder != x))
                    list.Add(folder);
            return list;
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            var sentenceBuilder = SentenceBuilder.Create();
            foreach (var error in errs)
                if (error is not HelpRequestedError)
                    WriteLineColor(sentenceBuilder.FormatError(error), ConsoleColor.Red);
        }

        private static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AddEnumValuesToHelpText = true;
                return h;
            }, e => e, verbsIndex: true);
            WriteLineColor(helpText, ConsoleColor.Red);
            HandleParseError(errs);
        }

        private static string RootPath(string path)
        {
            if (!Path.IsPathRooted(path))
                path = Path.Join(Directory.GetCurrentDirectory(), path);
            path = path.Replace("\\", "/");
            return path;
        }

        private static void WriteLineColor(string text, ConsoleColor? color = null)
        {
            if (color != null)
                Console.ForegroundColor = (ConsoleColor)color;
            else
                Console.ResetColor();
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static void WriteColor(string text, ConsoleColor? color = null)
        {
            if (color != null)
                Console.ForegroundColor = (ConsoleColor)color;
            else
                Console.ResetColor();
            Console.Write(text);
            Console.ResetColor();
        }
    }
}