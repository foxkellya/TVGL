using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using UnitsNet;
using UnitsNet.Units;

namespace GenericInputs
{
    [DataContract]
    public class MachiningInputs : Inputs
    {
        #region Properties
        [Required]
        [Display(Name = "Roughing Material Removal Rate (MRR)")]
        [DataMember]
        //Source: http://americanmachinist.com/cutting-tools/carbide-cutters-tame-titanium-machining
        public MaterialRemovalRate RoughingMRR { get; set; } = MaterialRemovalRate.FromCubicInchesPerMinute(3); 

        [Required]
        [Display(Name = "Finish Material Removal Rate (MRR)")]
        [DataMember]
        //Source: http://americanmachinist.com/cutting-tools/carbide-cutters-tame-titanium-machining
        public MaterialRemovalRate FinishMRR { get; set; } = MaterialRemovalRate.FromCubicInchesPerMinute(1.5);

        [Required]
        [Display(Name = "Roughing Thickness")]
        [DataMember]
        public Length RoughingAmountToLeave { get; set; } = Length.FromInches(1); //Guess

        [Required]
        [Display(Name = "Shop Cost Rate")]
        [DataMember]
        public CostRate ShopCostRate { get; set; } = CostRate.FromDollarsPerHour(156); //[dollars/hour]

        [Required]
        [Display(Name = "Titanium Chip Reclaim Value")]
        [DataMember]
        public CostPerMass TitaniumChipReclaimValue { get; set; } = CostPerMass.FromDollarsPerPound(7.4); //Source: https://www.metalprices.com/metal/titanium/titanium-ingot-6al-4v-rotterdam

        [Required]
        [Display(Name = "Tool Change Time")]
        [CostModelViewUnit(DurationUnit.Minute)]
        [DataMember]
        public Duration ToolChangeTime { get; set; } = Duration.FromMinutes(5); //Guess

        [Required]
        [Display(Name = "Cutting Tool Life")]
        [CostModelViewUnit(DurationUnit.Minute)]
        [DataMember]
        public Duration ToolLife { get; set; } = Duration.FromMinutes(120); //Guess

        [Required]
        [Display(Name = "Cutting Tool Cost")]
        [DataMember]
        public Cost ToolCost { get; set; } = Cost.FromDollars(200); //Guess

        [Required]
        [Display(Name = "Fixture Life")]
        [CostModelViewUnit(DurationUnit.Hour)]
        [DataMember]
        public Duration FixtureLife { get; set; } = Duration.FromHours(10000); //Guess

        [Required]
        [Display(Name = "Fixture Cost")]
        [DataMember]
        public Cost FixtureCost { get; set; } = Cost.FromDollars(10000); //Guess

        [Required]
        [Display(Name = "Machine Capital Cost")]
        [DataMember]
        public Cost MachineCost { get; set; } = Cost.FromMillions(1);  //Guess

        [Required]
        [Display(Name = "Machine Life")]
        [CostModelViewUnit(DurationUnit.Year)]
        [DataMember]
        public Duration MachineLife { get; set; } = Duration.FromYears(20);  //Guess

        [Required]
        [Display(Name = "Machine Availability")]
        [DataMember]
        public Ratio MachineAvailability { get; set; } = Ratio.FromPercent(80); //Guess [percent value]

        [Required]
        [Display(Name = "Consumables Rate")]
        [DataMember]
        public Multiplier ConsumablesRate { get; set; } = Multiplier.FromUnitless(0.15);

        [Required]
        [DataMember]
        [Display(Name = "Die Steel Machining MRR")]
        [Source("More conservative value from http://todaysmachiningworld.com/magazine/how-it-works-rapid-material-removal/ for a similar material")]
        public MaterialRemovalRate DieSteelMachiningMRR { get; set; } = MaterialRemovalRate.FromCubicInchesPerMinute(25);

        #endregion
    }
}
