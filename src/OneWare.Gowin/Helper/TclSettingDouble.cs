using OneWare.Essentials.Models;
using OneWare.Gowin.Helper;
using OneWare.Settings;
using OneWare.Settings.ViewModels.SettingTypes;
using System;
using System.Globalization;

namespace OneWare.Gowin.Helper;

public class TclSettingDouble : ITclSetting
{
    private readonly string _name;
    private readonly TclFile _tclFile;
    private readonly TextBoxSetting _setting;
    
    public TclSettingDouble(TclFile file, string name, string title, string description, double defaultValue)
    {
        _tclFile = file;
        _name = name;
        
        _setting = new TextBoxSetting(title, defaultValue,null)
        {
            HoverDescription = description
        };
        
        var setting = file.GetOption(name);
        
        if (!string.IsNullOrWhiteSpace(setting) && double.TryParse(setting, NumberStyles.Float, CultureInfo.InvariantCulture, out var val))
        {
            _setting.Value = val.ToString(CultureInfo.InvariantCulture);
        }
    }

    public TitledSetting GetSettingModel()
    {
        return _setting;
    }

    public void Save()
    {
        if (!string.IsNullOrWhiteSpace(_setting.Value?.ToString()) && 
            float.TryParse(_setting.Value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var val))
        {
            _tclFile.AddOption(_name, val.ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            _tclFile.RemoveOption(_name);
        }
    }
}