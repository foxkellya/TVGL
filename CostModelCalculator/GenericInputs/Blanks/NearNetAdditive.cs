using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using UnitsNet;

namespace GenericInputs
{
    [DataContract]
    public class NearNetAdditiveInputs : Inputs
    {
        [Required]
        [Display(Name = "Substrate Thickness (For Near-Net Printed Shape)")]
        [DataMember]
        public Length SubstrateThickness { get; set; } = Length.FromInches(.25); //Geometry guess


        [Required]
        [Display(Name = "Substrate Offset (For Near-Net Printed Shape)")]
        [DataMember]
        public Length SubstrateOffset { get; set; } = Length.FromInches(.25); //Geometry guess

    }
}
