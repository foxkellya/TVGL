using System.Collections.Generic;
using System.Runtime.Serialization;
using KatanaObjects.BaseClasses;
using TVGL;
using UnitsNet;

namespace KatanaObjects.Blanks
{
    [DataContract]
    public class AssemblyBlank : Blank
    {
        internal AssemblyBlank(SubVolume subVolume, IEnumerable<Blank> blanks,
            IList<List<Point>> intersectionPolygonsOfBaseBlank = null, 
            Length distanceOfBaseBlankAlongNormal = default(Length)) : base(subVolume)
        {
            Type = BlankType.Assembly;
           
            //ToDo: set perimeter on plane from joining operation
            //PerimeterOnPlane = Length.Zero;
            var stockVolume = Volume.Zero;
            var wasteVolume = Volume.Zero;
            var finishVolume = Volume.Zero;
            AssemblyBlanks = new List<Blank>();
            foreach (var blank in blanks)
            {
                if(blank.IsAssembly) AssemblyBlanks.AddRange(blank.AssemblyBlanks);
                else AssemblyBlanks.Add(blank);
                stockVolume += blank.StockVolume;
                wasteVolume += blank.WasteVolume;
                finishVolume += blank.FinishVolume;
            }
            StockVolume = stockVolume;
            WasteVolume = wasteVolume;
            FinishVolume = finishVolume;
            ShapeOnCuttingPlanePreMachining = intersectionPolygonsOfBaseBlank;
            DistanceAlongJoinDirection = distanceOfBaseBlankAlongNormal;
        }
    }
}
