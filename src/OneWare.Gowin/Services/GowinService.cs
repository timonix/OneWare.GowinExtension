using System.Diagnostics;
using System.Text.RegularExpressions;
using Avalonia.Media;
using Avalonia.Threading;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Services;
using OneWare.Gowin.Helper;
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
        
        var (success, _) = await childProcessService.ExecuteShellAsync(
            "gw_sh.exe",                            // Executable to run
            [TclHelper.GetTclPath(project)],                           // Arguments
            project.FullPath,                       // Working directory
            "Running Gowin Shell...",               // Status message
            AppState.Loading,                        // Application state
            true,                                    // Capture output
            (x) =>
            {
                
                var output = x.TrimStart();
                Dispatcher.UIThread.Post(() =>
                {
                    if (output.StartsWith("Error (")) 
                        outputService.WriteLine(x, Brushes.Red);
                    else if (output.StartsWith("Warning (") || output.StartsWith("Critical Warning ("))
                    {
                        outputService.WriteLine(x, Brushes.Orange);
                    }
                    else if (output.Contains("Generate file") && output.Contains("completed"))
                    {
                        // Extract file path
                        var match = Regex.Match(output, @"Generate file\s+""(.*?)""\s+completed");
                        if (match.Success)
                        {
                            string filePath = match.Groups[1].Value;
                            outputService.WriteLine($"Generated File: \"{filePath}\"");
                        }
                        else
                        {
                            outputService.WriteLine(x);
                        }
                    }
                    else
                    {
                        outputService.WriteLine(output);
                    }
                });

                
                return true;
            }
        );

        var compileTime = DateTime.Now - start;
        
        if(success)
            outputService.WriteLine($"==================\n\nCompilation finished after {(int)compileTime.TotalMinutes:D2}:{compileTime.Seconds:D2}\n");
        else
            outputService.WriteLine($"==================\n\nCompilation failed after {(int)compileTime.TotalMinutes:D2}:{compileTime.Seconds:D2}\n", Brushes.Red);
        
        return success;
    }
}