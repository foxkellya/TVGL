using GenericInputs;
using KatanaObjects.Blanks;
using KatanaObjects.Processes;

namespace KatanaObjects.CostModels
{
    public interface ICostModelFactory
    {
        ICostModel MaterialCostModel(Blank blank);

        ICostModel MachiningCostModel(Machining machining, bool isEstimate = false);

        SearchInputs Inputs { get; set; }
    }
}
