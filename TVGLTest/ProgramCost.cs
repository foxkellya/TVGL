using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            var filename = "../../../TestFiles/Beam_Clean.STL";
            //var filename = "../../../TestFiles/WEDGE.STL";
            //var filename = "../../../TestFiles/Beam_Boss.STL";
            //var filename = "../../../TestFiles/testblock2.STL";


            //open file with TessellatedSolid function
            Console.WriteLine("Attempting: " + filename);
            List<TessellatedSolid> solids = IO.Open(filename);






            //define solid:assuming it's just one solid
            var solidOG = solids[0];
           
            double[][] costxyz = new double[3][];
            double[][] costcoords = new double[3][];

            //define cutting slice
            double dx = 1; //uniform length of square

            CostArrays(solidOG, dx, out costxyz, out costcoords);
            Presenter.ShowAndHangHeatMap(solidOG, costxyz, costcoords, dx);//goal/final result:cool heat map with vertices!
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
                { 0, -1, 0, 0 },
                { 1, 0, 0, 0 },
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
                        double C1 = GetCostModels.ForGivenBlankType(posXsolid, BlankType.RectangularBarStock);
                        Cptot.Add(C1);
                        Vptot.Add(Math.Abs(posXsolid.Volume));

                    }
                    Cplist.Add(Cptot.Sum());
                    Vplist.Add(Vptot.Sum());
                    foreach (TessellatedSolid negXsolid in negXsolids)
                    {
                        double C1 = GetCostModels.ForGivenBlankType(negXsolid, BlankType.RectangularBarStock);
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
                maxarray[dir] = valmax.normalize().ToArray();
                parray[dir] = valp.normalize().ToArray();
                narray[dir] = valn.normalize().ToArray();
                avgarray[dir] = valavg.normalize().ToArray();
                midarray[dir] = valxmid.ToArray();




            }
            //I decided to make the max array, the cost array for this test, but note that all have been created
            costcoords = midarray;

            costxyz = maxarray;


        }


        }
    }
        
    

    

