using OneWare.Essentials.Models;
using OneWare.Gowin.Helper;
using OneWare.Settings;
using OneWare.Settings.ViewModels.SettingTypes;

namespace OneWare.Gowin.Helper;

public class TclSettingComboBox : ITclSetting
{
    private readonly string _name;
    private readonly TclFile _tclFile;
    private readonly Dictionary<string, string> _options;
    private readonly ComboBoxSetting _setting;
    
    public TclSettingComboBox(TclFile file, string name, string title, string description, Dictionary<string, string> options, string defaultValue)
    {
        _tclFile = file;
        _name = name;
        _options = options;
        
        _setting = new ComboBoxSetting(title, defaultValue, options.Values)
        {
            HoverDescription = description
        };
        
        var setting = file.GetOption(name);
        
        if (!string.IsNullOrWhiteSpace(setting))
        {
            options.TryAdd(setting, setting);
            _setting.Value = options[setting];
        }
    }

    public TitledSetting GetSettingModel()
    {
        return _setting;
    }

    public void Save()
    {
        _tclFile.AddOption(_name, _options.FirstOrDefault(x => x.Value == (string)_setting.Value).Value);
    }
}