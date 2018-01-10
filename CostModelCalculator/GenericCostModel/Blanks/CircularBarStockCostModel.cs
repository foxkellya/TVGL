using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.Blanks;
using KatanaObjects.CostModels;
using UnitsNet;

namespace GenericCostModel.Blanks
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
            Diameter = blank.SubVolume.CircularBarStockDiameter;
        }

        [Display(Name = "Length")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Length Length { get; }

        [Display(Name = "Area")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Area Area { get; }

        [Display(Name = "Diameter")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Length Diameter { get; }

        [Display(Name = "Stock Volume")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Volume StockVolume { get; }

        [Display(Name = "Stock Mass")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Mass StockMass => Mass.FromKilograms(_inputs.General.TitaniumDensity.KilogramsPerCubicMeter *  StockVolume.CubicMeters);

        //[dollars/kg]
        [Display(Name = "Price Per Mass")]
        [Equation("From Circular Bar Stock Inputs")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        public CostPerMass BarStockMaterialPrice => _inputs.CircularBarStock.MaterialPrice;

        //[dollars = (kg) * (dollar/kg)]
        [Display(Name = "Bar Cost")]
        [Equation("Material Price Per Mass * Mass")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost BarCost => Cost.FromDollars(StockMass.Kilograms * BarStockMaterialPrice.DollarsPerKilogram);

        //[dollars]
        [Display(Name = "Total Cost")]
        [Equation("Bar Cost + Saw Cost")]
        [Notes("COST RETURNED IS AN ESTIMATE")]
        [OutputUnitType(KatanaUnitType.TotalCost)]
        public Cost TotalCost => BarCost;


        public Duration TotalTime { get; set; } = Duration.Zero;
    }
}
