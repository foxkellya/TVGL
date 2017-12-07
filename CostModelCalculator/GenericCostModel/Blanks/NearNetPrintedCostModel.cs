using System.ComponentModel.DataAnnotations;
using GenericCostModel.Process;
using GenericInputs;
using KatanaObjects.CostModels;
using UnitsNet;

namespace GenericCostModel.Blanks
{
    public class NearNetPrintedCostModel : ICostModel
    {
        private readonly SearchInputs _inputs;

        public NearNetPrintedCostModel(SearchInputs inputs, Volume stockVolume, Volume substrateVolume)
        {
            _inputs = inputs;
            StockVolume = stockVolume;
            SubstrateVolume = substrateVolume;
        }

        #region geomtries for substrate and print

        [Display(Name = "Stock Volume")]
        [Source("Geometry From TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Volume StockVolume { get; }

        [Display(Name = "Substrate Volume")]
        [Source("Geometry From TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Volume SubstrateVolume { get; }

        [Display(Name = "Substrate Mass To Be Joined")]
        [Source("Geometry From TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Mass SubstrateMassToBeJoined => Mass.FromKilograms(SubstrateVolume.CubicMillimeters * _inputs.General.TitaniumDensity.KilogramsPerCubicMillimeter);

        [Display(Name = "Mass To Be Joined")]
        [Source("Geometry From TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Mass MassToBeJoined => Mass.FromKilograms(StockVolume.CubicMillimeters * _inputs.General.TitaniumDensity.KilogramsPerCubicMillimeter);

        [Display(Name = "Total Material Mass")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Mass TotalMaterialMass => SubstrateMassToBeJoined + MassToBeJoined;

        #endregion

        #region Material Cost

        [Display(Name = "Substrate Cost")]
        [Equation("Substrate Mass * Waterjet Price Per Mass")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost SubstrateCost => Cost.FromDollars(SubstrateMassToBeJoined.Kilograms * _inputs.Waterjet.PricePerMass.DollarsPerKilogram);

        [Display(Name = "Deposition Cost")]
        [Equation("Mass To Be Joined * Wire Feedstock Price")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost WireCost => Cost.FromDollars(MassToBeJoined.Kilograms * _inputs.WireFeedstock.WirePrice.DollarsPerKilogram);

        [Display(Name = "Total Material Cost")]
        [Equation("Substrate Cost + Deposition Cost")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost TotalMaterialCost => SubstrateCost + WireCost;

        #endregion

        #region Deposition and Machining Cost

        [Display(Name = "Time For Deposition")]
        [Equation("30 min + (Mass to be Joined / Deposition Rate")]
        [OutputUnitType(KatanaUnitType.CalculatedTime)]
        public Duration TimeForDeposition => Duration.FromHours(0.5 + MassToBeJoined.Kilograms / _inputs.WireFeed.AdditiveDepositionRate.KilogramsPerHour);

        [Display(Name = "Total Labor Cost")]
        [Equation("Time for Deposition * Labor RAte")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost TotalLaborCost => Cost.FromDollars(TimeForDeposition.Hours * _inputs.General.LaborRate.DollarsPerHour);

        [Display(Name = "Substrate Machining Cost")]
        [Equation("Rough only machining through machining cost model to removel mass of substrate")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost SubstrateMachiningCost => new MachiningCostModel(_inputs, Volume.FromCubicMillimeters(0.0), Area.FromSquareMillimeters(0.0), SubstrateVolume, true).PrepCost;

        #endregion

        #region Overhead and Machine Cost

        [Display(Name = "Machine Lifetime Availability")]
        [Equation("Machine Availability * Machine Life in Years * Work Hours per Year")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        public Duration MachineLifetimeAvailability => Duration.FromHours(_inputs.General.HoursPerYear.Unitless * _inputs.WireFeed.MachineLife.Years * _inputs.WireFeed.MachineAvailability.DecimalFractions);

        [Display(Name = "Amortized Machine Cost")]
        [Equation("Machine Cost * Time for Deposition/Machine Lifetime Availability)")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost AmortizedMachineCost => Cost.FromDollars(TimeForDeposition.Hours * _inputs.WireFeed.MachineCost.Dollars / MachineLifetimeAvailability.Hours);

        [Display(Name = "Total Manufacturing Cost")]
        [Equation("Total Material Cost Total Labor Cost + Amortized Machine Cost + Substrate Machining Cost)")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost TotalManufacturingCost => TotalMaterialCost + TotalLaborCost + AmortizedMachineCost + SubstrateMachiningCost;

        [Display(Name = "General Administrative Cost")]
        [Equation("(Total Labor Cost * Overhead")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost OverheadCost => TotalLaborCost * _inputs.General.Overhead.Unitless;

        #endregion

        [Display(Name = "Total Cost")]
        [Equation("Total Manufacturing Cost + Overhead Cost")]
        [Notes("COST RETURNED IS AN ESTIMATE")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost TotalCost => TotalManufacturingCost + OverheadCost;


        public Duration TotalTime { get; set; } = Duration.Zero;
    }
}
