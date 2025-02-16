using OneWare.Essentials.Models;
using OneWare.Settings.ViewModels.SettingTypes;

namespace OneWare.Gowin.Helper;

public interface ITclSetting
{
    public TitledSetting GetSettingModel();
    public void Save();
}