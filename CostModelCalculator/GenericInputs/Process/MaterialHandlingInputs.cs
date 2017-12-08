using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using UnitsNet;

namespace GenericInputs
{
    [DataContract]
    public class MaterialHandlingInputs : Inputs
    {
       #region Material Handling Methods Inputs

        [Required]
        [DataMember]
        [Display(Name = "Custom Cart Upper Mass Limit ")]
        public Mass CartMassLimit { get; set; } = Mass.FromPounds(35); //Guess

        [Required]
        [DataMember]
        [Display(Name = "Custom Cart Speed")]
        public Speed CartSpeed { get; set; } = Speed.FromFeetPerSecond(0.42); //Guess

        [Required]
        [DataMember]
        [Display(Name = "Cart Load and Unload Time")]
        public Duration CartLoadUnload { get; set; } = Duration.FromMinutes(2); //Guess

        [DataMember]
        [Display(Name = "Forklift Upper Mass Limit ")]
        public Mass ForkliftMassLimit { get; set; } = Mass.FromPounds(3000); //Guess

        [Required]
        [DataMember]
        [Display(Name = "Forklift Speed")]
        public Speed ForkliftSpeed { get; set; } = Speed.FromFeetPerSecond(1.6); //Guess

        [Required]
        [DataMember]
        [Display(Name = "Forklift Load and Unload Time")]
        public Duration ForkliftLoadUnload { get; set; } = Duration.FromMinutes(4); //Guess

        [Required]
        [DataMember]
        [Display(Name = "Overhead Crane Speed")]
        public Speed CraneSpeed { get; set; } = Speed.FromFeetPerSecond(0.25); //Guess

        [Required]
        [DataMember]
        [Display(Name = "Crane Load and Unload Time")]
        public Duration CraneLoadUnload { get; set; } = Duration.FromMinutes(7); //Guess

        #endregion
    }
}
