using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.CostModels;
using UnitsNet;

namespace BlankFactory.CostModels
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

        [Display(Name = "Stock Volume")]
        public Volume StockVolume { get; }

        [Display(Name = "Substrate Volume")]
        public Volume SubstrateVolume { get; }

        //[kg = mm^3 * kg/mm^3]
        [Display(Name = "Substrate Mass To Be Joined")]
        public Mass SubstrateMassToBeJoined => Mass.FromKilograms(SubstrateVolume.CubicMillimeters * _inputs.General.MaterialDensity.KilogramsPerCubicMillimeter);

        //[kg == mm^3 * kg/mm^3]
        [Display(Name = "Mass To Be Joined")]
        public Mass MassToBeJoined => Mass.FromKilograms(StockVolume.CubicMillimeters * _inputs.General.MaterialDensity.KilogramsPerCubicMillimeter);

        //[kg] - only used to calculate HT cost
        [Display(Name = "Total Material Mass")]
        public Mass TotalMaterialMass => SubstrateMassToBeJoined + MassToBeJoined;

        //[dollars == kg * dollars/kg]
        [Display(Name = "Substrate Cost")]
        public Cost SubstrateCost => Cost.FromDollars(SubstrateMassToBeJoined.Kilograms * _inputs.RectangularBarStock.MaterialPrice.DollarsPerKilogram);

        //[dollars == kg * dollars/kg]
        [Display(Name = "Deposition Cost")]
        public Cost DepositionCost => Cost.FromDollars(MassToBeJoined.Kilograms * _inputs.WireFeedstock.WirePrice.DollarsPerKilogram);

        //[dollars]
        [Display(Name = "Total Material Cost")]
        public Cost TotalMaterialCost => SubstrateCost + DepositionCost;

        //[hours == 0.5 + (kg / (kg/hour))], '0.5' from Boeing
        [Display(Name = "Time For Deposition")]
        public Duration TimeForDeposition => Duration.FromHours(0.5 + MassToBeJoined.Kilograms/_inputs.WireFeed.AdditiveDepositionRate.KilogramsPerHour);

        //[dollars == hours * dollars/hour]
        [Display(Name = "Labor Cost")]
        public Cost AMLaborCost => Cost.FromDollars(TimeForDeposition.Hours*_inputs.General.LaborRate.DollarsPerHour);

        //[dollars] Machine off the substrate.
        [Display(Name = "Substrate Machining Cost")]
        public Cost SubstrateMachiningCost => new MachiningCostModel(_inputs, Volume.FromCubicMillimeters(0.0), Area.FromSquareMillimeters(0.0), SubstrateVolume, true).TotalCost;

        //[hours during lifetime = (hours/day) * (days/week) * (machine life in weeks) * MachineUptimePercentage]
        //No need to expose this. => private
        private Duration MachineLifetimeAvailability => Duration.FromHours(_inputs.WireFeed.HoursPerDay.Unitless * _inputs.WireFeed.DaysPerWeek.Unitless * _inputs.WireFeed.MachineLife.Weeks * _inputs.WireFeed.MachineAvailability.DecimalFractions);

        //[dollars = (dollars/hours) * (hours)]]
        [Display(Name = "Machine Cost")]
        public Cost MachineCost => Cost.FromDollars(TimeForDeposition.Hours * _inputs.WireFeed.MachineCost.Dollars / MachineLifetimeAvailability.Hours);

        //[dollars]
        [Display(Name = "Total Manufacturing Cost")]
        public Cost TotalManufacturingCost => TotalMaterialCost + AMLaborCost + MachineCost + SubstrateMachiningCost;

        //[dollars]
        [Display(Name = "General Administrative Cost")]
        public Cost GACost => TotalManufacturingCost * _inputs.WireFeed.GAOverhead.Unitless;

        //[dollars]
        [Display(Name = "Total Cost")]
        public Cost TotalCost => TotalManufacturingCost + GACost ;//+HeatTreatCost 
    }
}
