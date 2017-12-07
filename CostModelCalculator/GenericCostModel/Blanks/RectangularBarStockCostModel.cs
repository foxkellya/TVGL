using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.Blanks;
using KatanaObjects.CostModels;
using UnitsNet;

namespace GenericCostModel.Blanks
{
    public class RectangularBarStockCostModel : ICostModel
    { 
        private readonly SearchInputs _inputs;

        public RectangularBarStockCostModel(SearchInputs inputs, Blank blank) 
        {
            _inputs = inputs;
            Area = blank.AreaOnCuttingPlane;
            StockVolume = blank.StockVolume;
            Length = blank.SubVolume.RectangularBlankLength;
            Width = blank.SubVolume.RectangularBlankWidth;
            Thickness = blank.SubVolume.RectangularBlankThickness;
        }

        [Display(Name = "Length")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Length Length { get; }

        [Display(Name = "Area")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Area Area { get; }

        [Display(Name = "Width")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Length Width { get; }

        [Display(Name = "Thickness")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Length Thickness { get; }

        [Display(Name = "Stock Volume")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Volume StockVolume { get; }

        [Display(Name = "Stock Mass")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Mass StockMass => Mass.FromKilograms(StockVolume.CubicMeters*_inputs.General.TitaniumDensity.KilogramsPerCubicMeter);

        [Display(Name = "Price Per Mass")]
        [Equation("From Rectangular Bar Stock Inputs")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        public CostPerMass PricePerMass => _inputs.RectangularBarStock.MaterialPrice;

        [Display(Name = "Bar Cost")]
        [Equation("Stock Mass * Price Per Mass")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost BarCost => Cost.FromDollars(StockMass.Kilograms * PricePerMass.DollarsPerKilogram);

        [Display(Name = "Total Cost")]
        [Equation("BarCost")]
        [Notes("COST RETURNED IS AN ESTIMATE")]
        [OutputUnitType(KatanaUnitType.TotalCost)]
        public Cost TotalCost => BarCost;

        public Duration TotalTime { get; set; } = Duration.Zero;
    }
}
