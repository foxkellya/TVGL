using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.Blanks;
using KatanaObjects.CostModels;
using UnitsNet;

namespace BlankFactory.CostModels
{
    public class HollowTubeCostModel : ICostModel
    {
        private readonly SearchInputs _inputs;

        public HollowTubeCostModel(SearchInputs inputs, Blank blank)
        {
            _inputs = inputs;
            StockVolume = blank.StockVolume;
            var tubeSize = blank.SubVolume.HollowTubeSize;
            OuterDiameter = tubeSize.OuterDiameter;
            InnerDiameter = tubeSize.InnerDiameter;
            HollowTubePricePerMass = _inputs.HollowTube.PricePerMass;
        }

        [Display(Name = "Stock Volume")]
        public Volume StockVolume { get; }

        [Display(Name = "Outer Diameter")]
        public Length OuterDiameter { get; }

        [Display(Name = "Inner Diameter")]
        public Length InnerDiameter { get; }

        [Display(Name = "Price Per Unit Mass")]
        public CostPerMass HollowTubePricePerMass { get; }

        [Display(Name = "Material Cost")]
        public Cost MaterialCost => Cost.FromDollars(StockVolume.CubicMeters * HollowTubePricePerMass.DollarsPerKilogram);

        [Display(Name = "Saw Cutting Cost")]
        public Cost SawCost => Cost.FromDollars(OuterDiameter.Meters * _inputs.General.SawCostRate.DollarsPerMeter);

        [Display(Name = "Saw Overhead Cost")]
        public Cost SawOverheadCost => SawCost;

        [Display(Name = "Total Saw Cost")]
        public Cost TotalSawCost => SawCost + SawOverheadCost;

        [Display(Name = "Total Cost")]
        public Cost TotalCost => MaterialCost + TotalSawCost;
    }
}
