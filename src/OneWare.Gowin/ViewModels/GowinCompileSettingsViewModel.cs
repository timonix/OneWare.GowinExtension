using OneWare.Essentials.Controls;
using OneWare.Essentials.Models;
using OneWare.Essentials.ViewModels;
using OneWare.Gowin.Helper;
using OneWare.Settings.ViewModels;
using OneWare.UniversalFpgaProjectSystem.Fpga;
using OneWare.UniversalFpgaProjectSystem.Models;
using OneWare.UniversalFpgaProjectSystem.Parser;
using OneWare.Gowin.Helper;

namespace OneWare.Gowin.ViewModels;

public class GowinCompileSettingsViewModel : FlexibleWindowViewModelBase
{
    private readonly UniversalFpgaProjectRoot _projectRoot;
    private readonly IFpga _fpga;

    private static bool GetBool(IDictionary<string, string> dict, string key, bool defaultValue = false)
    {
        return dict.TryGetValue(key, out var value) && bool.TryParse(value, out var result)
            ? result
            : defaultValue;
    }

    public SettingsCollectionViewModel SettingsCollection { get; } = new("Gowin Settings")
    {
        ShowTitle = false
    };

    private readonly Dictionary<string, string> _settings;

    private readonly ComboBoxSetting _verilogSetting;
    private readonly ComboBoxSetting _vhdlStdSetting;
    private readonly CheckBoxSetting _useMspiAsGpioSetting;
    private readonly CheckBoxSetting _useSspiAsGpioSetting;
    private readonly CheckBoxSetting _rwCheckOnRamSetting;

    private readonly CheckBoxSetting _genSdfSetting;
    private readonly CheckBoxSetting _genPostPlaceSetting;

    private readonly CheckBoxSetting _genTextTimingRptSetting;

    public GowinCompileSettingsViewModel(UniversalFpgaProjectRoot projectRoot, IFpga fpga)
    {
        Title = "Gowin Compile Settings";
        Id = "GowinCompileSettings";
        _projectRoot = projectRoot;
        _fpga = fpga;

        var defaultProperties = fpga.Properties;
        _settings = FpgaSettingsParser.LoadSettings(_projectRoot, _fpga.Name);

        _verilogSetting = new ComboBoxSetting(
            "Verilog std",
            _settings.GetValueOrDefault(SettingKeys.VerilogStd)
                ?? defaultProperties.GetValueOrDefault("Verilog std")
                ?? "sysv2017",
            [
                "sysv2017",
                "v2001",
                "v1995"
            ])
        {
            HoverDescription = "Set Verilog version"
        };

        _useMspiAsGpioSetting = new CheckBoxSetting(
            "Use MSPI as regular IO",
            GetBool(_settings, SettingKeys.UseMspiAsGpio, false))
        {
            HoverDescription = "Use MSPI as regular IO"
        };

        _useSspiAsGpioSetting = new CheckBoxSetting(
            "Use SSPI as regular IO",
            GetBool(_settings, SettingKeys.UseSspiAsGpio, false))
        {
            HoverDescription = "Use SSPI as regular IO"
        };

        _rwCheckOnRamSetting = new CheckBoxSetting(
            "Read write check RAM",
            GetBool(_settings, SettingKeys.RwCheckOnRam, true))
        {
            HoverDescription = "Read write check RAM"
        };

        _vhdlStdSetting = new ComboBoxSetting(
            "VHDL std",
            _settings.GetValueOrDefault(SettingKeys.VhdlStd) ?? "vhd2008",
            [
                "vhd2008",
                "vhd1993"
            ])
        {
            HoverDescription = "Set VHDL version"
        };

        _genSdfSetting = new CheckBoxSetting(
            "Generate SDF file",
            GetBool(_settings, SettingKeys.GenerateSdf, false))
        {
            HoverDescription = "Generate sdf file"
        };

        _genPostPlaceSetting = new CheckBoxSetting(
            "Generate post-place file",
            GetBool(_settings, SettingKeys.GeneratePostPlace, false))
        {
            HoverDescription = "Generate post-place file"
        };
        
        _genTextTimingRptSetting = new CheckBoxSetting(
            "Generate Text Timing Report",
            GetBool(_settings, SettingKeys.GenTextTimingRpt, false))
        {
            HoverDescription = "Generate text-based timing report"
        };

        SettingsCollection.SettingModels.Add(_useMspiAsGpioSetting);
        SettingsCollection.SettingModels.Add(_useSspiAsGpioSetting);
        SettingsCollection.SettingModels.Add(_rwCheckOnRamSetting);
        SettingsCollection.SettingModels.Add(_verilogSetting);
        SettingsCollection.SettingModels.Add(_vhdlStdSetting);
        SettingsCollection.SettingModels.Add(_genSdfSetting);
        SettingsCollection.SettingModels.Add(_genPostPlaceSetting);
        SettingsCollection.SettingModels.Add(_genTextTimingRptSetting);
    }
    
    private void SetIfNotNull(string key, object? value)
    {
        if (value != null)
            _settings[key] = value.ToString()!;
        else
            _settings.Remove(key); // ensures old value is cleared
    }

    void SetBoolIfNotNull(string key, object? value)
    {
        if (value is bool b)
            _settings[key] = b ? "true" : "false";
        else
            _settings.Remove(key);
    }
    public void Save(FlexibleWindow flexibleWindow)
    {
        SetIfNotNull(SettingKeys.VerilogStd, _verilogSetting.Value);
        SetIfNotNull(SettingKeys.VhdlStd, _vhdlStdSetting.Value);
        
        SetBoolIfNotNull(SettingKeys.UseMspiAsGpio, _useMspiAsGpioSetting.Value);
        SetBoolIfNotNull(SettingKeys.UseSspiAsGpio, _useSspiAsGpioSetting.Value);
        SetBoolIfNotNull(SettingKeys.RwCheckOnRam, _rwCheckOnRamSetting.Value);

        SetBoolIfNotNull(SettingKeys.GenerateSdf, _genSdfSetting.Value);
        SetBoolIfNotNull(SettingKeys.GeneratePostPlace, _genPostPlaceSetting.Value);
        
        SetBoolIfNotNull(SettingKeys.GenTextTimingRpt, _genTextTimingRptSetting.Value);

        FpgaSettingsParser.SaveSettings(_projectRoot, _fpga.Name, _settings);

        Close(flexibleWindow);
    }
}