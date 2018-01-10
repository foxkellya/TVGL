using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.CostModels;
using UnitsNet;

namespace BlankFactory.CostModels
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

        //DO NOT display this property, since the cost is captured in the blank properties.
        internal Volume StockVolume { get; }

        //DO NOT display this property, since the cost is captured in the blank properties.
        internal Volume SubstrateVolume { get; }
        
        //DO NOT display this property, since the cost is captured in the blank cost.
        internal Cost BlankMaterialCost { get; }

        //DO NOT display this property, since the cost is captured in the blank cost.
        internal Cost SubstrateMaterialCost { get; }

        //DO NOT display this property, since the cost is captured in the blank properties.
        internal Mass SubstrateMass => Mass.FromKilograms(SubstrateVolume.CubicMillimeters * _inputs.General.MaterialDensity.KilogramsPerCubicMillimeter);

        //DO NOT display this property, since the cost is captured in the blank properties.
        internal Mass BlankMass => Mass.FromKilograms(StockVolume.CubicMillimeters * _inputs.General.MaterialDensity.KilogramsPerCubicMillimeter);

        //[hours == 0.5 + (kg / (kg/hour))], '0.5' from Boeing
        [Display(Name = "Deposition Time")]
        public Duration DepositionTime => Duration.FromHours(0.5 + BlankMass.Kilograms / _inputs.WireFeed.AdditiveDepositionRate.KilogramsPerHour);

        //[dollars == hours * (dollars/hour)]
        [Display(Name = "Labor Cost")]
        public Cost AMLaborCost => Cost.FromDollars(DepositionTime.Hours * _inputs.General.LaborRate.DollarsPerHour);

        //[hours during lifetime = (hours/day) * (days/week) * (machine life in weeks) * MachineUptimePercentage]
        //No need to expose this. => private
        private Duration MachineLifetimeAvailability => Duration.FromHours(_inputs.WireFeed.HoursPerDay.Unitless * _inputs.WireFeed.DaysPerWeek.Unitless * _inputs.WireFeed.MachineLife.Weeks * _inputs.WireFeed.MachineAvailability.DecimalFractions);

        //[dollars = (dollars/hours) * (hours)]]
        [Display(Name = "Machine Cost")]
        public Cost MachineCost => Cost.FromDollars(DepositionTime.Hours * _inputs.WireFeed.MachineCost.Dollars / MachineLifetimeAvailability.Hours);

        //[dollars]
        [Display(Name = "Total Manufacturing Cost")]
        public Cost TotalManufacturingCost => AMLaborCost + MachineCost; //+MaterialOverheadCost 

        //[dollars]
        [Display(Name = "General Adminsitrative Cost")]
        public Cost GACost => TotalManufacturingCost * _inputs.WireFeed.GAOverhead.Unitless;

        //[dollars]
        [Display(Name = "Total Cost")]
        public Cost TotalCost => TotalManufacturingCost + GACost ; //+ HeatTreatCost
    }
}
