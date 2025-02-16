using OneWare.UniversalFpgaProjectSystem.Models;

namespace OneWare.Gowin.Helper;

public class TclHelper
{
    public static string GetTclPath(UniversalFpgaProjectRoot project)
    {
        return Path.Combine(project.RootFolderPath, Path.GetFileNameWithoutExtension(project.TopEntity?.FullPath ?? throw new Exception("TopEntity not set!")) + ".tcl");
    }

    public static TclFile ReadTcl(string path)
    {
        var tcl = File.Exists(path) ? File.ReadAllText(path) : string.Empty;

        return new TclFile(tcl.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
    }
    
    public static void WriteTcl(string path, TclFile file)
    {
        File.WriteAllLines(path, file.Lines);
    }
}