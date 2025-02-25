using System.Text.RegularExpressions;
using OneWare.Essentials.Extensions;
using OneWare.Essentials.Models;

namespace OneWare.Gowin.Helper;

public partial class CstFile(string[] lines)
{
    Regex set_location_regex = new Regex(@"IO_LOC\s+""(?<node>[^""]+)""\s+(?<pin>\S+);");

    public List<string> Lines { get; private set; } = lines.ToList();

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

}