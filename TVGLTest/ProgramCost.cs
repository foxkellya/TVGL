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
            //var filename = "../../../TestFiles/ABF.stl";
            var filename = "../../../TestFiles/sth2.stl";
            //var filename = "../../../TestFiles/casing.stl";

            //open file with TessellatedSolid function
            //Console.WriteLine("Attempting: " + filename);
            List<TessellatedSolid> solids = IO.Open(filename);

            //create list where all solids will be saved
            List<TessellatedSolid> volumeslist = new List<TessellatedSolid>();

            //define solid
            var solid1 = solids[0];
            double[,] backTransform;
            solid1.SetToOriginAndSquareTesselatedSolid(out backTransform);
            Presenter.ShowAndHang(solids[0]);

            List<double> deltClist = new List<double>();
            List<double> Xmidlist = new List<double>();

            //define the number of slices desired and creates small slices, dx
            var Xmax = solid1.XMax;
            var Xmin = solid1.XMin;
            //cutting uniform solids
            var dx = 10; //uniform length of square
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
                if (k == 0)
                {
                    //returns entire solid at xmin
                    negativeSolidsXslice1.Add(solid1);
                    Console.WriteLine("Display all negative solids at X1 slice at xmin");
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
                    C1tot.Add(C1);

                    //conditional statement for second x cut
                    double X2 = Xmax - (k+1) * dx;
                    List<TessellatedSolid> positiveSolidsXslice2 = new List<TessellatedSolid>();
                    //returns solid without second cut at xmin
                    if (k == nxslices)
                    {
                        positiveSolidsXslice2.Add(solid2);
                        Console.WriteLine("Display original negative solid from X1 at xmin");
                        //Presenter.ShowAndHang(positiveSolidsXslice2);
                    }
                    //returns solid with second cut at xmin
                    else if (X2 < solid2.XMin)
                    {
                        positiveSolidsXslice2.Add(solid2);
                        Console.WriteLine("Display negative solids after XMIN of new solid>second x slice");
                        //Presenter.ShowAndHang(positiveSolidsXslice2);
                    }
     
                    //returns positive solids after second x cut
                    else
                    {
                        Slice.OnFlat(solid2,
                        new Flat(new[] { X2, 0, 0 }, new[]
                        { 1.0,0,0}),
                        out positiveSolidsXslice2,
                        out List<TessellatedSolid> negativeSolidsXslice2);
                        Console.WriteLine("Display positive solids after another X2slice");
                        //Presenter.ShowAndHang(positiveSolidsXslice2);

                        foreach (TessellatedSolid finalsolid in positiveSolidsXslice2)
                        {

                            double C2 = GetCostModels.ForGivenBlankType(finalsolid, BlankType.RectangularBarStock);
                          
                            C2tot.Add(C2);
                            //save solids to a list
                            volumeslist.Add(finalsolid);

                            Console.WriteLine("Display positive solids FINAL");
                            //Presenter.ShowAndHang(volumeslist);
                        }
                        double deltC = C1tot.Sum() - C2tot.Sum();
                        double Xmid = (X1 - X2) / 2;
                        
                        deltClist.Add(deltC);
                        
                        
                        Xmidlist.Add(Xmid);
                        
                    }
                    
                }

            }
          
            TVGLTest.ExcelInterface.CreateNewGraph(

            Presenter.ShowAndHang(volumeslist);
            Console.WriteLine("Completed.");
            Console.ReadKey();
        }
    }
}