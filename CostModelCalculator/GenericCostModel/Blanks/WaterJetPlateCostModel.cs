using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.Blanks;
using KatanaObjects.CostModels;
using UnitsNet;

namespace GenericCostModel.Blanks
{
    public class WaterJetPlateCostModel : ICostModel
    {
        public WaterJetPlateCostModel(SearchInputs inputs, Blank blank)
        {
            StockVolume = blank.StockVolume;
            PlateThickness = blank.SubVolume.WaterjetDepth;
            Perimeter = blank.SubVolume.WaterjetCuttingPerimeter;
            WaterjetPricePerMass =
                CostPerMass.FromDollarsPerKilogram(
                    inputs.Waterjet.PricePerKilogramMultipliers[PlateThickness].Unitless*
                    inputs.Waterjet.PricePerMass.DollarsPerKilogram);
            StockMass = Mass.FromKilograms(StockVolume.CubicMeters*inputs.General.TitaniumDensity.KilogramsPerCubicMeter);
        }

        [Display(Name = "Waterjet Plate Volume")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Volume StockVolume { get; }

        [Display(Name = "Waterjet Plate Mass")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Mass StockMass { get; }

        [Display(Name = "Plate Thickness")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Length PlateThickness { get; }

        [Display(Name = "Perimeter")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Length Perimeter { get; }

        [Display(Name = "Price Per Unit Mass")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        public CostPerMass WaterjetPricePerMass { get; }

        [Display(Name = "Plate Material Cost")]
        [Equation("StockMass * WaterjetPricePerMass")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost PlateCost => Cost.FromDollars(StockMass.Kilograms * WaterjetPricePerMass.DollarsPerKilogram);

        [Display(Name = "Total Cost")]
        [Equation("PlateCost * WaterjettingCost")]
        [Source("")]
        [Notes("COST RETURNED IS AN ESTIMATE")]
        [OutputUnitType(KatanaUnitType.TotalCost)]
        public Cost TotalCost => PlateCost;


        public Duration TotalTime { get; set; } = Duration.Zero;
    }
}
