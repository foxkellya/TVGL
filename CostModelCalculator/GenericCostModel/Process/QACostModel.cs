using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.Blanks;
using KatanaObjects.CostModels;
using UnitsNet;
using UnitsNet.Units;

namespace GenericCostModel.Process
{
    public class QACostModel : ICostModel
    {

        private readonly SearchInputs _inputs;

        public QACostModel(SearchInputs inputs, Blank blank1, Blank blank2)
        {

            _inputs = inputs;
            Blank1Volume = blank1.StockVolume;
            Blank2Volume = blank2.StockVolume;
        }

        #region Blank and Weld Geometry

        private Mass Blank1Mass => Mass.FromKilograms(Blank1Volume.CubicMillimeters * _inputs.General.TitaniumDensity.KilogramsPerCubicMillimeter);

        private Mass Blank2Mass => Mass.FromKilograms(Blank2Volume.CubicMillimeters * _inputs.General.TitaniumDensity.KilogramsPerCubicMillimeter);

        private Volume Blank1Volume { get; }

        private Volume Blank2Volume { get; }

        [Display(Name = "Total Mass of Both Blanks")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Mass TotalBlankMass => Blank1Mass + Blank2Mass;

        #endregion

        #region EngQATime

        [Display(Name = "Engineering Quality Assurance Cost (EngQACost)")]
        [Equation("EngQATime * EngQARate")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost EngQACost => Cost.FromDollars(_inputs.EngineeringQA.EngQATime.Hours * _inputs.General.LaborRate.DollarsPerHour);
        
        [Display(Name = "Engineering Quality Assurance Time (EngQATime)")]
        [Equation("From Engineering QA Inputs")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        [CostModelViewUnit(DurationUnit.Hour)]
        public Duration EngQATime => _inputs.EngineeringQA.EngQATime;

        #endregion

        #region GA Overhead Costs

        [Display(Name = "General and Administrative Overhead Cost")]
        [Equation("Overhead * Engr QA Cost")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost OverheadCost => Cost.FromDollars(_inputs.General.Overhead.Unitless * EngQACost.Dollars);

        #endregion

        [Display(Name = "Total Cost")]
        [Equation("General and Administrative Overhead Cost + Engr QA Cost")]
        [Notes("COST RETURNED IS AN ESTIMATE")]
        [OutputUnitType(KatanaUnitType.TotalCost)]
        public Cost TotalCost => OverheadCost + EngQACost;


        public Duration TotalTime { get; set; } = Duration.Zero;
    }
}
