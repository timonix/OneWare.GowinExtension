using DynamicData;
using OneWare.Essentials.Controls;
using OneWare.Essentials.Models;
using OneWare.Essentials.ViewModels;
using OneWare.Settings;
using OneWare.Settings.ViewModels;
using OneWare.Settings.ViewModels.SettingTypes;
using OneWare.UniversalFpgaProjectSystem.Fpga;
using OneWare.UniversalFpgaProjectSystem.Models;
using OneWare.UniversalFpgaProjectSystem.Parser;

namespace OneWare.Gowin.ViewModels;

public class GowinLoaderSettingsViewModel : FlexibleWindowViewModelBase
{
    private readonly UniversalFpgaProjectRoot _projectRoot;
    private readonly Dictionary<string, string> _settings;
    private readonly IFpga _fpga;
    private readonly ComboBoxSetting _shortTermModeSetting;
    private readonly TitledSetting _shortTermOperationSetting;
    private readonly TitledSetting _shortTermArgumentsSetting;
    private readonly ComboBoxSetting _longTermModeSetting;
    private readonly TitledSetting _longTermOperationSetting;
    private readonly ComboBoxSetting _longTermFormatSetting;
    private readonly TitledSetting _longTermCpfArgumentsSetting;
    private readonly TitledSetting _longTermArgumentsSetting;
    
    public SettingsCollectionViewModel SettingsCollection { get; } = new("Gowin Loader Settings")
    {
        ShowTitle = false
    };

    public GowinLoaderSettingsViewModel(UniversalFpgaProjectRoot projectRoot, IFpga fpga)
    {
        _projectRoot = projectRoot;
        _fpga = fpga;
        
        Title = "Gowin Loader Settings";
        Id = "Gowin Loader Settings";
        
        var defaultProperties = fpga.Properties;
        _settings = FpgaSettingsParser.LoadSettings(projectRoot, fpga.Name);
        
        _shortTermModeSetting = new ComboBoxSetting("Short Term Mode",
            defaultProperties.GetValueOrDefault("GowinProgrammerShortTermMode") ?? "", ["JTAG", "AS", "PS", "SD"]);
        
        _shortTermOperationSetting = new TextBoxSetting("Short Term Operation", "Operation to use for Short Term Programming",
            defaultProperties.GetValueOrDefault("GowinProgrammerShortTermOperation") ?? "");
        
        _shortTermArgumentsSetting = new TextBoxSetting("Short Term Additional Arguments", "Additional Arguments to use for Short Term Programming",
            defaultProperties.GetValueOrDefault("GowinProgrammerShortTermArguments") ?? "");
        
        _longTermModeSetting = new ComboBoxSetting("Long Term Mode",
            defaultProperties.GetValueOrDefault("GowinProgrammerLongTermMode") ?? "", ["JTAG", "AS", "PS", "SD"]);
            
        _longTermOperationSetting = new TextBoxSetting("Long Term Operation", "Operation to use for Long Term Programming",
            defaultProperties.GetValueOrDefault("GowinProgrammerLongTermOperation") ?? "");
        
        _longTermFormatSetting = new ComboBoxSetting("Long Term Format",
            defaultProperties.GetValueOrDefault("GowinProgrammerLongTermFormat") ?? "", ["POF", "JIC"]);
        
        _longTermCpfArgumentsSetting = new TextBoxSetting("Long Term Cpf Arguments", "If format is different from POF, these arguments will be used to convert .sof to given format",
            defaultProperties.GetValueOrDefault("GowinProgrammerLongTermCpfArguments") ?? "");
        
        _longTermArgumentsSetting = new TextBoxSetting("Long Term Additional Arguments", "Additional Arguments to use for Long Term Programming",
            defaultProperties.GetValueOrDefault("GowinProgrammerLongTermArguments") ?? "");
        
        if (_settings.TryGetValue("GowinProgrammerShortTermMode", out var qPstMode))
            _shortTermModeSetting.Value = qPstMode;
        
        if (_settings.TryGetValue("GowinProgrammerShortTermOperation", out var qPstOperation))
            _shortTermOperationSetting.Value = qPstOperation;
        
        if (_settings.TryGetValue("GowinProgrammerShortTermArguments", out var qPstArguments))
            _shortTermArgumentsSetting.Value = qPstArguments;
        
        if (_settings.TryGetValue("GowinProgrammerLongTermMode", out var qPltMode))
            _longTermModeSetting.Value = qPltMode;
        
        if (_settings.TryGetValue("GowinProgrammerLongTermOperation", out var qPltOperation))
            _longTermOperationSetting.Value = qPltOperation;
        
        if (_settings.TryGetValue("GowinProgrammerLongTermFormat", out var qPltFormat))
            _longTermFormatSetting.Value = qPltFormat;
        
        if (_settings.TryGetValue("GowinProgrammerLongTermCpfArguments", out var qPltCpfArguments))
            _longTermCpfArgumentsSetting.Value = qPltCpfArguments;
        
        if (_settings.TryGetValue("GowinProgrammerLongTermArguments", out var qPltArguments))
            _longTermArgumentsSetting.Value = qPltArguments;
        
        SettingsCollection.SettingModels.Add(_shortTermModeSetting);
        SettingsCollection.SettingModels.Add(_shortTermOperationSetting);
        SettingsCollection.SettingModels.Add(_shortTermArgumentsSetting);
        SettingsCollection.SettingModels.Add(_longTermModeSetting);
        SettingsCollection.SettingModels.Add(_longTermOperationSetting);
        SettingsCollection.SettingModels.Add(_longTermFormatSetting);
        SettingsCollection.SettingModels.Add(_longTermCpfArgumentsSetting);
        SettingsCollection.SettingModels.Add(_longTermArgumentsSetting);
    }
    
    public void Save(FlexibleWindow flexibleWindow)
    {
        _settings["GowinProgrammerShortTermMode"] = _shortTermModeSetting.Value.ToString()!;
        _settings["GowinProgrammerShortTermOperation"] = _shortTermOperationSetting.Value.ToString()!;
        _settings["GowinProgrammerShortTermArguments"] = _shortTermArgumentsSetting.Value.ToString()!;
        _settings["GowinProgrammerLongTermMode"] = _longTermModeSetting.Value.ToString()!;
        _settings["GowinProgrammerLongTermOperation"] = _longTermOperationSetting.Value.ToString()!;
        _settings["GowinProgrammerLongTermFormat"] = _longTermFormatSetting.Value.ToString()!;
        _settings["GowinProgrammerLongTermCpfArguments"] = _longTermCpfArgumentsSetting.Value.ToString()!;
        _settings["GowinProgrammerLongTermArguments"] = _longTermArgumentsSetting.Value.ToString()!;

        FpgaSettingsParser.SaveSettings(_projectRoot, _fpga.Name, _settings);
        
        Close(flexibleWindow);
    }
    
    public void Reset()
    {
        foreach (var setting in SettingsCollection.SettingModels)
        {
            setting.Value = setting.DefaultValue;
        }
    }
}