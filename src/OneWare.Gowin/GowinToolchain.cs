using Microsoft.Extensions.Logging;
using OneWare.Essentials.Services;
using OneWare.Gowin.Helper;
using OneWare.UniversalFpgaProjectSystem.Models;
using OneWare.UniversalFpgaProjectSystem.Services;

public class GowinToolchain() : IFpgaToolchain
{
    public const string ToolChainId = "Gowin_Toolchain";

    public virtual string Id => ToolChainId;
    public string Name => "Gowin Toolchain";
    

    public void OnProjectCreated(UniversalFpgaProjectRoot project)
    {
        
    }

    public void LoadConnections(UniversalFpgaProjectRoot project, FpgaModel fpga)
    { try
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
            ContainerLocator.Container.Resolve<ILogger>().Error(e.Message, e);
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
            ContainerLocator.Container.Resolve<ILogger>().Error(e.Message, e);
        }
    }

    public Task<bool> CompileAsync(UniversalFpgaProjectRoot project, FpgaModel fpga)
    {
        return Task.FromResult(false);
    }
}