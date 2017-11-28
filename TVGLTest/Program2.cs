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
            var filename = "../../../TestFiles/sth2.stl";
            //open file with TessellatedSolid function
            Console.WriteLine("Attempting: " + filename);
            List<TessellatedSolid> solids = IO.Open(filename);

            //create list where all solids will be saved
            List<TessellatedSolid> volumeslist = new List<TessellatedSolid>();

            //define solid
            var solid = solids[0];
          
            
                var Xmax = solid.XMax;
                var Xmin = solid.XMin;
                //define array of directions
               // double[,] dirarray = { { 1.0, 0, 0 }, { 0, 1.0, 0 }, { 0, 0, 1.0 } };

                //define the number of slices desired and creates small slices, dx
                    double nxslices = 5;
                    double dx = (Xmax - Xmin) / nxslices;

            for (var k = 1; k <= nxslices; k++)
            {
                double X = Xmin + k * dx;

                Slice.OnFlat(solid,
                    new Flat(new[] { X, 0, 0 }, new[]
                    { 1.0,0,0 }),
                    out List<TessellatedSolid> positiveSolids,
                    out List<TessellatedSolid> negativeSolids);

                //cut each solid in negativeSolids in y direction
                foreach (TessellatedSolid smsolid in negativeSolids)
                {

                    //get new min and max of new solids and then iterate through them
                    var Ymaxsm = smsolid.YMax;
                    var Yminsm = smsolid.YMin;

                    //create size of slices
                    double nyslices = 5;
                    double dy = (Ymaxsm - Yminsm) / nyslices;
                    //create set of y values to iterate through


                    for (var m = 1; m <= nyslices; m++)
                    {
                        double Y = Yminsm + m * dy;
                        Slice.OnFlat(smsolid,
                        new Flat(new[] { 0, Y, 0 }, new[]
                        { 0,1.0,0}),
                        out List<TessellatedSolid> positiveSolidsYslice,
                        out List<TessellatedSolid> negativeSolidsYslice);
                        Console.WriteLine("Display negative solids after a y slice");
                        Presenter.ShowAndHang(negativeSolidsYslice);

                        //run through the z direction
                        foreach (TessellatedSolid xsmsolid in negativeSolidsYslice)
                        {

                            //get new min and max of new solids and then iterate through them
                            var Zmaxxsm = xsmsolid.ZMax;
                            var Zminxsm = xsmsolid.ZMin;

                            //create size of slices
                            double nzslices = 10;
                            double dz = (Zmaxxsm - Zminxsm) / nzslices;
                            //create set of z values to iterate through

                            for (var n = 1; n <= nzslices; n++)
                            {
                                double Z1= Zminxsm + n * dz;
                                Slice.OnFlat(xsmsolid,
                                new Flat(new[] { 0, 0, Z1}, new[]
                                { 0,0,1.0}),
                                out List<TessellatedSolid> positiveSolidsZslice1,
                                out List<TessellatedSolid> negativeSolidsZslice1);
                                //Console.WriteLine("Display negative solids after another z1slice");
                                
                                //Presenter.ShowAndHang(negativeSolidsZslice1);
                                foreach (TessellatedSolid xxsmallsolid in negativeSolidsZslice1)
                                {
                                    double Z2 = Zminxsm + (n - 1) * dz;
                                    Slice.OnFlat(xxsmallsolid,
                                    new Flat(new[] { 0, 0, Z2 }, new[]
                                    { 0,0,1.0}),
                                    out List<TessellatedSolid> positiveSolidsZslice2,
                                    out List<TessellatedSolid> negativeSolidsZslice2);
                                    Console.WriteLine("Display negative solids after another z2slice");
                                    Presenter.ShowAndHang(positiveSolidsZslice2);

                                    foreach (TessellatedSolid finalsolid in positiveSolidsZslice2)
                                    {
                                        //save solids to a list
                                        volumeslist.Add(finalsolid);
                                    }
                                
                                }
                            }
                        }
                    }





                }



            }
            Console.WriteLine("Completed.");
            Console.ReadKey();
        }
    }
}