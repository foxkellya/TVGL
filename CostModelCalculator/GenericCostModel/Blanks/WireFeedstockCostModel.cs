using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.CostModels;
using UnitsNet;

namespace GenericCostModel.Blanks
{
    public class WireFeedstockCostModel : ICostModel
    {
        private readonly SearchInputs _inputs;

        public WireFeedstockCostModel(SearchInputs inputs, Volume stockVolume)
        {
            _inputs = inputs;
            StockVolume = stockVolume;
        }

        //[mm^3]
        [Display(Name = "Stock Volume")]
        [Source("Calculated Geometry With TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Volume StockVolume { get; }

        //[kg]
        [Display(Name = "Feedstock Mass")]
        [Equation("Gemoetry from TVGL")]
        [OutputUnitType(KatanaUnitType.Geometric)]
        public Mass FeedstockMass => Mass.FromKilograms(StockVolume.CubicMillimeters * _inputs.General.TitaniumDensity.KilogramsPerCubicMillimeter) ;

        //[dollars / kg]
        [Display(Name = "Wire Price Per Mass")]
        [Equation("From Water Jet Plate Inputs")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        public CostPerMass WirePrice => _inputs.WireFeedstock.WirePrice;

        [Display(Name = "Material Cost")]
        [Equation("Mass * Wire Price Per Mass")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost MaterialCost => Cost.FromDollars(FeedstockMass.Kilograms * WirePrice.DollarsPerKilogram);

        [Display(Name = "Total Cost")]
        [Equation("Material Cost")]
        [Notes("COST RETURNED IS AN ESTIMATE")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost TotalCost => MaterialCost;


        public Duration TotalTime { get; set; } = Duration.Zero;
    }
}
