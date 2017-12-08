using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KatanaObjects.BaseClasses;
using KatanaObjects.CostModels;
using KatanaObjects.Processes;
using TVGL;
using UnitsNet;

namespace KatanaObjects.Blanks
{
    [DataContract]
    [KnownType(typeof (BlankType))]
    public abstract class Blank : ICostModel
    {
        private Lazy<TessellatedSolid> _stockMaterialSolid;

        #region Public Properties
        [DataMember]
        public BlankType Type { get; set; }

        [DataMember]
        public string PartName { get; set; }

        //Can not be serialized because of lazy properties and solid geometry
        public SubVolume SubVolume { get; set; }

        //Cannot currently be serialized
        public TessellatedSolid StockMaterialSolid
        {
            get
            {
                //_stockMaterialSolid can get set to null during saving / loading
                if (_stockMaterialSolid == null)
                {
                    //Reset the lazy instance as in the constructor
                    _stockMaterialSolid = new Lazy<TessellatedSolid>(GetStockMaterialSolid);
                }
                //Either way, return the _stockMaterialSolid.Value
                return _stockMaterialSolid.Value;
            } 
        } 

        //If this blank is an 
        [DataMember]
        public List<Blank> AssemblyBlanks { get; set; }

        [DataMember]
        public int IndexNewBlanks { get; set; }

        [DataMember]
        public int SubvolumeIndex { get; set; }

        [DataMember]
        public int IndexAllBlanks { get; set; }

        [DataMember]
        public double[] CrossSectionBuildDirection { get; set; }

        [DataMember]
        public Length CrossSectionBuildDistance { get; set; }

        //Cross sections are not saved because they take too much memory. //These cross sections avoid the need to re-find the forging/additive visual approximations when loading
        public List<List<List<double[]>>> CrossSections { get; set; }

        [DataMember]
        public Volume StockVolume { get; set; }

        [DataMember]
        public Volume WasteVolume { get; set; }

        [DataMember]
        public Volume FinishVolume { get; set; }

        [DataMember]
        public Area AreaOnCuttingPlane { get; set; }

        //Only used for near-net printed shape
        public TessellatedSolid SubstrateSolid { get; set; }

        [DataMember]
        public Length PerimeterOnPlane { get; set; }

        [DataMember]
        public bool AreaIsCircular { get; set; }

        [DataMember]
        public bool IsFeasible { get; set; }

        [DataMember]
        public bool VolumeIsUnderestimate { get; set; }

        [DataMember]
        public bool ChildrenAreCircularAndAligned { get; set; }

        [DataMember]
        public bool IsPartialAssembly { get; set; }

        [DataMember]
        public IList<List<Point>> ShapeOnCuttingPlanePreMachining { get; set; }

        [DataMember]
        public Length DistanceAlongJoinDirection { get; set; }

        //Can not be serialized. Set with SetCostModels() method.
        public ICostModel MachiningCostModelEstimate { get; set; }

        //Can not be serialized. Set with SetCostModels() method.
        public ICostModel MaterialCostModel { get; set; }

        #endregion

        #region Public Methods

        public Cost MachiningCostEstimate => MachiningCostModelEstimate?.TotalCost ?? Cost.Zero;

        public Cost MaterialCost => MaterialCostModel?.TotalCost ?? Cost.Zero;

        public Duration TotalTime => Duration.Zero;

        public Cost TotalCost => MaterialCost;
        #endregion

        #region Constructors
        //This internal constructor was added for the Blank Based Decomposition
        internal Blank()
        { 
        }

        internal Blank(SubVolume subVolume)
        {
            //If subvolume is null, then it is a partial assembly (more than one joining operation per decomposition).
            SubVolume = subVolume;
            _stockMaterialSolid = new Lazy<TessellatedSolid>(GetStockMaterialSolid);
        }

        public static Blank Create(BlankType blankType, SubVolume subVolume,
            ICostModelFactory costFactory)
        {
            Blank blank = null;
            switch (blankType)
            {
                case BlankType.WaterJetPlate:
                    blank = new WaterJetPlateBlank(subVolume);
                    break;

                case BlankType.CircularBarStock:
                    blank = new CircularBarStockBlank(subVolume);
                    break;

                case BlankType.RectangularBarStock:
                    blank = new RectangularBarStockBlank(subVolume);
                    break;

                case BlankType.ClosedDieForging:
                    blank = new ForgingBlank(subVolume);
                    break;

                case BlankType.NearNetAdditive:
                    blank = new NearNetPrintedShapeBlank(subVolume, costFactory.Inputs);
                    break;

                case BlankType.HollowTube:
                    blank = new HollowTubeBlank(subVolume);
                    break;
            }

            if (blank == null || !blank.IsFeasible) return blank;
            //blank.CostModel = costFactory.PostJoiningCostModel(blank);
            blank.MaterialCostModel = costFactory.MaterialCostModel(blank);

            //Get the machining cost model estimate
            var machiningEstimate = Machining.Create(costFactory, blank.FinishVolume, blank.SubVolume.SurfaceArea, blank.StockVolume, false);
            blank.MachiningCostModelEstimate = costFactory.MachiningCostModel(machiningEstimate, true);

            return blank;
        }

        public void SetCostModels(ICostModelFactory costFactory)
        {
            if (!IsFeasible) return;
            //CostModel = costFactory.PostJoiningCostModel(this);
            MaterialCostModel = costFactory.MaterialCostModel(this);
            
            //Get the machining cost model estimate
            var machiningEstimate = Machining.Create(costFactory, FinishVolume, SubVolume.SurfaceArea, StockVolume, false);
            MachiningCostModelEstimate = costFactory.MachiningCostModel(machiningEstimate, true);
        }

        #endregion

        public void SetCrossSections()
        {
            switch (Type)
            {
                case BlankType.WaterJetPlate:
                    CrossSections = SubVolume.WaterjetCrossSections;
                    break;

                case BlankType.CircularBarStock:
                    CrossSections = SubVolume.CircularBarStockCrossSections;
                    break;

                case BlankType.RectangularBarStock:
                    CrossSections = SubVolume.RectangularBlankCrossSections;
                    break;

                case BlankType.ClosedDieForging:
                    CrossSections = SubVolume.ForgingCrossSections;
                    break;

                case BlankType.NearNetAdditive:
                    CrossSections = SubVolume.AdditiveCrossSections;
                    break;

                case BlankType.HollowTube:
                    CrossSections = SubVolume.HollowTubeCrossSections;
                    break;
            }
        }

        public TessellatedSolid GetStockMaterialSolid()
        {
            if (CrossSections == null)
            {
                SetCrossSections();
            }
            var extrusionDistance = CrossSectionBuildDistance.TesselatedSolidBaseUnit(SubVolume.SolidUnitString);

            TessellatedSolid stockMaterialSolid = null;
            switch (Type)
            {
                case BlankType.WaterJetPlate:
                    stockMaterialSolid = SubVolume.WaterjetSolid;
                    break;

                case BlankType.CircularBarStock:
                    stockMaterialSolid = SubVolume.CircularBarStockSolid;
                    break;

                case BlankType.RectangularBarStock:
                    stockMaterialSolid = SubVolume.RectangularBlankSolid;
                    break;

                case BlankType.ClosedDieForging:
                    stockMaterialSolid = SubVolume.ForgingStockSolid;
                    break;

                case BlankType.NearNetAdditive:
                    stockMaterialSolid = SubVolume.AdditiveSolid;
                    break;

                case BlankType.HollowTube:
                    stockMaterialSolid = SubVolume.HollowTubeSolid;
                    break;
            }
            return stockMaterialSolid;
        }
    }
}
