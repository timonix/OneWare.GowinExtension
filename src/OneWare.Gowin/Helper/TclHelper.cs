using OneWare.UniversalFpgaProjectSystem.Models;

namespace OneWare.Gowin.Helper;

public class TclHelper
{
    private static TclFile? _currentTclFile;

    public static string GetTclPath(UniversalFpgaProjectRoot project)
    {
        return Path.ChangeExtension(project.TopEntity?.FullPath?? throw new Exception("TopEntity not set!"), ".tcl");
    }

    public static TclFile ReadTcl(string path)
    {
        if (_currentTclFile != null) return _currentTclFile;
        var tcl = File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        _currentTclFile = new TclFile(tcl.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        return _currentTclFile;
    }
    
    public static void WriteTcl(string path, TclFile file)
    {
        File.WriteAllLines(path, file.Lines);
    }
}