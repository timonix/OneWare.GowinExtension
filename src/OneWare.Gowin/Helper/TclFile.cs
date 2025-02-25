using System.Text.RegularExpressions;
using OneWare.Essentials.Extensions;
using OneWare.Essentials.Models;
using OneWare.UniversalFpgaProjectSystem.Parser;

namespace OneWare.Gowin.Helper;

public class TclFile
{

    public TclFile(string[] lines)
    {
        updateContent(lines);
    }
    
    private List<string> _options = new List<string>();
    private List<string> _files = new List<string>();
    
    private string _device = string.Empty;
    private string _deviceName = string.Empty;

    public void updateContent(string[] lines)
    {
        if (lines == null) return;
        Regex optionRegex = new Regex(@"^\s*set_option\s+", RegexOptions.IgnoreCase);
        Regex fileRegex = new Regex(@"^\s*add_file\s+", RegexOptions.IgnoreCase);

        foreach (var line in lines)
        {
            if (optionRegex.IsMatch(line))
            {
                var opt = line.Split(" ");
                AddOption(opt[1].Replace("-",""),opt[2]);
            }
            else if (fileRegex.IsMatch(line))
            {
                var opt = line.Split(" ");
                AddFile(opt[1]);
            }
        }
    }
    
    
    public List<string> Lines
    {
        get
        {
            string header = $"set_device {_device} -name {_deviceName}";
            string footer = "run all";
            
            return new List<string> { header }
                .Concat(_files)
                .Concat(_options)
                .Concat(new[] { footer })
                .ToList();
        }
    }
    
    ////create_project -name FIFO_HS_tcl -dir E:/tcl -pn GW1N-LV9LQ144C6/I5 -device_version C //maybe this one?
    //set_device GW1NR-LV9QN88PC6/I5 -name GW1NR-9C

    public void set_header(string device, string deviceName)
    {
        _device = device;
        _deviceName = deviceName;
    }

    public void RemoveOption(string propertyName)
    {
        _options.RemoveAll(x => Regex.IsMatch(x, $@"\bset_option\s+-{Regex.Escape(propertyName)}\b"));
    }

    public void AddOption(string propertyName, string value)
    {
        if (string.IsNullOrWhiteSpace(propertyName) || string.IsNullOrWhiteSpace(value))
            return;

        string newOption = $"set_option -{propertyName} {value}";

        _options.RemoveAll(x => Regex.IsMatch(x, $@"\bset_option\s+-{Regex.Escape(propertyName)}\b"));
        _options.Add(newOption);
    }

    public string? GetOption(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return null; // Invalid input

        // Regex to find a set_option with the specified property
        Regex regex = new Regex($@"^\s*set_option\s+-{Regex.Escape(propertyName)}\s+(?<value>.+)", RegexOptions.IgnoreCase);

        foreach (var option in _options)
        {
            Match match = regex.Match(option);
            if (match.Success)
            {
                return match.Groups["value"].Value.Trim(); // Extract value
            }
        }

        return null; // Return null if not found
    }

    
    public void RemoveFileAssignments()
    {
        _files = new List<string>();
    }
    
    public void AddFile(IProjectFile file)
    {
        _files.RemoveAll(x => Regex.IsMatch(x, $@"\add_file\s+-{Regex.Escape(file.RelativePath.ToUnixPath())}\b"));
        _files.Add($"add_file {file.RelativePath.ToUnixPath()}");
    }
    
    public void AddFile(String relativePath)
    {
        _files.RemoveAll(x => Regex.IsMatch(x, $@"\add_file\s+-{relativePath}\b"));
        _files.Add($"add_file {relativePath}");
    }
}