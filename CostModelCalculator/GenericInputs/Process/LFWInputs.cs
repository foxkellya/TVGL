using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using UnitsNet;
using UnitsNet.Units;

namespace GenericInputs
{
    [DataContract]
    public class LFWMachineSize
    {
        [DataMember]
        public string MachineName { get; set; }

        [DataMember]
        public Cost MachineCost { get; set; }

        [DataMember]
        public Duration MachineLife { get; set; }

        [DataMember]
        public Area MachineUpperWeldLimit { get; set; }

        [DataMember]
        public Area MachineLowerWeldLimit { get; set; }

        [DataMember]
        public Length MachineFootprintLength { get; set; }

        [DataMember]
        public Length MachineFootprintWidth { get; set; }

        public LFWMachineSize()
        {
        }

        public LFWMachineSize(string machineName, Cost machineCost, Duration machineLife, Area machineUpperWeldLimit, Area machineLowerWeldLimit,
            Length machineFootprintLength, Length machineFootprintWidth)
        {
            MachineName = machineName;
            MachineCost = machineCost;
            MachineLife = machineLife;
            MachineUpperWeldLimit = machineUpperWeldLimit;
            MachineLowerWeldLimit = machineLowerWeldLimit;
            MachineFootprintLength = machineFootprintLength;
            MachineFootprintWidth = machineFootprintWidth;
        }
    }

    [DataContract]
    public class LFWInputs : Inputs
    {
        #region Constraints

        [Required]
        [Display(Name = "Maximum Weld Area")]
        [DataMember]
        public Area MaxWeldArea { get; set; } = Area.FromSquareMillimeters(10000); // Source: http://machinedesign.com/archive/world-s-largest-linear-friction-welding-machine-debuts

        [Required]
        [Display(Name = "Minimum Stock Volume")]
        [DataMember]
        public Volume MinStockVolume { get; set; } = Volume.FromCubicCentimeters(10); // Guess

        [Required]
        [Display(Name = "Minimum Required Offset")]
        [DataMember]
        public Length LFWOffset { get; set; } = Length.FromInches(0.125); //Guess

        [Required]
        [Display(Name = "LFW Welding Time (Should be 3-10 sec)")]
        [DataMember]
        public Duration WeldTime { get; set; } = Duration.FromSeconds(10); //Source: Linear Friction Welding of Ti-6Al-4V : Processing , Microstructure , and Mechanical-Property Inter-Relationships, 2005

        #endregion  

        #region Machine Inputs   

        [Required]
        [Display(Name = "Machine Cost")]
        [DataMember]
        public Cost MachineCost { get; set; } = Cost.FromMillions(3); //Guess

        [Required]
        [Display(Name = "Machine Life")]
        [CostModelViewUnit(DurationUnit.Year)]
        [DataMember]
        public Duration MachineLife { get; set; } = Duration.FromYears(10); //Guess

        [Required]
        [Display(Name = "Machine Availability")]
        [DataMember]
        public Ratio MachineAvailability { get; set; } = Ratio.FromPercent(90); //Guess
        #endregion
    }
}
