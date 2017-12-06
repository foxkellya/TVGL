using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using UnitsNet;

namespace GenericInputs
{
    [DataContract]
    public class ForgingInputs : Inputs
    {
        //Literature Sources
        //[1] R. Nunes, I. Abbas, L. L. Algminas, T. Atlan, H. Alsworth, and D. Ashok, ASM Handbook: Forming and Forging, vol. 14. ASM International, 1988.
        //[2] Forging Industry Association, “Tolerances for Impression Dies.” [Online]. Available: https://www.forging.org/forging/design/9-appendix-a.html.

        #region Geometric Estimate Inputs

        [Required]
        [DataMember]
        [Display(Name = "Number of OBB Directions to Consider (1-3 with smallest directions considered first)")]
        public Multiplier NumberOfOBBDirectionsToConsider { get; set; } = Multiplier.FromUnitless(1); //Value is for geometry. Has no real forging comparison.

        [Required]
        [DataMember]
        [Display(Name = "Draft Angle")]
        public Angle DraftAngle { get; set; } = Angle.FromDegrees(3); //Value is an estimate from [1]

        [Required]
        [DataMember]
        [Display(Name = "Top Cover")]
        public Length TopCover { get; set; } = Length.FromInches(0.5); //Value is an guess. Need to update with more info from [2]

        [Required]
        [DataMember]
        [Display(Name = "Side Cover")]
        public Length SideCover { get; set; } = Length.FromInches(0.5); //Value is an guess. Need to update with more info from [2]

        [Required]
        [Display(Name = "Number of Slices (Step Size = length along direction / Number Of Slices)")]
        [DataMember]
        public Multiplier NumberOfSlices { get; set; } = Multiplier.FromUnitless(50); //Value is for geometry. Has no real forging comparison.

        [Required]
        [DataMember]
        [Display(Name = "Testing Coupon Volume (Added to forging volume estimate)")]
        public Volume TestingCouponsVolume { get; set; } = Volume.FromCubicInches(8); //Value is an estimate from testing standards

        [Required]
        [DataMember]
        [Display(Name = "Tooling Excess (Added to forging volume estimate) [decimal percentage]")]
        public Multiplier ToolingExcess { get; set; } = Multiplier.FromUnitless(0.2); //Value is a guess

        [Required]
        [DataMember]
        [Display(Name = "Yield Loss (Added to forging volume estimate) [decimal percentage]")]
        public Multiplier YieldLoss { get; set; } = Multiplier.FromUnitless(0.20); //Value is an estimate from [1]

        [Required]
        [DataMember]
        [Display(Name = "Min Flash and Gutter Width (Used to remove small holes on web) [decimal percentage of forging depth]")]
        public Multiplier MinFlashAndGutterWidth { get; set; } = Multiplier.FromUnitless(0.15); //Value is a guess

        #endregion

        [Required]
        [DataMember]
        [Display(Name = "Cost Per Mass")]
        public CostPerMass MaterialPrice { get; set; } = CostPerMass.FromDollarsPerPound(40); //Value is a guess

        [Required]
        [DataMember]
        [Display(Name = "Die Cost Percentage [decimal percentage, e.g. 0.15)")]
        public Multiplier DieCostPercentage { get; set; } = Multiplier.FromUnitless(0.15); //Value is a guess
    }
}
