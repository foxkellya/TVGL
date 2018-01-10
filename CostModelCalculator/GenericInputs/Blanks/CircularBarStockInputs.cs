using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using UnitsNet;

namespace GenericInputs
{
    [DataContract]
    public class CircularBarStockInputs : Inputs
    {
        [Display(Name = "Circular Bar Stock Titanium Price")]
        [DataMember]
        public CostPerMass MaterialPrice { get; set; } = CostPerMass.FromDollarsPerPound(43); //Source: largest sizes from https://www.titaniumjoe.com/index.cfm/products/tubing/
    }
}
