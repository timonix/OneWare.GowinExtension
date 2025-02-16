using Avalonia.Controls;
using OneWare.Essentials.Models;
using OneWare.Gowin.Helper;
using OneWare.Settings;
using OneWare.Settings.ViewModels.SettingTypes;

namespace OneWare.Gowin.Helper;

public class TclSettingCheckbox : ITclSetting
{
    private readonly string _name;
    private readonly TclFile _tclFile;
    private readonly CheckBoxSetting _setting;
    
    public TclSettingCheckbox(TclFile file, string name, string title, string description, bool defaultValue)
    {
        _tclFile = file;
        _name = name;
        
        _setting = new CheckBoxSetting(title, defaultValue)
        {
            HoverDescription = description
        };
        
        var setting = file.GetOption(name);
        
        if (!string.IsNullOrWhiteSpace(setting) && bool.TryParse(setting, out var val))
        {
            _setting.Value = val;
        }    
        else if (setting == "1")
        {
            _setting.Value = true;
        }
        else if (setting == "0")
        {
            _setting.Value = false;
        }
    }

    public TitledSetting GetSettingModel()
    {
        return _setting;
    }
    

    public void Save()
    {
        if ((bool)_setting.Value)
            _tclFile.AddOption(_name, "1");
        else 
            _tclFile.AddOption(_name, "0");
    }
}