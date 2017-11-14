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

            //Defines new variables to be displayed
            List<TessellatedSolid> positiveSolids;
            List<TessellatedSolid> negativeSolids;

            //(solids[0]) TVGLFileData.Center : double[];

           // public double[] Center;
            //Console.WriteLine(


        //TRY SOMETHING ELSE

        //    public TessellatedSolid(TVGLFileData fileData, string fileName) : this(fileData.Units, fileData.Name, fileName,
        //            fileData.Comments, fileData.Language)
        //{
        //        XMax = fileData.XMax;
        //        XMin = fileData.XMin;
        //        YMax = fileData.YMax;
        //        YMin = fileData.YMin;
        //        ZMax = fileData.ZMax;
        //        ZMin = fileData.ZMin;
        //        Center = fileData.Center;






        //Console.WriteLine(Center);



        /// <summary>

        Slice.OnFlat(solids[0],
                new Flat(new[] { 123.0, 14, 43 },
                new[] { -1.0, 0, 0 }),
                out positiveSolids,
                out negativeSolids);

            Presenter.ShowAndHang(positiveSolids);
            Presenter.ShowAndHang(negativeSolids);
            Console.WriteLine("Completed.");
            Console.ReadKey();
        }
    }
}