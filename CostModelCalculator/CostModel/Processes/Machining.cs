using System.Runtime.Serialization;
using BlankFactory.Processes;
using KatanaObjects.CostModels;
using UnitsNet;

namespace KatanaObjects.Processes
{
    [DataContract]
    public class Machining : IProcess
    {
        [DataMember]
        public ProcessType Type { get; set; }

        //Can not be serialized. Use SetCostModel() when deserializing.
        public ICostModel CostModel { get; set; }

        public Cost TotalCost => CostModel.TotalCost;

        [DataMember]
        public int IndexAllProcesses { get; set; }

        [DataMember]
        public int IndexByProcessType { get; set; }

        [DataMember]
        public Volume FinishVolume { get; set; }

        [DataMember]
        public Area SurfaceArea { get; set; }

        [DataMember]
        public Volume StockVolume { get; set; }

        [DataMember]
        public bool RoughCutAll { get; set; }

        private Machining(Volume finishVolume, Area surfaceArea, Volume stockVolume, bool roughCutAll = false)
        {
            FinishVolume = finishVolume;
            SurfaceArea = surfaceArea;
            StockVolume = stockVolume;
            RoughCutAll = roughCutAll;
            Type = ProcessType.Machining;
        }

        public static Machining Create(ICostModelFactory costFactory, Volume finishVolume, Area surfaceArea, Volume stockVolume,
            bool roughCutAll = false)
        {
            //Create the machining operation
            var machining = new Machining(finishVolume, surfaceArea, stockVolume, roughCutAll);
            //Set the cost model
            machining.SetCostModel(costFactory);

            return machining;
        }

        public void SetCostModel(ICostModelFactory costFactory)
        {
            CostModel = costFactory.MachiningCostModel(this);
        }
    }
}

