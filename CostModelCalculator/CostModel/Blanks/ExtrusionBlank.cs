using System.Runtime.Serialization;
using KatanaObjects.BaseClasses;

namespace KatanaObjects.Blanks
{
    [DataContract]
    public class ExtrusionBlank : Blank
    {
        internal ExtrusionBlank(SubVolume subVolume) : base(subVolume)
        {
            ////Type = BlankType.Extrusion;
            //StockVolume = SubVolume.ExtrusionVolume;
            //WasteVolume = StockVolume - SubVolume.SolidVolume;
            //FinishVolume = SubVolume.SolidVolume;
 
            ////ShapeOnPlanePreMachining = SubVolume.ExtrusionShapeOnPlane;
            ////PerimeterOnPlane = SubVolume.ExtrusionPerimeterOnPlane;
            //AreaIsCircular = false;
            //IsFeasible = SubVolume.ExtrusionIsFeasible;
            //CrossSections = SubVolume.ExtrusionCrossSections;
        }
    }
}
