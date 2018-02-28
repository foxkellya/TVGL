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

        public static double ForAllBlankTypes(TessellatedSolid ts)
        {
            ts.Units = UnitType.millimeter; //Guess

            var blankTypes = new HashSet<BlankType>(Enum.GetValues(typeof(BlankType)).Cast<BlankType>());
            blankTypes.Remove(BlankType.HollowTube);
            blankTypes.Remove(BlankType.CircularBarStock);
            blankTypes.Remove(BlankType.WaterJetPlate);

            //Gets all the search inputs from the Generic Inputs Project
            var searchInputs = new SearchInputs(); 
            var costFactory = new CostModelFactory(searchInputs);

            //Get the stock volume information for every type of Blank
            var subvolume = new SubVolume(ts, blankTypes, searchInputs);

            //Current Blank Type Settings:
            //Forging is currently only considering one direction. This value is set in
            //searchInputs.Forging.NumberOfOBBDirectionsToConsider.
            //Forging is invalid if smallest OBB dimension < 2 * searchInputs.Forging.TopCover
            //Additive is invalid if smallest OBB dimension < 2 * searchInputs.WireFeed.WireAccuracy
            //Rectangular Bar stock should be valid for as long as the OBB is valid (All OBB dimensions > 0).
            var minCost = Cost.MaxValue;
            var minCostBlank = BlankType.RectangularBarStock; //Need to initialize.
            foreach (var blankType in blankTypes)
            {
                var dollars = GetCostOfStock(subvolume, costFactory, blankType);
                if (dollars < minCost)
                {
                    minCost = dollars;
                    minCostBlank = blankType;
                }
            }
            Debug.Print("Min Cost Blank: " + minCostBlank + "  $" + minCost.Dollars);
            return minCost.Dollars;
        }

        public static double ForGivenBlankType(List<TessellatedSolid> solids, BlankType blankType)
        {
            return solids.Sum(solid => ForGivenBlankType(solid, blankType));
        }

        public static double ForGivenBlankType(TessellatedSolid ts, BlankType blankType)
        {
            ts.Units = UnitType.millimeter; //Guess

            //Gets all the search inputs from the Generic Inputs Project
            var searchInputs = new SearchInputs();
            var costFactory = new CostModelFactory(searchInputs);

            //Get the stock volume information for every type of Blank
            var subvolume = new SubVolume(ts, new HashSet<BlankType> { blankType}, searchInputs);

            //Current Blank Type Settings:
            //Forging is currently only considering one direction. This value is set in
            //searchInputs.Forging.NumberOfOBBDirectionsToConsider.
            //Forging is invalid if smallest OBB dimension < 2 * searchInputs.Forging.TopCover
            //Additive is invalid if smallest OBB dimension < 2 * searchInputs.WireFeed.WireAccuracy
            //Rectangular Bar stock should be valid for as long as the OBB is valid (All OBB dimensions > 0).
            var cost = GetCostOfStock(subvolume, costFactory, blankType);
            //Debug.Print("Min Cost Blank: " + blankType + "  $" + cost.Dollars + "  Volume: " + subvolume.RectangularBlankVolume.CubicInches);
            return cost.Dollars;
        }

        private static Cost GetCostOfStock(SubVolume subVolume, ICostModelFactory costFactory, BlankType blankType)
        {
            var blank = Blank.Create(blankType, subVolume, costFactory);
            if (!blank.IsFeasible)
            {
                Debug.WriteLine("Blank is invalid. Geometry/cost will be innacurate");
                return Cost.MaxValue;
            }
            var process = Machining.Create(costFactory, blank.FinishVolume, subVolume.SurfaceArea, blank.StockVolume);
            //Cost of material + cost of machining
            return blank.TotalCost + process.TotalCost;
        }
    }
}
