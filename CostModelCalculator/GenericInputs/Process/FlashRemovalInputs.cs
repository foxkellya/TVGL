using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using UnitsNet;

namespace GenericInputs
{
    [DataContract]
    public class FlashRemovalInputs : Inputs
    {
        [Required]
        [DataMember]
        [Display(Name = "Feed Rate")]
        public Speed FlashRemovalFeedRate{ get; set; } = Speed.FromCentimetersPerSecond(3.8); //Guess ~ 1.5 in/sec
    }
}
