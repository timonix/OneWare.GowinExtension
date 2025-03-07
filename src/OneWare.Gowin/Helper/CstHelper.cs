﻿using System.Drawing.Printing;
using System.Text.RegularExpressions;
using Avalonia.Media;
using OneWare.UniversalFpgaProjectSystem.Models;

namespace OneWare.Gowin.Helper;

public static partial class CstHelper
{
    public static string GetCstPath(UniversalFpgaProjectRoot project)
    {
        return Path.ChangeExtension(project.TopEntity?.FullPath?? throw new Exception("TopEntity not set!"), ".cst");
    }

    public static CstFile ReadCst(string path)
    {
        var cst = File.Exists(path) ? File.ReadAllText(path) : string.Empty;

        return new CstFile(cst.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
    }
    
    public static void WriteCst(string path, CstFile file)
    {
        File.WriteAllLines(path, file.Lines);
    }
}