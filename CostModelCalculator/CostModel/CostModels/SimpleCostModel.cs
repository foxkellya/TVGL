using UnitsNet;

namespace KatanaObjects.CostModels
{
    public class SimpleCostModel : ICostModel
    {
        public Cost TotalCost { get; }

        public Duration TotalTime { get; }

        public SimpleCostModel(Cost totalCost)
        {
            TotalCost = totalCost;
        }
    }
}
