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
            //cutting uniform solids
            var dx = 10; //uniform length of square
            var nxdec = (Xmax - Xmin) / dx;
            var nxslices = Math.Floor(nxdec);

            for (var k = 0; k <= nxslices; k++)
            {
                //create x location of slice
                double X1 = Xmin + (k+1) * dx;
                //conditional statement for first x cut
                List<TessellatedSolid> negativeSolidsXslice1 = new List<TessellatedSolid>();
                if (k==nxslices)
                {
                    //returns entire solid after xmax
                    negativeSolidsXslice1.Add(solid1);
                    Console.WriteLine("Display negative solids after XMAX");
                    //Presenter.ShowAndHang(negativeSolidsXslice1);
                }
                else if (Math.Abs(X1-Xmax)<.00001)
                {
                    //returns entire solid at xmax
                    negativeSolidsXslice1.Add(solid1);
                    Console.WriteLine("Display negative solids at XMAX");
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
                    //conditional statement for second x cut
                    double X2 = Xmin + k * dx;
                    List<TessellatedSolid> positiveSolidsXslice2 = new List<TessellatedSolid>();
                    //returns solid without second cut at xmin
                    if(k==0)
                    {
                        positiveSolidsXslice2.Add(solid2);
                        Console.WriteLine("Display negative solids after XMIN");
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
                    }


                    //cut each x cut solid in y direction
                    foreach (TessellatedSolid smsolid in positiveSolidsXslice2)
                    {

                        //on new solids: define the number of slices desired and creates small slices, dy
                        var Ymaxsm = smsolid.YMax;
                        var Yminsm = smsolid.YMin;
                        //cutting uniform solids
                        var dy = 10; //uniform length of square
                        var nydec = (Ymaxsm - Yminsm) / dy;
                        var nyslices = Math.Ceiling(nydec);


                        for (var m = 0; m < nyslices; m++)
                        {
                            //create location of first y cut
                            double Y1 = Yminsm + (m+1) * dy;
                            //conditional statement for first y cut
                            List<TessellatedSolid> negativeSolidsYslice1 = new List<TessellatedSolid>();
                            if(Y1>=Ymaxsm)
                            {
                                //returns entire solid greater than ymax
                                negativeSolidsYslice1.Add(smsolid);
                                Console.WriteLine("Display negative solids greater than YMAX");
                                //Presenter.ShowAndHang(negativeSolidsYslice1);
                            }
                         
                            else
                                //returns negative solids after first y cut
                                Slice.OnFlat(smsolid,
                            new Flat(new[] { 0, Y1, 0 }, new[]
                            { 0,1.0,0}),
                            out List<TessellatedSolid> positiveSolidsYslice1,
                            out negativeSolidsYslice1);
                            Console.WriteLine("Display negative solids after a Y1 slice");
                            //Presenter.ShowAndHang(negativeSolidsYslice1);

                            foreach (TessellatedSolid smsolid2 in negativeSolidsYslice1)
                            {
                                //conditional statement for second y cut
                                double Y2 = Yminsm + m * dy;
                                List<TessellatedSolid> positiveSolidsYslice2 = new List<TessellatedSolid>();
                                if (m==0) 
                                {
                                    //returns entire solid with second cut at ymin
                                    positiveSolidsYslice2.Add(smsolid2);
                                    Console.WriteLine("Display negative solids after YMIN");
                                   //Presenter.ShowAndHang(positiveSolidsYslice2);
                                }
                                else if (Y2 < smsolid2.YMin)
                                {
                                    //returns entire positive solid for smaller solid than set distance
                                    positiveSolidsYslice2.Add(smsolid2);
                                    Console.WriteLine("Display negative solids after YMIN of new solid<second y slice");
                                    //Presenter.ShowAndHang(positiveSolidsYslice2);
                                }
                                else if (Y2 >= smsolid2.YMax)
                                {
                                    //returns entire positive solid for smaller solid than set distance
                                    
                                    Console.WriteLine("Display negative solids after YMIN of new solid<second y slice");
                                    //Presenter.ShowAndHang(positiveSolidsYslice2);
                                }
                                else
                                {
                                    //returns positive solids after second y cut
                                    Slice.OnFlat(smsolid2,
                                    new Flat(new[] { 0, Y2, 0 }, new[]
                                    { 0,1.0,0}),
                                    out positiveSolidsYslice2,
                                    out List<TessellatedSolid> negativeSolidsYslice2);
                                    Console.WriteLine("Display positive solids after another Y2slice");
                                    //Presenter.ShowAndHang(positiveSolidsYslice2);
                                }
                                //cut each xy cut solid in z direction
                                foreach (TessellatedSolid xsmsolid in positiveSolidsYslice2)
                                {

                                    //on new solids: define the number of slices desired and creates small slices, dz
                                    var Zmaxxsm = xsmsolid.ZMax;
                                    var Zminxsm = xsmsolid.ZMin;
                                    //cutting uniform solids
                                    var dz = 10; //uniform length of square
                                    var nzdec = (Zmaxxsm - Zminxsm) / dz;
                                    var nzslices = Math.Ceiling(nzdec);
                                    Console.WriteLine("nzslices={0}", nzslices);
                                    
                             
                                        for (var n = 0; n < nzslices; n++)
                                    {
                                        //create z location of slice
                                        double Z1 = Zminxsm + (n+1) * dz;
                                        List<TessellatedSolid> negativeSolidsZslice1 = new List<TessellatedSolid>();
                                        //conditional statement at first z cut
                                     
                                        if (Z1 >= Zmaxxsm)
                                        {
                                            //returns entire solid due to being greater than Zmax
                                            negativeSolidsZslice1.Add(xsmsolid);
                                            Console.WriteLine("Display negative solids greater ZMAX");
                                            //Presenter.ShowAndHang(negativeSolidsZslice1);

                                        }

                                        else
                                        {
                                            //returns negatives solids after first z cut
                                            Slice.OnFlat(xsmsolid,
                                            new Flat(new[] { 0, 0, Z1 }, new[]
                                            { 0,0,1.0}),
                                            out List<TessellatedSolid> positiveSolidsZslice1,
                                            out negativeSolidsZslice1);
                                            Console.WriteLine("Display negative solids after another z1slice");
                                            // Presenter.ShowAndHang(negativeSolidsZslice1);
                                        }
                                        foreach (TessellatedSolid xsmallsolid2 in negativeSolidsZslice1)
                                        {
                                            //conditional statement for second z cut
                                            double Z2 = Zminxsm + n * dz;
                                            List<TessellatedSolid> positiveSolidsZslice2 = new List<TessellatedSolid>();
                                            if (n == 0)
                                            {
                                                //returns entire solid with second cut at zmin
                                                positiveSolidsZslice2.Add(xsmallsolid2);
                                                Console.WriteLine("Display negative solids after ZMIN");
                                                // Presenter.ShowAndHang(positiveSolidsZslice2);
                                            }
                                            else if (Z2 < xsmallsolid2.ZMin)
                                            {
                                                //returns entire solid with solid smaller than entire distance
                                                positiveSolidsZslice2.Add(xsmallsolid2);
                                                Console.WriteLine("Display negative solids after ZMIN of new solid<second z slice");
                                                //Presenter.ShowAndHang(positiveSolidsZslice2);
                                            }
                                            else if (Z2 >= xsmallsolid2.ZMax)
                                            {//returns entire solid with solid smaller than entire distance

                                                Console.WriteLine("Returns void since cut is over no space on Z2");
                                                //Presenter.ShowAndHang(positiveSolidsZslice2);
                                            }
                                            else
                                            {
                                                //returns positive solids after second y cut
                                                Slice.OnFlat(xsmallsolid2,
                                                new Flat(new[] { 0, 0, Z2 }, new[]
                                                { 0,0,1.0}),
                                                out positiveSolidsZslice2,
                                                out List<TessellatedSolid> negativeSolidsZslice2);
                                                Console.WriteLine("Display positive solids after another z2slice");
                                                //Presenter.ShowAndHang(positiveSolidsZslice2);
                                            }
                                            foreach (TessellatedSolid finalsolid in positiveSolidsZslice2)
                                            {
                                                //save solids to a list
                                                volumeslist.Add(finalsolid);

                                                Console.WriteLine("Display positive solids FINAL");
                                                //Presenter.ShowAndHang(volumeslist);
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
                Console.WriteLine("Completed.");
                Console.ReadKey();
            
        }
    }
}