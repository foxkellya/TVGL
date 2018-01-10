using System.Runtime.Serialization;
using KatanaObjects.BaseClasses;

namespace KatanaObjects.Blanks
{
    [DataContract]
    public class CircularBarStockBlank : Blank
    {
        internal CircularBarStockBlank(SubVolume subVolume) : base(subVolume)
        {
            //First, populate the values in the subvolume
            subVolume.SetCircularBarStockDimensions();

            //Second, use those values to set the blank values
            Type = BlankType.CircularBarStock;
            StockVolume = SubVolume.CircularBarStockVolume;
            WasteVolume = StockVolume - SubVolume.SolidVolume;
            FinishVolume = SubVolume.SolidVolume;
            AreaOnCuttingPlane = SubVolume.CircularBarStockArea;
            //ShapeOnPlanePreMachining = SubVolume.CircularBarStockPath;
            //PerimeterOnPlane = Math.PI * SubVolume.CircularBarStockDiameter;
            IsFeasible = SubVolume.CircularBarIsFeasible;
            CrossSections = SubVolume.CircularBarStockCrossSections;
            AreaIsCircular = true;
            CrossSectionBuildDirection = SubVolume.CircularBarStockCrossSectionBuildDirection;
            CrossSectionBuildDistance = SubVolume.CircularBarStockDepth;
            //StockMaterialSolid = SubVolume.CircularBarStockSolid;
        }
    }
}
