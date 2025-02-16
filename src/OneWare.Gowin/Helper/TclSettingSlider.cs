using Avalonia.Controls;
using OneWare.Essentials.Models;
using OneWare.Gowin.Helper;
using OneWare.Settings;
using OneWare.Settings.ViewModels.SettingTypes;

namespace OneWare.Gowin.Helper;

public class TclSettingSlider : ITclSetting
{
    private readonly string _name;
    private readonly TclFile _cstFile;
    private readonly SliderSetting _setting;
    
    public TclSettingSlider(TclFile file, string name, string title, string description, int defaultValue, int min, int max, int step)
    {
        _cstFile = file;
        _name = name;
        
        _setting = new SliderSetting(title, defaultValue, min, max, step)
        {
            HoverDescription = description
        };
        
        var setting = file.GetOption(name);
        
        if (!string.IsNullOrWhiteSpace(setting) && int.TryParse(setting, out var val))
        {
            _setting.Value = val;
        }
    }

    public TitledSetting GetSettingModel()
    {
        return _setting;
    }

    public void Save()
    {
        if(!string.IsNullOrWhiteSpace(_setting.Value.ToString()))
            _cstFile.AddOption(_name, _setting.Value.ToString()!);
        else 
            _cstFile.RemoveOption(_name);
    }
}