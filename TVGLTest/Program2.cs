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
            
            //define the number of slices desired and creates small slices, dx
            var Xmax = solid1.XMax;
            var Xmin = solid1.XMin;
            double nxslices = 5;
            double dx1 = (Xmax - Xmin) / nxslices;
            double dx = Math.Abs(dx1);

            for (var k = 1; k <= nxslices; k++)
            {
                //create x location of slice
                double X1 = Xmin + k * dx;
                //conditional statement for first x cut
                List<TessellatedSolid> negativeSolidsXslice1 = new List<TessellatedSolid>();
                if (Math.Abs(X1 - Xmax) < .000000001)
                {
                    //returns entire solid at xmax
                    negativeSolidsXslice1.Add(solid1);
                    //Console.WriteLine("Display negative solids after XMAX");
                    //Presenter.ShowAndHang(negativeSolidsXslice1);
                }
                else if (X1>Xmax)
                {
                    //Console.WriteLine("got x problem");
                }
                else
                {
                    //returns negative solids after first x cut
                    List<TessellatedSolid> positiveSolids;
                    Slice.OnFlat(solid1,
                    new Flat(new[] { X1, 0, 0 }, new[]
                    { 1.0,0,0 }),
                    out positiveSolids,
                    out negativeSolidsXslice1);
                    //Console.WriteLine("Display negative solids after X1 slice");
                    //Presenter.ShowAndHang(negativeSolidsXslice1);
                }
                foreach (TessellatedSolid solid2 in negativeSolidsXslice1)
                {
                    GetCostModels.ForAllBlankTypes(solid2);

                    //conditional statement for second x cut
                    double X2 = Xmin + (k - 1) * dx;
                    List<TessellatedSolid> positiveSolidsXslice2 = new List<TessellatedSolid>();
                    //returns solid without second cut at xmin
                    if(Math.Abs(X2 - Xmin) < .000000001)
                    {
                        positiveSolidsXslice2.Add(solid2);
                        //Console.WriteLine("Display negative solids after XMIN");
                        //Presenter.ShowAndHang(positiveSolidsXslice2);
                    }
                    //returns solid with second cut at xmin
                    else if (X2 < solid2.XMin)
                    {
                        positiveSolidsXslice2.Add(solid2);
                        //Console.WriteLine("Display negative solids after XMIN of new solid>second x slice");
                        //Presenter.ShowAndHang(positiveSolidsXslice2);
                    }
                    //returns positive solids after second x cut
                    else
                    {
                        List<TessellatedSolid> negativeSolidsXslice2;
                        Slice.OnFlat(solid2,
                        new Flat(new[] { X2, 0, 0 }, new[]
                        { 1.0,0,0}),
                        out positiveSolidsXslice2,
                        out negativeSolidsXslice2);
                        //Console.WriteLine("Display positive solids after another X2slice");
                        //Presenter.ShowAndHang(positiveSolidsXslice2);
                    }


                    //cut each x cut solid in y direction
                    foreach (TessellatedSolid smsolid in positiveSolidsXslice2)
                    {

                        //on new solids: define the number of slices desired and creates small slices, dy
                        var Ymaxsm = smsolid.YMax;
                        var Yminsm = smsolid.YMin;
                        double nyslices = 5;
                        double dy1 = (Ymaxsm - Yminsm) / nyslices;
                        double dy = Math.Abs(dy1);

    
                        for (var m = 1; m <= nyslices; m++)
                        {
                            //create location of first y cut
                            double Y1 = Yminsm + m * dy;
                            //conditional statement for first y cut
                            List<TessellatedSolid> negativeSolidsYslice1 = new List<TessellatedSolid>();
                            if (Math.Abs(Y1 - Ymaxsm) < .000000001)
                            {
                                //returns entire solid at ymax
                                negativeSolidsYslice1.Add(smsolid);
                                //Console.WriteLine("Display negative solids after YMAX");
                                //Presenter.ShowAndHang(negativeSolidsYslice1);
                            }
                            else
                            {
                                List<TessellatedSolid> positiveSolidsYslice1;
                                //returns negative solids after first y cut
                                Slice.OnFlat(smsolid,
                                    new Flat(new[] { 0, Y1, 0 }, new[]
                                    { 0,1.0,0}),
                                    out positiveSolidsYslice1,
                                    out negativeSolidsYslice1);
                                //Console.WriteLine("Display negative solids after a Y1 slice");
                                //Presenter.ShowAndHang(negativeSolidsYslice1);
                            }
                                

                            foreach (TessellatedSolid smsolid2 in negativeSolidsYslice1)
                            {
                                //conditional statement for second y cut
                                double Y2 = Yminsm + (m - 1) * dy;
                                List<TessellatedSolid> positiveSolidsYslice2 = new List<TessellatedSolid>();
                                if (Math.Abs(Y2 - Yminsm) < .000000001) 
                                {
                                    //returns entire solid with second cut at ymin
                                    positiveSolidsYslice2.Add(smsolid2);
                                    //Console.WriteLine("Display negative solids after YMIN");
                                   // Presenter.ShowAndHang(positiveSolidsYslice2);
                                }
                                else if (Y2 < smsolid2.YMin)
                                {
                                    //returns entire positive solid for smaller solid than set distance
                                    positiveSolidsYslice2.Add(smsolid2);
                                    //Console.WriteLine("Display negative solids after YMIN of new solid<second y slice");
                                    //Presenter.ShowAndHang(positiveSolidsYslice2);
                                }
                                else
                                {
                                    List<TessellatedSolid> negativeSolidsYslice2;
                                    //returns positive solids after second y cut
                                    Slice.OnFlat(smsolid2,
                                    new Flat(new[] { 0, Y2, 0 }, new[]
                                    { 0,1.0,0}),
                                    out positiveSolidsYslice2,
                                    out negativeSolidsYslice2);
                                    //Console.WriteLine("Display positive solids after another Y2slice");
                                    //Presenter.ShowAndHang(positiveSolidsYslice2);
                                }
                                //cut each xy cut solid in z direction
                                foreach (TessellatedSolid xsmsolid in positiveSolidsYslice2)
                                {

                                    //on new solids: define the number of slices desired and creates small slices, dz
                                    var Zmaxxsm = xsmsolid.ZMax;
                                    var Zminxsm = xsmsolid.ZMin;
                                    double nzslices = 20;
                                    double dz1 = (Zmaxxsm - Zminxsm) / nzslices;
                                    double dz = Math.Abs(dz1);
                                    
                                    for (var n = 1; n <= nzslices; n++)
                                    {
                                        //create z location of slice
                                        double Z1 = Zminxsm + n * dz;
                                        List<TessellatedSolid> negativeSolidsZslice1 = new List<TessellatedSolid>();
                                        //conditional statement at first z cut
                                        if (Math.Abs(Z1-Zmaxxsm)<.000000001)
                                        {
                                            //returns entire solid
                                            negativeSolidsZslice1.Add(xsmsolid);
                                            //Console.WriteLine("Display negative solids after ZMAX");
                                            //Presenter.ShowAndHang(negativeSolidsZslice1);
                                        }
                                        else
                                        {
                                            //returns negatives solids after first z cut
                                            List<TessellatedSolid> positiveSolidsZslice1;
                                            Slice.OnFlat(xsmsolid,
                                            new Flat(new[] { 0, 0, Z1 }, new[]
                                            { 0,0,1.0}),
                                            out positiveSolidsZslice1,
                                            out negativeSolidsZslice1);
                                            //Console.WriteLine("Display negative solids after another z1slice");
                                           // Presenter.ShowAndHang(negativeSolidsZslice1);
                                        }
                                        foreach (TessellatedSolid xsmallsolid2 in negativeSolidsZslice1)
                                        {
                                            //conditional statement for second z cut
                                            double Z2 = Zminxsm + (n - 1) * dz;
                                            List<TessellatedSolid> positiveSolidsZslice2 = new List<TessellatedSolid>();
                                            if (Math.Abs(Z2 - Zminxsm) < .00001)
                                                {
                                                //returns entire solid with second cut at zmin
                                                positiveSolidsZslice2.Add(xsmallsolid2);
                                                //Console.WriteLine("Display negative solids after ZMIN");
                                               // Presenter.ShowAndHang(positiveSolidsZslice2);
                                            }
                                            else if (Z2 < xsmallsolid2.ZMin)
                                            { 
                                                //returns entire solid with solid smaller than entire distance
                                                positiveSolidsZslice2.Add(xsmallsolid2);
                                                //Console.WriteLine("Display negative solids after ZMIN of new solid<second z slice");
                                               //Presenter.ShowAndHang(positiveSolidsZslice2);
                                            }

                                            else
                                            {
                                                //returns positive solids after second y cut
                                                List<TessellatedSolid> negativeSolidsZslice2;
                                                Slice.OnFlat(xsmallsolid2,
                                                new Flat(new[] { 0, 0, Z2 }, new[]
                                                { 0,0,1.0}),
                                                out positiveSolidsZslice2,
                                                out negativeSolidsZslice2);
                                                //Console.WriteLine("Display positive solids after another z2slice");
                                                //Presenter.ShowAndHang(positiveSolidsZslice2);
                                            }
                                            foreach (TessellatedSolid finalsolid in positiveSolidsZslice2)
                                            {
                                                //save solids to a list
                                                volumeslist.Add(finalsolid);
                                                //Console.WriteLine("Display positive solids FINAL");
                                                //Presenter.ShowAndHang(positiveSolidsZslice2);
                                            }

                                        }
                                    }
                                }
                            }
                        }





                    }



                }
               
                

            }
                 Presenter.ShowAndHang(volumeslist);
                //Console.WriteLine("Completed.");
                Console.ReadKey();
            
        }
    }
}