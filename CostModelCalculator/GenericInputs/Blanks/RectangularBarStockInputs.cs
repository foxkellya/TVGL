using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using UnitsNet;

namespace GenericInputs
{
    [DataContract]
    public class RectangularBarStockInputs : Inputs
    {

        [Required]
        [Display(
        Name = "Rectangular Bar Stock Titanium Price")]
        [DataMember]
        public CostPerMass MaterialPrice { get; set; } = CostPerMass.FromDollarsPerPound(30); //Guess

        [Required]
        [Display(   Name = "Minimum Allowed Dimension")]
        [DataMember]
        public Length MinLength { get; set; } = Length.FromInches(0.125); //Same as plate
    }
}
