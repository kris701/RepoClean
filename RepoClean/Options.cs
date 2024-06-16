using CommandLine;

namespace RepoClean
{
    public class Options
    {
        [Option('t', "target", Required = false, HelpText = "Target path to clean", Default = "")]
        public string TargetPath { get; set; } = "";
        [Option('r', "recursive", Required = false, HelpText = "Clean from folders and all subfolders", Default = false)]
        public bool IsRecursive { get; set; } = false;
        [Option('f', "forced", Required = false, HelpText = "Do not ask before deleting", Default = false)]
        public bool IsForced { get; set; } = false;
        [Option('s', "show", Required = false, HelpText = "Only display the ones that would be deleted, but dont actually delete any.", Default = false)]
        public bool IsShow { get; set; } = false;
        [Option('e', "extra", Required = false, HelpText = "Extra set of patterns to look for")]
        public IEnumerable<string> ExtraPatterns { get; set; } = new List<string>();
    }
}
