using System.Text.RegularExpressions;
using OneWare.Essentials.Extensions;
using OneWare.Essentials.Models;
using OneWare.UniversalFpgaProjectSystem.Parser;

namespace OneWare.Gowin.Helper;

public class TclFile
{
    
    
    private readonly HashSet<string> _options = [];
    private readonly HashSet<string> _files = [];
    
    private string _device = string.Empty;
    private string _deviceName = string.Empty;
    
    public List<string> Lines
    {
        get
        {
            var header = string.Concat("set_device ", _device, " -name ", _deviceName);
            const string footer = "run all";
            
            return [header, .._files, .._options, footer];
        }
    }
    public void set_header(string device, string deviceName)
    {
        //set_device GW1NR-LV9QN88PC6/I5 -name GW1NR-9C
        _device = device;
        _deviceName = deviceName;
    }

    public void AddOption(string propertyName, string value)
    {
        if (string.IsNullOrWhiteSpace(propertyName) || string.IsNullOrWhiteSpace(value))
            return;

        _options.Add(string.Concat("set_option -", propertyName," ", value));
    }

    public void AddFile(string relativePath)
    {
        _files.Add($"add_file {relativePath}");
    }
}