using OneWare.Essentials.Helpers;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using OneWare.Gowin.Services;
using OneWare.Gowin.ViewModels;
using OneWare.Gowin.Views;
using OneWare.UniversalFpgaProjectSystem.Models;
using OneWare.UniversalFpgaProjectSystem.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace OneWare.Gowin;

public class GowinModule : IModule
{
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<GowinService>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        var settingsService = containerProvider.Resolve<ISettingsService>();
        
        containerProvider.Resolve<IWindowService>().RegisterUiExtension("CompileWindow_TopRightExtension", new UiExtension(x => new GowinCompileWindowExtensionView()
        {
            DataContext = containerProvider.Resolve<GowinCompileWindowExtensionViewModel>()
        }));
        
        containerProvider.Resolve<IWindowService>().RegisterUiExtension("UniversalFpgaToolBar_DownloaderConfigurationExtension", new UiExtension(x =>
        {
            if (x is not UniversalFpgaProjectRoot cm) return null;
            return new GowinLoaderWindowExtensionView()
            {
                DataContext = containerProvider.Resolve<GowinLoaderWindowExtensionViewModel>((typeof(UniversalFpgaProjectRoot), cm))
            };
        }));
        
        containerProvider.Resolve<FpgaService>().RegisterToolchain<GowinToolchain>();
        containerProvider.Resolve<FpgaService>().RegisterLoader<GowinLoader>();
        
        settingsService.RegisterTitledFolderPath("Tools", "Gowin", "Gowin_Path", "Gowin IDE Path",
            "Sets the path for Gowin", "./", null, null, GowinIdePathValid);
        
        settingsService.RegisterTitledFolderPath("Tools", "Gowin", "Gowin_programmer_Path", "Gowin Programmer Path",
            "Sets the path for Gowin loader", "./", null, null, GowinProgrammerPathValid);
        
        settingsService.GetSettingObservable<string>("Gowin_Path").Subscribe(x =>
        {
            if (string.IsNullOrEmpty(x)) return;

            if (!GowinIdePathValid(x))
            {
                containerProvider.Resolve<ILogger>().Warning("Gowin path invalid", null, false);
                return;
            }
            var binPath = Path.Combine(x, "bin");
            ContainerLocator.Container.Resolve<IEnvironmentService>().SetPath("Gowin_Bin", binPath);
        });
        
        settingsService.GetSettingObservable<string>("Gowin_programmer_Path").Subscribe(x =>
        {
            if (string.IsNullOrEmpty(x)) return;

            if (!GowinProgrammerPathValid(x))
            {
                containerProvider.Resolve<ILogger>().Warning("Gowin programmer path invalid", null, false);
                return;
            }
            var binPath = Path.Combine(x, "bin");
            ContainerLocator.Container.Resolve<IEnvironmentService>().SetPath("Gowin_programmer_Bin", binPath);
        });
        
    }
    
    private static bool GowinIdePathValid(string path)
    {
        if (!Directory.Exists(path)) return false;
        if (!File.Exists(Path.Combine(path, "bin", $"gw_sh{PlatformHelper.ExecutableExtension}"))) return false;
        return true;
    }
    
    private static bool GowinProgrammerPathValid(string path)
    {
        if (!Directory.Exists(path)) return false;
        if (!File.Exists(Path.Combine(path, "bin", $"programmer{PlatformHelper.ExecutableExtension}"))) return false;
        return true;
    }
}