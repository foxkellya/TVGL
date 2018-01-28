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
            //var filename = "../../../TestFiles/sth2.stl";
            //var filename = "../../../TestFiles/casing.stl";
            //var filename = "../../../TestFiles/Pump2.stl";
            //var filename = "../../../TestFiles/Cuboide.stl";
            // var filename = "../../../TestFiles/adaptor.stl";
            // var filename = "../../../TestFiles/simple_damper.stl";
            //var filename = "../../../TestFiles/partsample.STL";
            //var filename = "../../../TestFiles/samplepart2.STL";
            var filename = "../../../TestFiles/samplepart4.STL";
            //open file with TessellatedSolid function
            //Console.WriteLine("Attempting: " + filename);
            List<TessellatedSolid> solids = IO.Open(filename);

            //create list where all solids will be saved
            List<TessellatedSolid> volumeslist = new List<TessellatedSolid>();

            //define solid
            var solidOG = solids[0];
            double[,] backTransform;


            //flip solid to examine other side

            //flip in the x direction
            //double[,] FlipMatrix =
            //    {
            //    { -1, 0, 0, 0 },
            //    { 0, -1, 0, 0 },
            //    { 0, 0, 1, 0 },
            //    { 0, 0, 0, 1}
            //    };

            ////turn in y direction
            //double[,] FlipMatrix =
            //   {
            //    { 0, -1, 0, 0 },
            //    { 1, 0, 0, 0 },
            //    { 0, 0, 1, 0 },
            //    { 0, 0, 0, 1}
            //    };

            ////turn in -y direction
            //double[,] FlipMatrix =
            //   {
            //    { 0, 1, 0, 0 },
            //    { -1, 0, 0, 0 },
            //    { 0, 0, 1, 0 },
            //    { 0, 0, 0, 1}
            //    };

            //////turn in z direction
            //double[,] FlipMatrix =
            //   {
            //    { 0, 0, 1, 0 },
            //    { 0, 1, 0, 0 },
            //    { -1, 0, 0, 0 },
            //    { 0, 0, 0, 1}
            //    };

            ////turn in -z direction
            //double[,] FlipMatrix =
            //   {
            //    { 0, 0, -1, 0 },
            //    { 0, 1, 0, 0 },
            //    { 1, 0, 0, 0 },
            //    { 0, 0, 0, 1}
            //    };

            //flip solid to correct orientation
            //solidOG = solidOG.TransformToGetNewSolid(FlipMatrix);
            //put the solid at the origin 
            var solid1 = solidOG.SetToOriginAndSquareTesselatedSolid(out backTransform);
            //Presenter.ShowAndHang(solid1);

            //define the number of slices desired and creates small slices, dx
            var Xmax = solid1.XMax;
            var Xmin = solid1.XMin;
            //cutting uniform solids
            var dx = 1; //uniform length of square
            var nxdec = (Xmax - Xmin) / dx;
            var nxslices = Math.Floor(nxdec);

            //create arrays to store info in
            List<double> Cplist = new List<double>();
            List<double> Cnlist = new List<double>();
            List<double> Vplist = new List<double>();
            List<double> Vnlist = new List<double>();
            List<double> deltaCp = new List<double>();
            List<double> deltaCn = new List<double>();

            for (var k = 1; k < nxslices; k++)
            {
                Console.WriteLine("The k iteration is{0}, the nxslices is {0}+1, the total nxslices is{0}", k, nxslices);
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
                    //negXsolid.Add(0);
                    Console.WriteLine("Display all negative solids at X1 slice at xmax");
                    //Presenter.ShowAndHang(negativeSolidsXslice1);
                }
                //add these conditions back in later
                //else if (k == nxslices)
                //{
                //    positiveSolidsXslice2.Add(solid2);
                //    //negativeSolidsXslice2.Add(solid2);
                //    Console.WriteLine("Display original negative solid from X1 at xmin, empty for next cost calculation");
                //    //Presenter.ShowAndHang(negativeSolidsXslice2);
                //}
                ////returns solid with second cut at xmin
                //else if (X2 < solid2.XMin)
                //{
                //    positiveSolidsXslice2.Add(solid2);
                //    //negativeSolidsXslice2.Add(solid2);
                //    Console.WriteLine("Display negative solids after XMIN of new solid>second x slice");
                //    //Presenter.ShowAndHang(positiveSolidsXslice2);
                //}
                //else if (Math.Abs(X2 - solid2.XMin) < 0.001)
                //{
                //    positiveSolidsXslice2.Add(solid2);
                //    //negativeSolidsXslice2.Add(solid2);
                //    Console.WriteLine("Display original negative solid from X1 when last slice is very small");
                //    //Presenter.ShowAndHang(positiveSolidsXslice2);
                //}
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

         
        
            for (var m = 1; m<deltaCn.Count; m++)
            {

                deltaCp.Add(Cplist[m] - Cplist[m + 1]);
                deltaCn.Add(Cnlist[m + 1] - Cnlist[m]);


            }

        //generate excel graphs of the data
        //TVGLTest.ExcelInterface.CreateNewGraph(new List<List<double[]>> { deltaCn }, filename, string.Format("Xposition"), string.Format("(C2-C1)/(V2-V1)"));

    Console.WriteLine("Completed.");
            Console.ReadKey();

        }

    }
}
    

