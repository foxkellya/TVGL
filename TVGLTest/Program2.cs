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

            //pull max and mins from the solid
            var solid = solids[0];//define solid
            var Xmax = solid.XMax;
            var Ymax = solid.YMax;
            var Zmax = solid.ZMax;
            var Xmin = solid.XMin;
            var Ymin = solid.YMin;
            var Zmin = solid.ZMin;

            //define the number of slices desired and creates small slices, dx,dy,dz
            double nslices = 5;
            double dx = (Xmax - Xmin) / nslices;
            double dy = (Ymax - Ymin) / nslices;
            double dz = (Zmax - Zmin) / nslices;




            for (var d = 0; d <= 2; d++) //iterates though directions in x, y,z
            {
                //define array of directions
                double[,] dirarray = { { 1.0, 0, 0 }, { 0, 1.0, 0 }, { 0, 0, 1.0 } };
                

                for (var k = 1; k <= nslices; k++)
                {
                    double X = Xmin + k * dx;
                    double Y = Ymin+ k*dy;
                    double Z = Zmin + k * dz;

                    Slice.OnFlat(solid,
                        new Flat(new[] { X, Y, Z }, new[]
                        { dirarray[d,0],dirarray[d,1],dirarray[d,2] }),
                        out List<TessellatedSolid> positiveSolids,
                        out List<TessellatedSolid> negativeSolids);
                    Console.WriteLine(negativeSolids);
                    Console.WriteLine("Display negative solids after cut in X");
                    Presenter.ShowAndHang(negativeSolids);


                    foreach (TessellatedSolid ts in negativeSolids)
                    {
                        //idea: get new min and max of new solids and then iterate through them
                        var Xmaxts = ts.XMax;
                        var Ymaxts = ts.YMax;
                        var Zmaxts = ts.ZMax;
                        var Xmints = ts.XMin;
                        var Ymints = ts.YMin;
                        var Zmints = ts.ZMin;
                        Console.WriteLine(Zmints);
                        Presenter.ShowAndHang(ts);

                        double Yhalf = (Ymints+Ymaxts)/2;
                        Slice.OnFlat(ts,
                        new Flat(new[] { 0, Yhalf, 0 }, new[]
                        { 0,1.0,0}),
                        out List<TessellatedSolid> positiveSolidsYslice,
                        out List<TessellatedSolid> negativeSolidsYslice);
                        Console.WriteLine("Display negative solids after another y slice");
                        Presenter.ShowAndHang(negativeSolidsYslice);
                    }

                    
                   


                }



            }
            Console.WriteLine("Completed.");
            Console.ReadKey();
        }
    }
}