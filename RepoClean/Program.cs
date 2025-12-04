using CommandLine;
using CommandLine.Text;
using RepoClean.Models;

namespace RepoClean
{
    internal class Program
    {
        public static List<FolderTarget> _targetFolders = new List<FolderTarget>()
        {
            new FolderTarget("*.csproj", new List<string>(){ "bin", "obj" }),
            new FolderTarget("*.sln", new List<string>(){ ".vs" }),
        };

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
            _targetFolders.Add(new FolderTarget("", opts.ExtraPatterns.ToList()));
            var option = SearchOption.TopDirectoryOnly;
            if (opts.IsRecursive)
                option = SearchOption.AllDirectories;

            WriteColor("Finding directories... ", ConsoleColor.Blue);
            var directories = SearchDirectories(opts.TargetPath, _targetFolders, option);
            WriteLineColor($"{directories.Count} found!", ConsoleColor.Green);
            if (directories.Count > 0)
                WriteLineColor("To delete:", ConsoleColor.Blue);

            foreach (var directory in directories)
            {
                WriteColor($"\t{directory.Replace(opts.TargetPath.Replace("/", "\\"), "")} ", ConsoleColor.DarkGray);
                if (opts.IsShow)
                {
                    WriteLineColor("", ConsoleColor.DarkGray);
                    continue;
                }
                if (opts.IsForced)
                {
                    DeleteDirectory(directory);
                    continue;
                }
                WriteColor($"Remove? [y/n] ", ConsoleColor.Yellow);
                var answer = Confirm();
                if (answer)
                    DeleteDirectory(directory);
                else
                    WriteLineColor("Skipped", ConsoleColor.Yellow);
            }

            if (!opts.IsShow)
                WriteLineColor("Repositories successfully cleaned!", ConsoleColor.Green);
        }

        private static void DeleteDirectory(string dir)
        {
            var dirInfo = new DirectoryInfo(dir);
            try
            {
				SetAttributesNormal(dirInfo);
				Directory.Delete(dir, true);
                WriteLineColor("Removed", ConsoleColor.Green);
            }
            catch (Exception e)
            {
                WriteLineColor($"Error: {e.Message}", ConsoleColor.Red);
            }
        }

		private static void SetAttributesNormal(DirectoryInfo dir)
		{
			foreach (var subDir in dir.GetDirectories())
				SetAttributesNormal(subDir);
			foreach (var file in dir.GetFiles())
			{
				file.Attributes = FileAttributes.Normal;
			}
			dir.Attributes = FileAttributes.Normal;
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

        private static List<string> SearchDirectories(string from, List<FolderTarget> targets, SearchOption option)
        {
            var folders = new HashSet<string>();

            foreach (var target in targets)
            {
                if (target.TargetParentFile != "")
                {
                    var projs = Directory.GetFiles(from, target.TargetParentFile, option);
                    foreach (var proj in projs)
                    {
                        var dir = new DirectoryInfo(proj).Parent;
                        if (dir == null)
                            continue;
                        foreach (var pattern in target.TargetPatterns)
                            if (Directory.Exists(Path.Combine(dir.FullName, pattern)))
                                folders.Add(Path.Combine(dir.FullName, pattern));
                    }
                }
                else
                    foreach (var pattern in target.TargetPatterns)
                        foreach (var folder in Directory.GetDirectories(from, pattern, option))
                            folders.Add(folder.Replace("/", "\\"));
            }

            var list = new List<string>();
            foreach (var folder in folders)
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