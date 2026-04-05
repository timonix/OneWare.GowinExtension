using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneWare.Essentials.Helpers;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.Gowin.Services;
using OneWare.Gowin.ViewModels;
using OneWare.Gowin.Views;
using OneWare.UniversalFpgaProjectSystem.Models;
using OneWare.UniversalFpgaProjectSystem.Services;
using OneWare.UniversalFpgaProjectSystem.ViewModels;

namespace OneWare.Gowin;
public class OneWareGowinModule : OneWareModuleBase
{
    public override void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<GowinService>();
    }

    private static Image CreateIcon(string resourceKey)
    {
        var source = Application.Current!
            .FindResource(Application.Current.RequestedThemeVariant, resourceKey) as IImage;

        return new Image { Source = source };
    }
    
    public override void Initialize(IServiceProvider serviceProvider)
    {
        var settingsService = serviceProvider.Resolve<ISettingsService>();
        var gowinService = serviceProvider.Resolve<GowinService>();
        var environmentService = serviceProvider.Resolve<IEnvironmentService>();
        var windowService = serviceProvider.Resolve<IWindowService>();
        var projectExplorerService = serviceProvider.Resolve<IProjectExplorerService>();
        var fpgaService = serviceProvider.Resolve<FpgaService>();
        var logger = serviceProvider.Resolve<ILogger>();
        
        serviceProvider.Resolve<FpgaService>().RegisterToolchain<GowinToolchain>();
        
        var settingsMenuItem = new OneWareUiExtension(x =>
        {
            if (x is not UniversalFpgaProjectRoot { Toolchain: GowinToolchain.ToolChainId } root) return null;

            return new MenuItem
            {
                Header = "Gowin settings",
                Icon = CreateIcon("Material.SettingsOutline"),
                Command = new AsyncRelayCommand(async () =>
                {
                    if (projectExplorerService
                            .ActiveProject is UniversalFpgaProjectRoot fpgaProjectRoot)
                    {
                        var selectedFpga = root.Properties["Fpga"]?.ToString();
                        var selectedFpgaPackage =
                            fpgaService.FpgaPackages.FirstOrDefault(obj => obj.Name == selectedFpga);

                        if (selectedFpgaPackage == null)
                        {
                            serviceProvider.Resolve<ILogger>()
                                .Warning("No FPGA Selected. Open Pin Planner first!");
                            return;
                        }
                        
                        await windowService.ShowDialogAsync(
                            new GowinCompileSettingsView
                            {
                                DataContext = new GowinCompileSettingsViewModel(fpgaProjectRoot,selectedFpgaPackage.LoadFpga())
                            });
                    }
                })
                
            };
        });
        
        var compileMenuItem = new OneWareUiExtension(x =>
        {
            if (x is not UniversalFpgaProjectRoot { Toolchain: GowinToolchain.ToolChainId } root) 
                return null;

            var name = root.Properties["Fpga"]?.ToString();
            var fpgaPackage = fpgaService.FpgaPackages.FirstOrDefault(obj => obj.Name == name);
            var fpga = fpgaPackage?.LoadFpga();

            return new MenuItem()
            {
                Header = "Run Synthesis",
                Command = new AsyncRelayCommand(async () =>
                {
                    await projectExplorerService.SaveOpenFilesForProjectAsync(root);
                    await gowinService.CompileAsync(root, new FpgaModel(fpga!));
                }, () => fpga != null)
            };
        });
        
        
        
        serviceProvider.Resolve<IWindowService>().RegisterUiExtension("UniversalFpgaToolBar_CompileMenuExtension",settingsMenuItem);
        serviceProvider.Resolve<IWindowService>().RegisterUiExtension("UniversalFpgaToolBar_CompileMenuExtension",compileMenuItem);


        settingsService.RegisterSetting("Tools", "Gowin", "Gowin_IDE_Path",new FolderPathSetting("Gowin IDE","",null,null,IsIdePathValid));
        settingsService.RegisterSetting("Tools", "Gowin", "Gowin_Programmer_Path",new FolderPathSetting("Gowin programmer","",null,null,IsProgrammerPathValid));
        
        settingsService.GetSettingObservable<string>("Gowin_IDE_Path").Subscribe(x =>
        {
            if (string.IsNullOrWhiteSpace(x))
                return;

            if (!IsIdePathValid(x))
            {
                logger.Warning("Gowin IDE path invalid", null, false);
                return;
            }

            var ideBinPath = Path.Combine(x, "bin");

            environmentService.SetPath("Gowin_IDE_Path", x);
            environmentService.SetPath("Gowin_IDE_Bin", ideBinPath);
        });

        settingsService.GetSettingObservable<string>("Gowin_Programmer_Path").Subscribe(x =>
        {
            if (string.IsNullOrWhiteSpace(x))
                return;

            if (!IsProgrammerPathValid(x))
            {
                logger.Warning("Gowin programmer path invalid", null, false);
                return;
            }

            var programmerBinPath = Path.Combine(x, "bin");

            environmentService.SetPath("Gowin_Programmer_Path", x);
            environmentService.SetPath("Gowin_Programmer_Bin", programmerBinPath);
        });
    }
    
    private static bool IsProgrammerPathValid(string path)
    {
        if (!Directory.Exists(path)) return false;
        var exeName = $"programmer_cli{PlatformHelper.ExecutableExtension}";
        return File.Exists(Path.Combine(path, "bin", exeName));
    }
    
    private static bool IsIdePathValid(string path)
    {
        if (!Directory.Exists(path)) return false;
        var exeName = $"gw_sh{PlatformHelper.ExecutableExtension}";
        return File.Exists(Path.Combine(path, "bin", exeName));
    }

}