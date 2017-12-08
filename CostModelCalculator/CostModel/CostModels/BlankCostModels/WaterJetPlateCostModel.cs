using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.Blanks;
using KatanaObjects.CostModels;
using UnitsNet;

namespace BlankFactory.CostModels
{
    public class WaterJetPlateCostModel : ICostModel
    {
        private readonly SearchInputs _inputs;

        public WaterJetPlateCostModel(SearchInputs inputs, Blank blank)
        {
            _inputs = inputs;
            StockVolume = blank.StockVolume;
            PlateThickness = blank.SubVolume.WaterjetDepth;
            Perimeter = blank.SubVolume.WaterjetCuttingPerimeter;
            WaterjetCuttingRate = inputs.Waterjet.CuttingRatesBasedOnThickness[PlateThickness];
            WaterjetPricePerMass =
                CostPerMass.FromDollarsPerKilogram(
                    inputs.Waterjet.PricePerKilogramMultipliers[PlateThickness].Unitless*
                    _inputs.Waterjet.PricePerMass.DollarsPerKilogram);
            StockMass = Mass.FromKilograms(StockVolume.CubicMeters*inputs.General.MaterialDensity.KilogramsPerCubicMeter);
        }

        [Display(Name = "Waterjet Plate Volume")]
        public Volume StockVolume { get; }

        [Display(Name = "Waterjet Plate Mass")]
        public Mass StockMass { get; }

        [Display(Name = "Plate Thickness")]
        public Length PlateThickness { get; }

        [Display(Name = "Perimeter")]
        public Length Perimeter { get; }

        [Display(Name = "Waterjet Cutting Rate")]
        public CostPerDistance WaterjetCuttingRate { get; }

        [Display(Name = "Price Per Unit Mass")]
        public CostPerMass WaterjetPricePerMass { get; }

        //[dollars = (kg) * (dollar/kg)]
        [Display(Name = "Plate Material Cost")]
        public Cost PlateCost => Cost.FromDollars(StockMass.Kilograms * WaterjetPricePerMass.DollarsPerKilogram);

        //[dollars = mm * (dollars/mm)]
        [Display(Name = "Waterjet Cutting Cost")]
        public Cost WaterJetCost => Cost.FromDollars(Perimeter.Millimeters * WaterjetCuttingRate.DollarsPerMillimeter);

        //Waterjet plate is purchased, so the overhead is not incurred by Boeing
        //This is based on a meeting that was mentioned where Eric Eide and Matt Carter stated that waterjet cutting is very messy and would likely not be done in a Blank Factory

        //[Display(Name = "Waterjet Overhead Cost")]
        //public Cost WaterJetOverheadCost => WaterJetCost * _inputs.Waterjet.Overhead.Unitless;

        [Display(Name = "Total Waterjet Cutting Cost")]
        public Cost TotalWaterJetCost => WaterJetCost; // + WaterJetOverheadCost;

        [Display(Name = "Total Cost")]
        public Cost TotalCost => PlateCost + TotalWaterJetCost;
    }
}
