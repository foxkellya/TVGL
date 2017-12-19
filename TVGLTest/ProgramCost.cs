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
            var filename = "../../../TestFiles/ABF.stl";
            //var filename = "../../../TestFiles/sth2.stl";
            //var filename = "../../../TestFiles/casing.stl";
            //var filename = "../../../TestFiles/Pump2.stl";
            //var filename = "../../../TestFiles/Cuboide.stl";

            //open file with TessellatedSolid function
            //Console.WriteLine("Attempting: " + filename);
            List<TessellatedSolid> solids = IO.Open(filename);

            //create list where all solids will be saved
            List<TessellatedSolid> volumeslist = new List<TessellatedSolid>();

            //define solid
            var solid1 = solids[0];
            double[,] backTransform;
            solid1.SetToOriginAndSquareTesselatedSolid(out backTransform);
            //Presenter.ShowAndHang(solids[0]);

            List<double> deltCVlist = new List<double>();
            List<double> Xmidlist = new List<double>();
            var values = new List<double[]>();

            //define the number of slices desired and creates small slices, dx
            var Xmax = solid1.XMax;
            var Xmin = solid1.XMin;
            //cutting uniform solids
            var dx = 1; //uniform length of square
            var nxdec = (Xmax - Xmin) / dx;
            var nxslices = Math.Floor(nxdec);

            for (var k = 0; k <= nxslices; k++)
            {
                //create x location of slice
                double X1 = Xmax - k * dx;
                //conditional statement for first x cut
                List<TessellatedSolid> negativeSolidsXslice1 = new List<TessellatedSolid>();
                List<double> C1tot = new List<double>();
                List<double> C2tot = new List<double>();
                List<double> V1tot = new List<double>();
                List<double> V2tot = new List<double>();

                if (k == 0)
                {
                    //returns entire solid at xmax
                    negativeSolidsXslice1.Add(solid1);
                    Console.WriteLine("Display all negative solids at X1 slice at xmax");
                    //Presenter.ShowAndHang(negativeSolidsXslice1);
                }
                else if(Math.Abs(X1-Xmin)<.001)
                {
                    //do nothing
                    
                    Console.WriteLine("X1 slice at xmin is very close to X1 so do nothing");
                    //Presenter.ShowAndHang(negativeSolidsXslice1);
                }

                else
                {
                    //returns negative solids after first x cut
                    Slice.OnFlat(solid1,
                        new Flat(new[] { X1, 0, 0 }, new[]
                        { 1.0,0,0 }),
                        out List<TessellatedSolid> positiveSolids,
                        out negativeSolidsXslice1);
                    Console.WriteLine("Display negative solids after X1 slice");
                    //Presenter.ShowAndHang(negativeSolidsXslice1);
                    
                }

                foreach (TessellatedSolid solid2 in negativeSolidsXslice1)
                {
                    double C1= GetCostModels.ForGivenBlankType(solid2, BlankType.RectangularBarStock);
                    Console.WriteLine("Above is the cost of the solid after 1st cut");
                    C1tot.Add(C1);
                    V1tot.Add(solid2.Volume);
                    //Presenter.ShowAndHang(solid2);


                    //conditional statement for second x cut
                    double X2 = Xmax - (k+1) * dx;
                    List<TessellatedSolid> positiveSolidsXslice2 = new List<TessellatedSolid>();
                    List<TessellatedSolid> negativeSolidsXslice2 = new List<TessellatedSolid>();
                    //returns solid without second cut at xmin
                    if (k == nxslices)
                    {
                        positiveSolidsXslice2.Add(solid2);
                        negativeSolidsXslice2.Add(solid2);
                        Console.WriteLine("Display original negative solid from X1 at xmin");
                        //Presenter.ShowAndHang(negativeSolidsXslice2);
                    }
                    //returns solid with second cut at xmin
                    else if (X2 < solid2.XMin)
                    {
                        positiveSolidsXslice2.Add(solid2);
                        negativeSolidsXslice2.Add(solid2);
                        Console.WriteLine("Display negative solids after XMIN of new solid>second x slice");
                        //Presenter.ShowAndHang(positiveSolidsXslice2);
                    }
                    else if (Math.Abs(X2 - solid2.XMin) < 0.001)
                    {
                        positiveSolidsXslice2.Add(solid2);
                        negativeSolidsXslice2.Add(solid2);
                        Console.WriteLine("Display original negative solid from X1 when last slice is very small");
                        //Presenter.ShowAndHang(positiveSolidsXslice2);
                    }

                    //returns positive solids after second x cut
                    else
                    {
                        Slice.OnFlat(solid2,
                        new Flat(new[] { X2, 0, 0 }, new[]
                        { 1.0,0,0}),
                        out positiveSolidsXslice2,
                        out negativeSolidsXslice2);
                        Console.WriteLine("Display negative solids after another X2slice");
                    }
                        //Presenter.ShowAndHang(negativeSolidsXslice2);
                        foreach (TessellatedSolid costsolid in negativeSolidsXslice2)
                        {
                            double C2 = GetCostModels.ForGivenBlankType(costsolid, BlankType.RectangularBarStock);
                            Console.WriteLine("Above is the cost of the solid after 2nd cut");
                            C2tot.Add(C2);
                            V2tot.Add(costsolid.Volume);
                        //Presenter.ShowAndHang(costsolid);
                    }
                    foreach (TessellatedSolid finalsolid in positiveSolidsXslice2)
                        {

                            //Presenter.ShowAndHang(finalsolid);
                            //save solids to a list
                            volumeslist.Add(finalsolid);

                            Console.WriteLine("Display positive solids FINAL");
                            //Presenter.ShowAndHang(volumeslist);
                        }
                    double deltV = V1tot.Sum() - V2tot.Sum();
                    if (deltV<0)
                        {
                        Console.WriteLine("Volume of small slice is less than 0");
                            }
                    double deltC = C1tot.Sum() - C2tot.Sum();
                        double Xmid = X2 + dx/2;

                    double deltCV = deltC / deltV;
                    
                        values.Add(new[] { Xmid, deltCV });
                        
                    }
                    
                }

            //create paired list

            //  List < List < string >> list_array = new List<List<string>> { first, second };
            TVGLTest.ExcelInterface.CreateNewGraph(new List<List<double[]>> { values });

            Presenter.ShowAndHang(volumeslist);
            Console.WriteLine("Completed.");
            Console.ReadKey();
        }
          
        }
    }
