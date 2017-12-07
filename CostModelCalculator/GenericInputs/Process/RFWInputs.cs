using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using UnitsNet;
using UnitsNet.Units;

namespace GenericInputs
{
    [DataContract]
    public class RFWInputs : Inputs
    {
        #region Constraints

        [Required]
        [Display(Name = "Maximum Weld Area")]
        [DataMember]
        public Area MaxWeldArea { get; set; } = Area.FromSquareInches(100); //Guess

        [Required]
        [Display(Name = "Minimum Stock Volume")]
        [DataMember]
        public Volume MinStockVolume { get; set; } = Volume.FromCubicCentimeters(10); // Guess


        [Required]
        [Display(Name = "RFW Welding Time (Should be 3-10 sec if similar to LFW)")]
        [DataMember]
        public Duration WeldTime { get; set; } = Duration.FromSeconds(10); //Source: Linear Friction Welding of Ti-6Al-4V : Processing , Microstructure , and Mechanical-Property Inter-Relationships, 2005


        [Required]
        [Display(Name = "Tooling Cost")]
        [DataMember]
        public Cost FixtureCost { get; set; } = Cost.FromDollars(10000); //Guess
        #endregion


        #region Machine Inputs
        [Required]
        [Display(Name = "Machine Cost")]
        [DataMember]
        public Cost MachineCost { get; set; } = Cost.FromMillions(2);  //Guess

        [Required]
        [Display(Name = "Machine Life")]
        [CostModelViewUnit(DurationUnit.Year)]
        [DataMember]
        public Duration MachineLife { get; set; } = Duration.FromYears(10);  //Guess

        [Required]
        [Display(Name = "Machine Availability")]
        [DataMember]
        public Ratio MachineAvailability { get; set; } = Ratio.FromPercent(90); //Guess
        #endregion
    }
}
