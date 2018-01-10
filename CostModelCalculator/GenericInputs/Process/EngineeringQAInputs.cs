using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using UnitsNet;

namespace GenericInputs
{
    [DataContract]
    public class EngineeringQAInputs : Inputs
    {
        #region Properties
        
        [Required]
        [Display(Name = "Engineering Quality Assurance Rate")]
        [DataMember]
        public CostRate EngQARate { get; set; } = CostRate.FromDollarsPerHour(50); //Guess 

        [Required]
        [Display(Name = "Non-Destructive Testing Rate")]
        [DataMember]
        public MassFlow NDTRate { get; set; } = MassFlow.FromKilogramsPerHour(50); //Guess 

        [Required]
        [Display(Name = "Engineering Quality Assurance Time")]
        [DataMember]
        public Duration EngQATime { get; set; } = Duration.FromMinutes(60); //Guess

        #endregion

    }
}
