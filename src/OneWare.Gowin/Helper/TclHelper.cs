using OneWare.UniversalFpgaProjectSystem.Models;

namespace OneWare.Gowin.Helper;

public class TclHelper
{
    public static string GetTclPath(UniversalFpgaProjectRoot project)
    {
        return Path.Combine(project.RootFolderPath, Path.GetFileNameWithoutExtension(project.TopEntity ?? throw new Exception("TopEntity not set!")) + ".tcl");

    }
    public static void WriteTcl(string path, TclFile file)
    {
        File.WriteAllLines(path, file.Lines);
    }
}