using System.Runtime.Serialization;
using GenericInputs;
using KatanaObjects.CostModels;
using PropertyTools.DataAnnotations;
using UnitsNet;

namespace BlankFactory.Processes 
{
    public enum ProcessType
    {
        [Description("Joining Operation")]
        Joining,

        [Description("Material Handling")]
        MaterialHandling,

        [Description("Flash Removal")]
        FlashRemoval,

        [Description("Machining")]
        Machining,

        [Description("Dimensional Testing")]
        DimensionalTesting,

        [Description("Engineering Quality Assurance")]
        EngQA,

        [Description("Non-Destructive Testing")]
        NDT,

        [Description("Heat Treat")]
        HeatTreat
    }

    [DataContract]
    public abstract class Process : ICostModel
    {
        [DataMember]
        public ProcessType Type { get; private set; }

        public int ProcessNumber { get; set; }

        public Cost TotalCost { get; set; }

        public SearchInputs Inputs { get; set; }
    }
}
