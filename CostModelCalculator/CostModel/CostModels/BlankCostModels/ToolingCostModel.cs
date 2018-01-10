using KatanaObjects.CostModels;
using UnitsNet;

namespace BlankFactory.CostModels
{
    public class ToolingCostModel : ICostModel
    {
        public Cost HeatTreatCost;

        public Cost MaterialCost;

        public Cost MachiningCost;

        public Cost TotalCost => MaterialCost + MachiningCost + HeatTreatCost;
    }
}
