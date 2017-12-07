using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GenericInputs;
using KatanaObjects.BaseClasses;
using StarMathLib;
using TVGL;
using UnitsNet;

namespace KatanaObjects.Blanks
{
    [DataContract]
    public class NearNetPrintedShapeBlank : Blank
    {
        internal NearNetPrintedShapeBlank(SubVolume subVolume, SearchInputs inputs) : base(subVolume)
        {
            var direction = subVolume.AdditiveBuildDirection;
            //Get the substrate plate.
            var crossSection = subVolume.AdditiveCrossSections.Last();

            //First convert the points to vertices, so we can project them.
            var loops = crossSection.Select(path => path.Select(doublePoint => new Vertex(doublePoint)).ToList()).ToList();
            var backTransform = new double[,] {}; //Note: backTransform will the same for each loop
            var points2D = new List<List<Point>>();
            foreach (var loop in loops)
            {
                points2D.Add(MiscFunctions.Get2DProjectionPointsReorderingIfNecessary(loop, direction, out backTransform));
            }

            //Now that we have the projected points, we can offset them appropriately
            var paths = PolygonOperations.OffsetSquare(points2D, inputs.NearNetAdditive.SubstrateOffset.TesselatedSolidBaseUnit(subVolume.SolidUnitString));

            //Now convert the points back to 3D
            var contours = new List<List<double[]>>();
            foreach (var path in paths)
            {
                var contour = new List<double[]>();
                foreach (var point in path)
                {
                    var position = new[] { point.X, point.Y, 0.0, 1.0 };                
                    contour.Add(backTransform.multiply(position));
                }
                contours.Add(contour);
            }

            //Now we can get the substrate. 
            SubstrateSolid = GetSubstrateSolid(contours, direction,
                inputs.NearNetAdditive.SubstrateThickness.TesselatedSolidBaseUnit(subVolume.SolidUnitString));

            SubstrateVolume = Volume.FromTesselatedSolidBaseUnit(SubstrateSolid.Volume, subVolume.SolidUnitString);

            //Set the rest of the parameters
            Type = BlankType.NearNetAdditive;
            StockVolume = VolumeIsUnderestimate ? SubVolume.AdditiveVolumeUnderestimate : SubVolume.AdditiveVolume;
            WasteVolume = StockVolume + SubstrateVolume - SubVolume.SolidVolume;
            FinishVolume = SubVolume.SolidVolume;

            //CrossSections = SubVolume.AdditiveCrossSections;
            //ShapeOnPlanePreMachining = SubVolume.AdditiveShapeOnPlane;
            PerimeterOnPlane = SubVolume.AdditivePerimeterOnPlane;
            AreaIsCircular = false;
            IsFeasible = SubVolume.NearNetPrintedShapeIsFeasible;
        }   

        [DataMember]
        public Volume SubstrateVolume { get; internal set; }

        public TessellatedSolid GetSubstrateSolid(List<List<double[]>> contours, double[] direction, double distance)
        {
            return TVGL.Extrude.FromLoops(contours, direction, distance);
        }
                    
    }
}
