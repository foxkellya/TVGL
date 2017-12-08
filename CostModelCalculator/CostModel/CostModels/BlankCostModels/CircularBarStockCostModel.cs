using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.Blanks;
using KatanaObjects.CostModels;
using UnitsNet;

namespace BlankFactory.CostModels
{
    public class CircularBarStockCostModel : ICostModel
    {
        private readonly SearchInputs _inputs;

        public CircularBarStockCostModel(SearchInputs inputs, Blank blank)
        {
            _inputs = inputs;
            Area = blank.AreaOnCuttingPlane;
            StockVolume = blank.StockVolume;
            Length = blank.SubVolume.CircularBarStockDiameter;
            Width = blank.SubVolume.CircularBarStockDiameter;
            Depth = blank.SubVolume.CircularBarStockDepth;
            BarCostRate = inputs.CircularBarStock.MaterialPrice ;
        }

        [Display(Name = "Length")]
        public Length Length { get; }

        [Display(Name = "Area")]
        public Area Area { get; }

        [Display(Name = "Width")]
        public Length Width { get; }

        [Display(Name = "Depth")]
        public Length Depth { get; }

        [Display(Name = "Stock Volume")]
        public Volume StockVolume { get; }

        //[dollars/kg]
        [Display(Name = "Bar Cost Rate")]
        public CostPerMass BarCostRate { get; }

        //[dollars = (mm^3 * kg/mm^3) * (dollar/kg)]
        [Display(Name = "Bar Cost")]
        public Cost BarCost => Cost.FromDollars(StockVolume.CubicMeters * _inputs.General.MaterialDensity.KilogramsPerCubicMeter * BarCostRate.DollarsPerKilogram);

        //[dollars = m * (dollars/m)]
        [Display(Name = "Saw Cost")]
        public Cost SawCost => Cost.FromDollars(Width.Meters * _inputs.General.SawCostRate.DollarsPerMeter);

        //[dollars]
        [Display(Name = "Total Cost")]
        public Cost TotalCost => BarCost + SawCost;
    }
}
