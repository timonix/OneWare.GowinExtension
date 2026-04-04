using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
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
        
        serviceProvider.Resolve<FpgaService>().RegisterToolchain<GowinToolchain>();
        
        var compileMenuEntry = new OneWareUiExtension(x =>
        {
            if (x is not UniversalFpgaProjectRoot { Toolchain: GowinToolchain.ToolChainId } root) return null;

            return new MenuItem
            {
                Header = "Gowin settings",
                Icon = CreateIcon("Material.SettingsOutline")
            };
        });
        
        serviceProvider.Resolve<IWindowService>().RegisterUiExtension("UniversalFpgaToolBar_CompileMenuExtension",compileMenuEntry);


    }

}