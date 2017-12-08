using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.Blanks;
using KatanaObjects.CostModels;
using UnitsNet;

namespace GenericCostModel.Blanks
{
    public class HollowTubeCostModel : ICostModel
    {
        private readonly SearchInputs _inputs;

        public HollowTubeCostModel(SearchInputs inputs, Blank blank, Length outerDiameter, Length innerDiameter)
        {
            _inputs = inputs;
            StockVolume = blank.StockVolume;
            OuterDiameter = outerDiameter;
            InnerDiameter = innerDiameter;
            HollowTubePricePerMass = _inputs.HollowTube.PricePerMass;
        }

        [Display(Name = "Stock Volume")]
        [Source("Geometry From TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Volume StockVolume { get; }

        [Display(Name = "Stock Mass")]
        [Source("Geometry From TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Mass StockMass => Mass.FromKilograms(StockVolume.CubicMeters*_inputs.General.TitaniumDensity.KilogramsPerCubicMeter);

        [Display(Name = "Outer Diameter")]
        [Source("Geometry From TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Length OuterDiameter { get; }

        [Display(Name = "Inner Diameter")]
        [Source("Geometry From TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Length InnerDiameter { get; }

        [Display(Name = "Price Per Unit Mass")]
        [Equation("From Hollow Tube Inputs")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        public CostPerMass HollowTubePricePerMass { get; }

        [Display(Name = "Material Cost")]
        [Equation("Stock Mass * Price per Mass")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost MaterialCost => Cost.FromDollars(StockMass.Kilograms * HollowTubePricePerMass.DollarsPerKilogram);


        [Display(Name = "Total Cost")]
        [Equation("Material Cost + Saw Cost")]
        [OutputUnitType(KatanaUnitType.TotalCost)]
        [Notes("COST RETURNED IS AN ESTIMATE")]
        public Cost TotalCost => MaterialCost;


        public Duration TotalTime { get; set; } = Duration.Zero;
    }
}
