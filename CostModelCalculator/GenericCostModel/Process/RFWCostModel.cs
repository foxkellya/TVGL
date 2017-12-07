using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.Blanks;
using KatanaObjects.CostModels;
using UnitsNet;
using UnitsNet.Units;

namespace GenericCostModel.Process
{
    public class RFWCostModel : ICostModel
    {

        #region Private: inputs, mass limits, setup times

        //ToDo:Make all these internal values inputs
        private readonly Mass _lowMassMax = Mass.FromPounds(10);
        private readonly Mass _medMassMax = Mass.FromPounds(20);

        private readonly Duration _lowMassLoadUnloadTime = Duration.FromMinutes(10);
        private readonly Duration _medMassLoadUnloadTime = Duration.FromMinutes(20);
        private readonly Duration _highMassLoadUnloadTime = Duration.FromMinutes(30);

        private readonly SearchInputs _inputs;

        #endregion

        public RFWCostModel(SearchInputs inputs, Area weldArea, Blank blank1, Blank blank2)
        {
            _inputs = inputs;
            WeldArea = weldArea;
            Blank1Volume = blank1.StockVolume;
            Blank2Volume = blank2.StockVolume;
            Blank1ClampingPerimeter = blank1.PerimeterOnPlane;
            Blank2ClampingPerimeter = blank2.PerimeterOnPlane;
        }

        #region Blank and Weld Geometry

        [Display(Name = "Weld Area")]
        [Equation("Geometry From TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Area WeldArea { get; }

        //Note that the number in this name does not line up with the configuration UI output,
        //So I went with a more generic "First" and "Second"
        [Display(Name = "Clamping Perimeter of First Blank")]
        [Equation("Geometry From TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Length Blank1ClampingPerimeter { get; }

        [Display(Name = "Clamping Perimeter of Second Blank")]
        [Equation("Geometry From TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Length Blank2ClampingPerimeter { get; }

        //No need to expose these properties, Since they are stored in the blank info. therefore private.
        private Mass Blank1Mass => Mass.FromKilograms(Blank1Volume.CubicMillimeters * _inputs.General.TitaniumDensity.KilogramsPerCubicMillimeter);

        private Mass Blank2Mass => Mass.FromKilograms(Blank2Volume.CubicMillimeters * _inputs.General.TitaniumDensity.KilogramsPerCubicMillimeter);

        private Volume Blank1Volume { get; }

        private Volume Blank2Volume { get; }

        [Display(Name = "Total Mass of Both Blanks")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Mass TotalBlankMass => Blank1Mass + Blank2Mass;

        #endregion

        #region Times

        [Display(Name = "Load & Unload Time")]
        [Equation("Tiered from Mass of Part")]
        [Source("Guess")]
        [OutputUnitType(KatanaUnitType.InternalValue)]
        [CostModelViewUnit(DurationUnit.Minute)]
        public Duration LoadUnloadTime
            =>
                TotalBlankMass <= _lowMassMax
                    ? _lowMassLoadUnloadTime
                    : (TotalBlankMass <= _medMassMax ? _medMassLoadUnloadTime : _highMassLoadUnloadTime);

        [Display(Name = "Welding Time")]
        [Source("Linear Friction Welding of Ti-6Al-4V : Processing , Microstructure , and Mechanical-Property Inter-Relationships, 2005")]
        [Notes("Assumed similar to LFW. Should be between 3 and 10 seconds")]
        [OutputUnitType(KatanaUnitType.CalculatedTime)]
        [CostModelViewUnit(DurationUnit.Second)]
        public Duration WeldingTime => _inputs.RFW.WeldTime;

        [Display(Name = "Total Welding Time")]
        [Equation("Welding Time + Load & Unload Time + Tool Change Time")]
        [OutputUnitType(KatanaUnitType.CalculatedTime)]
        [CostModelViewUnit(DurationUnit.Minute)]
        public Duration TotalWeldingTime => WeldingTime + LoadUnloadTime;

        #endregion

        #region Welding & Machine Cost Calc

        [Display(Name = "Welding Cost")]
        [Equation("Total Welding Time * Labor Rate")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost WeldingCost => Cost.FromDollars(TotalWeldingTime.Hours * _inputs.General.LaborRate.DollarsPerHour);

        [Display(Name = "Machine Lifetime Availability")]
        [Equation("Work Hours per Year * RFW Machine Life * RFW Machine Availability")]
        [OutputUnitType(KatanaUnitType.CalculatedTime)]
        [CostModelViewUnit(DurationUnit.Hour)]
        private Duration MachineLifetimeAvailability => Duration.FromHours(_inputs.General.HoursPerYear.Unitless * _inputs.RFW.MachineLife.Weeks * _inputs.RFW.MachineAvailability.DecimalFractions);

        [Display(Name = "Machine Capital Cost")]
        [Equation("From RFW Inputs")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        public Cost MachineCapitalCost => _inputs.RFW.MachineCost;

        [Display(Name = "Amortized Machine Cost")]
        [Equation("Total Welding Time * RFW Machine Cost / Machine Lifetime Availability")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost AmortizedMachineCost => Cost.FromDollars(TotalWeldingTime.Hours * _inputs.RFW.MachineCost.Dollars / MachineLifetimeAvailability.Hours);

        #endregion

        #region Fixture Tooling Cost

        [Display(Name = "Fixture Cost")]
        [Equation("Always 10000")]
        //Internal value, since the user does not have access to it yet. 
        //Then it will be an Input. 
        //Not a calculated cost, since nothing is really being done inside here.
        //This will make it so that it does not show up on the Results Window.
        [OutputUnitType(KatanaUnitType.InternalValue)]
        public Cost FixtureCost => Cost.FromDollars(10000);
        //TotalBlankMass <= _lowMassMax ? Cost.FromDollars(10000) :
        //TotalBlankMass <= _medMassMax ? Cost.FromDollars(22000) : Cost.FromDollars(34000);

        [Display(Name = "Fixture Life")]
        [Equation("5 Years * Work Hours per Year")]
        [OutputUnitType(KatanaUnitType.CalculatedTime)]
        public Duration FixtureLife => Duration.FromHours(5 * _inputs.General.HoursPerYear.Unitless);

        [Display(Name = "Amortized Tooling Cost")]
        [Equation("Fixture Cost * ( Total Welding Time / Fixture Life)")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost AmortizedToolingCost => Cost.FromDollars(FixtureCost.Dollars * (TotalWeldingTime.Hours / FixtureLife.Hours));

        #endregion

        #region Overhead & Factory Cost

        [Display(Name = "Total Factory Cost")]
        [Equation("Total Labor Cost + Amortized Machine Cost")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost TotalFactoryCost => WeldingCost + AmortizedMachineCost + AmortizedToolingCost;

        [Display(Name = "General and Administrative Overhead Cost")]
        [Equation("Overhead * (Welding Costt)")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost OverheadCost => _inputs.General.Overhead.Unitless * (WeldingCost);

        #endregion

        [Display(Name = "Total Cost")]
        [Equation("Total Factory Cost + General and Administrative Overhead Cost")]
        [Notes("COST RETURNED IS AN ESTIMATE")]
        [OutputUnitType(KatanaUnitType.TotalCost)]
        public Cost TotalCost => TotalFactoryCost + OverheadCost;


        public Duration TotalTime { get; set; } = Duration.Zero;
    }
}
