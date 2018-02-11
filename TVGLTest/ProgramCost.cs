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
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            //interfacing with TVGL s.t. it'll print to console
            var writer = new TextWriterTraceListener(Console.Out);
            Debug.Listeners.Add(writer);
            TVGL.Message.Verbosity = VerbosityLevels.OnlyCritical;

            //pull shape files from folder and define
            //var filename = "../../../TestFiles/partsample.STL";
            //var filename = "../../../TestFiles/samplepart2.STL";
            var filename = "../../../TestFiles/samplepart4.STL";

            //open file with TessellatedSolid function
            Console.WriteLine("Attempting: " + filename);
            List<TessellatedSolid> solids = IO.Open(filename);


            //define solid
            var solidOG = solids[0];
            double[,] backTransform;

            //define cutting slice
            var dx = 1; //uniform length of square

            //create list of flip matrices to transform the solid in the x,y,z directions

            List<double[,]> FlipMatrices = new List<double[,]>();

            //create storage areas
            List<double> xvalmax = new List<double>();
            List<double> xvalp = new List<double>();
            List<double> xvaln = new List<double>();
            List<double> xvalavg = new List<double>();
            List<double> xvalxmid = new List<double>();
            List<double> yvalmax = new List<double>();
            List<double> yvalp = new List<double>();
            List<double> yvaln = new List<double>();
            List<double> yvalavg = new List<double>();
            List<double> yvalxmid = new List<double>();
            List<double> zvalmax = new List<double>();
            List<double> zvalp = new List<double>();
            List<double> zvaln = new List<double>();
            List<double> zvalavg = new List<double>();
            List<double> zvalxmid = new List<double>();


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
                //Presenter.ShowAndHang(solid1);
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

                var valuesp = new List<double[]>();
                var valuesn = new List<double[]>();
                var valuesmax = new List<double[]>();
                var valuesavg = new List<double[]>();

                for (var k = 1; k < nxslices + 1; k++)
                {
                    Console.WriteLine("The k iteration is{0}", k);
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
                        //returns entire solid at xmax for first iteration

                        posXsolids.Add(solid1);

                        Console.WriteLine("Display all negative solids at X1 slice at xmax");

                    }
                
                    else if (k == nxslices)
                    {
                        negXsolids.Add(solid1);

                        Console.WriteLine("Display original negative solid from X1 at xmin, empty for next cost calculation");

                    }            
                    else
                    {
                        //returns solids after cut
                        Slice.OnFlat(solid1,
                            new Flat(new[] { X1, 0, 0 }, new[]
                            { 1.0,0,0 }),
                            out posXsolids,
                            out negXsolids);
                        //Console.WriteLine("Display negative solids after X1 slice");
                        //Presenter.ShowAndHang(negXsolid);

                    }
                    //get cost of solids after solving and save them
                    foreach (TessellatedSolid posXsolid in posXsolids)
                    {
                        double C1 = GetCostModels.ForGivenBlankType(posXsolid, BlankType.RectangularBarStock);
                        Console.WriteLine("Above is the cost of the negative solid");
                        Cptot.Add(C1);
                        Vptot.Add(Math.Abs(posXsolid.Volume));

                    }
                    Cplist.Add(Cptot.Sum());
                    Vplist.Add(Vptot.Sum());
                    foreach (TessellatedSolid negXsolid in negXsolids)
                    {
                        double C1 = GetCostModels.ForGivenBlankType(negXsolid, BlankType.RectangularBarStock);
                        Console.WriteLine("Above is the cost of the positive solid");
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
                        //take out extreme points for EXCEL
                        double[] valuem = valuesmax[m - 2];
                        double value1 = valuem[1];
                        double[] valuen = valuesmax[m - 1];
                        double value2 = valuen[1];
                        double value3 = deltaCmax;
                        if (value2 > (6 * value1) & value2 > (6 * value1))
                        {
                            value2 = (value3 + value1) / 2;
                            double Xmid2 = dx * ((m - 1) + 0.5);
                            valuesmax.RemoveAt(m - 1);
                            valuesmax.Add(new[] { Xmid2, value2 });
                        }

                       //take out extreme points for valmax list
                        double val1 = valmax[m-2];
                        double val2 = valmax[m-1];
                        double val3 = deltaCmax;
                        if (val2 > (6 * val1) & val2 > (6 * val1))
                        {
                            val2 = (val3 + val1) / 2;
                           
                            valmax.RemoveAt(m - 1);
                            valmax.Add(val2);
                        }

                    }
                    //save data to lists for EXCEL
                    valuesp.Add(new[] { Xmidval, deltaCpVpval });
                    valuesn.Add(new[] { Xmidval, deltaCnVnval });
                    valuesmax.Add(new[] { Xmidval, deltaCmax });
                    valuesavg.Add(new[] { Xmidval, deltaCavgval });

                    //save data to lists
                    valp.Add(deltaCpVpval);
                    valn.Add(deltaCnVnval);
                    valavg.Add(deltaCavgval);
                    valmax.Add(deltaCmax);
                    valxmid.Add(Xmidval);

                }
                //removed max end points for EXCEL
                valuesmax.RemoveAt(0);
                valuesmax.RemoveAt(Cnlist.Count - 3);

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

                ////generate excel graphs of the data
                //TVGLTest.ExcelInterface.CreateNewGraph(new List<List<double[]>> { valuesp }, filename, string.Format("Xposition"), string.Format("(C2-C1)/(V2-V1):positive"));
                //TVGLTest.ExcelInterface.CreateNewGraph(new List<List<double[]>> { valuesn }, filename, string.Format("Xposition"), string.Format("(C2-C1)/(V2-V1):negative"));
                //TVGLTest.ExcelInterface.CreateNewGraph(new List<List<double[]>> { valuesmax }, filename, string.Format("Xposition"), string.Format("(C2-C1)/(V2-V1):max"));
                //TVGLTest.ExcelInterface.CreateNewGraph(new List<List<double[]>> { valuesavg }, filename, string.Format("Xposition"), string.Format("(C2-C1)/(V2-V1):avg"));

                //normalize data
                valp.normalize();
                valn.normalize();
                valavg.normalize();
                valmax.normalize();
                valxmid.normalize();

                if (dir == 0)
                {
                    xvalp.AddRange(valp);
                    xvaln.AddRange(valn);
                    xvalavg.AddRange(valavg);
                    xvalmax.AddRange(valmax);
                    xvalxmid.AddRange(valxmid);
                }

                if (dir==1)
                {
                    yvalp.AddRange(valp);
                    yvaln.AddRange(valn);
                    yvalavg.AddRange(valavg);
                    yvalmax.AddRange(valmax);
                    yvalxmid.AddRange(valxmid);

                }

                if (dir==2)
                {
                    zvalp.AddRange(valp);
                    zvaln.AddRange(valn);
                    zvalavg.AddRange(valavg);
                    zvalmax.AddRange(valmax);
                    zvalxmid.AddRange(valxmid);
                }
                


            }


            Console.WriteLine("Completed.");
            //Console.ReadKey();

            //PART 2

            //TESTING with some sample points

            //TESTING:create points within the data
            double xv = 220;
            double yv = 4;
            double zv = 4;

            //create array to store x,y,z costs
            //double[,] array[,] = new Array();


            for (var dir1=0; dir1<3,dir1++)
            { 
            //create places to store data
            double cpv=new double(); 
            double cnv=new double ();
            double cavgv = new double();
            double cmaxv = new double();
            //extrapolation case for end points
            //for first points
            if (xv<xvalxmid[0])
            {
                cpv = xvalp[0];
                cnv = xvaln[0];
                cavgv = xvalavg[0];
                cmaxv = xvalmax[0];

            }
            //for end points
            else if (xv > xvalxmid[xvalxmid.Count-1])
            {
                cpv = xvalp[xvalxmid.Count-1];
                cnv = xvaln[xvalxmid.Count-1];
                cavgv = xvalavg[xvalxmid.Count-1];
                cmaxv = xvalmax[xvalxmid.Count-1];

            }
            //interpolation case for mid points
            else
            {

                //find the low point
                var xlsearch = 0;
                while ((xv - (xvalxmid[xlsearch]) <= dx))
                {
                    xlsearch++;
                }
                var xmidlow = xvalxmid[xlsearch];
                //add one step for the high point

                var xmidhigh = xvalxmid[xlsearch + 1];

                //calculate for interpolation in the x points
                var interp = (xv - xmidlow) / (xmidhigh - xmidlow);

                //apply interpolation to find cost at that point for the different lists
                cpv = interp * (xvalp[xlsearch + 1] - xvalp[xlsearch]) + xvalp[xlsearch];
                cnv = interp * (xvaln[xlsearch + 1] - xvaln[xlsearch]) + xvaln[xlsearch];
                cavgv = interp * (xvalavg[xlsearch + 1] - xvalavg[xlsearch]) + xvalavg[xlsearch];
                cmaxv = interp * (xvalmax[xlsearch + 1] - xvalmax[xlsearch]) + xvalmax[xlsearch];

                //save this cool stuff to an array
                varray[]
            }









        }
    }
}
    

