using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.Blanks;
using KatanaObjects.CostModels;
using UnitsNet;

namespace GenericCostModel.Blanks
{
    public class ForgingCostModel : ICostModel
    {
        private readonly SearchInputs _inputs;

        public ForgingCostModel(SearchInputs inputs, Blank blank)
        {
            _inputs = inputs;
            FinishVolume = blank.FinishVolume;
            ForgingVolume = blank.StockVolume;
        }

        #region Geometries

        [Display(Name = "Finish Volume")]
        [Source("TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Volume FinishVolume { get; }

        [Display(Name = "Finish Mass")]
        [Equation("Finish Volume * Titanium Density")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Mass FinishMass => Mass.FromKilograms(FinishVolume.CubicMillimeters * _inputs.General.TitaniumDensity.KilogramsPerCubicMillimeter);

        [Display(Name = "Forging Volume")]
        [Source("Forging Volume Estimation Method")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Volume ForgingVolume { get; }

        [Display(Name = "Forging Mass")]
        [Equation("Forging Volume * Titanium Density")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Mass ForgingMass => Mass.FromKilograms(ForgingVolume.CubicMillimeters * _inputs.General.TitaniumDensity.KilogramsPerCubicMillimeter);

        #endregion


        [Display(Name = "Forging Cost")]
        [Equation("Forging Mass * Material Price")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost ForgingCost
            => Cost.FromDollars(ForgingMass.Kilograms*_inputs.Forging.MaterialPrice.DollarsPerKilogram);

        [Display(Name = "Die Cost Percentage")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        public Multiplier DieCostPercentage => _inputs.Forging.DieCostPercentage;

        [Display(Name = "Die Cost")]
        [Equation("Die Cost Percentage * Forging Cost")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost DieCost => DieCostPercentage.Unitless * ForgingCost;

        [Display(Name = "Total Cost")]
        [Equation("Forging Cost +  Die Cost")]
        [Notes("COST RETURNED IS AN ESTIMATE")]
        [OutputUnitType(KatanaUnitType.TotalCost)]
        public Cost TotalCost => ForgingCost + DieCost;


        public Duration TotalTime { get; set; } = Duration.Zero;
    }
}
