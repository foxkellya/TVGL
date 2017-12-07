using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using UnitsNet;

namespace GenericInputs
{
    [DataContract]
    public class WireFeedstockInputs : Inputs
    {
        [Required]
        [DataMember]
        [Display(Name = "Wire Price Per Mass")]
        public CostPerMass WirePrice { get; set; } = CostPerMass.FromDollarsPerPound(44); //Source: Lowest price from http://www.sciaky.com/additive-manufacturing/wire-am-vs-powder-am

        [Required]
        [Display(Name = "Number of Slices (Step Size = length along direction / Number Of Slices)")]
        [DataMember]
        public Multiplier NumberOfSlices { get; set; } = Multiplier.FromUnitless(50); //For geometry. Value has nothing to do real wirefeed info. 
    }
}
