using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using StarMathLib;
using UnitsNet;

namespace GenericInputs
{
    public class TubeSize : Size
    {

        public Length OuterDiameter;

        public Length InnerDiameter;

        public Area Area;

        public bool Invalid;

        public TubeSize(Length outerDiameter, Length innerDiameter)
        {
            if (!innerDiameter.Millimeters.IsLessThanNonNegligible(outerDiameter.Millimeters)) Invalid = true;
            if (outerDiameter.Millimeters.IsNegligible() || innerDiameter.Millimeters.IsNegligible()) Invalid = true;
            if (!innerDiameter.Millimeters.IsLessThanNonNegligible(outerDiameter.Millimeters)) Invalid = true;
            OuterDiameter = outerDiameter;
            InnerDiameter = innerDiameter;
            Area = (Math.PI / 4) * (OuterDiameter * OuterDiameter - InnerDiameter * InnerDiameter);
            Invalid = false;
        }
    }

    [DataContract]
    public class HollowTubeInputs : Inputs
    {
        [Required]
        [DataMember]
        [Display(Name = "Maximum Outer Diameter")]
        //Tube sizes ordered based on outer diameter then inner diameter
        public Length OuterDiameterMax { get; set; } = Length.FromInches(5.5); //Source: largest size from https://www.titaniumjoe.com/index.cfm/products/tubing/

        [Required]
        [DataMember]
        [Display(Name = "Minimum Inner Diameter")]
        public Length InnerDiameterMin { get; set; } = Length.FromInches(0.055); //Source: smallest size from https://www.titaniumjoe.com/index.cfm/products/tubing/

        [Required]
        [DataMember]
        [Display(Name = "Maximum Outer Diameter/Thickness Ratio")]
        public Ratio MaximumOuterDiameterToThicknessRatio { get; set; }  = Ratio.FromDecimalFractions(70); //Source: largest size from https://www.titaniumjoe.com/index.cfm/products/tubing/

        [Required]
        [DataMember]
        [Display(Name = "Price Per Mass")]
        public CostPerMass PricePerMass { get; set; } =CostPerMass.FromDollarsPerPound(54); //Source: largest size from https://www.titaniumjoe.com/index.cfm/products/tubing/
    }
}
