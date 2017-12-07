using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GenericInputs;
using PropertyTools.DataAnnotations;
using StarMathLib;
using TVGL;
using TVGL.IOFunctions;
using UnitsNet;
using Point = TVGL.Point;

namespace KatanaObjects.BaseClasses
{
    public enum BlankType
    {
        [Description("Waterjet Plate")]
        WaterJetPlate,

        [Description("Rectangular Bar Stock")]
        RectangularBarStock,

        [Description("Circular Bar Stock")]
        CircularBarStock,

        [Description("Hollow Tube")]
        HollowTube,

        [Description("Closed-Die Forging")]
        ClosedDieForging,

        [Description("Near-Net Additive")]
        NearNetAdditive,

        //Assembly Blank Type. Not included in User Interface or the search process.
        [Description("Assembly")]
        Assembly
    }

    public class SubVolume
    {
        #region Private Properties
        //General internal parameters
        private BoundingBox _minimumBoundingBox;
        private readonly IList<Point> _convexHull2D;
        private readonly SearchInputs _searchInputs;
        private readonly IList<List<Point>> _silhouetteAlongNormal;
        private readonly IList<Point> _projectedPoints;
        private readonly Lazy<IList<List<Point>>> _silhouetteWithShortDimensionOBB;
        private readonly Length _subvolumeDepthAlongPlaneNormal;
        
        //Additive 
        private List<DirectionalDecomposition.DecompositionData> _additiveDecompositionData;
        private double[] _additiveBuildDirection;
        private readonly Lazy<List<List<List<double[]>>>> _additiveCrossSections;
        private readonly Lazy<TessellatedSolid> _additiveStockSolid;

        //Watetjet
        //Depth, shape, and area use a different type of lazy operation using a boolean
        //because they all need to be set in the same function.
        private double[] _waterjetDirection;
        private Length _waterjetDepth;
        private IList<List<Point>> _waterjetShape;
        private bool _waterjetShapeHasBeenSet = false;
        private bool _waterjetShapeIsFeasible = false;
        private readonly Lazy<TessellatedSolid> _waterjetStockSolid;

        //Forging
        public List<ForgingShapeData> _minForgingBlankData;
        private double[] _forgingStrokeDirection;
        private readonly Volume _forgingVolume;
        private readonly Lazy<List<List<List<double[]>>>> _forgingCrossSections;
        private readonly Lazy<TessellatedSolid> _forgingStockSolid;
        private ForgingShapeData _partingLineForgingData;

        //Circular bar stock
        //Depth and diameter use a different type of lazy operation using a boolean
        //because they all need to be set in the same function.
        private double[] _circularBarStockDirection;
        private Length _circularBarStockDepth;
        private Length _circularBarStockDiameter;
        private BoundingCircle _outerBoundingCircle;
        private bool _circularBarStockDimensionsHaveBeenSet = false;
        private readonly List<List<List<double[]>>> _circularBarStockCrossSections;
        private readonly Lazy<TessellatedSolid> _circularBarStockSolid;

        //Variables for Generic Rectangular Blanks (Plate, Bar Stock, Forging)
        //Length width and depth use a different type of lazy operation using a boolean
        //because they all need to be set in the same function.
        private double[] _rectangularBlankNormalDirection; //Direction of the cutting plane normal
        private double[] _rectangularBlankBuildDirection; //This is the direction to build the .STL file.
        private bool _rectangularBlankDimensionsHaveBeenSet = false;
        private Length _rectangularBlankLength;
        private Length _rectangularBlankWidth;
        private Length _rectangularBlankThickness;
        private Length _rectangularBlankDistanceAlongBuildDirection; //Distance along the direction to build the .STL file.
        private readonly Lazy<TessellatedSolid> _rectangularBlankSolid;
        private IList<List<Point>> _rectangularBlankPath;

        //Hollow HollowTube
        private double[] _hollowTubeDirection;
        private Length _hollowTubeDepth ;
        private TubeSize _hollowTubeSize;
        private BoundingCircle _innerBoundingCircle;
        private bool _hollowTubeDimensionsHaveBeenSet = false;
        private bool _hollowTubeIsFeasible = false;
        private readonly Lazy<TessellatedSolid> _hollowTubeStockSolid;

        #endregion

        #region Public Properties

        #region  General Properties
        //These get only parameters, must only be set in the constructor
        public Flat Flat { get; }

        public string SolidUnitString { get; }


        public Area SurfaceArea { get; }

        public Volume SolidVolume { get; }

        //These properties may be get or set.
        public Color SolidColor { get; set; }

        public int IndexInList { get; set; }

        public bool IsPositiveChild { get; set; }

        public bool IsSeed { get; set; }

        public bool HasBeenDecomposed { get; set; }

        public TessellatedSolid Solid { get; set; }

        public List<List<Point>> MachinedPartShapeOnPlane { get; set; }

        //These general properties are pointers to private properties
        public double[] OBBNormal => _minimumBoundingBox.Directions[0];

        public BoundingBox MinimumBoundingBox => _minimumBoundingBox;

        public IList<List<Point>> SilhouetteOnPlane => _silhouetteAlongNormal;

        private Length SubvolumeDepthAlongPlaneNormal => _subvolumeDepthAlongPlaneNormal;
        #endregion

        #region  Additive
        public Volume AdditiveVolume { get; private set; }

        public Area AdditiveAreaOnPlane { get; private set; }

        public Length AdditivePerimeterOnPlane { get; private set; }

        public IList<List<Point>> AdditiveShapeOnPlane { get; private set; }

        public List<List<List<double[]>>> AdditiveCrossSections => _additiveCrossSections.Value;

        public bool AdditiveIsFeasible { get; private set; }

        public bool NearNetPrintedShapeIsFeasible { get; private set; }

        public TessellatedSolid AdditiveSolid => _additiveStockSolid.Value;

        public double[] AdditiveBuildDirection
        {
            get
            {
                if (_additiveBuildDirection == null) GetAdditiveVolume();
                return _additiveBuildDirection;
            }
            set { _additiveBuildDirection = value; }
        }
        #endregion

        #region Waterjet
        //This waterjet cutting perimeter is the length of the WaterjetShape
        public Length WaterjetCuttingPerimeter { get; private set; }

        //The waterjet perimeter on plane is the length of the WaterjetShapeOnCuttingPlane.
        //If the join direction aligns with the waterjet direction, WaterjetPerimeterOnPlane == WaterjetCuttingPerimeter
        public Length WaterjetPerimeterOnPlane { get; private set; }

        public Volume WaterjetVolume { get; private set; }

        public List<List<List<double[]>>> WaterjetCrossSections { get; private set; }

        public double[] WaterjetCrossSectionBuildDirection => _waterjetDirection.multiply(-1);

        public Length WaterjetDistanceAlongJoinDirection { get; private set; }

        public TessellatedSolid WaterjetSolid => _waterjetStockSolid.Value;

        public Length WaterjetDepth
        {
            get
            {
                if (!_waterjetShapeHasBeenSet) SetWaterjetShape();
                return _waterjetDepth;
            }
            private set { _waterjetDepth = value; }
        }

        public bool WaterjetPlateIsFeasible
        {
            get
            {
                if (!_waterjetShapeHasBeenSet) SetWaterjetShape();
                return _waterjetShapeIsFeasible;
            }
            private set { _waterjetShapeIsFeasible = value; }
        }

        public IList<List<Point>> WaterjetShape
        {
            get
            {
                if (!_waterjetShapeHasBeenSet) SetWaterjetShape();
                return _waterjetShape;
            }
            private set
            {
                _waterjetShape = value;
                _waterjetShapeHasBeenSet = true;
            }
        }

        private IList<List<Point>> _waterjetShapeOnCuttingPlane;
        public IList<List<Point>> WaterjetShapeOnCuttingPlane
        {
            get
            {
                if (!_waterjetShapeHasBeenSet) throw new Exception("This value has not been set. It is for Blank Based. Regular" +
                                                                   "slicing gets this shape by cutting the solid.");
                return _waterjetShapeOnCuttingPlane;
            }
            private set
            {
                _waterjetShapeOnCuttingPlane = value;
            }
        }
        #endregion

        #region Forging
        //This is the volume recieved by a plant prior to machining, includes tooling excess.
        public Volume ForgingVolume => _forgingVolume + ForgingToolingExcess;

        //Testing coupon volume is a fixed user input value that accounts for material added for material property testing on every forging
        public Volume ForgingTestingCoupon => _searchInputs.Forging.TestingCouponsVolume;

        //Tooling excess as a percentage of the forging volume
        public Volume ForgingToolingExcess => _forgingVolume * _searchInputs.Forging.ToolingExcess.Value;

        //Yield loss is a percentage of the total forged volume (forging volume + tooling excess + testing coupon)
        public Volume ForgingYieldLoss => (_forgingVolume + ForgingToolingExcess + ForgingTestingCoupon) * _searchInputs.Forging.YieldLoss.Value;

        public bool ForgingIsFeasible;

        public List<List<List<double[]>>> ForgingCrossSections => _forgingCrossSections.Value;

        //public List<TessellatedSolid> ForgingStockSolid => _forgingStockSolid.Value; 
        //Rather than using lazy, don't save the stock solid.
        public TessellatedSolid ForgingStockSolid => GetForgingStockSolid();
        #endregion

        #region Circular Bar Stock
        public Length CircularBarStockDepth
        {
            get
            {
                if (!_circularBarStockDimensionsHaveBeenSet) SetCircularBarStockDimensions();
                return _circularBarStockDepth;
            }
        }
        public Length CircularBarStockDiameter
        {
            get
            {
                if (!_circularBarStockDimensionsHaveBeenSet) SetCircularBarStockDimensions();
                return _circularBarStockDiameter;
            }
        }

        public double[] CircularBarStockCrossSectionBuildDirection
        {
            get
            {
                if (!_circularBarStockDimensionsHaveBeenSet) SetCircularBarStockDimensions();
                return _circularBarStockDirection.multiply(-1);
            }
        }

        public TessellatedSolid CircularBarStockSolid
        {
            get
            {
                if (!_circularBarStockDimensionsHaveBeenSet) SetCircularBarStockDimensions();
                return _circularBarStockSolid.Value;
            }
        }

        public Area CircularBarStockArea { get; }

        public Volume CircularBarStockVolume { get; }

        public bool CircularBarIsFeasible;

        public IList<List<Point>> CircularBarStockPath => new List<List<Point>> {CreateCirclePath(_outerBoundingCircle,
            _searchInputs.General.StandardMachiningOffset.TesselatedSolidBaseUnit(SolidUnitString))};

        public Point CenterPointOfCircularShapedStock =>_outerBoundingCircle.Center;

        public List<List<List<double[]>>> CircularBarStockCrossSections => _circularBarStockCrossSections;

        #endregion

        #region Rectangular Blanks
        public Area RectangularBlankAreaOnPlane { get; private set; }

        public Volume RectangularBlankVolume { get; private set; }

        //This is the perimeter of RectangularBlankShapeOnCuttingPlane, which may or may not
        //be the same as the perimeter of the cross sections
        public Length RectangularBlankPerimeterOnPlane { get; private set; }

        public List<List<List<double[]>>> RectangularBlankCrossSections { get; private set; }

        public Length RectangularBlankDistanceAlongNormalDirection { get; private set; }

        //Is Feasible Booleans for the different rectangular blank types
        public bool RectangularPlateIsFeasible { get; private set; }

        public bool RectangularBarStockIsFeasible { get; private set; }

        public bool ForgedRectangularBarIsFeasible { get; private set; }


        public Length RectangularBlankLength
        {
            get
            {
                if (!_rectangularBlankDimensionsHaveBeenSet) SetAllRectangularBlankDimensions();
                return _rectangularBlankLength;
            }
        }

        public Length RectangularBlankWidth
        {
            get
            {
                if (!_rectangularBlankDimensionsHaveBeenSet) SetAllRectangularBlankDimensions();
                return _rectangularBlankWidth;
            }
        }

        public Length RectangularBlankThickness
        {
            get
            {
                if (!_rectangularBlankDimensionsHaveBeenSet) SetAllRectangularBlankDimensions();
                return _rectangularBlankThickness;
            }
        }

        private IList<List<Point>> _rectangularBlankShapeOnCuttingPlane;
        public IList<List<Point>> RectangularBlankShapeOnCuttingPlane
        {
            get
            {
                if (!_rectangularBlankDimensionsHaveBeenSet) throw new Exception("This value has not been set. It is for Blank Based. " +
                                                                   "Regular slicing gets this shape by cutting the solid.");
                return _rectangularBlankShapeOnCuttingPlane;
            }
            set { _rectangularBlankShapeOnCuttingPlane = value; }
        }

        public double[] RectangularBlankCrossSectionBuildDirection
        {
            get
            {
                if (!_rectangularBlankDimensionsHaveBeenSet) SetAllRectangularBlankDimensions();
                return _rectangularBlankBuildDirection.multiply(-1);
            }
        }

        public Length RectangularBlankCrossSectionBuildDistance
        {
            get
            {
                if (!_rectangularBlankDimensionsHaveBeenSet) SetAllRectangularBlankDimensions();
                return _rectangularBlankDistanceAlongBuildDirection;
            }
        }

        public TessellatedSolid RectangularBlankSolid
        {
            get
            {
                if (!_rectangularBlankDimensionsHaveBeenSet) SetAllRectangularBlankDimensions();
                //ShowRectangularBarStock();
                return _rectangularBlankSolid.Value;
            }
        }
        #endregion

        #region Hollow Tube
        public Volume HollowTubeStockVolume { get; }

        public Area HollowTubeAreaOnPlane { get; }

        public Length HollowTubeOuterDiameter
        {
            get
            {
                if (!_hollowTubeDimensionsHaveBeenSet) SetHollowTubeDimensions();
                return _hollowTubeSize.OuterDiameter;
            }
        }

        public bool HollowTubeIsFeasible
        {
            get
            {
                if (!_hollowTubeDimensionsHaveBeenSet) SetHollowTubeDimensions();
                return _hollowTubeIsFeasible;
            }
        }

        public TubeSize HollowTubeSize
        {
            get
            {
                if (!_hollowTubeDimensionsHaveBeenSet) SetHollowTubeDimensions();
                return _hollowTubeSize;
            }
        }

        public IList<List<Point>> HollowTubePaths
        {
            get
            {
                if (!_hollowTubeDimensionsHaveBeenSet) SetHollowTubeDimensions();
                var radialOffset = _searchInputs.HollowTube.RadialOffset.TesselatedSolidBaseUnit(SolidUnitString);
                var outerCirclePath = CreateCirclePath(_outerBoundingCircle, radialOffset);
                var innerCirclePath = CreateCirclePath(_innerBoundingCircle, -radialOffset);
                var paths = new List<List<Point>> {outerCirclePath};
                if(innerCirclePath != null) paths.Add(innerCirclePath);
                return paths;
            }
        }

        public List<List<List<double[]>>> HollowTubeCrossSections { get; }

        public double[] HollowTubeCrossSectionBuildDirection
        {
            get
            {
                if (!_hollowTubeDimensionsHaveBeenSet) SetHollowTubeDimensions();
                return _hollowTubeDirection.multiply(-1);
            }
        }

        public Length HollowTubeDepth
        {
            get
            {
                if(!_hollowTubeDimensionsHaveBeenSet) SetHollowTubeDimensions();
                return _hollowTubeDepth;
            }
        }

        public TessellatedSolid HollowTubeSolid
        {
            get
            {
                if (!_hollowTubeDimensionsHaveBeenSet) SetHollowTubeDimensions();
                return _hollowTubeStockSolid.Value;
            }
        }

        #endregion

        public Volume ForgingVolumeUnderestimate
            =>
                SolidVolume +
                Area.FromTesselatedSolidBaseUnit(Solid.SurfaceArea, SolidUnitString)*
                _searchInputs.Forging.SideCover;

        public Volume AdditiveVolumeUnderestimate 
            =>
                SolidVolume +
                Area.FromTesselatedSolidBaseUnit(Solid.SurfaceArea, SolidUnitString) *
                _searchInputs.WireFeed.WireAccuracy;

        //This is a boolean used for assemblies in the manfucturing plan generation
        //If the children are not circular, then this subvolume cannnot be RFWed onto a non-circular part.
        public bool ChildrenAreCircularAndAligned { get; set; }


        #endregion

        #region Constructor
        public SubVolume(TessellatedSolid solid, ISet<BlankType> blankTypes, 
            SearchInputs searchInputs)
        {
            //Initialize some public parameters
            AdditiveIsFeasible = true;
            CircularBarIsFeasible = true;
            ForgingIsFeasible = true;
            NearNetPrintedShapeIsFeasible = true;

            SolidUnitString = solid.Units.ToString();
            Solid = solid;
            SolidColor = solid.SolidColor;
            SolidVolume = Volume.FromTesselatedSolidBaseUnit(Solid.Volume, SolidUnitString);
            SurfaceArea = Area.FromTesselatedSolidBaseUnit(Solid.SurfaceArea, SolidUnitString);
            _waterjetShapeHasBeenSet = false;
            _hollowTubeDimensionsHaveBeenSet = false;
            if (searchInputs != null) _searchInputs = searchInputs;

            _minimumBoundingBox = GetMinimumBoundingBox(); //Must come before GetProjectedPoints()
            _projectedPoints = GetProjectedPoints();
            _convexHull2D = GetConvexHull2D();
            _silhouetteAlongNormal = GetSilhouetteAlongNormal();
            //This silhouette is used for additive on the seed part and waterjet when it is orthoganal to 
            //the cutting plane normal.
            _silhouetteWithShortDimensionOBB = new Lazy<IList<List<Point>>>(GetSilhouetteWithShortDimensionOBB);

            //If the blank type is being considered, go ahead and generate all the geomteric information for it
            //Otherwise, the parameters are null.
            //Additive
            //The flat is null only for the seed. So, if flat is null, we can consider near net additive, but past 
            //the seed we are actually considering additive feedstock.
            if (blankTypes.Contains(BlankType.NearNetAdditive))
            {
                AdditiveShapeOnPlane = GetAdditiveSilhouette();
                AdditiveVolume = GetAdditiveVolume();
                AdditiveAreaOnPlane = GetAdditiveAreaOnPlane();
                AdditivePerimeterOnPlane = GetAdditivePerimeterOnPlane();
                //Additive cross sections are not set to lower memory because they are not needed in all cases.
                _additiveCrossSections = new Lazy<List<List<List<double[]>>>>(GetAdditiveCrossSections);
                //Tesselated Solids are not set to lower memory because they are not needed in all cases.
                _additiveStockSolid = new Lazy<TessellatedSolid>(GetAdditiveStockSolid);
            }
            //Watetjet 
            if (blankTypes.Contains(BlankType.WaterJetPlate))
            {
                WaterjetVolume = GetWaterjetVolume();
                WaterjetCuttingPerimeter = GetWaterjetCuttingPerimeter();
                WaterjetCrossSections = GetWaterjetCrossSections();
                //Tesselated Solids are not set to lower memory because they are not needed in all cases.
                _waterjetStockSolid = new Lazy<TessellatedSolid>(GetWaterjetStockSolid);
            }
            //Forging
            if (blankTypes.Contains(BlankType.ClosedDieForging))
            {
                _forgingVolume = GetForgingVolume();
                //Forgin cross sections are not set to lower memory because they are not needed in all cases.
                _forgingCrossSections = new Lazy<List<List<List<double[]>>>>(GetForgingCrossSections);
                //Tesselated Solids are not set to lower memory because they are not needed in all cases.
                _forgingStockSolid = new Lazy<TessellatedSolid>(GetForgingStockSolid);
            }
            //Circular Bar Stock
            if (blankTypes.Contains(BlankType.CircularBarStock))
            {
                CircularBarStockVolume = GetCircularBarStockVolume();
                CircularBarStockArea = GetCircularBarStockArea();
                _circularBarStockCrossSections = GetCircularBarStockCrossSections();
                //Tesselated Solids are not set to lower memory because they are not needed in all cases.
                _circularBarStockSolid = new Lazy<TessellatedSolid>(GetCircularBarStockSolid);
            }
            //Hollow HollowTube
            if (blankTypes.Contains(BlankType.HollowTube))
            {
                HollowTubeStockVolume = GetHollowTubeVolume();
                HollowTubeAreaOnPlane = GetHollowTubeAreaOnPlane();
                HollowTubeCrossSections = GetHollowTubeCrossSections();
                //Tesselated Solids are not set to lower memory because they are not needed in all cases.
                _hollowTubeStockSolid = new Lazy<TessellatedSolid>(GetHollowTubeStockSolid);
            }
            //Rectangular Blanks
            if (blankTypes.Contains(BlankType.RectangularBarStock))
            {
                SetAllRectangularBlankDimensions();
                RectangularBlankVolume = GetRectangularBlankVolume();
                RectangularBlankAreaOnPlane = GetRectangularBlankAreaOnPlane();
                RectangularBlankCrossSections = GetRectangularBlankCrossSections();
                //Tesselated Solids are not set to lower memory because they are not needed in all cases.
                _rectangularBlankSolid = new Lazy<TessellatedSolid>(GetRectangularBlankSolid);
            }
        } 
        #endregion

        #region Rectangular Blanks
        //This methods sets the blank dimensions, given the information set in the main constructor
        internal void SetAllRectangularBlankDimensions()
        {
            //Set the normal direction. (Primarily used for determining cross section and area on the cutting planes)
            _rectangularBlankNormalDirection = Flat == null ? OBBSmallestNormal : Flat.Normal;
            SetAllRectangularBlankDimensions(_minimumBoundingBox, _rectangularBlankNormalDirection);
        }

        //Internal method so that the internal objects can be called directly
        //This method mostly uses static inputs, but is non-static because it sets global rectangular blank information
        internal void SetAllRectangularBlankDimensions(BoundingBox minimumBoundingBox, double[] rectangularBlankNormalDirection)
        {
            int thicknessDirectionInt, lengthDirectionInt, widthDirectionInt; 
            Length length, width, thickness, distanceAlongNormal;
            List<Point> rectangularBlankPath;
            List<Vertex> rectangularBlankVertexPath;
            GetRectangularBlankDimensions(minimumBoundingBox, rectangularBlankNormalDirection,
                SolidUnitString, _searchInputs, out length, out width, out thickness, out distanceAlongNormal,
                out thicknessDirectionInt, out lengthDirectionInt, out widthDirectionInt,
                out rectangularBlankPath, out rectangularBlankVertexPath);

            //Set the rectangular blank dimensions that are not unique to the type of blank
            _rectangularBlankWidth = width;
            _rectangularBlankLength = length;
            _rectangularBlankThickness = thickness;
            RectangularBlankDistanceAlongNormalDirection = distanceAlongNormal;
            _rectangularBlankDimensionsHaveBeenSet = true;
            _rectangularBlankBuildDirection = rectangularBlankNormalDirection; //ToDo: erase this build direction
            _rectangularBlankDistanceAlongBuildDirection = distanceAlongNormal; //ToDo: erase this build direction
            _rectangularBlankPath = new List<List<Point>>() { rectangularBlankPath};
            RectangularBarStockIsFeasible = true;          
        }

        private Volume GetRectangularBlankVolume()
        {
            if (!_rectangularBlankDimensionsHaveBeenSet) SetAllRectangularBlankDimensions();
            // ReSharper disable once SuggestVarOrType_SimpleTypes
            Volume stockVolume = _rectangularBlankWidth *_rectangularBlankLength * _rectangularBlankThickness;
            if (stockVolume >= SolidVolume) return stockVolume;
            if (stockVolume < SolidVolume * .95)
            {
                Debug.WriteLine("Error in Rectangular Bar Stock Volume implemenation (or with part geometry). Stock volume should always be larger than the final volume.");

                //Stream stream = File.Create("rectangleBarStockErrorPart.STL");
                //IO.Save(stream, new List<TessellatedSolid>() { Solid }, FileType.STL_ASCII);
                //stream.Close();

                //ShowRectangularBarStock();
            }
            stockVolume = SolidVolume;
            return stockVolume;
        }

        private Area GetRectangularBlankAreaOnPlane()
        {
            if (!_rectangularBlankDimensionsHaveBeenSet) SetAllRectangularBlankDimensions();
            // ReSharper disable once SuggestVarOrType_SimpleTypes
            //Divide the area by the distance along the normal direction to get the area on the cutting plane.
            Area area = RectangularBlankVolume / RectangularBlankDistanceAlongNormalDirection;
            if (Math.Sign(area.SquareMillimeters) != 1) throw new Exception("Error in implemenation. This number should be positive and great than zero");
            return area;
        }

        /// <summary>
        /// Gets the rectangular bar stock dimensions by applying the correct offsets from the inputs.
        /// Length is the largest dimension, width the middle dimension, and thickness the smallest dimension
        /// </summary>
        /// <param name="minimumBoundingBox"></param>
        /// <param name="normal"></param>
        /// <param name="solidUnitString"></param>
        /// <param name="searchInputs"></param>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="thickness"></param>
        /// <param name="distanceAlongNormal"></param>
        /// <param name="thicknessDirectionInt"></param>
        /// <param name="rectangularBlankPathAlongNormal"></param>
        /// <param name="rectangularBlankVertexPath"></param>
        /// <param name="isButtWeld"></param>
        /// <param name="lengthDirectionInt"></param>
        /// <param name="widthDirectionInt"></param>
        /// //The build direction is strictly used to apply the offsets correctly when making the build path  
        public static void GetRectangularBlankDimensions(BoundingBox minimumBoundingBox, double[] normal,
            string solidUnitString, SearchInputs searchInputs, out Length length, out Length width, out Length thickness, out Length distanceAlongNormal,
            out int thicknessDirectionInt, out int lengthDirectionInt, out int widthDirectionInt,
            out List<Point> rectangularBlankPathAlongNormal, out List<Vertex> rectangularBlankVertexPath, bool isButtWeld = false)
        {
            var boundingBoxDirections = minimumBoundingBox.Directions;
            var boundingBoxDimensions = minimumBoundingBox.Dimensions;

            /**************************************************************************
            **************            Sort the given dimensions          **************
            **************************************************************************/
            var dimensions = boundingBoxDimensions;
            var smallestDimensionIndex = 0;
            var largestDimensionIndex = 0;
            var largestDimension = double.NegativeInfinity;
            var smallestDimension = double.PositiveInfinity;
            for (var i = 0; i < 3; i++)
            {
                if (dimensions[i] < 0) throw new Exception("Bounding Box Cannot Have Negative Dimensions");
                if (dimensions[i] > largestDimension)
                {
                    largestDimension = dimensions[i];
                    largestDimensionIndex = i;
                }
            }
            for (var i = 0; i < 3; i++)
            {
                //Make sure to not set the largest dimension to the smallest dimension 
                //This happened if all the values were equal.
                if (i == largestDimensionIndex) continue;
                if (dimensions[i] < smallestDimension)
                {
                    smallestDimension = dimensions[i];
                    smallestDimensionIndex = i;
                }
            }
            var middleDimensionIndex = 3 - (largestDimensionIndex + smallestDimensionIndex);
            lengthDirectionInt = largestDimensionIndex;
            thicknessDirectionInt = smallestDimensionIndex;
            widthDirectionInt = middleDimensionIndex;

            /**************************************************************************
            ***********   Determine which direction is along the normal     ***********
            **************************************************************************/
            var indexOfNormalDirection = -1;
            for (var i = 0; i < 3; i++)
            {
                var direction = boundingBoxDirections[i];
                //Note: if any of the bounding box directions are 2D, ignore them. 
                if (direction.Count() < 3) continue;

                //Get the dot product to see if the direction lines up with the normal
                //Take the absolute value because we are looking for the direction most aligned with the axis
                var dot = Math.Abs(direction.dotProduct(normal));
                //We can use a loose tolerance, since only one direction should be anywhere close
                if (dot.IsPracticallySame(1, 0.1))
                {
                    indexOfNormalDirection = i;
                }
            }
            if (indexOfNormalDirection == -1) throw new Exception("One of the given directions must align with the normal");



            /**************************************************************************
            ***********       Set the offsets used on the dimensions        ***********
            **************************************************************************/
            var standardMachiningOffset = searchInputs.General.StandardMachiningOffset;

            /**************************************************************************
            ***********       Set the rectangular blank dimensions          ***********
            **************************************************************************/
            //Make length the largest dimension, width the middle dimension, and thickness the smallest dimension 
            //(This is as requested by Boeing)
            //For now, just call them max, min, and median to match help images and since the offsets are not yet applied.
            var maxDim = Length.FromTesselatedSolidBaseUnit(largestDimension, solidUnitString);
            var medianDim = Length.FromTesselatedSolidBaseUnit(dimensions[middleDimensionIndex], solidUnitString);
            var minDim = Length.FromTesselatedSolidBaseUnit(smallestDimension, solidUnitString);


            length = maxDim + 2 * searchInputs.General.StandardMachiningOffset;
            width = medianDim + 2 * searchInputs.General.StandardMachiningOffset;
            thickness = minDim + 2 * searchInputs.General.StandardMachiningOffset;          

            if (thickness.Millimeters.IsNegligible()) throw new Exception("Rectangular Blank Thickness was not set properly.");

            //Set the distance along the normal
            if (largestDimensionIndex == indexOfNormalDirection) distanceAlongNormal = length;
            else if (middleDimensionIndex == indexOfNormalDirection) distanceAlongNormal = width;
            else distanceAlongNormal = thickness;

            if (Math.Sign(thickness.Millimeters) +
                Math.Sign(length.Millimeters) +
                Math.Sign(width.Millimeters) != 3)
                throw new Exception("Error in implemenation. All 3 should be positive numbers.");

            //Set the blank dimensions that are uniue, depending on the type of blank
            //Only set one blank type as feasible. 
            //For the build direction, choose the shortest direction for forging, since the side cover will need to be the offset
            //For plate, the build direction (used for making the .STL file) need not be along the shortest direction.
            //Instead, plate and bar stock use the normal direction since they used a LFW or general machining in-plane offset.
            var lengthDirection = boundingBoxDirections[lengthDirectionInt];
            var widthDirection = boundingBoxDirections[widthDirectionInt];
            var thicknessDirection = boundingBoxDirections[thicknessDirectionInt];

            //Get the path along the build direction
            //This is more complicated than it sounds, since we will have to used the minimum bounding box to define the vertices
            //and then repeat whichever offsets we used in determining the length, width, and thickness. 
            double offsetDimension1, offsetDimension2;
            double[] offsetDirection1, offsetDirection2;
            var area2 = 0.0;

            //1) Get the correct dimensions and their corresponing 3D direction.
            //Instead of repeating the offsets, we are going to locally move the points using the length, width, and thickness
            //We are not useing the Clipper offset function because it does not do different offsets for different directions.
            if (thicknessDirectionInt == indexOfNormalDirection)
            {
                //Offset the width and length
                offsetDimension1 = length.TesselatedSolidBaseUnit(solidUnitString);
                offsetDirection1 = lengthDirection;
                offsetDimension2 = width.TesselatedSolidBaseUnit(solidUnitString);
                offsetDirection2 = widthDirection;
                area2 = (width * length).TesselatedSolidBaseUnit(solidUnitString);
            }
            else if (lengthDirectionInt == indexOfNormalDirection)
            {
                //Offset the thickness and width
                offsetDimension1 = thickness.TesselatedSolidBaseUnit(solidUnitString);
                offsetDirection1 = thicknessDirection;
                offsetDimension2 = width.TesselatedSolidBaseUnit(solidUnitString);
                offsetDirection2 = widthDirection;
                area2 = (width * thickness).TesselatedSolidBaseUnit(solidUnitString);
            }
            else //Normal along width
            {
                //Offset the length and thickness
                offsetDimension1 = length.TesselatedSolidBaseUnit(solidUnitString);
                offsetDirection1 = lengthDirection;
                offsetDimension2 = thickness.TesselatedSolidBaseUnit(solidUnitString);
                offsetDirection2 = thicknessDirection;
                area2 = (length * thickness).TesselatedSolidBaseUnit(solidUnitString);
            }

            //2) Get the rectangular blank path with the given dimensions are directions
            //rectangularBlankPath =
            //    CreateRectangularSolidPath(rectangularBlankBuildDirection,
            //        minimumBoundingBox, projectionTolerance,
            //        offsetDimension1.TesselatedSolidBaseUnit(solidUnitString),
            //        offsetDirection1,
            //        offsetDimension2.TesselatedSolidBaseUnit(solidUnitString), offsetDirection2);

            //2) Get the rectangular blank path with the given dimensions and directions
            rectangularBlankPathAlongNormal = CreateRectangularSolidPathAlongSearchDirection(normal, minimumBoundingBox, 
                offsetDimension1, offsetDirection1,
                offsetDimension2, offsetDirection2, out rectangularBlankVertexPath);

            //Check to make sure the path was made correctly
            var area1 = MiscFunctions.AreaOfPolygon(rectangularBlankPathAlongNormal);
            if (!area1.IsPracticallySame(area2, 0.01 * area1)) throw new Exception("Path made incorrectly");
        }

        //Creates the rectangular path for any rectangular blank.
        private static List<Point> CreateRectangularSolidPathAlongSearchDirection(double[] searchDirection, BoundingBox minimumBoundingBox
            , double blankDimension1, double[] offsetDirection1,
            double blankDimension2, double[] offsetDirection2, out List<Vertex> rectangularBlankVertexPath)
        {
            //Get the four lower vertices along the given build direction
            List<Vertex> sortedVertices;
            List<int[]> duplicateIndexRanges;
            MiscFunctions.SortAlongDirection(new[] { searchDirection },
                minimumBoundingBox.CornerVertices, out sortedVertices, out duplicateIndexRanges);
            var inPlaneVertices = sortedVertices.Take(4).ToList();

            return OffsetRectangle(inPlaneVertices, searchDirection, 
                new []{offsetDirection1, offsetDirection2}, 
                new [] {blankDimension1, blankDimension2},
                out rectangularBlankVertexPath);
        }

        //This function offsets the original rectangle in two directions, each with their own unique offset.
        private static List<Point> OffsetRectangle(List<Vertex> cornerVertices, double[] buildDirection,
            double[][] directions, double[] newDimensions, out List<Vertex> vertexPath)
        {
            if(cornerVertices.Count != 4) throw new Exception("Must have 4 vertices in rectangular shape");
            if (directions.Count() != 2 || newDimensions.Count() != 2) throw new Exception("Must have 2 sets of directions & offsets");
            var sum = new double[] {0.0, 0.0, 0.0};
            foreach (var vertex in cornerVertices)
            {
                sum = sum.add(vertex.Position);
            }
            //The center is the center of the rectangle in 3D space. 
            //The distance along the build direction to the corner vertices should be 0.
            //We can use this center to define the start for the points.
            var d0HalfVector = directions[0].multiply(newDimensions[0] / 2);
            var d1HalfVector = directions[1].multiply(newDimensions[1] / 2);
            var center = sum.divide(4);

            //CCW positive assuming direction0 ~ x and direction1 ~ y. We will reverse the path later if mistaken.
            //Point 1: center + direction[0]*d0HalfLength + direction[1]*d1HalfLength
            //Point 2: center -                           +
            //Point 3: center -                           -
            //Point 4: center +                           -
            vertexPath = new List<Vertex>
            {
                new Vertex(center.add(d0HalfVector).add(d1HalfVector)),
                new Vertex(center.subtract(d0HalfVector).add(d1HalfVector)),
                new Vertex(center.subtract(d0HalfVector).subtract(d1HalfVector)),
                new Vertex(center.add(d0HalfVector).subtract(d1HalfVector))
            };
            var area = MiscFunctions.AreaOf3DPolygon(vertexPath, buildDirection);
            if (area < 0) vertexPath.Reverse();

            double[,] backTransform;
            var points = MiscFunctions.Get2DProjectionPointsReorderingIfNecessary(vertexPath, buildDirection, out backTransform).ToList();

            return points;
        } 

        //Creates the rectangular path for any rectangular blank.
        private static List<Point> CreateRectangularSolidPath(double[] buildDirection, BoundingBox minimumBoundingBox,
            double projectionTolerance, double blankDimension1, double[] offsetDirection1,
            double blankDimension2, double[] offsetDirection2)
        {
            //Get the four lower vertices along the given build direction
            List<Vertex> sortedVertices;
            List<int[]> duplicateIndexRanges;
            MiscFunctions.SortAlongDirection(new [] {buildDirection}, 
                minimumBoundingBox.CornerVertices, out sortedVertices, out duplicateIndexRanges);
            var inPlaneVertices = sortedVertices.Take(4).ToList();

            //Get copies of the vertices so we don't need to worry about changing the minimum bounding box
            var cleanVertices = inPlaneVertices.Select(vertex => vertex.Copy()).ToList();

            //Sort the clean vertices along the offsetDirections
            //The first two vertices will be offset forward along that direction and the last
            //two vertices will be offset in the reverse
            List<Vertex> sortedVerticesDirection1, sortedVerticesDirection2;

            MiscFunctions.SortAlongDirection(new[] { offsetDirection1},
                cleanVertices, out sortedVerticesDirection1, out duplicateIndexRanges);
            var negativeOffsetVerticesDirection1 = sortedVerticesDirection1.Take(2).ToList();
            var positiveOffsetVerticesDirection1 = sortedVerticesDirection1.Skip(2).Take(2).ToList();
            List<Vertex> bottomVertices, topVertices;
            var distanceAlongDirection1 = MinimumEnclosure.GetLengthAndExtremeVertices(offsetDirection1, cleanVertices,
                out bottomVertices, out topVertices);
            var offsetDimension1 = (blankDimension1 - distanceAlongDirection1)/2;

            MiscFunctions.SortAlongDirection(new[] { offsetDirection2 },
                cleanVertices, out sortedVerticesDirection2, out duplicateIndexRanges);
            var negativeOffsetVerticesDirection2 = sortedVerticesDirection2.Take(2).ToList();
            var positiveOffsetVerticesDirection2 = sortedVerticesDirection2.Skip(2).Take(2).ToList();
            var distanceAlongDirection2 = MinimumEnclosure.GetLengthAndExtremeVertices(offsetDirection2, cleanVertices,
                out bottomVertices, out topVertices);
            var offsetDimension2 = (blankDimension2 - distanceAlongDirection2)/2;

            //Now we know which vertices need to be moved, how far, and in which direction to move them.
            foreach (var vertex in positiveOffsetVerticesDirection1)
            {
                vertex.Position = vertex.Position.add(offsetDirection1.multiply(offsetDimension1));
            }
            foreach (var vertex in negativeOffsetVerticesDirection1)
            {
                vertex.Position = vertex.Position.subtract(offsetDirection1.multiply(offsetDimension1));
            }
            foreach (var vertex in positiveOffsetVerticesDirection2)
            {
                vertex.Position = vertex.Position.add(offsetDirection2.multiply(offsetDimension2));
            }
            foreach (var vertex in negativeOffsetVerticesDirection2)
            {
                vertex.Position = vertex.Position.subtract(offsetDirection2.multiply(offsetDimension2));
            }

            var points = MiscFunctions.Get2DProjectionPoints(cleanVertices, buildDirection, projectionTolerance).ToList();

            if (points.Count != 4) throw new NotImplementedException("Flat Normal does not line up with bounding box, but it should!");
            //Check to make aure they are in the proper order. CCW.

            //We need to get a pair of opposite points. Just get the furthest point from point1
            var maxDistance = double.MinValue;
            var point1 = points[0];
            int point2Index = 0;
            Point point2 = null;
            for (var i = 1; i < points.Count; i++)
            {
                var d = MiscFunctions.DistancePointToPoint(point1.Position2D, points[i].Position2D);
                if (d < maxDistance) continue;
                maxDistance = d;
                point2Index = i;
                point2 = points[i];
            }

            Point middleLeft = null, middleRight = null;
            switch (point2Index)
            {
                case 0:
                    throw new Exception("Error in implementation of code");
                case 1:
                    middleLeft = points[2];
                    middleRight = points[3];
                    break;
                case 2:
                    middleLeft = points[1];
                    middleRight = points[3];
                    break;
                case 3:
                    middleLeft = points[1];
                    middleRight = points[2];
                    break;
            }
            if (point2 == null || middleLeft == null || middleRight == null) throw new Exception("Error in implementation of code");

            //Now we just need to determine which side to place the middle points
            var v1 = point1.Position2D.subtract(middleLeft.Position2D);
            var v2 = middleRight.Position2D.subtract(point1.Position2D);
            var cross2D = v1[0] * v2[1] - v1[1] * v2[0];
            List<Point> path;
            if (Math.Sign(cross2D) > 0) //CCW ordered
            {
                path = new List<Point>()
                {
                    middleLeft,
                    point1,
                    middleRight,
                    point2
                };
            }
            else //CW ordered. Need to adjust order.
            {
                path = new List<Point>()
                {
                    middleRight,
                    point1,
                    middleLeft,
                    point2
                };
            }

            return path;
        }

        private List<List<List<double[]>>> GetRectangularBlankCrossSections(double distanceFromOriginToBlankCenter)
        {
            return GetBlankCrossSections(_rectangularBlankPath, _rectangularBlankBuildDirection, _rectangularBlankDistanceAlongBuildDirection, distanceFromOriginToBlankCenter);
        }

        private List<List<List<double[]>>> GetRectangularBlankCrossSections()
        {
            var crossSections = GetBlankCrossSections(_rectangularBlankPath, _rectangularBlankBuildDirection, _rectangularBlankDistanceAlongBuildDirection);
            return crossSections;
        }

        private TessellatedSolid GetRectangularBlankSolid()
        {
            return TVGL.Extrude.FromLoops(RectangularBlankCrossSections[0], _rectangularBlankBuildDirection.multiply(-1),
                _rectangularBlankDistanceAlongBuildDirection.TesselatedSolidBaseUnit(SolidUnitString));
        }

        private void ShowRectangularBlank()
        {
            _rectangularBlankSolid.Value.SolidColor.A = 20;
            Presenter.ShowVertexPathsWithSolid(RectangularBlankCrossSections, new List<TessellatedSolid>() { _rectangularBlankSolid.Value });
        }
        #endregion

        #region Circular Bar Stock

        internal void SetCircularBarStockDimensions()
        {
            _circularBarStockDimensionsHaveBeenSet = true;
            var totalVerticalOffset = 2*_searchInputs.General.SawCutOffset;

            //If the first subvolume in search
            if (Flat == null)
            {
                var dimensions = _minimumBoundingBox.Dimensions;
                var largestDimension = Math.Max(Math.Max(dimensions[0], dimensions[1]), dimensions[2]);
                _circularBarStockDirection = OBBLargestNormal;

                //Make depth the largest dimension
                var depth = Length.FromTesselatedSolidBaseUnit(largestDimension, SolidUnitString) + totalVerticalOffset;
                if (Math.Sign(depth.Millimeters) != 1) throw new Exception("Error in implemenation. This number should be positive and great than zero");
                _circularBarStockDepth = depth;

                //Get the bounding circle with its axis aligned on the largest normal (GetProjectedPoints does this).
                _outerBoundingCircle = MinimumEnclosure.MinimumCircle(GetProjectedPoints());
            }
            else
            {
                //Make the depth along the plane normal
                _circularBarStockDirection = Flat.Normal;
                var depth = _subvolumeDepthAlongPlaneNormal + totalVerticalOffset;
                if (Math.Sign(depth.Millimeters) != 1) throw new Exception("Error in implemenation. This number should be positive and great than zero");
                _circularBarStockDepth = depth;

                //Get the bounding circle with its axis aligned along the plane normal
                _outerBoundingCircle = MinimumEnclosure.MinimumCircle(_convexHull2D);
            }
            var diameter = Length.FromTesselatedSolidBaseUnit(_outerBoundingCircle.Radius * 2, SolidUnitString) + 2 * _searchInputs.General.StandardMachiningOffset;
            if (Math.Sign(diameter.Millimeters) != 1) throw new Exception("Error in implemenation. This number should be positive and great than zero");
            _circularBarStockDiameter = diameter;

        }
    
        private Area GetCircularBarStockArea()
        {
            if(!_circularBarStockDimensionsHaveBeenSet) SetCircularBarStockDimensions();
            var area = Area.FromSquareMillimeters(Math.PI * _circularBarStockDiameter.Millimeters * _circularBarStockDiameter.Millimeters / 4);
            if (Math.Sign(area.SquareMillimeters) != 1) throw new Exception("Error in implemenation. This number should be positive and great than zero");
            return area;
        }

        private Volume GetCircularBarStockVolume()
        {
            if (!_circularBarStockDimensionsHaveBeenSet) SetCircularBarStockDimensions();
            var stockVolume = Volume.FromCubicMillimeters(_circularBarStockDepth.Millimeters * CircularBarStockArea.SquareMillimeters);
            if (stockVolume >= SolidVolume) return stockVolume;
            if (stockVolume < SolidVolume * .95)
            {
                Debug.WriteLine("Error in Circular Bar Stock Volume implemenation (or with part geometry). Stock volume should always be larger than the final volume.");

                Stream stream = File.Create("circularBarStockErrorPart.STL");
                IO.Save(stream, new List<TessellatedSolid>() { Solid }, FileType.STL_ASCII);
                stream.Close();

                //ShowCircularBarStock();
            }
            stockVolume = SolidVolume;
            return stockVolume;
        }

        private List<List<List<double[]>>> GetCircularBarStockCrossSections()
        {
            return GetBlankCrossSections(CircularBarStockPath, _circularBarStockDirection, _circularBarStockDepth);
        }

        private TessellatedSolid GetCircularBarStockSolid()
        {
            return TVGL.Extrude.FromLoops(_circularBarStockCrossSections[0], _circularBarStockDirection.multiply(-1),
                _circularBarStockDepth.TesselatedSolidBaseUnit(SolidUnitString));
        }

        private void ShowCircularBarStock()
        {
            Presenter.ShowVertexPathsWithSolid(_circularBarStockCrossSections, new List<TessellatedSolid>() { _circularBarStockSolid.Value });
        }

        #endregion

        #region Hollow Tube
        internal void SetHollowTubeDimensions()
        {
            if(!_circularBarStockDimensionsHaveBeenSet) SetCircularBarStockDimensions();
            _hollowTubeDimensionsHaveBeenSet= true;
            var totalVerticalOffset = 2 * _searchInputs.General.SawCutOffset;

            //If the first subvolume in search
            if (Flat == null)
            {
                var dimensions = _minimumBoundingBox.Dimensions;
                var largestDimension = Math.Max(Math.Max(dimensions[0], dimensions[1]), dimensions[2]);

                //Make depth the largest dimension
                var depth = Length.FromTesselatedSolidBaseUnit(largestDimension, SolidUnitString) + totalVerticalOffset;
                if (Math.Sign(depth.Millimeters) != 1) throw new Exception("Error in implemenation. This number should be positive and great than zero");
                _hollowTubeDepth = depth;

                //Get the silhouette with the axis lined up with the biggest dimension
                _hollowTubeDirection = OBBLargestNormal;
                var silhouette = GetSilhouette(_hollowTubeDirection);
                _innerBoundingCircle = MinimumEnclosure.MaximumInnerCircle(silhouette, _outerBoundingCircle.Center);
            }
            else
            {
                //Make the depth along the plane normal
                var depth = _subvolumeDepthAlongPlaneNormal + totalVerticalOffset;
                if (Math.Sign(depth.Millimeters) != 1) throw new Exception("Error in implemenation. This number should be positive and great than zero");
                _hollowTubeDepth = depth;
                _hollowTubeDirection = Flat.Normal;

                //Get the bounding circle with its axis aligned along the plane normal
                _innerBoundingCircle = MinimumEnclosure.MaximumInnerCircle(_silhouetteAlongNormal, _outerBoundingCircle.Center);
                //var paths = new List<List<Point>>(SilhouetteOnPlane) { CreateCirclePath(_innerBoundingCircle) };
                //Presenter.ShowAndHang(paths);
            }

            //Offsetting will result in a solid cylinder.
            if (_innerBoundingCircle.Radius < 2*_searchInputs.HollowTube.RadialOffset.TesselatedSolidBaseUnit(SolidUnitString))
                return;
            
            //Set the inner and outer diameter given the inner and outer circle.
            //Add offset for hollow tube extruding
            var innerDiameter = Length.FromTesselatedSolidBaseUnit(_innerBoundingCircle.Radius*2, SolidUnitString) -
                                2*_searchInputs.HollowTube.RadialOffset;
            if (Math.Sign(innerDiameter.Millimeters) != 1)
                throw new Exception("Error in implemenation. This number should be positive and great than zero");

            var outerDiameter = Length.FromTesselatedSolidBaseUnit(_outerBoundingCircle.Radius * 2, SolidUnitString) +
                                2 * _searchInputs.HollowTube.RadialOffset; 

            //Check if dimensions are valid
            var thickness = (outerDiameter - innerDiameter)/2;
            if (outerDiameter > _searchInputs.HollowTube.OuterDiameterMax ||
                innerDiameter < _searchInputs.HollowTube.InnerDiameterMin ||
                outerDiameter/ thickness > _searchInputs.HollowTube.MaximumOuterDiameterToThicknessRatio.DecimalFractions) return;
            var tubeSize = new TubeSize(outerDiameter, innerDiameter);
            if (tubeSize.Invalid) return;

            //Else, this tube size is valid. 
            _hollowTubeSize = tubeSize;
            _hollowTubeIsFeasible = true; //Default is false
            
        }

        private Area GetHollowTubeAreaOnPlane()
        {
            if (!_hollowTubeDimensionsHaveBeenSet) SetHollowTubeDimensions();
            var area = _hollowTubeSize.Area;
            if (Math.Sign(area.SquareMillimeters) != 1) throw new Exception("Error in implemenation. This number should be positive and great than zero");
            return area;
        }

        private Volume GetHollowTubeVolume()
        {
            if (!_hollowTubeDimensionsHaveBeenSet) SetHollowTubeDimensions();
            var stockVolume = Volume.FromCubicMillimeters(_hollowTubeDepth.Millimeters * HollowTubeAreaOnPlane.SquareMillimeters);
            if (stockVolume >= SolidVolume) return stockVolume;
            if (stockVolume < SolidVolume * .95)
            {
                throw new Exception(
                    "Error in implemenation. Stock volume should always be larger than the final volume.");
                //ShowHollowTube();
            }
            stockVolume = SolidVolume;
            return stockVolume;
        }

        public List<List<List<double[]>>> GetHollowTubeCrossSections()
        {
            return GetBlankCrossSections(HollowTubePaths, _hollowTubeDirection, _hollowTubeDepth);
        }

        private TessellatedSolid GetHollowTubeStockSolid()
        {
            return TVGL.Extrude.FromLoops(HollowTubeCrossSections[0], _hollowTubeDirection.multiply(-1),
                _hollowTubeDepth.TesselatedSolidBaseUnit(SolidUnitString));
        }

        private void ShowHollowTube()
        {
            Presenter.ShowVertexPathsWithSolid(HollowTubeCrossSections, new List<TessellatedSolid>() { _hollowTubeStockSolid.Value });
        }
        #endregion

        #region Forging
        private Volume GetForgingVolume()
        {
            var minForgingVolume = Volume.MaxValue;
            var numDirections = _searchInputs.Forging.NumberOfOBBDirectionsToConsider.Value;
            //Get the 3 directions we are interested in.
            var sortedDirections = SortedOBBDirectionsByLength;
            for (var i = 0; i < numDirections; i++)
            {
                var direction = sortedDirections[i];
                var draftAngle = _searchInputs.Forging.DraftAngle.Radians;
                var topCover = _searchInputs.Forging.TopCover.TesselatedSolidBaseUnit(SolidUnitString);
                var sideCover = _searchInputs.Forging.SideCover.TesselatedSolidBaseUnit(SolidUnitString);
           
                List<Vertex> v1, v2;
                var length = MinimumEnclosure.GetLengthAndExtremeVertices(direction, Solid.Vertices, out v1, out v2);
                //This sets a minimum size forging. It must be at least 2x the top cover dimension
                if (length < 2*_searchInputs.Forging.TopCover.TesselatedSolidBaseUnit(SolidUnitString)) continue;
                var stepSize = length/_searchInputs.Forging.NumberOfSlices.Unitless;
                 
                var originalDecompositionData = DirectionalDecomposition.UniformAreaDecomposition(Solid, direction, stepSize);
                if(originalDecompositionData.Count < _searchInputs.Forging.NumberOfSlices.Unitless) Debug.WriteLine("Forging step size is too large or part is too small.");
            
                if (!originalDecompositionData.Any())
                {
                   Debug.WriteLine("Could not successfully find forging volume. Part was likely too small.");
                    return Volume.MaxValue;
                } 
                var dataAlongSearchDirection = new HashSet<ForgingShapeData>();
                var tempData = new HashSet<ForgingShapeData>();
                foreach (var decompData in originalDecompositionData)
                {
                    var offsetCrossSection = PolygonOperations.OffsetRound(decompData.Paths, sideCover);
                    dataAlongSearchDirection.Add(new ForgingShapeData(offsetCrossSection, decompData.DistanceAlongDirection - topCover));
                    tempData.Add ( new ForgingShapeData(offsetCrossSection, decompData.DistanceAlongDirection + topCover));
                }
                var dataAlongReverseSearchDirection = new HashSet<ForgingShapeData>(tempData.Reverse());

                ForgingShapeData previousData = null;
                _lastContribingDataAlongSearchDirection = null;
                var dataJoinedAlongSearchDirection = new List<ForgingShapeData>();
                foreach (var data in dataAlongSearchDirection)
                {
                    if (previousData == null)
                    {
                        previousData = data;
                        previousData.IsContributing = true;
                        _lastContribingDataAlongSearchDirection = previousData;
                    }
                    else
                    {
                        var d = data.DistanceAlongDirection - previousData.DistanceAlongDirection;
                        if (d.IsLessThanNonNegligible()) throw new NotImplementedException();
                        var draftedPreviousCrossSection = Draft(previousData.Paths, draftAngle, d);
                        var area = MiscFunctions.AreaOfPolygon(draftedPreviousCrossSection);
                        var joinedCrossSection = PolygonOperations.Union(draftedPreviousCrossSection, data.Paths);
                        previousData = new ForgingShapeData(joinedCrossSection, data.DistanceAlongDirection);

                        //Set whether the data contributed to the shape. It did not contribute if it was completely inside the draftedPreviousCrossSection
                        previousData.IsContributing = previousData.Area > area;
                        data.IsContributing = previousData.IsContributing;
                        if (previousData.IsContributing)
                        {
                            _lastContribingDataAlongSearchDirection = previousData;
                        }
                    }
                    dataJoinedAlongSearchDirection.Add(previousData);
                }

                //Build up the cross sections along the reverse direction
                _lastContribingDataAlongReverseSearchDirection = null;
                var dataJoinedAlongReverseSearchDirection = new List<ForgingShapeData>();
                previousData = null;
                foreach (var data in dataAlongReverseSearchDirection)
                {
                    if (previousData == null)
                    {
                        previousData = data;
                        previousData.IsContributing = true;
                        _lastContribingDataAlongReverseSearchDirection = previousData;
                    }
                    else
                    {
                        var d = previousData.DistanceAlongDirection - data.DistanceAlongDirection;
                        if(d.IsLessThanNonNegligible()) throw new NotImplementedException();
                        var draftedPreviousCrossSection = Draft(previousData.Paths, draftAngle, d);
                        var area = MiscFunctions.AreaOfPolygon(draftedPreviousCrossSection);
                        var joinedCrossSection = PolygonOperations.Union(draftedPreviousCrossSection, data.Paths);
                        previousData = new ForgingShapeData(joinedCrossSection, data.DistanceAlongDirection);

                        //Set whether the data contributed to the shape. It did not contribute if it was completely inside the draftedPreviousCrossSection
                        previousData.IsContributing = previousData.Area > area;
                        data.IsContributing = previousData.IsContributing;
                        if (previousData.IsContributing)
                        {
                            _lastContribingDataAlongReverseSearchDirection = previousData;
                        }
                    }
                    dataJoinedAlongReverseSearchDirection.Add(previousData);
                }
                if (_lastContribingDataAlongSearchDirection == null || _lastContribingDataAlongReverseSearchDirection == null) throw new Exception("These should never be null");
                dataJoinedAlongReverseSearchDirection.Reverse();

                //Find parting line by minimizing the difference in area.
                //The parting line distance along the search direction should be between the two last contributing data points.
                double xOpt;
                ForgingShapeData ya, yb;
                if (
                    _lastContribingDataAlongSearchDirection.DistanceAlongDirection.IsPracticallySame(
                        _lastContribingDataAlongReverseSearchDirection.DistanceAlongDirection))
                {
                    xOpt = _lastContribingDataAlongSearchDirection.DistanceAlongDirection;
                    ya = _lastContribingDataAlongSearchDirection;
                    yb = _lastContribingDataAlongReverseSearchDirection;
                }
                else
                {
                    var x0 = _lastContribingDataAlongSearchDirection.DistanceAlongDirection;
                    var x3 = _lastContribingDataAlongReverseSearchDirection.DistanceAlongDirection;
                    if (x3 < x0) //reverse
                    {
                        var temp = x0;
                        x0 = x3;
                        x3 = temp;
                    }
                    var goldenRatio = (Math.Sqrt(5)-1)/2;
                    var d = goldenRatio*(x3-x0);
                    var x2 = x0 + d; 
                    var x1 = x3 - d;
                    const double tolerance = 0.0001;
                    var err = x3-x0;
                    var n = 0;
                    
                    var f1 = DifferenceFunction(x1, draftAngle, out ya, out yb);
                    var f2 = DifferenceFunction(x2, draftAngle, out ya, out yb);

                    while (err > tolerance)
                    {
                        n ++;
                        if (f1 > f2)
                        {
                            x0 = x1;
                            x1 = x2;
                            x2 = x0 + goldenRatio * (x3 - x0);
                            f1 = f2;
                            f2 = DifferenceFunction(x2, draftAngle, out ya, out yb);        
                        }
                        else
                        {
                            x3 = x2;
                            x2 = x1;
                            x1 = x3 - goldenRatio*(x3 - x0);
                            f2 = f1;
                            f1 = DifferenceFunction(x1, draftAngle, out ya, out yb);
                        }
                        err = Math.Abs(x3 - x0) / (Math.Abs(x1) + Math.Abs(x2));
                    }
                    xOpt = f1 < f2 ? x1 : x2;
                }
                
                DifferenceFunction(xOpt, draftAngle, out ya, out yb);
                var shapeOnPartingLine = PolygonOperations.Union(ya.Paths, yb.Paths);

                //Set the min web based on Table 4 (page 28) from the Aluminum Forging Design Manual [STD.AA 15-ENGR 1995] [0604500 0012776 683]
                var planViewArea = Area.FromTesselatedSolidBaseUnit(MiscFunctions.AreaOfPolygon(shapeOnPartingLine), SolidUnitString);
                var minWeb = Length.FromInches((2E-11) * Math.Pow(planViewArea.SquareInches, 3) - (7E-08) * Math.Pow(planViewArea.SquareInches, 2) + 0.0003 * planViewArea.SquareInches + 0.1442);//estimate from page 28

                //Set the min gutter flash clearance based on Table 1 of https://www.scribd.com/doc/18789021/Drop-Forging-Die-Design
                var depth = Math.Abs(dataAlongSearchDirection.First().DistanceAlongDirection - dataAlongSearchDirection.Last().DistanceAlongDirection);
                var minGutterFlashClearance = depth*_searchInputs.Forging.MinFlashAndGutterWidth.Value;//Based on depth of part

                var mitershapeOnPartingLine = PolygonOperations.OffsetMiter(shapeOnPartingLine, minGutterFlashClearance);
                shapeOnPartingLine = PolygonOperations.OffsetMiter(mitershapeOnPartingLine, -minGutterFlashClearance);
                var dataOnPartingLine = new ForgingShapeData(shapeOnPartingLine, xOpt);
                

                //Now, split the data
                var forgingGeometryData = new List<ForgingShapeData>();
                foreach (var data in dataJoinedAlongSearchDirection)
                {
                    if (!data.DistanceAlongDirection.IsLessThanNonNegligible(xOpt - minWeb.TesselatedSolidBaseUnit(SolidUnitString) / 2)) break;
                    forgingGeometryData.Add(data);
                }
                var offsetShape = Draft(dataOnPartingLine.Paths, -draftAngle, minWeb.TesselatedSolidBaseUnit(SolidUnitString) / 2);
                forgingGeometryData.Add(new ForgingShapeData(offsetShape, xOpt - minWeb.TesselatedSolidBaseUnit(SolidUnitString) / 2));
                //forgingGeometryData.Add(new ForgingShapeData(mitershapeOnPartingLine, xOpt));
                forgingGeometryData.Add(dataOnPartingLine);
                forgingGeometryData.Add(new ForgingShapeData(offsetShape, xOpt + minWeb.TesselatedSolidBaseUnit(SolidUnitString) / 2));
                foreach (var data in dataJoinedAlongReverseSearchDirection)
                {
                    if (!data.DistanceAlongDirection.IsGreaterThanNonNegligible(xOpt + minWeb.TesselatedSolidBaseUnit(SolidUnitString) / 2)) continue;
                    forgingGeometryData.Add(data);
                }

                //Round all data
                //ToDo: Add fillets (or at least chamfers to the data)

                //Use trapezoidal reimann sum to get volume
                //Cannot use Simpson's rule unless we were to gaurantee even sets of deltaX 
                var iterativeForgingVolumeEstimate = 0.0;
                //Trapezoidal summation with unequal intervals
                for (var j = 1; j < forgingGeometryData.Count; j++)
                {
                    var data1 = forgingGeometryData[j - 1];
                    var data2 = forgingGeometryData[j];

                    //delta x will be different for the parting line because of the min web.
                    var deltaX = data2.DistanceAlongDirection - data1.DistanceAlongDirection;
                    if(deltaX.IsLessThanNonNegligible()) throw new Exception("Should never be negative");
                    iterativeForgingVolumeEstimate += (deltaX/2)*(data1.Area + data2.Area);
                }

                
                //Presenter.ShowVertexPathsWithSolid(crossSections, new List<TessellatedSolid>() { Solid});
                //Add other forging volume information

                //Don't add the additional volume parameters, they will be set later
                var forgingVolume = Volume.FromTesselatedSolidBaseUnit(iterativeForgingVolumeEstimate, SolidUnitString);
                if (forgingVolume < minForgingVolume)
                {
                    minForgingVolume = forgingVolume;
                    _forgingStrokeDirection = direction;
                    _minForgingBlankData = forgingGeometryData;
                    _partingLineForgingData = dataOnPartingLine;
                }
            }
            //If the forging is crazy large, make it infeasible.
            if(minForgingVolume > 100 * SolidVolume)
            {
                ForgingIsFeasible = false;
            }
            //ShowForgingBlank();
            if (minForgingVolume >= SolidVolume) return minForgingVolume;
            if (minForgingVolume < SolidVolume * .95)
            {
                ShowForgingBlank();
                throw new Exception(
                    "Error in implemenation. Stock volume should always be larger than the final volume.");
            }

            
            return SolidVolume;
        }

        private TessellatedSolid GetForgingStockSolid()
        {
            if (_minForgingBlankData == null) GetForgingCrossSections();


            //Get the forging stock shape as a collection of small extrusions. 
            var stockSolidCompilation = new List<TessellatedSolid>();
            var faces = new List<PolygonalFace>();
            var vertices = new List<Vertex>();
            //Until the cross sections get to the part line, extrude toward the parting line. 
            var partingLineDistance = _partingLineForgingData.DistanceAlongDirection;
            double distanceToNextCrossSection = 0.0;
            var i = 0;
            do
            {
                // First, get the distance between this cross section and the next cross section.
                var crossSection = _forgingCrossSections.Value[i];
                var distanceToCrossSection = _minForgingBlankData[i].DistanceAlongDirection;
                distanceToNextCrossSection = _minForgingBlankData[i + 1].DistanceAlongDirection;
                //Delta X will be positive (it was checked during the volume calculation in the GetForgingVolume() function)
                var deltaX =  distanceToNextCrossSection - distanceToCrossSection;
                if (deltaX.IsLessThanNonNegligible()) throw new Exception("Should never be negative");

                //Ideally we would use an extrude with draft function, but this may be ok for now. If extruding with draft and a hole closes, then don't include it in the extrude.
                var stockSolidPiece = TVGL.Extrude.FromLoops(crossSection,
                    _forgingStrokeDirection.multiply(1), deltaX);
                stockSolidCompilation.Add(stockSolidPiece);
                faces.AddRange(stockSolidPiece.Faces);
                vertices.AddRange(stockSolidPiece.Vertices);
                //update counter
                i++;

            } while (distanceToNextCrossSection < partingLineDistance);

            //Once you have extruded to the parting line, reverse the list and extrude the other direction.
            i = _forgingCrossSections.Value.Count - 1;
            double distanceToPriorCrossSection = 0.0;
            do
            {
                // First, get the distance between this cross section and the next cross section.
                var crossSection = _forgingCrossSections.Value[i];
                var distanceToCrossSection = _minForgingBlankData[i].DistanceAlongDirection;
                distanceToPriorCrossSection = _minForgingBlankData[i - 1].DistanceAlongDirection;
                //Delta X will be positive (it was checked during the volume calculation in the GetForgingVolume() function)
                var deltaX = distanceToCrossSection - distanceToPriorCrossSection;
                if (deltaX.IsLessThanNonNegligible()) throw new Exception("Should never be negative");

                //Extrude in opposite direction from first loop
                //Ideally we would use an extrude with draft function, but this may be ok for now. If extruding with draft and a hole closes, then don't include it in the extrude.
                var stockSolidPiece = TVGL.Extrude.FromLoops(crossSection,
                    _forgingStrokeDirection.multiply(-1), deltaX);
                stockSolidCompilation.Add(stockSolidPiece);
                faces.AddRange(stockSolidPiece.Faces);
                vertices.AddRange(stockSolidPiece.Vertices);

                //update counter
                i--;

            } while (distanceToPriorCrossSection > partingLineDistance);

            return new TessellatedSolid(faces, vertices, null, Solid.Units, Solid.Name);
        }

        private List<List<List<double[]>>> GetForgingCrossSections()
        {
            if (_minForgingBlankData == null) GetForgingVolume();
            var crossSectionDict = new Dictionary<int, IList<List<Point>>>();
            var distances = new Dictionary<int, double>();
            for (var i = 0; i < _minForgingBlankData.Count; i++)
            {
                crossSectionDict.Add(i, _minForgingBlankData[i].Paths);
                distances.Add(i, _minForgingBlankData[i].DistanceAlongDirection);
            }
            return GetBlankCrossSections(_forgingStrokeDirection, crossSectionDict, distances);
        }

        private void ShowForgingBlank()
        {
            Presenter.ShowVertexPathsWithSolid(ForgingCrossSections, new List<TessellatedSolid>() { GetForgingStockSolid()});
            //Presenter.ShowAndHangTransparentsAndSolids(GetForgingStockSolid(), new List<TessellatedSolid>() { Solid });
        } 

        private ForgingShapeData _lastContribingDataAlongSearchDirection;
        private ForgingShapeData _lastContribingDataAlongReverseSearchDirection;



        private double DifferenceFunction(double x, double draftAngle,  out ForgingShapeData newForgingDataAlongDirection, out ForgingShapeData newForgingShapeDataAlongReverseDirection )
        {
            var leftX = _lastContribingDataAlongSearchDirection.DistanceAlongDirection;
            var rightX = _lastContribingDataAlongReverseSearchDirection.DistanceAlongDirection;
            newForgingDataAlongDirection = new ForgingShapeData(Draft(_lastContribingDataAlongSearchDirection.Paths, draftAngle, x - leftX), x);
            newForgingShapeDataAlongReverseDirection = new ForgingShapeData(Draft(_lastContribingDataAlongReverseSearchDirection.Paths, draftAngle, rightX - x), x);
            return Math.Abs(newForgingShapeDataAlongReverseDirection.Area - newForgingDataAlongDirection.Area);
        }

        private static IList<List<Point>> Draft(List<List<Point>> paths, double draftAngleInRadians, double distance)
        {
            var draftOffset = Math.Tan(draftAngleInRadians)*distance;
            return PolygonOperations.OffsetMiter(paths, draftOffset);
        }
        #endregion

        #region Waterjet Plate

        private void SetWaterjetShape()
        {
            List<List<Point>> waterjetShapePreTolerance;
            Length waterjetDepthPreTolerance;
            double[] waterjetDirection;

            //Don't use the normal of the plane. Rather, use the shortest dimension.
            if (Flat == null)
            {
                //ToDo: Getting the best direction is more complicated than this.
                waterjetShapePreTolerance = _silhouetteWithShortDimensionOBB.Value.ToList();
                waterjetDepthPreTolerance = OBBSmallestDimension;
                waterjetDirection = OBBSmallestNormal;
            }
            else
            {
                //Get the smallest dimension from the bounding rectangle and depth.
                var dim1 = SubvolumeDepthAlongPlaneNormal;
                var boundingRectangleDimensions = GetBoundingRectangleDimensions();
                var boundingRectangleDirections = GetBoundingRectangleDirections3D();
                var dim2 = boundingRectangleDimensions[0];
                var dim3 = boundingRectangleDimensions[1];
                if (Math.Sign(dim1.Millimeters) + Math.Sign(dim2.Millimeters) + Math.Sign(dim3.Millimeters) != 3)
                    throw new Exception("Error in implemenation. All 3 should be positive numbers.");
                if (dim1 <= dim2 && dim1 <= dim3)
                {
                    waterjetDepthPreTolerance = dim1;
                    waterjetDirection = Flat.Normal;
                    waterjetShapePreTolerance = _silhouetteAlongNormal.ToList();
                }
                else if (dim2 <= dim3)
                {
                    waterjetDepthPreTolerance = dim2;
                    waterjetDirection = boundingRectangleDirections[0];
                    waterjetShapePreTolerance = GetSilhouette(waterjetDirection);
                }
                else
                {
                    waterjetDepthPreTolerance = dim3;
                    waterjetDirection = boundingRectangleDirections[1];
                    waterjetShapePreTolerance = GetSilhouette(waterjetDirection);
                }
            }
            if(waterjetShapePreTolerance == null || !waterjetShapePreTolerance.Any()) throw new Exception("Silhouette not found");

            var waterjetShape = GetWaterjetShapeGivenSilhouette(waterjetShapePreTolerance, _searchInputs, SolidUnitString);
            var plateThickness = waterjetDepthPreTolerance + _searchInputs.General.StandardMachiningOffset * 2;
            var waterjetPlateIsFeasible = plateThickness < Length.FromInches(4);
            SetWaterjetShape(waterjetDirection, plateThickness, waterjetShape, waterjetPlateIsFeasible);
        }

        public static List<List<Point>> GetWaterjetShapeGivenSilhouette(List<List<Point>> waterjetShapePreTolerance, 
            SearchInputs searchInputs, string solidUnitString)
        {
            var tempShape = PolygonOperations.OffsetRound(waterjetShapePreTolerance,
                        searchInputs.Waterjet.CuttingOffset.TesselatedSolidBaseUnit(solidUnitString));

            //Apply mininum internal feature offset AFTER the cutting offset has been applied
            var waterjetShape = new List<List<Point>>();
            foreach (var polygon in tempShape)
            {
                //If the area is positive, we want to ignore it
                if (MiscFunctions.AreaOfPolygon(polygon) > 0)
                {
                    waterjetShape.Add(polygon);
                    continue;
                }

                //Else the area is negative
                //Apply the offset forward (inward in the case of negative polygons)
                try
                {
                    //Because we are only offsetting one polygon at a time, Clipper will assume the negative 
                    //polygon (our hole) is actually supposed to be positive. To overcome this, we offset negative.
                    var tempPoly = PolygonOperations.OffsetRound(polygon,
                        -searchInputs.Waterjet.MinInternalFeatureRadius.TesselatedSolidBaseUnit(solidUnitString));

                    //if there is still some shape left, offset backward.
                    if (tempPoly != null && tempPoly.Any())
                    {
                        //note innerPoly may contain more than one polygon if offsetting forward split the negative space into
                        //multiple smaller polygons.
                        var innerPolygons = PolygonOperations.OffsetRound(tempPoly,
                            searchInputs.Waterjet.MinInternalFeatureRadius.TesselatedSolidBaseUnit(solidUnitString));
                        waterjetShape.AddRange(innerPolygons);
                    }
                }
                catch
                {
                    //This polygon probably closed. Don't add it back to the shape.
                }
            }
            //Union even odd to properly reorder the polygons
            //This is especially important since Clipper probalby reordered the negative polygons to be positive. 
            waterjetShape = PolygonOperations.Union(waterjetShape, true, PolygonFillType.EvenOdd);
            if (waterjetShape.Count > 10) Debug.WriteLine("It us unlikely that the waterjet shape should have more than a few polygons.");

            return waterjetShape;
        }

        private void SetWaterjetShape(double[] waterjetDirection, Length plateThickness, 
            List<List<Point>> waterjetShape, bool isFeasible)
        {
            _waterjetShapeHasBeenSet = true;
            _waterjetShapeIsFeasible = isFeasible;
            _waterjetDirection = waterjetDirection;
            _waterjetShape = waterjetShape;
            _waterjetDepth = plateThickness;
        }

        private Length GetWaterjetCuttingPerimeter()
        {
            if (!_waterjetShapeHasBeenSet) SetWaterjetShape();
            var perimeter = _waterjetShape.Sum(path => MiscFunctions.Perimeter(path));
            if (Math.Sign(perimeter) != 1) throw new Exception("Error in implemenation. This number should be positive and great than zero");
            return Length.FromTesselatedSolidBaseUnit(perimeter, SolidUnitString);
        }

        private Volume GetWaterjetVolume()
        {
            if (!_waterjetShapeHasBeenSet) SetWaterjetShape();
            if(!_waterjetShapeIsFeasible) return Volume.MaxValue;
            var area = Area.FromTesselatedSolidBaseUnit(_waterjetShape.Sum(path => MiscFunctions.AreaOfPolygon(path)), SolidUnitString);
            var stockVolume = Volume.FromCubicMillimeters(_waterjetDepth.Millimeters* area.SquareMillimeters);
            
            
            if (stockVolume >= SolidVolume) return stockVolume;
            
            if (stockVolume < SolidVolume * .95)
            {
                Debug.WriteLine("Error in Waterjet Volume implemenation (or with part geometry). Stock volume should always be larger than the final volume.");
                
                //Stream stream = File.Create("silhouetteErrorPart.STL");
                //IO.Save(stream, new List<TessellatedSolid>() { Solid }, FileType.STL_ASCII);
                //stream.Close();
            }
            stockVolume = SolidVolume;
            return stockVolume;
        }

        private List<List<List<double[]>>> GetWaterjetCrossSections(double centerDistance)
        {
            var crossSections = GetBlankCrossSections(_waterjetShape, _waterjetDirection, _waterjetDepth, centerDistance);
            return crossSections;
        }

        private List<List<List<double[]>>> GetWaterjetCrossSections()
        {
            var crossSections = GetBlankCrossSections(_waterjetShape, _waterjetDirection, _waterjetDepth);
            return crossSections;
        }

        private TessellatedSolid GetWaterjetStockSolid()
        {
            return TVGL.Extrude.FromLoops(WaterjetCrossSections[0], _waterjetDirection.multiply(-1),
                _waterjetDepth.TesselatedSolidBaseUnit(SolidUnitString));
        }

        private void ShowWaterjetShape()
        {
            Presenter.ShowVertexPathsWithSolid(WaterjetCrossSections, new List<TessellatedSolid>() { _waterjetStockSolid.Value });
        }
        #endregion

        #region Additive
        private IList<List<Point>> GetAdditiveSilhouette()
        {
            if (Flat != null)
            {
                return PolygonOperations.OffsetRound(_silhouetteAlongNormal,
                    _searchInputs.WireFeed.WireAccuracy.TesselatedSolidBaseUnit(SolidUnitString));
            }

            //The normal had not been defined. Get the silhouette along the smallest OBB direciton
            //ToDo: Getting the best direction is more complicated than this.
            return PolygonOperations.OffsetRound(_silhouetteWithShortDimensionOBB.Value,
                _searchInputs.WireFeed.WireAccuracy.TesselatedSolidBaseUnit(SolidUnitString));
        }

        private Length GetAdditivePerimeterOnPlane()
        {
            //we want the outer perimeter, so join everything
            var unionSilhouette = PolygonOperations.Union(AdditiveShapeOnPlane);
            var perimeter = unionSilhouette.Sum(path => MiscFunctions.Perimeter(path));
            if (Math.Sign(perimeter) != 1) throw new Exception("Error in implemenation. This number should be positive and great than zero");
            return Length.FromTesselatedSolidBaseUnit(perimeter, SolidUnitString);
        }

        private Area GetAdditiveAreaOnPlane()
        {
            var area = AdditiveShapeOnPlane.Sum(path => MiscFunctions.AreaOfPolygon(path));
            if (Math.Sign(area) != 1) throw new Exception("Error in implemenation. This number should be positive and great than zero");
            return Area.FromTesselatedSolidBaseUnit(area, SolidUnitString);
        }

        private Volume GetAdditiveVolume()
        {
            var directions = new List<double[]>();
            if (Flat != null)
            {
                //NOT reverse of plane normal. 
                //Rather, it must be the direction toward the plane.
                List<Vertex> bottomVertices, topVertices;
                MinimumEnclosure.GetLengthAndExtremeVertices(Flat.Normal, Solid.Vertices,
                    out bottomVertices, out topVertices);
                var furthestForward = topVertices[0].Position.dotProduct(Flat.Normal) - Flat.DistanceToOrigin;
                var furthestReverse = bottomVertices[0].Position.dotProduct(Flat.Normal) - Flat.DistanceToOrigin;
                directions.Add(Math.Abs(furthestForward) > Math.Abs(furthestReverse)
                    ? Flat.Normal.multiply(-1)
                    : Flat.Normal);
            }              
            else
            {
                //ToDo: In fact, the best direction could be any of the six primary or other directions
                //The normal had not been defined. Get the volume along the smallest OBB direciton
                //Also try the reverse direction.
                var buildDirection = OBBSmallestNormal;
                directions.Add(buildDirection); 
                directions.Add(buildDirection.multiply(-1));
            } 
            
            //No need it taking a step size smaller than the wire feed accuracy
            //var stepSize = _searchInputs.WireFeed.WireAccuracy.TesselatedSolidBaseUnit(SolidUnitString);
            var minAdditiveVolume = double.MaxValue;
            var additiveOutputData = new List<DirectionalDecomposition.DecompositionData>();

            foreach (var direction in directions)
            {
                List<Vertex> bottomVertices, topVertices;
                var length = MinimumEnclosure.GetLengthAndExtremeVertices(direction, Solid.Vertices,
                    out bottomVertices, out topVertices);
                var stepSize = length / _searchInputs.WireFeedstock.NumberOfSlices.Unitless;
                if (length < 2 * stepSize)
                {
                    AdditiveIsFeasible = false;
                    return Volume.MaxValue;
                }

                var originalDecompositionData = DirectionalDecomposition.UniformAreaDecomposition(Solid, direction, stepSize);
                

                var stockVolume = DirectionalDecomposition.AdditiveVolume(originalDecompositionData,
                    _searchInputs.WireFeed.WireAccuracy.TesselatedSolidBaseUnit(SolidUnitString), out additiveOutputData);

                if (stockVolume < minAdditiveVolume)
                {
                    minAdditiveVolume = stockVolume;
                    _additiveBuildDirection = direction;

                }
            }

            if (minAdditiveVolume < Solid.Volume)
            {
                ShowAdditiveBlank(additiveOutputData); 
                Debug.WriteLine("Error in implementation of additive volume: For solid volume of " + Volume.FromTesselatedSolidBaseUnit(Solid.Volume, SolidUnitString).CubicInches + " in^3");
                minAdditiveVolume = 1.05 * Solid.Volume; // Use an estimate.
            }

            return Volume.FromTesselatedSolidBaseUnit(minAdditiveVolume, SolidUnitString);
        }

        private TessellatedSolid GetAdditiveStockSolid()
        {
            var additiveCrossSections = AdditiveCrossSections;         
            
            //Get the additive stock shape as a collection of small extrusions. 
            var stockSolidCompilation = new List<TessellatedSolid>();
            var faces = new List<PolygonalFace>();
            var vertices = new List<Vertex>();
            TessellatedSolid stockSolidPiece;
            List<List<double[]>> crossSection;
            for (var i = 0; i < additiveCrossSections.Count -1; i++)
            {
                // First, get the distance between this cross section and the next cross section.
                crossSection = additiveCrossSections[i];
                var distanceToCrossSection = _additiveDecompositionData[i].DistanceAlongDirection;
                var distanceToNextCrossSection = _additiveDecompositionData[i + 1].DistanceAlongDirection;
                //Delta X will be positive (it was checked during the volume calculation in the GetForgingVolume() function)
                var deltaX = distanceToNextCrossSection - distanceToCrossSection;
                if (deltaX.IsLessThanNonNegligible()) throw new Exception("Should never be negative");

                //Ideally we would use an extrude with draft function, but this may be ok for now. If extruding with draft and a hole closes, then don't include it in the extrude.
                //if(i == 3) Presenter.ShowVertexPathsWithSolid(new List<List<List<double[]>>>() {crossSection}, new List<TessellatedSolid>() { Solid });
                try
                {
                    stockSolidPiece = TVGL.Extrude.FromLoops(crossSection,
                        _additiveBuildDirection.multiply(1), deltaX);
                    stockSolidCompilation.Add(stockSolidPiece);
                    faces.AddRange(stockSolidPiece.Faces);
                    vertices.AddRange(stockSolidPiece.Vertices);
                }
                catch
                {
                    //Presenter.ShowVertexPaths(new List<List<List<double[]>>>() { crossSection });
                    Debug.WriteLine("Did not find additive stock volume accurately");
                    throw new Exception("This Additive Stock Solid is not valid.");
                }
            }

            //Do the last extrude. Add a little extra so it can be cut to find the join area.
            crossSection = additiveCrossSections.Last();
            stockSolidPiece = TVGL.Extrude.FromLoops(crossSection,
                    _additiveBuildDirection.multiply(1), _searchInputs.WireFeed.WireAccuracy.TesselatedSolidBaseUnit(SolidUnitString) / 100);
            stockSolidCompilation.Add(stockSolidPiece);
            faces.AddRange(stockSolidPiece.Faces);
            vertices.AddRange(stockSolidPiece.Vertices);

            //Do an extra extrusion on the other side as well.
            //Do the last extrude. Add a little extra so it can be cut to find the join area.
            crossSection = additiveCrossSections.First();
            stockSolidPiece = Extrude.FromLoops(crossSection,
                    _additiveBuildDirection.multiply(-1), _searchInputs.WireFeed.WireAccuracy.TesselatedSolidBaseUnit(SolidUnitString)/100);
            stockSolidCompilation.Add(stockSolidPiece);
            faces.AddRange(stockSolidPiece.Faces);
            vertices.AddRange(stockSolidPiece.Vertices);

            return new TessellatedSolid(faces, vertices, null, Solid.Units, Solid.Name);
        }

        private List<List<List<double[]>>> GetAdditiveCrossSections()
        {
            //No need it taking a step size smaller than the wire feed accuracy, except when the part has thin features.
            //For now, take a minimum number of step sizes as determined by the user.
            List<Vertex> v1, v2;
            var length = MinimumEnclosure.GetLengthAndExtremeVertices(AdditiveBuildDirection, Solid.Vertices, out v1,
                out v2);
            var stepSize = length / _searchInputs.WireFeedstock.NumberOfSlices.Unitless;
            var originalDecompositionData = DirectionalDecomposition.UniformAreaDecomposition(Solid, AdditiveBuildDirection, stepSize);
            //ShowAdditiveBlank(originalDecompositionData, _additiveBuildDirection);
            List<DirectionalDecomposition.DecompositionData> additiveOutputData;
            DirectionalDecomposition.AdditiveVolume(originalDecompositionData,
                _searchInputs.WireFeed.WireAccuracy.TesselatedSolidBaseUnit(SolidUnitString), out additiveOutputData);
            _additiveDecompositionData = additiveOutputData;
            //ShowAdditiveBlank(additiveOutputData, _additiveBuildDirection);

            //Convert additiveOutputData to cross sections.
            var crossSectionDict = new Dictionary<int, IList<List<Point>>>();
            var distances = new Dictionary<int, double>();
            for (var i = 0; i < additiveOutputData.Count; i++)
            {
                crossSectionDict.Add(i, additiveOutputData[i].Paths);
                distances.Add(i, additiveOutputData[i].DistanceAlongDirection);
            }
            return GetBlankCrossSections(AdditiveBuildDirection, crossSectionDict, distances);
        }

        private void ShowAdditiveBlank(List<DirectionalDecomposition.DecompositionData> additiveOutputData = null, double[] direction = null)
        {
            if (additiveOutputData == null)
            {
                Presenter.ShowVertexPathsWithSolid(AdditiveCrossSections, new List<TessellatedSolid>() {Solid});
            }
            else
            {
                //Convert additiveOutputData to cross sections.
                var crossSectionDict = new Dictionary<int, IList<List<Point>>>();
                var distances = new Dictionary<int, double>();
                for (var i = 0; i < additiveOutputData.Count; i++)
                {
                    crossSectionDict.Add(i, additiveOutputData[i].Paths);
                    distances.Add(i, additiveOutputData[i].DistanceAlongDirection);
                }
                if (direction == null) direction = _additiveBuildDirection;
                Presenter.ShowVertexPathsWithSolid(GetBlankCrossSections(direction, crossSectionDict, distances), new List<TessellatedSolid>() { Solid });
            }           
        }
        #endregion

        #region Supporting Functions: Bounding Objects, Silhouette, Convex Hull
        private static List<Point> CreateCirclePath(BoundingCircle circle, double offset = 0.0)
        {
            var center = circle.Center;
            var r = circle.Radius + offset;
            var path = new List<Point>();
            for (var theta = 0.0; theta < Math.PI*2; theta += Math.PI/50.0)
            {
                path.Add(new Point(r*Math.Cos(theta)+center.X, r * Math.Sin(theta) + center.Y));
            }
            return path;
        }
        private BoundingBox GetMinimumBoundingBox()
        {
            //If the convex hull was not successfull for this solid, just use all its vertices
            var vertices = Solid.ConvexHull.Succeeded ? Solid.ConvexHull.Vertices : Solid.Vertices;

            if (Flat == null)
            {
                return MinimumEnclosure.OrientedBoundingBox(vertices);
            }
            return MinimumEnclosure.OBBAlongDirection(vertices, Flat.Normal);
        }

        private List<double[]> GetBoundingRectangleDirections3D()
        {
            if (Flat == null)
            {
                throw new Exception("Should not call this for seed.");
            }

            //Return whichever two directions are not the flat normal
            var directions = _minimumBoundingBox.Directions.Where(direction => !Math.Abs(direction.dotProduct(Flat.Normal)).IsPracticallySame(1.0)).ToList();
            if(directions.Count != 2) throw new Exception("Flat normal not found in bounding box dimensions");
            return directions;
        } 

        private List<Length> GetBoundingRectangleDimensions()
        {
            var directions = _minimumBoundingBox.Directions;
            var dimensions = _minimumBoundingBox.Dimensions;

            List<Vertex> v1, v2;
            //Check to make sure the directions match the dimensions
            for (var i = 0; i < 3; i++)
            {
                var dim = MinimumEnclosure.GetLengthAndExtremeVertices(directions[i], Solid.Vertices, out v1, out v2);
                //Check within 10%. For some reason the values do not always line up perfectly.
                if(!dim.IsPracticallySame(dimensions[i], 0.1* dimensions[i])) throw new Exception("Dimensions do not match directions.");
            }

            var boundingRectangleDirections3D = GetBoundingRectangleDirections3D();
            var lengths = new List<Length>();
            for (var i = 0; i < 2; i++)
            {
                var dim = MinimumEnclosure.GetLengthAndExtremeVertices(boundingRectangleDirections3D[i], Solid.Vertices, out v1, out v2);
                lengths.Add(Length.FromTesselatedSolidBaseUnit(dim, SolidUnitString));
            }
            return lengths;
        }

        private double[] OBBLargestNormal => _minimumBoundingBox.SortedDirectionsByLength.Last();

        private double[] OBBSmallestNormal => _minimumBoundingBox.SortedDirectionsByLength.First();
        
        private IList<double[]> SortedOBBDirectionsByLength => _minimumBoundingBox.SortedDirectionsByLength;

        private Length OBBSmallestDimension => 
            Length.FromTesselatedSolidBaseUnit(_minimumBoundingBox.SortedDimensions.First(), SolidUnitString);

        private List<List<Point>> GetSilhouetteAlongNormal()
        {
            return Flat == null ? null : GetSilhouette(Flat.Normal);
        }

        private IList<List<Point>> GetSilhouetteWithShortDimensionOBB()
        {
            return GetSilhouette(OBBSmallestNormal);
        }

        private IList<Point> GetConvexHull2D()
        {
            return MinimumEnclosure.ConvexHull2D(_projectedPoints, Solid.SameTolerance);
        }

        private IList<Point> GetProjectedPoints()
        {
            if (Flat == null)
            {
                return MiscFunctions.Get2DProjectionPoints(Solid.Vertices, OBBLargestNormal);
            }
            return MiscFunctions.Get2DProjectionPoints(Solid.Vertices, Flat.Normal);
        }

        private Length GetSubvolumeDepthAlongPlaneNormal()
        {
            if (Flat == null)
            {
                //ToDo: remove after testing
                throw new Exception("This function should not be called in the flat is null");
            }
            List<Vertex> vLow, vHigh;
            return Length.FromTesselatedSolidBaseUnit(MinimumEnclosure.GetLengthAndExtremeVertices(Flat.Normal, Solid.Vertices, out vLow, out vHigh), SolidUnitString);
        }

        private Length GetLengthOfSilhouetteNearPlane(IList<List<Point>> silhouette, Length tolerance, double[] silhouetteNormal)
        {

            //First, we need to convert the cutting plane into a line, since the current view is perpendicular
            var directions = GetBoundingRectangleDirections3D();
            //Get a line vectors and point on plane.
            var normal2 = directions[0];
            if(silhouetteNormal == normal2) normal2 = directions[1];
            var vertexOnPlane = new Vertex(Flat.Normal.multiply(Flat.DistanceToOrigin));
            var vertex2 = new Vertex(vertexOnPlane.Position.add(normal2.multiply(100)));

            //Now project these points onto the current 2D plane
            var pLow = MiscFunctions.Get2DProjectionPoints(new List<Vertex> { vertexOnPlane}, silhouetteNormal).First();
            var pHigh = MiscFunctions.Get2DProjectionPoints(new List<Vertex> { vertex2 }, silhouetteNormal).First();
            var lineVector = pHigh.Position.subtract(pLow.Position);

            //Build a set of polygons, setting the polygon index for each.
            var toleranceInSolidUnits = tolerance.TesselatedSolidBaseUnit(SolidUnitString);
            var polygons = silhouette.Select((t, i) => new Polygon(t, false, i)).ToList();

            //Now each point has a polygon index and path index in that polygon.

            var allPointDistances = new List<Dictionary<int, double>>();
            foreach (var polygon in polygons)
            {
                var pointDistances = new Dictionary<int, double>();
                foreach (var line in polygon.PathLines)
                {
                    if (!pointDistances.ContainsKey(line.FromPoint.IndexInPath))
                    {
                        pointDistances.Add(line.FromPoint.IndexInPath,
                            MiscFunctions.DistancePointToLine(line.FromPoint.Position, pLow.Position, lineVector));
                    }
                    if (!pointDistances.ContainsKey(line.ToPoint.IndexInPath))
                    {
                        pointDistances.Add(line.ToPoint.IndexInPath,
                            MiscFunctions.DistancePointToLine(line.ToPoint.Position, pLow.Position, lineVector));
                    }
                }
                allPointDistances.Add(pointDistances);
            }
            var lengthOnTop = 0.0;
            for (var i = 0; i < polygons.Count; i++) // polygon in polygons)
            {
                var polygon = polygons[i];
                var pointDistances = allPointDistances[i];
                foreach (var line in polygon.PathLines)
                {
                    var value1 = pointDistances[line.FromPoint.IndexInPath];
                    var value2 = pointDistances[line.ToPoint.IndexInPath];
                    //ToDo: Figure out this tolerance. Note that it is effected by the slice offset and the waterjetShape offset
                    if (value1.IsNegligible(toleranceInSolidUnits) && value2.IsNegligible(toleranceInSolidUnits))
                    {
                        lengthOnTop += line.Length;
                    }
                }
            }

            if (!lengthOnTop.IsNegligible()) return Length.FromTesselatedSolidBaseUnit(lengthOnTop, SolidUnitString);
            
            //Else, correct the issue. Just use an estimate.
            //var listPoints = new List<List<Point>>(silhouette) {new List<Point>() {pLow, pHigh} };
            //Presenter.ShowAndHang(listPoints);
            //Presenter.ShowAndHang(Solid);
            Debug.WriteLine("Error in finding Length on top of silhouette.");
            List<Vertex> v1, v2;
            lengthOnTop = MinimumEnclosure.GetLengthAndExtremeVertices(normal2, Solid.Vertices, out v1, out v2);
            return Length.FromTesselatedSolidBaseUnit(lengthOnTop, SolidUnitString);
        }

        public List<List<Point>> GetSilhouette(double[] normal)
        {
            var solution = new List<List<Point>>();
            var attempt = 0;
            var tolerance = 1.0; //degree
            var success = false;
            while (attempt < 4 && !success)
            {
                try
                {
                    solution = Silhouette.Run(Solid, normal, tolerance);
                    if (!solution.Any()) throw new Exception();
                    success = true;
                }
                catch
                {
                    attempt++;
                    tolerance = tolerance + 1;
                }
            }
            if (success) return solution;
            Debug.WriteLine("Error in Finding Silhouette");
            Stream stream = File.Create("silhouetteErrorPart.STL");
            IO.Save(stream, new List<TessellatedSolid>() { Solid }, FileType.STL_ASCII);
            stream.Close();

            return solution;
        }

        public List<List<List<double[]>>> GetBlankCrossSections(IEnumerable<List<Point>> paths, double[] direction,
            Length depth)
        {
            List<Vertex> posVertex, negVertex;
            MinimumEnclosure.GetLengthAndExtremeVertices(direction, Solid.Vertices, out negVertex, out posVertex);

            //component b onto a = (a.dot(b) / |a|) => since direction is a unit vector: = a.dot(b)
            var forwardDistanceToEdgeOfSolid = direction.dotProduct(posVertex.First().Position);
            var reverseDistanceToEdgeOfSolid = direction.dotProduct(negVertex.First().Position);
            var centerDistance = (forwardDistanceToEdgeOfSolid - reverseDistanceToEdgeOfSolid)/2 +
                                 reverseDistanceToEdgeOfSolid;

            return GetBlankCrossSections(paths, direction, depth, centerDistance);
        }


        /// <summary>
        /// This function is used the find the shape of cutting plane for rectangular blanks, given their cross section. 
        /// This function is only needed for blank based, since normal slicing gets this info from cutting the blank.
        /// </summary>
        /// <param name="crossSection"></param>
        /// <param name="joinDirection"></param>
        /// <param name="buildDirection"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public List<Point> GetRectangularBlankShapeOnCuttingPlane(IList<double[]> crossSection, 
            double[] joinDirection, double[] buildDirection, Length depth)
        {
            var crossSectionVertices = crossSection.Select(position => new Vertex(position)).ToList();

            double[,] backTransform;
            //The join direction is the same as the direction the cross sections were defined along.
            //NOTE: The search direction is the same as the join direction. We DO NOT want to reverse this 
            //direction for a projection. It must be along the join direction
            if (Math.Abs(joinDirection.dotProduct(buildDirection))
                .IsPracticallySame(1.0, Constants.SameFaceNormalDotTolerance))
            {
                var path = MiscFunctions.Get2DProjectionPoints(crossSectionVertices, joinDirection, out backTransform);
                return MiscFunctions.AreaOfPolygon(path) < 0 ? path.Reverse().ToList() : path.ToList();
            }

            //Because the vertices are centerd along the build direction, add half the depth 
            //and subtract half the depth on the vertices on one extreme of the joining direction
            //(It does not matter if it is max or min distance) to get the join cross section.
            List<Vertex> sortedVertices;
            List<int[]> duplicateRanges;
            MiscFunctions.SortAlongDirection(new[] { joinDirection }, crossSectionVertices, out sortedVertices, out duplicateRanges);
            var halfDepthVector = buildDirection.multiply(depth.TesselatedSolidBaseUnit(SolidUnitString));
            var contour = new List<Vertex>()
            {
                new Vertex(sortedVertices[0].Position.add(halfDepthVector)),
                new Vertex(sortedVertices[0].Position.subtract(halfDepthVector)),
                new Vertex(sortedVertices[1].Position.subtract(halfDepthVector)),
                new Vertex(sortedVertices[1].Position.add(halfDepthVector)),
            };
            var joinCrossSection = MiscFunctions.Get2DProjectionPoints(contour, joinDirection, out backTransform).ToList();
            //Make sure the cross section is ordered properly
            if (MiscFunctions.AreaOfPolygon(joinCrossSection) < 0)
            {
                joinCrossSection.Reverse();
            }
            if (MiscFunctions.AreaOfPolygon(joinCrossSection).IsNegligible())
            {
                Debug.WriteLine("Rectangular Blank join area is negligible");
            }
            return joinCrossSection;

        }

        public List<List<List<double[]>>> GetBlankCrossSections(IEnumerable<List<Point>> paths, double[] direction, 
            Length depth, double centerDistanceOfBlank)
        {
            //Rotate axis back to the original, and then transform points along the given direction.
            //Center the depth on the solid
            double[,] backTransform;
            MiscFunctions.TransformToXYPlane(direction, out backTransform);
            var contours = new List<List<double[]>>();
            var contours2 = new List<List<double[]>>();
            foreach (var path in paths)
            {
                var contour = new List<double[]>();
                var contour2 = new List<double[]>();
                foreach (var point in path)
                {
                    var position = new[] { point.X, point.Y, 0.0, 1.0 };
                    var untransformedPosition = backTransform.multiply(position);
                    var vertexPositionForward = untransformedPosition.Take(3).ToArray().add(direction.multiply(centerDistanceOfBlank + depth.TesselatedSolidBaseUnit(SolidUnitString) / 2));
                    var vertexPositionReverse = untransformedPosition.Take(3).ToArray().add(direction.multiply(centerDistanceOfBlank - depth.TesselatedSolidBaseUnit(SolidUnitString) / 2));

                    contour.Add(vertexPositionForward);
                    contour2.Add(vertexPositionReverse);
                }
                contours.Add(contour);
                contours2.Add(contour2);
            }
            return new List<List<List<double[]>>>() { contours, contours2 };
        }

        public static List<List<List<double[]>>> GetBlankCrossSections(double[] direction, Dictionary<int, IList<List<Point>>> blankData, Dictionary<int, double> distances )
        {
            var crossSections = new List<List<List<double[]>>>();
            double[,] backTransform;
            MiscFunctions.TransformToXYPlane(direction, out backTransform);
            foreach (var key in blankData.Keys)
            {
                var contours = new List<List<double[]>>();
                var paths = blankData[key];
                var distance = distances[key];
                foreach (var path in paths)
                {
                    var contour = new List<double[]>();
                    foreach (var point in path)
                    {
                        var position = new[] { point.X, point.Y, 0.0, 1.0 };
                        var untransformedPosition = backTransform.multiply(position);
                        var vertexPosition = untransformedPosition.Take(3).ToArray().add(direction.multiply(distance));
                        contour.Add(vertexPosition);
                    }
                    contours.Add(contour);
                }
                crossSections.Add(contours);    
            }
            return crossSections;
        }
        #endregion
    }


    public class ForgingShapeData : DirectionalDecomposition.DecompositionData
    {
        public bool IsContributing { get; set; }

        private readonly Lazy<double> _area;

        public double Area => _area.Value;

        internal ForgingShapeData(IList<List<Point>> paths, double distanceAlongDirection) : base(paths, distanceAlongDirection)
        {
            _area = new Lazy<double>(GetArea);
        }

        internal double GetArea()
        {
            return MiscFunctions.AreaOfPolygon(Paths);
        }
    }
}

