using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using UnitsNet;

namespace GenericInputs
{
    [DataContract]
    public class GeneralInputs : Inputs
    {
        #region Material Density & Price
        [Required]
        [Display(Name = "Titanium Density")]
        [DataMember]
        public Density TitaniumDensity { get; set; } = Density.FromKilogramsPerCubicMeter(4420); //Constant

        [Required]
        [Display(Name = "Steel Density")]
        [DataMember]
        public Density SteelDensity { get; set; } = Density.FromKilogramsPerCubicMeter(8050); //Constant

        [Required]
        [Display(Name = "Steel Price")]
        [DataMember]
        public CostPerMass SteelPrice { get; set; } = CostPerMass.FromDollarsPerPound(0.15); //Source: https://www.quandl.com/collections/markets/industrial-metals

        #endregion

        #region Operations Inputs

        [Required]
        [Display(Name = "General Overhead")]
        [DataMember]
        public Multiplier Overhead { get; set; } = Multiplier.FromUnitless(0.50); //Guess

        [Required]
        [Display(Name = "Labor Rate")]
        [DataMember]
        public CostRate LaborRate { get; set; } = CostRate.FromDollarsPerHour(20.00); //Guess

        [Required]
        [Display(Name = "Average Annual Volume")]
        [DataMember]
        public Multiplier AvgAnnualVol { get; set; } = Multiplier.FromUnitless(10000); //Guess

        [Required]
        [Display(Name = "Work Hours Per Year")]
        [DataMember]
        public Multiplier HoursPerYear { get; set; } = Multiplier.FromUnitless(2080); //Work hours per year assuming 1, 8 hour shift

        #endregion
     
        [Required]
        [Display(Name = "Standard Machining Offset")]
        [DataMember]
        public Length MachiningOffset { get; set; } = Length.FromMillimeters(.1); //Value from Milling in http://www2.mae.ufl.edu/designlab/Lab%20Assignments/EML2322L-Tolerances.pdf

    }
}
