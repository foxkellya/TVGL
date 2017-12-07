using System.Runtime.Serialization;
using KatanaObjects.BaseClasses;

namespace KatanaObjects.Blanks
{
    [DataContract]
    public class WaterJetPlateBlank : Blank
    {
        internal WaterJetPlateBlank(SubVolume subVolume) : base(subVolume)
        {
            //For certain blank types, the dimensions are linked together and should be set altogether.
            //Checking isFeasible first, will set the waterjet shape if it is not already set
            IsFeasible = SubVolume.WaterjetPlateIsFeasible;
            
            //If not feasible, stop making blank
            if (!IsFeasible) return;

            Type = BlankType.WaterJetPlate;
            StockVolume = SubVolume.WaterjetVolume;
            WasteVolume = StockVolume - SubVolume.SolidVolume;
            FinishVolume = SubVolume.SolidVolume;  
            CrossSections = SubVolume.WaterjetCrossSections;
            ShapeOnCuttingPlanePreMachining = SubVolume.WaterjetShapeOnCuttingPlane;
            PerimeterOnPlane = SubVolume.WaterjetPerimeterOnPlane;
            DistanceAlongJoinDirection = SubVolume.WaterjetDistanceAlongJoinDirection;
            AreaIsCircular = false;
            CrossSectionBuildDirection = SubVolume.WaterjetCrossSectionBuildDirection;
            CrossSectionBuildDistance = SubVolume.WaterjetDepth;
        }
    }
}
