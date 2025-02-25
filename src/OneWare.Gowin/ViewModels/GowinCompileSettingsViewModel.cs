using OneWare.Essentials.Controls;
using OneWare.Essentials.ViewModels;
using OneWare.Gowin.Helper;
using OneWare.Settings.ViewModels;
using OneWare.UniversalFpgaProjectSystem.Models;

namespace OneWare.Gowin.ViewModels;

public class GowinCompileSettingsViewModel : FlexibleWindowViewModelBase
{
    private readonly string _tclPath;
    private readonly TclFile _tclFile;
    
    public SettingsCollectionViewModel SettingsCollection { get; } = new("Gowin Settings")
    {
        ShowTitle = false
    };

    private readonly List<ITclSetting> _settings = [];
    
    public GowinCompileSettingsViewModel(UniversalFpgaProjectRoot fpgaProjectRoot)
    {
        Title = "Gowin Compile Settings";
        Id = "GowinCompileSettings";
        
        _tclPath = TclHelper.GetTclPath(fpgaProjectRoot);
        _tclFile = TclHelper.ReadTcl(_tclPath);
        
        _settings.Add(new TclSettingCheckbox(_tclFile,"use_mspi_as_gpio","use mspi as regular io","",false));
        _settings.Add(new TclSettingCheckbox(_tclFile,"use_sspi_as_gpio","use sspi as regular io","",false));
        _settings.Add(new TclSettingCheckbox(_tclFile,"rw_check_on_ram","read write check ram","",true));
        
        _settings.Add(new TclSettingComboBox(_tclFile, "synthesis_tool", "Synth tool", "Select Gowin synth tool", 
              new Dictionary<string, string>()
                    {
                        { "DEFAULT", "gowinsynthesis" }
                    }, "gowinsynthesis"));
        
        _settings.Add(new TclSettingComboBox(_tclFile, "verilog_std", "Verilog standad", "", 
            new Dictionary<string, string>()
            {
                { "DEFAULT", "sysv2017" },
                { "v2001", "v2001" },
                { "v1995", "v1995" }
            }, "sysv2017"));
        
        _settings.Add(new TclSettingComboBox(_tclFile, "vhdl_std", "vhdl standad", "", 
            new Dictionary<string, string>()
            {
                { "DEFAULT", "vhd2008" },
                { "VHDL1993", "vhd1993" }
            }, "vhd2008"));
        
        _settings.Add(new TclSettingCheckbox(_tclFile,"gen_sdf","generate sdf file","",false));
        _settings.Add(new TclSettingCheckbox(_tclFile,"gen_posp","generate post-place file","",false));

        foreach (var setting in _settings)
        {
            SettingsCollection.SettingModels.Add(setting.GetSettingModel());
        }
    }
    
    public void Save(FlexibleWindow flexibleWindow)
    {
        foreach (var setting in _settings)
        {
            setting.Save();
        }
        TclHelper.WriteTcl(_tclPath, _tclFile);
        Close(flexibleWindow);
    }
}