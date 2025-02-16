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
    
    private string? FirstFileInPath(string path, string extension)
    {
        try
        {
            return Directory
                .GetFiles(path)
                .FirstOrDefault(x => Path.GetExtension(x).Equals(extension, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception e)
        {
            logger.Error(e.Message, e);
            return null;
        }
    }
    

    public async Task DownloadAsync(UniversalFpgaProjectRoot project)
    {
        var fpga = project.GetProjectProperty("Fpga");
        if (fpga == null) return;
        
        var longTerm = settingsService.GetSettingValue<bool>("UniversalFpgaProjectSystem_LongTermProgramming");
        if (longTerm)
        {
            logger.Error("Long term programming is disabled, update coming.");
        }
        
        var properties = FpgaSettingsParser.LoadSettings(project, fpga);
        
        var outputDirRelative = "impl\\pnr";
        
        var outputDir = Path.Combine(project.FullPath, outputDirRelative);
        
    
        var fsFile = Path.GetFullPath(FirstFileInPath(outputDir,".fs"));

        if (string.IsNullOrEmpty(fsFile))
        {
            logger.Error("no .fs found! compile Design first!");
            return;
        }
        
        var deviceName = properties.GetValueOrDefault("gowinDeviceName") ?? throw new Exception("No Device name set!");
        List<string> pgmArgs = ["-d", deviceName, "--operation_index", "2", "-f",fsFile];
        
        await childProcessService.ExecuteShellAsync("programmer_cli", pgmArgs,
            outputDir, "Running Quartus programmer (Long-Term)...", AppState.Loading, true);
        
    }
}