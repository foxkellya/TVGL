using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using UnitsNet;
using UnitsNet.Units;

namespace GenericInputs
{
    [DataContract]
    public class WireFeedInputs : Inputs
    {
        #region Properties

        [Required]
        [Display(Name = "Minimum Printed Volume")]
        [DataMember]
        public Volume MinFinishVolume { get; set; } = Volume.FromCubicCentimeters(10); // Geometry limit. Don't want to consider features smaller than this

        [Required]
        [Display(Name = "Wire Accuracy")]
        [DataMember]
        public Length WireAccuracy { get; set; } = Length.FromMillimeters(2); // One half the diameter from largest size http://www.sciaky.com/additive-manufacturing/wire-am-vs-powder-am

        [Required]
        [Display(Name = "Additive Deposition Rate")]
        [DataMember]
        public MassFlow AdditiveDepositionRate { get; set; } = MassFlow.FromKilogramsPerHour(5.0); //Assuming midrange value from  http://www.sciaky.com/additive-manufacturing/wire-am-vs-powder-am

        [Required]
        [Display(Name = "Machine Cost")]
        [DataMember]
        public Cost MachineCost { get; set; } = Cost.FromMillions(2.0); //Guess

        [Required]
        [Display(Name = "Machine Life")]
        [CostModelViewUnit(DurationUnit.Year)]
        [DataMember]
        public Duration MachineLife { get; set; } = Duration.FromYears(10); //Guess

        [Required]
        [Display(Name = "Machine Availability")]
        [DataMember]
        public Ratio MachineAvailability { get; set; } = Ratio.FromPercent(80); //Guess

        #endregion
    }
}
