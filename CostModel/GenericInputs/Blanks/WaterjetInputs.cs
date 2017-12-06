using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using StarMathLib;
using UnitsNet;

namespace GenericInputs
{
    [DataContract]
    public class WaterjetInputs : Inputs
    {
        #region Properties

        [Required]
        [DataMember]
        [Display(Name = "Base Price for Plate (See multipliers for each available size)")]
        public CostPerMass PricePerMass { get; set; } = CostPerMass.FromDollarsPerPound(25); //Guess. Should be less than rect. bar stock

        [Required]
        [Display(Name = "Cutting Offset")]
        [DataMember]
        public Length CuttingOffset { get; set; } = Length.FromMillimeters(1.0); //For abrasive waterjet http://waterjets.org/archive/getting-the-most/tips/waterjet-machining-tolerances/

        [Required]
        [Display(Name = "Thickness Machining Offset")]
        [DataMember]
        public Length ThicknessMachiningOffset { get; set; } = Length.FromMillimeters(1.0); //Guess


        //Serialize the next few lists, since the constructor is not called during startup. Only if defaults are set.
        [Required]
        [DataMember]
        [HideFromViews(true)]
        public List<Length> OrderedWaterjetPlateThicknesses { get; set; }

        [Required]
        [DataMember]
        [HideFromViews(true)]
        public Dictionary<Length, CostPerDistance> CuttingRatesBasedOnThickness { get; set; }

        [Required]
        [DataMember]
        [HideFromViews(true)]
        public Dictionary<Length, Multiplier> PricePerKilogramMultipliers { get; set; }

        public WaterjetInputs()
        {
            #region Set Sizes and Associated Costs
            var waterjetPlateThicknesses = new List<Length>
            {
                WaterjetPlateThickness0,
                WaterjetPlateThickness1,
                WaterjetPlateThickness2,
                WaterjetPlateThickness3,
                WaterjetPlateThickness4,
                WaterjetPlateThickness5,
                WaterjetPlateThickness6,
                WaterjetPlateThickness7,
                WaterjetPlateThickness8,
                WaterjetPlateThickness9,
                WaterjetPlateThickness10,
                WaterjetPlateThickness11,
                WaterjetPlateThickness12,
                WaterjetPlateThickness13,
                WaterjetPlateThickness14,
                WaterjetPlateThickness15,
                WaterjetPlateThickness16,
                WaterjetPlateThickness17,
                WaterjetPlateThickness18,
                WaterjetPlateThickness19,
                WaterjetPlateThickness20,
                WaterjetPlateThickness21,
                WaterjetPlateThickness22,
                WaterjetPlateThickness23,
                WaterjetPlateThickness24,
                WaterjetPlateThickness25,
                WaterjetPlateThickness26,
                WaterjetPlateThickness27,
                WaterjetPlateThickness28,
                WaterjetPlateThickness29,
                WaterjetPlateThickness30
            };

            //Set plate thicknesses
            var cuttingRates = new List<CostPerDistance>
            {
                CuttingRate0,
                CuttingRate1,
                CuttingRate2,
                CuttingRate3,
                CuttingRate4,
                CuttingRate5,
                CuttingRate6,
                CuttingRate7,
                CuttingRate8,
                CuttingRate9,
                CuttingRate10,
                CuttingRate11,
                CuttingRate12,
                CuttingRate13,
                CuttingRate14,
                CuttingRate15,
                CuttingRate16,
                CuttingRate17,
                CuttingRate18,
                CuttingRate19,
                CuttingRate20,
                CuttingRate21,
                CuttingRate22,
                CuttingRate23,
                CuttingRate24,
                CuttingRate25,
                CuttingRate26,
                CuttingRate27,
                CuttingRate28,
                CuttingRate29,
                CuttingRate30
            };

            var pricePerKilogramMultipliers = new List<Multiplier>
            {
                PricePerKilogramMultiplier0,
                PricePerKilogramMultiplier1,
                PricePerKilogramMultiplier2,
                PricePerKilogramMultiplier3,
                PricePerKilogramMultiplier4,
                PricePerKilogramMultiplier5,
                PricePerKilogramMultiplier6,
                PricePerKilogramMultiplier7,
                PricePerKilogramMultiplier8,
                PricePerKilogramMultiplier9,
                PricePerKilogramMultiplier10,
                PricePerKilogramMultiplier11,
                PricePerKilogramMultiplier12,
                PricePerKilogramMultiplier13,
                PricePerKilogramMultiplier14,
                PricePerKilogramMultiplier15,
                PricePerKilogramMultiplier16,
                PricePerKilogramMultiplier17,
                PricePerKilogramMultiplier18,
                PricePerKilogramMultiplier19,
                PricePerKilogramMultiplier20,
                PricePerKilogramMultiplier21,
                PricePerKilogramMultiplier22,
                PricePerKilogramMultiplier23,
                PricePerKilogramMultiplier24,
                PricePerKilogramMultiplier25,
                PricePerKilogramMultiplier26,
                PricePerKilogramMultiplier27,
                PricePerKilogramMultiplier28,
                PricePerKilogramMultiplier29,
                PricePerKilogramMultiplier30
            };
            #endregion

            //Remove 0s and pair costs
            var i = 0;
            var nonZeroThicknesses = new List<Length>();
            CuttingRatesBasedOnThickness = new Dictionary<Length, CostPerDistance>();
            PricePerKilogramMultipliers = new Dictionary<Length, Multiplier>();
            foreach (var thickness in waterjetPlateThicknesses)
            {
                if (!thickness.Millimeters.IsNegligible())
                {
                    nonZeroThicknesses.Add(thickness);
                    CuttingRatesBasedOnThickness.Add(thickness, cuttingRates[i]);
                    PricePerKilogramMultipliers.Add(thickness, pricePerKilogramMultipliers[i]);
                }
                i++;
            }

            //Order thicknesses and then remove 0s (extra, unused variables). 
            var orderedThicknesses = waterjetPlateThicknesses.OrderBy(length => length.Millimeters).ToList();
            OrderedWaterjetPlateThicknesses = new List<Length>();
            foreach (var thickness in orderedThicknesses.Where(thickness => !thickness.Millimeters.IsNegligible()))
            {
                OrderedWaterjetPlateThicknesses.Add(thickness);
            }

        }
        #endregion

        #region Waterjet Plate Available Sizes
        
        //From 0 - 2 inches: incremeneted by 1/8 inch 
        //From 2 - 5 inches:  incremented by 1/4 inch

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness0 { get; set; } = Length.FromInches(0.125);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness1 { get; set; } = Length.FromInches(0.25);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness2 { get; set; } = Length.FromInches(0.375);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness3 { get; set; } = Length.FromInches(0.5);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness4 { get; set; } = Length.FromInches(0.625);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness5 { get; set; } = Length.FromInches(0.75);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness6 { get; set; } = Length.FromInches(0.875);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness7 { get; set; } = Length.FromInches(1.0);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness8 { get; set; } = Length.FromInches(1.125);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness9 { get; set; } = Length.FromInches(1.25);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness10 { get; set; } = Length.FromInches(1.375);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness11 { get; set; } = Length.FromInches(1.5);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness12 { get; set; } = Length.FromInches(1.625);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness13 { get; set; } = Length.FromInches(1.75);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness14 { get; set; } = Length.FromInches(1.865);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness15 { get; set; } = Length.FromInches(2);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness16 { get; set; } = Length.FromInches(2.25);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness17 { get; set; } = Length.FromInches(2.5);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness18 { get; set; } = Length.FromInches(2.75);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness19 { get; set; } = Length.FromInches(3.0);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness20 { get; set; } = Length.FromInches(3.25);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness21 { get; set; } = Length.FromInches(3.5);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness22 { get; set; } = Length.FromInches(3.75);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness23 { get; set; } = Length.FromInches(4);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness24 { get; set; } = Length.FromInches(4.25);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness25 { get; set; } = Length.FromInches(4.5);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness26 { get; set; } = Length.FromInches(4.75);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness27 { get; set; } = Length.FromInches(5.0);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness28 { get; set; }

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness29 { get; set; }

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(0)]
        public Length WaterjetPlateThickness30 { get; set; }
        #endregion

        #region Waterjet Plate Cutting Rates - Based on Thicknesses

        // < 2 inches =  1 USD/inch
        // 2-5 inches =  2 USD/inch
        private static readonly CostPerDistance CostPerDistanceTier1 = CostPerDistance.FromDollarsPerInch(1); //Guess
        private static readonly CostPerDistance CostPerDistanceTier2 = CostPerDistance.FromDollarsPerInch(2); //Guess

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate0 { get; set; } = CostPerDistanceTier1;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate1 { get; set; } = CostPerDistanceTier1;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate2 { get; set; } = CostPerDistanceTier1;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate3 { get; set; } = CostPerDistanceTier1;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate4 { get; set; } = CostPerDistanceTier1;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate5 { get; set; } = CostPerDistanceTier1;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate6 { get; set; } = CostPerDistanceTier1;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate7 { get; set; } = CostPerDistanceTier1;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate8 { get; set; } = CostPerDistanceTier1;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate9 { get; set; } = CostPerDistanceTier1;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate10 { get; set; } = CostPerDistanceTier1;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate11 { get; set; } = CostPerDistanceTier1;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate12 { get; set; } = CostPerDistanceTier1;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate13 { get; set; } = CostPerDistanceTier1;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate14 { get; set; } = CostPerDistanceTier1;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate15 { get; set; } = CostPerDistanceTier1;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate16 { get; set; } = CostPerDistanceTier2;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate17 { get; set; } = CostPerDistanceTier2;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate18 { get; set; } = CostPerDistanceTier2;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate19 { get; set; } = CostPerDistanceTier2;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate20 { get; set; } = CostPerDistanceTier2;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate21 { get; set; } = CostPerDistanceTier2;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate22 { get; set; } = CostPerDistanceTier2;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate23 { get; set; } = CostPerDistanceTier2;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate24 { get; set; } = CostPerDistanceTier2;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate25 { get; set; } = CostPerDistanceTier2;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate26 { get; set; } = CostPerDistanceTier2;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate27 { get; set; } = CostPerDistanceTier2;

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate28 { get; set; }

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate29 { get; set; }

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(1)]
        public CostPerDistance CuttingRate30 { get; set; }
        #endregion

        #region Prices per kilogram

        // < 2 inches =  1 (guess)
        // 2-5 inches =  1.1, which is a 10% increase in price per pound  (guess)

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier0 { get; set; } = Multiplier.FromUnitless(1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier1 { get; set; } = Multiplier.FromUnitless(1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier2 { get; set; } = Multiplier.FromUnitless(1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier3 { get; set; } = Multiplier.FromUnitless(1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier4 { get; set; } = Multiplier.FromUnitless(1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier5 { get; set; } = Multiplier.FromUnitless(1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier6 { get; set; } = Multiplier.FromUnitless(1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier7 { get; set; } = Multiplier.FromUnitless(1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier8 { get; set; } = Multiplier.FromUnitless(1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier9 { get; set; } = Multiplier.FromUnitless(1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier10 { get; set; } = Multiplier.FromUnitless(1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier11 { get; set; } = Multiplier.FromUnitless(1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier12 { get; set; } = Multiplier.FromUnitless(1);


        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier13 { get; set; } = Multiplier.FromUnitless(1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier14 { get; set; } = Multiplier.FromUnitless(1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier15 { get; set; } = Multiplier.FromUnitless(1);


        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier16 { get; set; } = Multiplier.FromUnitless(1.1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier17 { get; set; } = Multiplier.FromUnitless(1.1);


        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier18 { get; set; } = Multiplier.FromUnitless(1.1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier19 { get; set; } = Multiplier.FromUnitless(1.1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier20 { get; set; } = Multiplier.FromUnitless(1.1);


        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier21 { get; set; } = Multiplier.FromUnitless(1.1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier22 { get; set; } = Multiplier.FromUnitless(1.1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier23 { get; set; } = Multiplier.FromUnitless(1.1);


        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier24 { get; set; } = Multiplier.FromUnitless(1.1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier25 { get; set; } = Multiplier.FromUnitless(1.1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier26 { get; set; } = Multiplier.FromUnitless(1.1);


        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier27 { get; set; } = Multiplier.FromUnitless(1.1);

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier28 { get; set; } 

        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier29 { get; set; } 


        [Required]
        [DataMember]
        [AvailableSize(true)]
        [SizeDimensionTypeIndex(2)]
        public Multiplier PricePerKilogramMultiplier30 { get; set; } 
        #endregion
    }
}
