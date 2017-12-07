using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.CostModels;
using UnitsNet;
using UnitsNet.Units;

namespace GenericCostModel.Process
{
    //Description: This model is used to estimate the cost of the flash removal process
    //ASSUMES: cost of cutting fluid is accounted for with the overhead rate 
    //ASSUMES: Materials chipped returned for recycling are negligible
    //ASSUMES: Flash removal for RFW takes place on RFW Machine & for LFW takes place on Mill (would incur Material Handling)

    public class FlashRemovalCostModel : ICostModel
    {

        #region Private: Time, from, inputs

        private bool FromRFW { get; } //1 if from RFW, 0 if from LFW

        private readonly Mass _lowMassMax = Mass.FromPounds(10);
        private readonly Mass _medMassMax = Mass.FromPounds(25);

        //Time Aspects
        private readonly Duration _lowMassLoadUnloadSetupTime = Duration.FromMinutes(10);
        private readonly Duration _medMassLoadUnloadSetupTime = Duration.FromMinutes(20);
        private readonly Duration _highMassLoadUnloadSetupTime = Duration.FromMinutes(30);

        private readonly SearchInputs _inputs;

        #endregion

        public FlashRemovalCostModel(SearchInputs inputs, Length weldPerimeter, Volume stockVolume, bool fromRFW = true)
        {
            _inputs = inputs;
            WeldPerimeter = weldPerimeter;
            StockVolume = stockVolume;
            FromRFW = fromRFW;

        }

        #region Geometries and From param

        [Display(Name = "Weld Perimeter")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Length WeldPerimeter { get; }

        [Display(Name = "Stock Volume")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Volume StockVolume { get; }

        [Display(Name = "Part Mass")]
        [Equation("Stock Volume * Titanium Density")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Mass StockMass => Mass.FromKilograms(StockVolume.CubicMillimeters * _inputs.General.TitaniumDensity.KilogramsPerCubicMillimeter);

        #endregion

        #region Time Calculations

        [Display(Name = "Setup Time")]
        [Equation("Tiered based on Material Mass (None if RFW)")]
        [OutputUnitType(KatanaUnitType.CalculatedTime)]
        [CostModelViewUnit(DurationUnit.Minute)]
        public Duration SetupTime =>
            FromRFW ? Duration.FromMinutes(0) :
            StockMass <= _lowMassMax ? _lowMassLoadUnloadSetupTime :
            StockMass <= _medMassMax ? _medMassLoadUnloadSetupTime : _highMassLoadUnloadSetupTime;

        [Display(Name = "Flash Removal Time")]
        [Equation("Weld Perimenter / Feed Rate")]
        [OutputUnitType(KatanaUnitType.CalculatedTime)]
        [CostModelViewUnit(DurationUnit.Minute)]
        public Duration FlashRemovalTime => Duration.FromMinutes(WeldPerimeter.Centimeters / _inputs.FlashRemoval.FlashRemovalFeedRate.CentimetersPerMinutes);

        [Display(Name = "Total Time")]
        [Equation("Setup Time + Flash Removal Time")]
        [OutputUnitType(KatanaUnitType.CalculatedTime)]
        [CostModelViewUnit(DurationUnit.Minute)]
        public Duration TotalTime => Duration.FromMinutes(SetupTime.Minutes + FlashRemovalTime.Minutes);

        #endregion

        #region Machine Cost Calculations

        [Display(Name = "Machine Capital Cost")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        public Cost MachineCapitalCost =>
            FromRFW ? _inputs.RFW.MachineCost : _inputs.Machining.MachineCost;

        [Display(Name = "Machine Lifetime Availability")]
        [Equation("Machine Availability * Machine Life in Years * Work Hours per Year")]
        //User input, since other inputs are not displayed
        [OutputUnitType(KatanaUnitType.UserInput)]
        [CostModelViewUnit(DurationUnit.Hour)]
        public Duration MachineLifetimeAvailability =>
            FromRFW ? Duration.FromHours(_inputs.General.HoursPerYear.Unitless * _inputs.RFW.MachineLife.Years * _inputs.RFW.MachineAvailability.DecimalFractions) :
            Duration.FromHours(_inputs.General.HoursPerYear.Unitless *_inputs.Machining.MachineAvailability.DecimalFractions * _inputs.Machining.MachineLife.Years);

        [Display(Name = "Amortized Machine Cost")]
        [Equation("Total Time * Machine Capital Cost / Machine Lifetime Availability")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost AmortizedMachineCost => Cost.FromDollars(TotalTime.Minutes * MachineCapitalCost.Dollars / MachineLifetimeAvailability.Minutes );

        #endregion

        #region Tooling/Fixture Cost Calculations

        [Display(Name = "Fixture Cost")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        public Cost FixtureCost => FromRFW ?
            _inputs.RFW.FixtureCost : _inputs.Machining.FixtureCost;

        [Display(Name = "Fixture Life")]
        [Equation("Always 5 Years (Guess)")]
        [OutputUnitType(KatanaUnitType.InternalValue)]
        public Duration FixtureLife => FromRFW ? 
            Duration.FromHours(5 * _inputs.General.HoursPerYear.Unitless) : _inputs.Machining.FixtureLife;

        [Display(Name = "Amortized Fixture Cost")]
        [Equation("Fixture Cost * Total Time / Fixture Life")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost AmortizedFixtureCost => FixtureCost * (TotalTime.Hours/FixtureLife.Hours);

        [Display(Name = "Cutting Tool Life")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        [CostModelViewUnit(DurationUnit.Minute)]
        public Duration CuttingToolLife => _inputs.Machining.ToolLife;

        [Display(Name = "Cutting Tool Cost")]
        [Equation("Tool Cost * Flash Removal Time / Cutting Tool Life (Or $10 one time use cutting tool for RFW")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost CuttingToolCost => FromRFW ? 
            Cost.FromDollars(50) : Cost.FromDollars(_inputs.Machining.ToolCost.Dollars * FlashRemovalTime.Minutes / CuttingToolLife.Minutes);

        [Display(Name = "Total Tooling Cost")]
        [Equation("Fixture Cost + Cutting Tool Cost")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost TotalToolingCost => AmortizedFixtureCost + CuttingToolCost;

        #endregion

        #region Labor Cost and Overhead Calculations

        [Display(Name = "Labor Cost")]
        [Equation("Total Time * Labor Rate")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost LaborCost => Cost.FromDollars(TotalTime.Hours * _inputs.General.LaborRate.DollarsPerHour);

        [Display(Name = "Overhead Cost")]
        [Equation("(Labor cost) * Overhead Rate")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost OverheadCost => LaborCost * _inputs.General.Overhead.Unitless;

        #endregion

        [Display(Name = "Total Cost")]
        [Equation("Labor Cost + Machine Cost + Tooling Cost + Overhead Cost")]
        [Notes("COST RETURNED IS AN ESTIMATE")]
        [OutputUnitType(KatanaUnitType.TotalCost)]
        public Cost TotalCost => LaborCost + AmortizedMachineCost + TotalToolingCost + OverheadCost;
    }
}
