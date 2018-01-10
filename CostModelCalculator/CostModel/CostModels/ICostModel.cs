using UnitsNet;

namespace KatanaObjects.CostModels
{
    public interface ICostModel
    {
        Cost TotalCost { get; }
    }
}
