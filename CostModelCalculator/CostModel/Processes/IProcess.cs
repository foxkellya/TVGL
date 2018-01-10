using BlankFactory.Processes;
using KatanaObjects.CostModels;

namespace KatanaObjects.Processes
{
    public interface IProcess : ICostModel
    {
        ProcessType Type { get; set; }

        ICostModel CostModel { get; set; }

        int IndexAllProcesses { get; set; }

        int IndexByProcessType { get; set; }
    }
}
