using System;
using System.ComponentModel.DataAnnotations;
using GenericInputs;
using KatanaObjects.Blanks;
using KatanaObjects.CostModels;
using UnitsNet;

namespace GenericCostModel.Blanks
{
    //Forging model was aquired by OSU from Kevin Slattery at Boeing.
    //Model was created for inputs as lbs and outputs a USD/lbs rate
    //Tiers based on finish mass

    public class ForgingCostModel : ICostModel
    {
        private readonly SearchInputs _inputs;

        public ForgingCostModel(SearchInputs inputs, Blank blank)
        {
            _inputs = inputs;
            var density = _inputs.General.MaterialDensity;

            //Cast to forging blank, so we can get at its specific properties
            var forgingBlank = (ForgingBlank)blank;

            #region Volume and Mass Properties (Note: The default KatanaUnitType is Geometric)
            //Volume
            FinishVolume = new VolumeProperty("Finish Volume", blank.FinishVolume);
            //The forging geometry estimate was included so that tooling excess could be updated in excel if desired
            //Tooling excess is part of the stock (delivered) volume, so it should be kept this way.
            ForgingGeometryEstimate = new VolumeProperty("Forging Volume Estimate", forgingBlank.StockVolume - forgingBlank.ToolingExcess) { Source = "Geometry from TVGL" };
            ToolingExcessVolume = new VolumeProperty("Tooling Excess Volume", forgingBlank.ToolingExcess)
            {
                Equation = ForgingGeometryEstimate.ExcelName + " * Tooling Excess"
            };
            DeliveredForgingVolume = new VolumeProperty("Delivered Forging Volume", blank.StockVolume)
            {
                Equation = ForgingGeometryEstimate.ExcelName + " + " + ToolingExcessVolume.ExcelName,
                Notes = "This volume includes tooling excess, but does not include thtesting coupon volume or yeild loss"
            };
            YieldLoss = new VolumeProperty("Yield Loss Volume", forgingBlank.YieldLoss)
            {
                //"(Forging Volume Estimate + Tooling Escess + Testing Coupon Volume) * Yield Loss) "
                Equation = "(" + ForgingGeometryEstimate.ExcelName + " + " + ToolingExcessVolume.ExcelName + " + Testing Coupon Volume) * Yield Loss",
                Notes = "Testing Coupon Volume is directly from the user input",
            };
            DieVolume = new VolumeProperty("Die Volume", 3 * (DeliveredForgingVolume.Volume + forgingBlank.TestingCoupon))
            {
                Equation = "3 * (" + ForgingGeometryEstimate.ExcelName + " + " + ToolingExcessVolume.ExcelName + " + Testing Coupon Volume)",
                Source = "Boeing Discussion in Bi-Weekly Review",
                Notes = "Yield loss does not directly determine the size of the die, so it is left out",
            };

            //Mass
            FinishMass = new MassProperty("Finish Mass",
                    Mass.FromKilograms(FinishVolume.CubicMillimeters * density.KilogramsPerCubicMillimeter),
                    FinishVolume.Name + " * " + "Titanium Density");

            ForgingBilletMass = new MassProperty("Forging Billet Mass",
                Mass.FromKilograms((ForgingGeometryEstimate.CubicMillimeters + ToolingExcessVolume.CubicMillimeters +
                                    forgingBlank.TestingCoupon.CubicMillimeters + YieldLoss.CubicMillimeters) *
                                   density.KilogramsPerCubicMillimeter))
            {
                //(Forging Geometry Estimate + Tooling Excess Volume + Testing Coupon Volume + Yield Loss Volume) * Titanium Density
                Equation =
                    "(" + ForgingGeometryEstimate.ExcelName + " + " + ToolingExcessVolume.ExcelName +
                    " + Testing Coupon Volume + " + YieldLoss.ExcelName + ") * Titanium Density"
            };

            //The deliverd forging mass is not used for any calculations, but may be a useful property to include??    
            DeliveredForgingMass = new MassProperty("Delivered Forging Mass",
                Mass.FromKilograms(DeliveredForgingVolume.CubicMillimeters * density.KilogramsPerCubicMillimeter),
                DeliveredForgingVolume.Name + " * " + "Titanium Density");

            #endregion
        }


        #region Geometries
        public VolumeProperty FinishVolume { get; }

        public MassProperty FinishMass { get; }

        public VolumeProperty ForgingGeometryEstimate { get; }

        public VolumeProperty ToolingExcessVolume { get; }

        public VolumeProperty DeliveredForgingVolume { get; }

        public MassProperty DeliveredForgingMass { get; }

        public VolumeProperty YieldLoss { get; }

        public MassProperty ForgingBilletMass { get; }

        #endregion

        #region Parameters for Forging Model & Cost Calculation of Forging

        [Display(Name = "Cost Multiplier")]
        [Source("Kevin Slattery (Boeing)")]
        [Notes("Tiers based on finish mass")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        public Multiplier CostMultiplier
             =>
                FinishMass.Mass <= _inputs.Forging.FinishMassTier1Max
                    ? _inputs.Forging.Tier1Multiplier
                    : (FinishMass.Mass <= _inputs.Forging.FinishMassTier2Max
            ? _inputs.Forging.Tier2Multiplier : _inputs.Forging.Tier3Multiplier);

        [Display(Name = "Cost Exponent")]
        [Source("Kevin Slattery (Boeing)")]
        [Notes("Tiers based on finish mass")]
        [OutputUnitType(KatanaUnitType.UserInput)]
        public Multiplier CostExponent
            =>
                FinishMass.Mass <= _inputs.Forging.FinishMassTier1Max
                    ? _inputs.Forging.ExponentTier1
                    : (FinishMass.Mass <= _inputs.Forging.FinishMassTier2Max
            ? _inputs.Forging.ExponentTier2 : _inputs.Forging.ExponentTier3);

        [Display(Name = "Forging Cost")]
        [Equation("Cost Multiplier * (Forging Billet Mass ^ Cost Exponent)")]
        [Source("Kevin Slattery (Boeing)")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost ForgingCost => CalculateForgingCost(CostMultiplier, CostExponent);

        #endregion

        #region Custom Forging Die Cost
        public VolumeProperty DieVolume { get; }

        [Display(Name = "Die Material Cost")]
        [Equation("Die Volume * Steel Density * Steel Price")]
        [Source("Boeing Discussion in Bi-Weekly Review")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost CostOfDieMaterial => Cost.FromDollars(DieVolume.CubicMeters * _inputs.General.SteelDensity.KilogramsPerCubicMeter * _inputs.General.SteelPrice.DollarsPerKilogram); //Named weird to avoid showing up in cost model view in UI

        [Display(Name = "Die Machining Cost")]
        [Equation("Die Steel Machining Cost Rate * (Die Volume - Finish Volume) / (60 * Die Steel Machining MRR)")]
        [Source("Boeing Discussion in Bi-Weekly Review")]
        [Notes("Since this machining is usually done by the forging house, a simple machining equation is used")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost CostOfDieMachining => Cost.FromDollars(_inputs.Machining.ShopCostRate.DollarsPerHour * (DieVolume.CubicMeters - FinishVolume.CubicMeters)
            / _inputs.Machining.DieSteelMachiningMRR.CubicMetersPerHour); // $/hr * (m^3) * hr/m^3

        [Display(Name = "Die Cost")]
        [Equation("Die Material Cost + Die Machining Cost")]
        [Source("Boeing Discussion in Bi-Weekly Review")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost CostofDie => CostOfDieMaterial + CostOfDieMachining;

        [Display(Name = "Amortized Die Cost")]
        [Equation("Die Cost / Die Life Number Of Uses")]
        [OutputUnitType(KatanaUnitType.CalculatedCost)]
        public Cost AmortizedDieCost => Cost.FromDollars(CostofDie.Dollars / _inputs.Forging.DieLife.Unitless);

        #endregion

        [Display(Name = "Total Time")]
        [OutputUnitType(KatanaUnitType.CalculatedTime)]
        [Notes("NO TIMES ASSOCIATED WITH BLANKS")]
        public Duration TotalTime => Duration.Zero;

        [Display(Name = "Total Cost")]
        [Equation("Forging Cost + Amortized Die Cost")]
        [Notes("COST RETURNED IS AN ESTIMATE")]
        [OutputUnitType(KatanaUnitType.TotalCost)]
        public Cost TotalCost => ForgingCost + AmortizedDieCost;

        private Cost CalculateForgingCost(Multiplier multiplier, Multiplier exponent)
        {
            return Cost.FromDollars(multiplier.Value * Math.Pow(ForgingBilletMass.Mass.Pounds, exponent.Value));
        }
    }
}

