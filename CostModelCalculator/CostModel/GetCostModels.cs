using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericInputs;
using KatanaObjects.BaseClasses;
using KatanaObjects.Blanks;
using KatanaObjects.CostModels;
using KatanaObjects.Processes;
using TVGL;
using UnitsNet;

namespace CostModelCalculator
{
    public class GetCostModels
    {
        private static void Main(string[] args)
        {

        }

        public static void ForAllBlankTypes(TessellatedSolid ts)
        {
            ts.Units = UnitType.millimeter; //Guess

            var blankTypes = new HashSet<BlankType>(Enum.GetValues(typeof(BlankType)).Cast<BlankType>());
            blankTypes.Remove(BlankType.HollowTube);
            blankTypes.Remove(BlankType.CircularBarStock);
            
            //Gets all the search inputs from the Generic Inputs Project
            var searchInputs = new SearchInputs(); 
            var costFactory = new CostModelFactory(searchInputs);

            //Get the stock volume information for every type of Blank
            var subvolume = new SubVolume(ts, blankTypes, searchInputs);

            //ToDo: Add other blank types.
            Debug.Print(GetCostOfRectangularBarStock(subvolume, costFactory).Dollars.ToString(CultureInfo.InvariantCulture));
        }

        private static Cost GetCostOfRectangularBarStock(SubVolume subVolume, ICostModelFactory costFactory)
        {
            var rbs = Blank.Create(BlankType.RectangularBarStock, subVolume, costFactory);
            var process = Machining.Create(costFactory, rbs.FinishVolume, subVolume.SurfaceArea, rbs.StockVolume);
            //Cost of material + cost of machining
            return rbs.TotalCost + process.TotalCost;
        }
    }
}
