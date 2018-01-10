using System.Runtime.Serialization;
using KatanaObjects.BaseClasses;

namespace KatanaObjects.Blanks
{
    [DataContract]
    public class HollowTubeBlank : Blank
    {
        internal HollowTubeBlank(SubVolume subVolume) : base(subVolume)
        {
            Type = BlankType.HollowTube;

            //For certain blank types, the dimensions are linked together and should be set altogether.
            subVolume.SetHollowTubeDimensions();

            //If not feasible, stop making blank
            IsFeasible = SubVolume.HollowTubeIsFeasible;
            if (!IsFeasible) return;

            StockVolume = SubVolume.HollowTubeStockVolume;
            WasteVolume = StockVolume - SubVolume.SolidVolume;
            FinishVolume = SubVolume.SolidVolume;
            AreaOnCuttingPlane = SubVolume.HollowTubeAreaOnPlane;
            CrossSections = SubVolume.HollowTubeCrossSections;
            //ShapeOnPlanePreMachining = SubVolume.HollowTubePaths;
            //PerimeterOnPlane = Math.PI*SubVolume.HollowTubeOuterDiameter;
            AreaIsCircular = true;
            CrossSectionBuildDirection = SubVolume.HollowTubeCrossSectionBuildDirection;
            CrossSectionBuildDistance = SubVolume.HollowTubeDepth;
        }
    }
}
