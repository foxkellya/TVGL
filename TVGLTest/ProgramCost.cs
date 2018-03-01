using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StarMathLib;
using TVGL;
using TVGL.Boolean_Operations;
using TVGL.IOFunctions;
using CostModelCalculator;
using KatanaObjects.BaseClasses;

namespace TVGL_Test
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)

        {
            //interfacing with TVGL s.t. it'll print to console
            var writer = new TextWriterTraceListener(Console.Out);
            Debug.Listeners.Add(writer);
            TVGL.Message.Verbosity = VerbosityLevels.OnlyCritical;

            //pull shape files from folder and define
            //var filename = "../../../TestFiles/partsample.STL";
            //var filename = "../../../TestFiles/samplepart2.STL";
            //var filename = "../../../TestFiles/samplepart4.STL";
            //var filename = "../../../TestFiles/cuboide.STL";
            //var filename = "../../../TestFiles/Beam_Clean.STL"; //throws an error in 2D convex hull
            //var filename = "../../../TestFiles/WEDGE.STL";
            //var filename = "../../../TestFiles/Beam_Boss.STL";
            //var filename = "../../../TestFiles/testblock2.STL";
            //var filename = "../../../TestFiles/sth2.STL";


            var filename = "../../../TestFiles/Square_Support.STL";
            //var filename = "../../../TestFiles/Aerospace_Beam.STL";
            //var filename = "../../../TestFiles/Aerospace_Beam2.STL";
            //var filename = "../../../TestFiles/Bracket_Plate.STL";

            //open file with TessellatedSolid function
            Console.WriteLine("Attempting: " + filename);
            var solid = IO.Open(filename)[0];
            
            //////define cutting slice
            //double dx = 1; //uniform length of square
            //var costxyz = new double[3][];
            //var costcoords = new double[3][];
            //CostArrays(solid, dx, out costxyz, out costcoords);
            //Presenter.ShowAndHangHeatMap(solid, costxyz, costcoords, dx);//goal/final result:cool heat map with vertices!
            
            
            //solid = IO.Open(filename)[0];
            var averageNumSlices = 60; //could set with a dx value instead
            var values = SliceAndGetObjectiveFunctionValues(solid, averageNumSlices);
            Presenter.ShowAndHangHeatMap(solid, values);
        }

        public static Dictionary<double[], List<double[]>> SliceAndGetObjectiveFunctionValues(TessellatedSolid solid, int averageNumSlices)
        {
            //Intitialize output dictionary. First double[] is a direciton. 
            //Second double[] is double[0] = distance along direction. double[1] = objective function value
            var outputValues = new Dictionary<double[], List<double[]>>();
            var blankType = BlankType.RectangularBarStock;
            var originalCost = GetCostModels.ForGivenBlankType(solid, solid, blankType);
            var originalVolume = solid.Volume;

            double[,] backTransform;
            //solid = solid.SetToOriginAndSquareTesselatedSolid(out backTransform);
            var minBoundingBox = MinimumEnclosure.OrientedBoundingBox(solid);
            // Do averageNumSlices slices for each direction on a box(l == w == h).
            //Else, weight the averageNumSlices slices based on the average obb distance
            var obbAverageLength = minBoundingBox.Dimensions.Average();
            //Set step size to an even increment over the entire length of the solid
            var dx = obbAverageLength / averageNumSlices;

            //var directions = new double[3][]
            //{
            //    new[] {1.0, 0, 0},
            //    new[] {0, 1.0, 0},
            //    new[] {0, 0, 1.0}
            //};
            foreach (var direction in minBoundingBox.Directions)
            {
                //Get all the distances along this direction, starting at minDistance + dx
                //and ending at maxDistance - dx;
                List<Vertex> bottomVertices, topVertices;
                var length = MinimumEnclosure.GetLengthAndExtremeVertices(direction, solid.Vertices,
                    out bottomVertices, out topVertices);
                //var dx = length / averageNumSlices;
                var minDistanceAlong = direction.dotProduct(bottomVertices[0].Position);
                var maxDistanceAlong = direction.dotProduct(topVertices[0].Position);
                var distanceAlong = minDistanceAlong + dx;
                var distances = new List<double>();
                while (!distanceAlong.IsPracticallySame(maxDistanceAlong, dx/2))
                {
                    distances.Add(distanceAlong);
                    distanceAlong += dx;               
                }

                //Set up concurrent bags for use in the parallel loop.
                //Since bags are unordered, we will need to sort these by double[0]
                //when we convert the bags to lists.
                var costsAlongDirection = new ConcurrentBag<double[]>();
                var volumesAlongDirection = new ConcurrentBag<double[]>();
                //foreach (var d in distances)
                //{
                    Parallel.ForEach(distances, d =>
                    {
                        //Slice the solid and save its positive and negative side costs and volumes
                        var posXsolids = new List<TessellatedSolid>();
                        var negXsolids = new List<TessellatedSolid>();
                        var flat = new Flat(d, direction);
                        Slice.OnFlat(solid, flat, out posXsolids, out negXsolids);

                        var negCost = GetCostModels.ForGivenBlankType(solid, negXsolids, blankType);
                        var posCost = GetCostModels.ForGivenBlankType(solid, posXsolids, blankType);
                        costsAlongDirection.Add(new[] {d, negCost, posCost});

                        var negVolume = negXsolids.Sum(s => s.Volume);
                        var posVolume = posXsolids.Sum(s => s.Volume);
                        volumesAlongDirection.Add(new[] {d, negVolume, posVolume});
                    });
                //}

                var orderedCostsAlong = costsAlongDirection.OrderBy(s => s[0]).ToList();
                var orderedVolumesAlong = volumesAlongDirection.OrderBy(s => s[0]).ToList();

                //Set the objective function values for this direction and its reverse
                //outputValues.Add(direction, ObjectiveFunction1(orderedCostsAlong, orderedVolumesAlong, false));
                //outputValues.Add(direction.multiply(-1), ObjectiveFunction1(orderedCostsAlong, orderedVolumesAlong, true));

                outputValues.Add(direction, ObjectiveFunction2(originalCost, originalVolume, orderedCostsAlong, orderedVolumesAlong));
            }

            return outputValues;
        }

        public static List<double[]> ObjectiveFunction1(List<double[]> orderedCosts,
            List<double[]> orderedVolumes, bool reverse)
        {
            const int d = 0; //distance index
            const int n = 1; //neg value index
            const int p = 2; //positive value index
            var costs = new List<double[]>(orderedCosts);
            var volumes = new List<double[]>(orderedVolumes);
            var distances = orderedCosts.Select(s => s[0]).Reverse().ToList();
            if (reverse) //If reversing, we just have to reorder the distances. 
            {
                for (var i = 0; i < costs.Count; i++)
                {
                    costs[i][d] = distances[i] * -1;
                    volumes[i][d] = distances[i] * -1;
                }
            }

            //y = Change in Cost / Change in volume
            var output = new List<double[]>(); //where a double array contains: distance along (x) and cost (y)
            var priorDistance = costs.First()[d];
            for (var i = 1; i < costs.Count; i++)
            {
                var deltaNCost = costs[i][n] - costs[i - 1 ][n];
                var deltaPCost = costs[i - 1][p] - costs[i][p];
                var deltaNVolume = volumes[i][n] - volumes[i - 1][n];
                var deltaPVolume = volumes[i - 1][p] - volumes[i][p];
                var y = ((deltaNCost / deltaNVolume) + (deltaPCost / deltaPVolume)) / 2; //average change

                var currentDistance = costs[i][d];
                var x = (priorDistance + currentDistance) / 2;
                output.Add(new []{x, y});
            }
            return output;
        }

        public static List<double[]> ObjectiveFunction2(double originalCost, double originalVolume, List<double[]> orderedCosts,
            List<double[]> orderedVolumes)
        {
            const int d = 0; //distance index
            const int n = 1; //neg value index
            const int p = 2; //positive value index
            var costs = orderedCosts;
            var volumes = orderedVolumes;

            var output = new List<double[]>(); //where a double array contains: distance along (x) and cost (y)
            for (var i = 0; i < costs.Count; i++)
            {
                double y;
                if (costs[i][n] < costs[i][p])
                {
                    y = (originalCost - costs[i][n]) / volumes[i][n];
                }
                else y = (originalCost - costs[i][p]) / volumes[i][p];
                var x = costs[i][d];
                output.Add(new[] { x, y });
            }
            return output;
        }

        public static List<double[]> ObjectiveFunction3(double originalCost, double originalVolume, List<double[]> orderedCosts,
            List<double[]> orderedVolumes)
        {
            const int d = 0; //distance index
            const int n = 1; //neg value index
            const int p = 2; //positive value index
            var costs = orderedCosts;
            var volumes = orderedVolumes;

            var output = new List<double[]>(); //where a double array contains: distance along (x) and cost (y)
            for (var i = 0; i < costs.Count; i++)
            {
                double y;
                if (costs[i][n] < costs[i][p])
                {
                    y = (originalCost - costs[i][p]) / volumes[i][n];
                }
                else y = (originalCost - costs[i][n]) / volumes[i][p];
                var x = costs[i][d];
                output.Add(new[] { x, y });
            }
            return output;
        }

        public static List<double[]> ObjectiveFunction4(double originalCost, double originalVolume, List<double[]> orderedCosts,  
            List<double[]> orderedVolumes)
        {
            const int d = 0; //distance index
            const int n = 1; //neg value index
            const int p = 2; //positive value index
            var costs = orderedCosts;
            var volumes = orderedVolumes;

            var output = new List<double[]>(); //where a double array contains: distance along (x) and cost (y)
            for (var i = 0; i < costs.Count; i++)
            {
                double y;
                if (costs[i][n] < costs[i][p])
                {
                    y = ((originalCost - costs[i][n]) / originalCost) / (volumes[i][n] / originalVolume);
                }
                else y = ((originalCost - costs[i][p]) / originalCost) / (volumes[i][p] / originalVolume);
            var x = costs[i][d];
                output.Add(new[] { x, y });
            }
            return output;
        }

        public static void CostArrays(TessellatedSolid ts, double dx, out double[][] costxyz, out double[][] costcoords)
        {

            //define solid
            var solidOG = ts;
            double[,] backTransform;



            //create list of flip matrices to transform the solid in the x,y,z directions

            List<double[,]> FlipMatrices = new List<double[,]>();


            //create array storage areas
            double[][] maxarray = new double[3][];
            double[][] parray = new double[3][];
            double[][] narray = new double[3][];
            double[][] avgarray = new double[3][];
            double[][] midarray = new double[3][];
            //Create flip matrices
            //examine in the x direction(identity matrix)
            //Presenter.ShowAndHang(solidOG);
            double[,] xFlipMatrix =
                {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1}
                };

            ////turn in y direction
            double[,] yFlipMatrix =
               {
                { 0, 1, 0, 0 },
                { -1, 0, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1}
                };


            ////turn in z direction
            double[,] zFlipMatrix =
               {
                { 0, 0, 1, 0 },
                { 0, 1, 0, 0 },
                { -1, 0, 0, 0 },
                { 0, 0, 0, 1}
                };
            FlipMatrices.Add(xFlipMatrix);
            FlipMatrices.Add(yFlipMatrix);
            FlipMatrices.Add(zFlipMatrix);

            //put the solid at the origin 
            solidOG = solidOG.SetToOriginAndSquareTesselatedSolid(out backTransform);

            for (var dir = 0; dir < FlipMatrices.Count; dir++)
            {

                //flip solid to correct orientation
                var solid1 = solidOG.TransformToGetNewSolid(FlipMatrices[dir]);
                solid1 = solid1.SetToOriginAndSquareTesselatedSolid(out backTransform);
                //Presenter.ShowAndHang(solid1);

                //define the number of slices desired and creates small slices, dx
                var Xmax = solid1.XMax;
                var Xmin = solid1.XMin;
                //cutting uniform solids
                var nxdec = (Xmax - Xmin) / dx;
                var nxslices = Math.Floor(nxdec);
                //create arrays to store info in
                List<double> Cplist = new List<double>();
                List<double> Cnlist = new List<double>();
                List<double> Vplist = new List<double>();
                List<double> Vnlist = new List<double>();
                List<double> deltaCp = new List<double>();
                List<double> deltaCn = new List<double>();
                List<double> deltaVn = new List<double>();
                List<double> deltaVp = new List<double>();


                List<double> valmax = new List<double>();
                List<double> valp = new List<double>();
                List<double> valn = new List<double>();
                List<double> valavg = new List<double>();
                List<double> valxmid = new List<double>();

                for (var k = 1; k < nxslices + 1; k++)
                {

                    //create x location of slice
                    double X1 = (k - 1) * dx;
                    //conditional statement for first x cut
                    List<TessellatedSolid> posXsolids = new List<TessellatedSolid>();
                    List<TessellatedSolid> negXsolids = new List<TessellatedSolid>();
                    List<double> Cptot = new List<double>();
                    List<double> Cntot = new List<double>();
                    List<double> Vptot = new List<double>();
                    List<double> Vntot = new List<double>();


                    if (k == 1)
                    {
                        //returns entire solid at xmax for first iteration in positive solids
                        posXsolids.Add(solid1);

                    }

                    else if (k == nxslices)
                    {
                        //returns entire solid at xmax for last iteration in negative solids
                        negXsolids.Add(solid1);

                    }
                    else
                    {
                        //returns solids after cut
                        Slice.OnFlat(solid1,
                            new Flat(new[] { X1, 0, 0 }, new[]
                            { 1.0,0,0 }),
                            out posXsolids,
                            out negXsolids);

                    }
                    //get cost of solids after solving and save them
                    foreach (TessellatedSolid posXsolid in posXsolids)
                    {
                        double C1 = GetCostModels.ForGivenBlankType(solid1, posXsolid, BlankType.RectangularBarStock);
                        Cptot.Add(C1);
                        Vptot.Add(Math.Abs(posXsolid.Volume));

                    }
                    Cplist.Add(Cptot.Sum());
                    Vplist.Add(Vptot.Sum());
                    foreach (TessellatedSolid negXsolid in negXsolids)
                    {
                        double C1 = GetCostModels.ForGivenBlankType(solid1, negXsolid, BlankType.RectangularBarStock);
                        Cntot.Add(C1);
                        Vntot.Add(Math.Abs(negXsolid.Volume));

                    }
                    Cnlist.Add(Cntot.Sum());
                    Vnlist.Add(Vntot.Sum());

                }
                //get costs to plot
                for (var m = 0; m < (Cnlist.Count - 1); m++)
                {
                    //compute change in cost
                    double deltaCpval = Cplist[m] - Cplist[m + 1];
                    double deltaCnval = Cnlist[m + 1] - Cnlist[m];
                    //compute change in volume
                    double deltaVpval = Vplist[m] - Vplist[m + 1];
                    double deltaVnval = Vnlist[m + 1] - Vnlist[m];

                    //compute change in cost over change in volume
                    double deltaCpVpval = deltaCpval / deltaVpval;
                    double deltaCnVnval = deltaCnval / deltaVnval;

                    //find the average of the two
                    double deltaCavgval = (deltaCpVpval + deltaCnVnval) / 2;


                    double Xmidval = dx * (m + 0.5);

                    //save data
                    deltaCp.Add(deltaCpval);
                    deltaCn.Add(deltaCnval);
                    deltaVp.Add(deltaVpval);
                    deltaVn.Add(deltaVnval);

                    double deltaCmax = deltaCnVnval;

                    if (deltaCpVpval > deltaCnVnval)
                    {
                        deltaCmax = deltaCpVpval;
                    }

                    //take out extreme points in the lists
                    if (m > 1)
                    {

                        //take out extreme points for valavg list
                        double val1 = valavg[m - 2];
                        double val2 = valavg[m - 1];
                        double val3 = deltaCavgval;
                        if (val2 > (6 * val1) & val2 > (6 * val1))
                        {
                            val2 = (val3 + val1) / 2;

                            valavg.RemoveAt(m - 1);
                            valavg.Add(val2);
                        }

                        //take out extreme points for valmax list
                        double val1m = valmax[m - 2];
                        double val2m = valmax[m - 1];
                        double val3m = deltaCmax;
                        if (val2m > (6 * val1m) & val2m > (6 * val1m))
                        {
                            val2m = (val3m + val1m) / 2;

                            valmax.RemoveAt(m - 1);
                            valmax.Add(val2m);
                        }



                    }

                    //save data to lists
                    valp.Add(deltaCpVpval);
                    valn.Add(deltaCnVnval);
                    valavg.Add(deltaCavgval);
                    valmax.Add(deltaCmax);
                    valxmid.Add(Xmidval);

                }


                //remove all end points
                valp.RemoveAt(0);
                valp.RemoveAt(Cnlist.Count - 3);
                valn.RemoveAt(0);
                valn.RemoveAt(Cnlist.Count - 3);
                valmax.RemoveAt(0);
                valmax.RemoveAt(Cnlist.Count - 3);
                valavg.RemoveAt(0);
                valavg.RemoveAt(Cnlist.Count - 3);
                valxmid.RemoveAt(0);
                valxmid.RemoveAt(Cnlist.Count - 3);



                //save data to arrays
                maxarray[dir] = valmax.ToArray();
                avgarray[dir] = valavg.ToArray();
                midarray[dir] = valxmid.ToArray();
        

            }

            //scale data 0 to 1 across arrays
            //find max value of all directions
            double[] maxes = new[] { maxarray[0].Max(), maxarray[1].Max(), maxarray[2].Max() };
            double scale = maxes.Max();


            for (var dir = 0; dir < 3; dir++)
            {

                for (var s = 0; s < maxarray[dir].Length; s++)
                {
                  
                    maxarray[dir][ s] = maxarray[dir][s] / scale;
              

                }
                midarray[dir].ToList().ForEach(Console.WriteLine);
                maxarray[dir].ToList().ForEach(Console.WriteLine);
            }


            //I decided to make the max array, the cost array for this test, but note that all have been created
            costcoords = midarray;
            

            costxyz = maxarray;


        }


        }
    }
        
    

    

