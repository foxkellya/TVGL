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

        public static double ForAllBlankTypes(TessellatedSolid originalSolid, TessellatedSolid ts, 
            double[] originalBuildDirection)
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
            var subvolume = new SubVolume(originalSolid, ts, blankTypes, searchInputs, originalBuildDirection);

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

        public static double ForGivenBlankType(TessellatedSolid originalSolid, List<TessellatedSolid> solids, 
            BlankType blankType, double[] originalBuildDirection)
        {
            return solids.Sum(solid => ForGivenBlankType(originalSolid, solid, blankType, originalBuildDirection));
        }

        public static double ForGivenBlankType(TessellatedSolid originalSolid, TessellatedSolid ts, BlankType blankType,
            double[] originalBuildDirection)
        {
            ts.Units = UnitType.millimeter; //Guess

            //Gets all the search inputs from the Generic Inputs Project
            var searchInputs = new SearchInputs {Forging = {NumberOfOBBDirectionsToConsider = Multiplier.FromUnitless(3)}};
            var costFactory = new CostModelFactory(searchInputs);

            //Get the stock volume information for every type of Blank
            var subvolume = new SubVolume(originalSolid, ts, new HashSet<BlankType> { blankType}, searchInputs,
                originalBuildDirection);

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

        public static double ForGivenBlankType(TessellatedSolid originalSolid, TessellatedSolid ts, BlankType blankType,
            out double[] buildDirection)
        {
            ts.Units = UnitType.millimeter; //Guess
            var searchInputs = new SearchInputs { Forging = { NumberOfOBBDirectionsToConsider = Multiplier.FromUnitless(3) } };
            var costFactory = new CostModelFactory(searchInputs);
            var subvolume = new SubVolume(originalSolid, ts, new HashSet<BlankType> { blankType }, searchInputs, null);
            var cost = GetCostOfStock(subvolume, costFactory, blankType);

            if (blankType == BlankType.ClosedDieForging)
            {
                buildDirection = subvolume.ForgingStrokeDirection;
            }
            else if (blankType == BlankType.RectangularBarStock)
            {
                buildDirection = subvolume.RectangularBlankCrossSectionBuildDirection;
            }
            else if (blankType == BlankType.NearNetAdditive)
            {
                buildDirection = subvolume.AdditiveBuildDirection;
            }
            else throw new NotImplementedException();

            return cost.Dollars;
        }

        private static Cost GetCostOfStock(SubVolume subVolume, ICostModelFactory costFactory, BlankType blankType)
        {
            var blank = Blank.Create(blankType, subVolume, costFactory);
            //Presenter.ShowAndHang(blank.GetStockMaterialSolid());

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
