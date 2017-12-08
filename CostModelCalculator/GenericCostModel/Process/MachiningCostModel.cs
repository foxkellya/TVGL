using System;
using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.CostModels;
using UnitsNet;
using UnitsNet.Units;

namespace GenericCostModel.Process
{
    public class MachiningCostModel : ICostModel
    {
        #region Private: inputs, mass limits, setup times, roughcut

        //ToDo:Make all these internal values inputs
        private readonly Mass _lowMassMax = Mass.FromPounds(10);
        private readonly Mass _medMassMax = Mass.FromPounds(25);

        private readonly Duration _lowMassLoadUnloadSetupTime = Duration.FromMinutes(5);
        private readonly Duration _medMassLoadUnloadSetupTime = Duration.FromMinutes(10);
        private readonly Duration _highMassLoadUnloadSetupTime = Duration.FromMinutes(20);

        private readonly SearchInputs _inputs;

        private readonly bool _roughCutAll;

        #endregion

        public MachiningCostModel(SearchInputs inputs, Volume finishVolume, Area wettedSurfaceArea, Volume stockVolume, bool roughCutAll = false)
        {
            _inputs = inputs;
            FinishVolume = finishVolume;
            WettedSurfaceArea = wettedSurfaceArea;
            StockVolume = stockVolume;
            _roughCutAll = roughCutAll;
        }

        #region General Geometries (Volume, Mass, Areas, Load unload times)

        [Display(Name = "Finish Volume")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Volume FinishVolume { get; }

        [Display(Name = "Wetted Surface Area")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Area WettedSurfaceArea { get; }

        [Display(Name = "Stock Volume")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Volume StockVolume { get; }

        [Display(Name = "Stock Mass")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Mass StockMass => Mass.FromKilograms(StockVolume.CubicMillimeters * _inputs.General.TitaniumDensity.KilogramsPerCubicMillimeter);

        [Display(Name = "Total Volume Removed")]
        [Equation("Stock Volume - Finish Volume")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Volume TotalVolumeRemoved => StockVolume - FinishVolume;

        [Display(Name = "Setup Load Unload Time")]
        [Equation("Tiered based on stock mass")]
        [OutputUnitType(KatanaUnitType.CalculatedTime)]
        [CostModelViewUnit(DurationUnit.Minute)]
        public Duration LoadUnloadSetupTime
            =>
                StockMass <= _lowMassMax
                    ? _lowMassLoadUnloadSetupTime
                    : (StockMass <= _medMassMax ? _medMassLoadUnloadSetupTime : _highMassLoadUnloadSetupTime);

        #endregion

        #region Roughing Cost Calculations

        [Display(Name = "Roughing Volume Remove")]
        [Equation("Estimate from TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Volume RoughingVolumeRemoved
            =>
                _roughCutAll
                    ? TotalVolumeRemoved
                    : Volume.FromCubicMillimeters(Math.Max(0, TotalVolumeRemoved.CubicMillimeters - (WettedSurfaceArea.SquareMillimeters*_inputs.Machining.RoughingAmountToLeave.Millimeters)));

        [Display(Name = "Roughing Time")]
        [Equation("Roughing Volume Removed / (Machining MRR * 60)")]
        [OutputUnitType(KatanaUnitType.CalculatedTime)]
        public Duration RoughingTime => Duration.FromMinutes(RoughingVolumeRemoved.CubicCentimeters / _inputs.Machining.RoughingMRR.CubicCentimetersPerMinute);

        [Display(Name = "Total Time To Rough")]
        [Equation("Roughing Time + (Setup Load Unload Time / 60) / 2")]
        [OutputUnitType(KatanaUnitType.CalculatedTime)]
        public Duration TotalTimeToRough => Duration.FromHours(RoughingTime.Hours + (LoadUnloadSetupTime.Hours) / 2);

        [Display(Name = "Roughing Cost")]
        [Equation("Total Time to Rough * Labor Rate Rate")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost RoughingCost => Cost.FromDollars(TotalTimeToRough.Hours * _inputs.General.LaborRate.DollarsPerHour);

        [Display(Name = "Roughing Scrap Cost")]
        [Equation("Roughing Volume Removed * Titanium Density * Titanium Chip Reclaim Value")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost RoughingScrapCost => Cost.FromDollars(-RoughingVolumeRemoved.CubicMillimeters * _inputs.Machining.TitaniumChipReclaimValue.DollarsPerKilogram * _inputs.General.TitaniumDensity.KilogramsPerCubicMillimeter);
        #endregion

        #region Finishing Cost Calculations

        [Display(Name = "Finish Volume Removed")]
        [Equation("Estimate From TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Volume FinishVolumeRemoved
            =>
                _roughCutAll
                ? Volume.FromCubicMillimeters(0)
                : (RoughingVolumeRemoved.CubicMillimeters <= 0 ? TotalVolumeRemoved 
                : Volume.FromCubicMillimeters(WettedSurfaceArea.SquareMillimeters * _inputs.Machining.RoughingAmountToLeave.Millimeters));

        [Display(Name = "Finish Time")]
        [Equation("Finish Volume Removed / (Finish MRR * 60)")]
        [OutputUnitType(KatanaUnitType.CalculatedTime)]
        public Duration FinishTime => Duration.FromMinutes(FinishVolumeRemoved.CubicCentimeters / _inputs.Machining.FinishMRR.CubicCentimetersPerMinute);

        [Display(Name = "Total Time To Finish")]
        [Equation("Finish Time + (Setup Load Unload Time) /2")]
        [OutputUnitType(KatanaUnitType.CalculatedTime)]
        public Duration TotalTimeToFinish => Duration.FromHours(FinishTime.Hours + (LoadUnloadSetupTime.Hours) / 2);

        [Display(Name = "Finishing Cost")]
        [Equation("Total Time To Finish * Labor Rate")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Cost FinishingCost => Cost.FromDollars(TotalTimeToFinish.Hours * _inputs.General.LaborRate.DollarsPerHour);

        [Display(Name = "Finishing Scrap Cost")]
        [Equation(" - Finished Volume Removed * Titanium Chip Reclaim Value * Titanium Density")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost FinishingScrapCost => Cost.FromDollars(-FinishVolumeRemoved.CubicMillimeters * _inputs.Machining.TitaniumChipReclaimValue.DollarsPerKilogram * _inputs.General.TitaniumDensity.KilogramsPerCubicMillimeter);
        #endregion

        #region Machine Cost Calculations

        [Display(Name = "Machine Lifetime Availability")]
        [Equation("Machine Availability * Machine Life in Years * Work Hours per Year")]
        //User input, since other inputs are not displayed
        [OutputUnitType(KatanaUnitType.UserInput)]
        public Duration MachineLifetimeAvailability => Duration.FromHours(_inputs.General.HoursPerYear.Unitless * _inputs.Machining.MachineLife.Years * _inputs.Machining.MachineAvailability.DecimalFractions);

        private Cost MachineCost(Duration time) => Cost.FromDollars(time.Hours * _inputs.Machining.MachineCost.Dollars / MachineLifetimeAvailability.Hours);

        private Cost RoughingMachineCost => MachineCost(TotalTimeToRough);

        private Cost FinishMachineCost => MachineCost(TotalTimeToFinish);
        
        [Display(Name = "Total Machining Time")]
        [Equation("Total Time to Rough + Total Time to Finish")]
        [OutputUnitType(KatanaUnitType.CalculatedTime)]
        public Duration TotalMachiningTime => TotalTimeToRough + TotalTimeToFinish;

        [Display(Name = "Amortized Machine Cost")]
        [Equation("Total Machining Time * Machine Capital Cost / Machine Lifetime Availability")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost AmortizedMachineCost => RoughingMachineCost + FinishMachineCost;

        #endregion

        #region Tooling Cost Calculations

        [Display(Name = "Cutting Tool Cost")]
        [Equation("Total Machining Time * Tool Cost / Tool Life")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost ToolCost => Cost.FromDollars(TotalMachiningTime.Minutes * _inputs.Machining.ToolCost.Dollars / _inputs.Machining.ToolLife.Minutes);
        
        [Display(Name = "Amortized Fixture Cost")]
        [Equation("Total Machining Time * Tool Cost / Tool Life")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost AmortizedFixtureCost => Cost.FromDollars(TotalMachiningTime.Minutes * _inputs.Machining.FixtureCost.Dollars / _inputs.Machining.FixtureLife.Minutes);

        [Display(Name = "Total Tooling Cost")]
        [Equation("Amortized Fixture Cost + Tool Cost")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost TotalToolingCost => AmortizedFixtureCost + ToolCost;

        #endregion

        #region Overhead Cost Calculations

        [Display(Name = "Consumables Cost")]
        [Equation("Overhead Rate * (Finishing Cost + Roughing Cost)")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost ConsumablesCost => _inputs.Machining.ConsumablesRate.Unitless * (FinishingCost + RoughingCost);

        #endregion

        //Internal property, because we do not want to display it in the UI.
        internal Cost PrepCost => RoughingCost + RoughingMachineCost + RoughingScrapCost;

        [Display(Name = "Total Cost")]
        [Equation("Roughing Cost + Roughing Scrap Cost + Finishing Cost + Finishing Scrap Cost + Total Tooling Cost + Consumables Cost + Amortized Machine Cost")]
        [Notes("COST RETURNED IS AN ESTIMATE")]
        [OutputUnitType(KatanaUnitType.TotalCost)]
        public Cost TotalCost => RoughingCost + FinishingCost + RoughingScrapCost + FinishingScrapCost + 
            TotalToolingCost + ConsumablesCost + AmortizedMachineCost;


        public Duration TotalTime { get; set; } = Duration.Zero;
    }
}
