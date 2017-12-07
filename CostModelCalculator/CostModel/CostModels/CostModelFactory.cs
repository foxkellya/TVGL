using System;
using BlankFactory.CostModels;
using GenericCostModel.Blanks;
using GenericInputs;
using KatanaObjects.BaseClasses;
using KatanaObjects.Blanks;
using KatanaObjects.Processes;

namespace KatanaObjects.CostModels
{
    public class CostModelFactory : ICostModelFactory
    {
        public SearchInputs Inputs { get; set; }

        public CostModelFactory(SearchInputs inputs)
        {
            Inputs = inputs;
        }


        public ICostModel MachiningCostModel(Machining machining, bool isEstimate)
        {
            return new MachiningCostModel(Inputs, machining.FinishVolume, machining.SurfaceArea, machining.StockVolume, isEstimate);
        }

        public ICostModel MaterialCostModel(Blank blank)
        {
            switch (blank.Type)
            {
                case BlankType.WaterJetPlate:
                    return new WaterJetPlateCostModel(Inputs, blank);

                case BlankType.RectangularBarStock:
                    return new RectangularBarStockCostModel(Inputs, blank);

                case BlankType.CircularBarStock:
                    return new CircularBarStockCostModel(Inputs, blank);

                case BlankType.ClosedDieForging:
                    return new ForgingCostModel(Inputs, blank);

                case BlankType.NearNetAdditive:
                    var nearNetBlank = (NearNetPrintedShapeBlank)blank;
                    return new NearNetPrintedCostModel(Inputs, blank.StockVolume, nearNetBlank.SubstrateVolume);

                case BlankType.HollowTube:
                    //Note: blank.SubVolume.HollowTubeSize.OuterDiameter is used instead of sending tube size
                    //because tube size is class created in the inputs (Generic or Boeing) namespace.  
                    return new HollowTubeCostModel(Inputs, blank);
            }
            return null;
        }
    }
}
