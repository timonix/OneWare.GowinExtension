using Microsoft.Extensions.Logging;
using OneWare.Essentials.Services;
using OneWare.UniversalFpgaProjectSystem.Models;

public class GowinService(
    IChildProcessService childProcessService,
    ILogger logger,
    IOutputService outputService,
    IMainDockService dockService,
    IToolService toolService,
    IToolExecutionDispatcherService toolExecutionDispatcherService)
{
    public async Task<bool> CompileAsync(UniversalFpgaProjectRoot project, FpgaModel fpga)
    {
        outputService.WriteLine("Compiling...\n==================");
        outputService.WriteLine("Failed not implemented\n");
        return false;
    }
}