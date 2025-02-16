using AvaloniaEdit.Utils;
using DynamicData;
using Microsoft.Extensions.Logging;
using OneWare.Essentials.Behaviors;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Services;
using OneWare.UniversalFpgaProjectSystem.Models;
using OneWare.UniversalFpgaProjectSystem.Parser;
using OneWare.UniversalFpgaProjectSystem.Services;
using ILogger = OneWare.Essentials.Services.ILogger;

namespace OneWare.Gowin;

public class GowinLoader(IChildProcessService childProcessService, ISettingsService settingsService, ILogger logger)
    : IFpgaLoader
{
    public string Name => "Gowin";
    

    public async Task DownloadAsync(UniversalFpgaProjectRoot project)
    {
        
    }
}