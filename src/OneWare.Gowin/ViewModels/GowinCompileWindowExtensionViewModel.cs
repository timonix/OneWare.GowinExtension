using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using OneWare.Essentials.Services;
using OneWare.Gowin.Views;
using OneWare.Gowin.ViewModels;
using OneWare.UniversalFpgaProjectSystem.Models;
using Prism.Ioc;

namespace OneWare.Gowin.ViewModels;

public class GowinCompileWindowExtensionViewModel : ObservableObject
{
    private readonly IWindowService _windowService;
    private readonly IProjectExplorerService _projectExplorerService;
    
    private bool _isVisible = false;
    
    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    public GowinCompileWindowExtensionViewModel(IProjectExplorerService projectExplorerService, IWindowService windowService)
    {
        _windowService = windowService;
        _projectExplorerService = projectExplorerService;
        
        IDisposable? disposable = null;
        projectExplorerService.WhenValueChanged(x => x.ActiveProject).Subscribe(x =>
        {
            disposable?.Dispose();
            if (x is not UniversalFpgaProjectRoot fpgaProjectRoot) return;
            disposable = fpgaProjectRoot.WhenValueChanged(y => y.Toolchain).Subscribe(z =>
            {
                IsVisible = z is GowinToolchain;
            });
        });
    }

    public async Task OpenCompileSettingsAsync(Control owner)
    {
        var ownerWindow = TopLevel.GetTopLevel(owner) as Window;
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                if (_projectExplorerService.ActiveProject is UniversalFpgaProjectRoot fpgaProjectRoot)
                {
                    await _windowService.ShowDialogAsync(new GowinCompileSettingsView()
                        { DataContext = new GowinCompileSettingsViewModel(fpgaProjectRoot) }, ownerWindow);
                }
            }
            catch (Exception e)
            {
                ContainerLocator.Container.Resolve<ILogger>().Error(e.Message, e);
            }
        });
    }
}