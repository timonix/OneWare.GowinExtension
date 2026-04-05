using System.Diagnostics;
using System.Text.RegularExpressions;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Helpers;
using OneWare.Essentials.Services;
using OneWare.Gowin.Helper;
using OneWare.UniversalFpgaProjectSystem.Models;
using OneWare.UniversalFpgaProjectSystem.Parser;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace OneWare.Gowin.Services;
public class GowinService(
    IOutputService outputService,
    ISettingsService settingsService)
{

    private async Task<bool> RunGwAsync(UniversalFpgaProjectRoot project, string tclPath)
    {

        var idePath = settingsService.GetSetting("Gowin_IDE_Path").Value?.ToString()
                      ?? throw new Exception("Gowin_IDE_Path not set");

        var gwBin = Path.Combine(idePath, "bin");
        var gwSh = Path.Combine(gwBin, $"gw_sh{PlatformHelper.ExecutableExtension}");

        if (!File.Exists(gwSh))
            throw new FileNotFoundException("gw_sh not found", gwSh);

        var psi = new ProcessStartInfo
        {
            FileName = gwSh,
            WorkingDirectory = project.FullPath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        psi.ArgumentList.Add(tclPath);
        psi.Environment["QT_DEBUG_PLUGINS"] = "1";
        psi.Environment.Remove("QT_PLUGIN_PATH");
        psi.Environment.Remove("QT_QPA_PLATFORM_PLUGIN_PATH");

        var path = psi.Environment.TryGetValue("PATH", out var value) ? value ?? "" : "";
        psi.Environment["PATH"] = string.Concat(gwBin, ";", path);

        using var process = new Process();
        process.StartInfo = psi;

        process.OutputDataReceived += (_, e) =>
        {
            if (string.IsNullOrEmpty(e.Data)) return;
            Dispatcher.UIThread.Post(() => outputService.WriteLine(e.Data));
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (string.IsNullOrEmpty(e.Data)) return;
            Dispatcher.UIThread.Post(() => outputService.WriteLine(e.Data, Brushes.Red));
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();
        return process.ExitCode == 0;
    }
    public async Task<bool> CompileAsync(UniversalFpgaProjectRoot project, FpgaModel fpga)
    {
        var start = DateTime.Now;
        var settings = FpgaSettingsParser.LoadSettings(project, fpga.Fpga.Name);
        outputService.WriteLine($"Compiling for device: {settings[SettingKeys.GowinDevice]}\n==================");
        outputService.WriteLine("Generating tcl. \n");
        var tclPath = CreateTcl(project, settings);
        outputService.WriteLine("Running Synthesis and pnr \n");
        var success = await RunGwAsync(project, tclPath);
        var compileTime = DateTime.Now - start;
        if (success)
        {
            var sourceFile = Path.Combine(project.FullPath, "impl", "pnr", "project.fs");
            var buildDir = Path.Combine(project.FullPath, "build");
            var destFile = Path.Combine(buildDir, "pack.fs");

            Directory.CreateDirectory(buildDir);

            if (File.Exists(sourceFile))
            {
                if (File.Exists(destFile)){
                    File.SetAttributes(destFile, FileAttributes.Normal);
                    File.Delete(destFile); // File.Move won't overwrite by default
                }

                File.Copy(sourceFile, destFile);
                outputService.WriteLine($"Moved output file to: {destFile}");
            }
            else
            {
                outputService.WriteLine($"Expected output file not found: {sourceFile}", Brushes.Red);
            }

            outputService.WriteLine(
                $"==================\n\nCompilation finished after {(int)compileTime.TotalMinutes:D2}:{compileTime.Seconds:D2}\n");
        }
        else
            outputService.WriteLine($"==================\n\nCompilation failed after {(int)compileTime.TotalMinutes:D2}:{compileTime.Seconds:D2}\n", Brushes.Red);
        
        return success;
    }

    private string CreateTcl(UniversalFpgaProjectRoot project, Dictionary<string, string> settings)
    {
        var topFile = project.TopEntity ?? throw new Exception("TopEntity not set!");
        var device = settings[SettingKeys.GowinDevice];
        var deviceName = settings[SettingKeys.GowinDeviceName];
        var tclPath = TclHelper.GetTclPath(project);
        var tclFile = new TclFile();
        tclFile.set_header(device,deviceName);
        tclFile.AddOption("vhdl_std",settings[SettingKeys.VhdlStd]);
        tclFile.AddOption("verilog_std",settings[SettingKeys.VerilogStd]);
        tclFile.AddOption("rw_check_on_ram", settings[SettingKeys.RwCheckOnRam]);
        tclFile.AddOption("gen_sdf", settings[SettingKeys.GenerateSdf]);
        tclFile.AddOption("gen_posp", settings[SettingKeys.GeneratePostPlace]);
        tclFile.AddOption("gen_text_timing_rpt", settings[SettingKeys.GenTextTimingRpt]);

        tclFile.AddOption("use_mspi_as_gpio", settings[SettingKeys.UseMspiAsGpio]);
        tclFile.AddOption("use_sspi_as_gpio", settings[SettingKeys.UseSspiAsGpio]);
        
        var topEntity = Path.GetFileNameWithoutExtension(topFile);
        
        tclFile.AddOption("top_module", topEntity);
        var includedFiles = project.GetFiles("*.v")
            .Concat(project.GetFiles("*.sv"))
            .Concat(project.GetFiles("*.vhd"))
            .Concat(project.GetFiles("*.cst"))
            .Concat(project.GetFiles("*.vhdl"))
            .Where(x => !project.IsCompileExcluded(x))
            .Where(x => !project.IsTestBench(x));
        
        foreach (var path in includedFiles)
        {
            var normalized = path.Replace("\\", "/");
            tclFile.AddFile(normalized);
        }
        tclFile.AddFile(CstHelper.GetRelativeCstPath(project).Replace('\\', '/'));
        
        TclHelper.WriteTcl(tclPath,tclFile);
        return tclPath;
    }
}