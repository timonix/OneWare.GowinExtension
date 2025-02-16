using Avalonia.Media;
using OneWare.Essentials.Services;
using OneWare.UniversalFpgaProjectSystem.Models;

namespace OneWare.Gowin.Services;

public class GowinService(IChildProcessService childProcessService, ILogger logger, IOutputService outputService, IDockService dockService)
{
    public async Task<bool> CompileAsync(UniversalFpgaProjectRoot project, FpgaModel fpga)
    {
        if (project.TopEntity == null)
        {
            logger.Error("No TopEntity set");
            return false;
        }

        dockService.Show<IOutputService>();
        
        var start = DateTime.Now;
        outputService.WriteLine("Compiling...\n==================", Brushes.CornflowerBlue);
        
        /*var (success, _) =await childProcessService.ExecuteShellAsync("quartus_sh",
            ["--flow", "compile", Path.GetFileNameWithoutExtension(project.TopEntity.Header)],
            project.FullPath, "Running Quartus Prime Shell...", AppState.Loading, true, (x) =>
            {
                var output = x.TrimStart();
                Dispatcher.UIThread.Post(() =>
                {
                    if(output.StartsWith("Error (")) outputService.WriteLine(x, Brushes.Red);
                    else if(output.StartsWith("Warning (") || output.StartsWith("Critical Warning ("))  outputService.WriteLine(x, Brushes.Orange);
                    else outputService.WriteLine(x);
                });
                return true;
            });
        */
        var compileTime = DateTime.Now - start;
        
        /*if(success)
            outputService.WriteLine($"==================\n\nCompilation finished after {(int)compileTime.TotalMinutes:D2}:{compileTime.Seconds:D2}\n");
        else
            outputService.WriteLine($"==================\n\nCompilation failed after {(int)compileTime.TotalMinutes:D2}:{compileTime.Seconds:D2}\n", Brushes.Red);

        return success;*/
        return false;
    }
}