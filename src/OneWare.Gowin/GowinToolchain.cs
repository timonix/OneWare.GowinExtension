using OneWare.Essentials.Services;
using OneWare.Gowin.Helper;
using OneWare.Gowin.Services;
using OneWare.UniversalFpgaProjectSystem.Models;
using OneWare.UniversalFpgaProjectSystem.Parser;
using OneWare.UniversalFpgaProjectSystem.Services;

namespace OneWare.Gowin;

public class GowinToolchain(GowinService gowinService, ILogger logger) : IFpgaToolchain
{

    public string Name => "Gowin";

    public void OnProjectCreated(UniversalFpgaProjectRoot project)
    {
        
    }

    public void LoadConnections(UniversalFpgaProjectRoot project, FpgaModel fpga)
    {
        try
        {
            var cstPath = CstHelper.GetCstPath(project);
            var cst = CstHelper.ReadCst(cstPath);
        
            foreach (var (pin, node) in cst.GetLocationAssignments())
            {

                if(!fpga.PinModels.TryGetValue(pin, out var pinModel)) return;
                if(!fpga.NodeModels.TryGetValue(node, out var nodeModel)) return;
                    
                fpga.Connect(pinModel, nodeModel);
            }
        }
        catch (Exception e)
        {
            logger.Error(e.Message,e);
        }
    }

    public void SaveConnections(UniversalFpgaProjectRoot project, FpgaModel fpga)
    {
        try
        {
            var cstPath = CstHelper.GetCstPath(project);
            var cst = CstHelper.ReadCst(cstPath);

            cst.RemoveLocationAssignments();
            foreach (var (_, pinModel) in fpga.PinModels.Where(x => x.Value.ConnectedNode != null))
            {
                cst.AddLocationAssignment(pinModel.Pin.Name, pinModel.ConnectedNode!.Node.Name);
            }

            CstHelper.WriteCst(cstPath, cst);
        }
        catch (Exception e)
        {
            logger.Error("Error while saving connections",e);
        }
    }

    public Task<bool> CompileAsync(UniversalFpgaProjectRoot project, FpgaModel fpga)
    {
        var topEntity = project.TopEntity?.Header ?? throw new Exception("No TopEntity set!");
        topEntity = Path.GetFileNameWithoutExtension(topEntity);
            
        var properties = FpgaSettingsParser.LoadSettings(project, fpga.Fpga.Name);

        var tclPath = TclHelper.GetTclPath(project);
        var tcl = TclHelper.ReadTcl(tclPath);

        var device = properties.GetValueOrDefault("gowinDevice") ?? throw new Exception("No Device set!");
        var deviceName = properties.GetValueOrDefault("gowinDeviceName") ?? throw new Exception("No Device name set!");
        
        tcl.set_header(device, deviceName);
        
        tcl.RemoveFileAssignments();
        foreach (var file in project.Files)
        {
            tcl.AddFile(file);
        }
        
        TclHelper.WriteTcl(tclPath, tcl);
        return Task.FromResult(true);
    }
}