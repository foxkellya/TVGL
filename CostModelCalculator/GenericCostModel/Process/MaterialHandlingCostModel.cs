using System;
using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.CostModels;
using KatanaObjects.Processes;
using UnitsNet;

namespace GenericCostModel.Process
{
    public class MaterialHandlingCostModel : ICostModel
    {
        private readonly SearchInputs _inputs;

        public MaterialHandlingCostModel(Location fromLocation, Location toLocation, Volume totalVolume, SearchInputs inputs)
        {
            if (fromLocation.LocationName == toLocation.LocationName) return;
            _inputs = inputs;
            TotalMass = totalVolume*inputs.General.TitaniumDensity;
            _xDistance = Length.FromMeters(Math.Abs(toLocation.X.Meters - fromLocation.X.Meters));
            _yDistance = Length.FromMeters(Math.Abs(toLocation.Y.Meters - fromLocation.Y.Meters));
        }


        #region Machine Cost and Material Handling Method 

        [Display(Name = "Total Mass Moved")]
        [Equation("Estimate from TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Mass TotalMass { get; }

        [Display(Name = "Material Handling Method Speed")]
        [Equation("Estimate from TVGL")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        public Speed MaterialHandlingMethodSpeed =>
            TotalMass < _inputs.MaterialHandling.CartMassLimit ? _inputs.MaterialHandling.CartSpeed :
            TotalMass < _inputs.MaterialHandling.ForkliftMassLimit ? _inputs.MaterialHandling.ForkliftSpeed :
            _inputs.MaterialHandling.CraneSpeed;

        #endregion

        #region Distance & Time Calculations

        private readonly Length _xDistance;
        private readonly Length _yDistance;

        [Display(Name = "Total Distance Travelled")]
        [Equation("From Material Handling Input (Facility Layout)")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Length TotalDistance => _xDistance + _yDistance;

        [Display(Name = "Load/Unload Time")]
        [Equation("Tiered based on Total Mass")]
        [OutputUnitType(KatanaUnitType.CalculatedTime)]
        public Duration LoadUnloadTime =>
            TotalMass < _inputs.MaterialHandling.CartMassLimit ? _inputs.MaterialHandling.CartLoadUnload :
            TotalMass < _inputs.MaterialHandling.ForkliftMassLimit ? _inputs.MaterialHandling.ForkliftLoadUnload :
            _inputs.MaterialHandling.CraneLoadUnload;

        [Display(Name = "Total Time")]
        [Equation("Load/Unload Time + Total Distance / Material Handling Method Speed")]
        [OutputUnitType(KatanaUnitType.CalculatedTime)]
        public Duration TotalTime => Duration.FromSeconds(LoadUnloadTime.Seconds + (TotalDistance.Feet / MaterialHandlingMethodSpeed.FeetPerSecond));

        #endregion

        #region Labor and Overhead Cost

        [Display(Name = "Labor Cost")]
        [Equation("Total Time * Labor Rate")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost LaborCost => Cost.FromDollars(TotalTime.Hours * _inputs.General.LaborRate.DollarsPerHour);

        [Display(Name = "General and Administrative Overhead Multiplier")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        public Multiplier OverheadMultiplier =>  _inputs.General.Overhead;

        [Display(Name = "Overhead Cost")]
        [Equation("Labor Cost* Overhead Multiplier")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost OverheadCost => (LaborCost) * OverheadMultiplier.Unitless;

        #endregion

        [Display(Name = "Total Cost")]
        [Equation("LaborCost + OverheadCost")]
        [OutputUnitType(KatanaUnitType.TotalCost)]
        public Cost TotalCost => LaborCost + OverheadCost;
    }
}
