namespace RepoClean.Models
{
    public class FolderTarget
    {
        public string TargetParentFile { get; set; }
        public List<string> TargetPatterns { get; set; }

        public FolderTarget(string targetParentFile, List<string> targetPatterns)
        {
            TargetParentFile = targetParentFile;
            TargetPatterns = targetPatterns;
        }
    }
}
