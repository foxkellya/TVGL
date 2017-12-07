using System.Runtime.Serialization;
using KatanaObjects.BaseClasses;
using UnitsNet;

namespace KatanaObjects.Blanks
{
    [DataContract]
    public class RectangularBarStockBlank : Blank
    {
        public RectangularBarStockBlank(Volume stockVolume, Volume finishVolume, Area areaOnCuttingPlane)
            : base()
        {
            Type = BlankType.RectangularBarStock;
            StockVolume = stockVolume;
            FinishVolume = finishVolume;
            WasteVolume = StockVolume - FinishVolume;
            AreaOnCuttingPlane = areaOnCuttingPlane;
            IsFeasible = true; //Assume it is feasible
        }

        public RectangularBarStockBlank(SubVolume subVolume) : base(subVolume)
        {
            Type = BlankType.RectangularBarStock;
            StockVolume = SubVolume.RectangularBlankVolume;
            WasteVolume = StockVolume - SubVolume.SolidVolume;
            FinishVolume = SubVolume.SolidVolume;
            AreaOnCuttingPlane = SubVolume.RectangularBlankAreaOnPlane;
            CrossSections = SubVolume.RectangularBlankCrossSections;
            ShapeOnCuttingPlanePreMachining = SubVolume.RectangularBlankShapeOnCuttingPlane;
            PerimeterOnPlane = SubVolume.RectangularBlankPerimeterOnPlane;
            DistanceAlongJoinDirection = SubVolume.RectangularBlankDistanceAlongNormalDirection;
            AreaIsCircular = false;

            IsFeasible = SubVolume.RectangularBarStockIsFeasible;
            CrossSectionBuildDirection = SubVolume.RectangularBlankCrossSectionBuildDirection;
            CrossSectionBuildDistance = SubVolume.RectangularBlankCrossSectionBuildDistance;
        }
    }
}
