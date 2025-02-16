using System.Text.RegularExpressions;
using OneWare.Essentials.Extensions;
using OneWare.Essentials.Models;

namespace OneWare.Gowin.Helper;

public partial class CstFile(string[] lines)
{
    Regex set_location_regex = new Regex(@"IO_LOC\s+""(?<node>[^""]+)""\s+(?<pin>\S+);");

    public List<string> Lines { get; private set; } = lines.ToList();

    public string? GetQsfProperty(string propertyName)
    {
        var regex = new Regex(propertyName + @"\s(.+)");
        foreach (var line in Lines)
        {
            var match = regex.Match(line);
            if (match is { Success: true, Groups.Count: > 1 })
            {
                var value = match.Groups[1].Value;
                if(value.Length > 0 && value[0] == '"' && value[^1] == '"') return value[1..^1];
                return value;
            }
        }
        return null;
    }
    
    public void SetQsfProperty(string propertyName, string value)
    {
        var regex = new Regex(propertyName + @"\s(.+)");
        var line= Lines.FindIndex(x => regex.IsMatch(x));
        var newAssignment = $"{propertyName} {value}";
        
        if(line != -1)
        {
            Lines[line] = newAssignment;
        }
        else
        {
            Lines.Add(newAssignment);
        }
    }

    public IEnumerable<(string,string)> GetLocationAssignments()
    {
        foreach (var line in lines)
        {
            var match = set_location_regex.Match(line);
            if (!match.Success) continue;
            
            string pin = match.Groups["pin"].Value;
            string node = match.Groups["node"].Value;

            yield return (pin, node);
        }
    }
    
    public void AddLocationAssignment(string pin, string node)
    {
        Lines.Add($"IO_LOC \"{node}\" {pin};");
    }
    
    
    public void RemoveLocationAssignments()
    {
        Lines = Lines.Where(x => !set_location_regex.IsMatch(x)).ToList();
    }
    
    public void AddFile(IProjectFile file)
    {
        switch (file.Extension)
        {
            case ".vhd" or ".vhdl":
                Lines.Add($"set_global_assignment -name VHDL_FILE {file.RelativePath.ToUnixPath()}");
                break;
            case ".v":
                Lines.Add($"set_global_assignment -name VERILOG_FILE {file.RelativePath.ToUnixPath()}");
                break;
            case ".sv":
                Lines.Add($"set_global_assignment -name SYSTEMVERILOG_FILE {file.RelativePath.ToUnixPath()}");
                break;
            case ".qip":
                Lines.Add($"set_global_assignment -name QIP_FILE {file.RelativePath.ToUnixPath()}");
                break;
            case ".qsys":
                Lines.Add($"set_global_assignment -name QSYS_FILE {file.RelativePath.ToUnixPath()}");
                break;
            case ".bdf":
                Lines.Add($"set_global_assignment -name BDF_FILE {file.RelativePath.ToUnixPath()}");
                break;
            case ".ahdl":
                Lines.Add($"set_global_assignment -name AHDL_FILE {file.RelativePath.ToUnixPath()}");
                break;
            case ".smf":
                Lines.Add($"set_global_assignment -name SMF_FILE {file.RelativePath.ToUnixPath()}");
                break;
            case ".tcl":
                Lines.Add($"set_global_assignment -name TCL_SCRIPT_FILE {file.RelativePath.ToUnixPath()}");
                break;
            case ".hex":
                Lines.Add($"set_global_assignment -name HEX_FILE {file.RelativePath.ToUnixPath()}");
                break;
            case ".mif":
                Lines.Add($"set_global_assignment -name MIF_FILE {file.RelativePath.ToUnixPath()}");
                break;
        }
    }
}