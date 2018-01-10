using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.CostModels;
using UnitsNet;
using UnitsNet.Units;

namespace GenericCostModel.Process
{
    public class WireFeedDepositionCostModel : ICostModel
    {
        private readonly SearchInputs _inputs;

        public WireFeedDepositionCostModel(SearchInputs inputs, Volume stockVolume, Cost blankMaterialCost, Cost substrateMaterialCost, Volume substrateVolume)
        {
            _inputs = inputs;
            StockVolume = stockVolume;
            BlankMaterialCost = blankMaterialCost;
            SubstrateMaterialCost = substrateMaterialCost;
            SubstrateVolume = substrateVolume;
        }

        #region Geometries

        //DO NOT display this property, since the cost is captured in the blank properties.
        internal Volume StockVolume { get; }

        //DO NOT display this property, since the cost is captured in the blank properties.
        internal Volume SubstrateVolume { get; }
        
        //DO NOT display this property, since the cost is captured in the blank cost.
        internal Cost BlankMaterialCost { get; }

        //DO NOT display this property, since the cost is captured in the blank cost.
        internal Cost SubstrateMaterialCost { get; }

        [Display(Name = "Substrate Mass")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Mass SubstrateMass => Mass.FromKilograms(SubstrateVolume.CubicMillimeters * _inputs.General.TitaniumDensity.KilogramsPerCubicMillimeter);

        [Display(Name = "Wire Feedstock Mass")]
        [Equation("Geometry From TVGLL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Mass WireFeedstockMass => Mass.FromKilograms(StockVolume.CubicMillimeters * _inputs.General.TitaniumDensity.KilogramsPerCubicMillimeter);

        #endregion

        #region Labor Cost Calculations

        [Display(Name = "Deposition Time")]
        [Equation("Wire Feedstock Mass / Wirefeed Deposition Rate")]
        [Source("Midrange rate from  http://www.sciaky.com/additive-manufacturing/wire-am-vs-powder-amg")]
        [OutputUnitType(KatanaUnitType.CalculatedTime)]
        public Duration DepositionTime => Duration.FromHours(0.5 + WireFeedstockMass.Kilograms / _inputs.WireFeed.AdditiveDepositionRate.KilogramsPerHour);

        [Display(Name = "Labor Cost")]
        [Equation("Deposition Time * Labor Rate")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost LaborCost => Cost.FromDollars(DepositionTime.Hours * _inputs.General.LaborRate.DollarsPerHour);

        #endregion

        #region Machine Cost Calculations

        [Display(Name = "Machine Lifetime Availability")]
        [Equation("Machine Availability * Machine Life in Years * Work Hours per Year")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        public Duration MachineLifetimeAvailability => Duration.FromHours(_inputs.General.HoursPerYear.Unitless * _inputs.WireFeed.MachineLife.Years * _inputs.WireFeed.MachineAvailability.DecimalFractions);

        [Display(Name = "Machine Capital Cost")]
        [Equation("From Wire Feed Inputs")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        public Cost MachineCapitalCost => _inputs.WireFeed.MachineCost;

        [Display(Name = "Amortized Machine Cost")]
        [Equation("Deposition Time * Maching Capital Cost /  Machine Lifetime Availability")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost AmortizedMachineCost => Cost.FromDollars(DepositionTime.Hours * MachineCapitalCost.Dollars / MachineLifetimeAvailability.Hours);

        #endregion

        #region Overhead Calculations

        [Display(Name = "Overhead Multiplier")]
        [Equation("From General Inputs")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        public Multiplier OverheadMultiplier => _inputs.General.Overhead;

        [Display(Name = "General & Adminsitrative Overhead Costs")]
        [Equation("Total Manufacturing Cost * Overhead Multiplier")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost OverheadCost => (LaborCost) * OverheadMultiplier.Unitless;

        #endregion

        [Display(Name = "Total Cost")]
        [Equation("Labor Cost + Overhead Cost + Amortized Machine Cost")]
        [Notes("COST RETURNED IS AN ESTIMATE")]
        [OutputUnitType(KatanaUnitType.TotalCost)]
        public Cost TotalCost => LaborCost + AmortizedMachineCost + OverheadCost;



        public Duration TotalTime { get; set; } = Duration.Zero;
    }
}
