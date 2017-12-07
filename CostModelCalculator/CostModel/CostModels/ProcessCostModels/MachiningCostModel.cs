using System;
using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.CostModels;
using UnitsNet;
using UnitsNet.Units;

namespace BlankFactory.CostModels
{
    public class MachiningCostModel : ICostModel
    {
        //[kg]
        private readonly Mass _lowMassMax = Mass.FromKilograms(2.5);
        private readonly Mass _medMassMax = Mass.FromKilograms(10);

        //[minutes]
        private readonly Duration _lowMassSetupTime = Duration.FromMinutes(3);
        private readonly Duration _lowMassLoadTime = Duration.FromMinutes(1);
        private readonly Duration _lowMassUnloadTime = Duration.FromMinutes(1);
        private readonly Duration _medMassSetupTime = Duration.FromMinutes(5);
        private readonly Duration _medMassLoadTime = Duration.FromMinutes(5);
        private readonly Duration _medMassUnloadTime = Duration.FromMinutes(5);
        private readonly Duration _highMassSetupTime = Duration.FromMinutes(8);
        private readonly Duration _highMassLoadTime = Duration.FromMinutes(10);
        private readonly Duration _highMassUnloadTime = Duration.FromMinutes(10);

        private readonly SearchInputs _inputs;

        private readonly bool _roughCutAll;

        public MachiningCostModel(SearchInputs inputs, Volume finishVolume, Area wettedSurfaceArea, Volume stockVolume, bool roughCutAll = false)
        {
            _inputs = inputs;
            FinishVolume = finishVolume;
            WettedSurfaceArea = wettedSurfaceArea;
            StockVolume = stockVolume;
            _roughCutAll = roughCutAll;
        }

        //[mm^3]
        [Display(Name = "Finish Volume")]
        public Volume FinishVolume { get; }

        //[mm^2]
        [Display(Name = "Wetted Surface Area")]
        public Area WettedSurfaceArea { get; }

        //[mm^3]
        [Display(Name = "Stock Volume")]
        public Volume StockVolume { get; }

        //[kg = mm^3 * (kg/mm^3)]
        [Display(Name = "Stock Mass")]
        public Mass StockMass => Mass.FromKilograms(StockVolume.CubicMillimeters * _inputs.General.MaterialDensity.KilogramsPerCubicMillimeter);

        //[minutes]
        [Display(Name = "Setup Time")]
        [CostModelViewUnit(DurationUnit.Minute)]
        public Duration SetupTime
            =>
                StockMass <= _lowMassMax
                    ? _lowMassSetupTime
                    : (StockMass <= _medMassMax ? _medMassSetupTime : _highMassSetupTime);

        //[minutes]
        [Display(Name = "Load Time")]
        [CostModelViewUnit(DurationUnit.Minute)]
        public Duration LoadTime
            =>
                StockMass <= _lowMassMax
                    ? _lowMassLoadTime
                    : (StockMass <= _medMassMax ? _medMassLoadTime : _highMassLoadTime);

        //[minutes]
        [Display(Name = "Unload Time")]
        [CostModelViewUnit(DurationUnit.Minute)]
        public Duration UnloadTime
            =>
                StockMass <= _lowMassMax
                    ? _lowMassUnloadTime
                    : (StockMass <= _medMassMax ? _medMassUnloadTime : _highMassUnloadTime);

        [Display(Name = "Total Volume Removed")]
        //[mm^3]
        public Volume TotalVolumeRemoved => StockVolume - FinishVolume;

        //[mm^3 = mm^3 - mm^3 - (mm^2 * mm)]
        [Display(Name = "Roughing Volume Remove")]
        public Volume RoughingVolumeRemoved
            =>
                _roughCutAll
                    ? TotalVolumeRemoved
                    : Volume.FromCubicMillimeters(Math.Max(0, TotalVolumeRemoved.CubicMillimeters - (WettedSurfaceArea.SquareMillimeters*_inputs.Machining.RoughingAmountToLeave.Millimeters)));
        
        //[mintues = cm^3 / (cm^3/minutes)]
        [Display(Name = "Roughing Time")]
        public Duration RoughingTime => Duration.FromMinutes(RoughingVolumeRemoved.CubicCentimeters / _inputs.Machining.RoughingMRR.CubicCentimetersPerMinute);

        //[hours] -- (setup, load, and unload time divided by 2 to split between rough and finishing time) 
        [Display(Name = "Total Time To Rough")]
        public Duration TotalTimeToRough => Duration.FromHours(RoughingTime.Hours + (SetupTime.Hours + LoadTime.Hours + UnloadTime.Hours + _inputs.Machining.ToolChangeTime.Hours) / 2);

        //[dollars = hours * (dollars/hour)]
        [Display(Name = "Roughing Cost")]
        public Cost RoughingCost => Cost.FromDollars(TotalTimeToRough.Hours * _inputs.Machining.ShopCostRate.DollarsPerHour);

        //[dollars = mm^3 * (dollars/kg) * (kg/mm^3)]
        [Display(Name = "Roughing Scrap Cost")]
        public Cost RoughingScrapCost => Cost.FromDollars(-RoughingVolumeRemoved.CubicMillimeters * _inputs.Machining.TitaniumChipReclaimValue.DollarsPerKilogram * _inputs.General.MaterialDensity.KilogramsPerCubicMillimeter);

        //[dollars]
        [Display(Name = "Total Roughing Cost")]
        public Cost TotalRoughingCost => RoughingCost + RoughingScrapCost;

        //[mm^3 = mm^2 * mm]
        [Display(Name = "Finish Volume Removed")]
        public Volume FinishVolumeRemoved
            =>
                _roughCutAll
                ? Volume.FromCubicMillimeters(0)
                : (RoughingVolumeRemoved.CubicMillimeters <= 0 ? TotalVolumeRemoved 
                : Volume.FromCubicMillimeters(WettedSurfaceArea.SquareMillimeters * _inputs.Machining.RoughingAmountToLeave.Millimeters));

        //[mintues = cm^3 / (cm^3/minutes)]
        [Display(Name = "Finish Time")]
        public Duration FinishTime => Duration.FromMinutes(FinishVolumeRemoved.CubicCentimeters / _inputs.Machining.FinishMRR.CubicCentimetersPerMinute);

        //[hours] -- (divide by 2 to split between rough and finishing)
        [Display(Name = "Total Time To Finish")]
        public Duration TotalTimeToFinish => Duration.FromHours(FinishTime.Hours + (SetupTime.Hours + LoadTime.Hours + UnloadTime.Hours + _inputs.Machining.ToolChangeTime.Hours) / 2);

        //[dollars = hours * (dollars/hour)]
        [Display(Name = "Finishing Cost")]
        public Cost FinishingCost => Cost.FromDollars(TotalTimeToFinish.Hours * _inputs.Machining.ShopCostRate.DollarsPerHour);

        //[dollars = mm^3 * (dollars/kg) * (kg/mm^3)]
        [Display(Name = "Finishing Scrap Cost")]
        public Cost FinishingScrapCost => Cost.FromDollars(-FinishVolumeRemoved.CubicMillimeters * _inputs.Machining.TitaniumChipReclaimValue.DollarsPerKilogram * _inputs.General.MaterialDensity.KilogramsPerCubicMillimeter);

        //[dollars = hours * (dollars/hour)]
        [Display(Name = "Total Finish Cost")]
        public Cost TotalFinishCost => FinishingCost + FinishingScrapCost;

        //[hours]
        [Display(Name = "Total Machining Time")]
        public Duration TotalMachiningTime => TotalTimeToRough + TotalTimeToFinish;

        //[Dollars/Minute]
        [Display(Name = "Cutting Tool cost")]
        public CostRate ToolCostRate => CostRate.FromDollarsPerMinute(_inputs.Machining.ToolCost.Dollars /_inputs.Machining.ToolLife.Minutes);

        //[dollars/minute]
        [Display(Name = "Fixture Cost")]
        public CostRate FixtureCostRate => CostRate.FromDollarsPerMinute(_inputs.Machining.FixtureCost.Dollars / _inputs.Machining.FixtureLife.Minutes);

        //[dollars]
        [Display(Name = "Total Tooling Cost")]
        public Cost TotalToolingCost => Cost.FromDollars((ToolCostRate.DollarsPerMinute + FixtureCostRate.DollarsPerMinute) * (TotalMachiningTime.Minutes));

        //[dollars]
        [Display(Name = "Total Cost")]
        public Cost TotalCost => TotalRoughingCost + TotalFinishCost;
    }
}
