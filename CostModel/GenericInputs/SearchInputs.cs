using System.IO;
using System.Runtime.Serialization;
using TVGL;

namespace GenericInputs
{
    [DataContract]
    public class SearchInputs : Inputs
    {
        [DataMember]
        public bool UserSettingForUnitSystemIsMetric { get; set; }

        [DataMember]
        public UnitType UserInputGeometryUnits { get; set; }

        [DataMember]
        public GeneralInputs General { get; set; }

        [DataMember]
        public MachiningInputs Machining { get; set; }

        [DataMember]
        public LFWInputs LFW { get; set; }

        [DataMember]
        public RFWInputs RFW { get; set; }

        [DataMember]
        public MaterialHandlingInputs MaterialHandling { get; set; }

        [DataMember]
        public FlashRemovalInputs FlashRemoval { get; set; }

        [DataMember]
        public WireFeedInputs WireFeed { get; set; }

        [DataMember]
        public EngineeringQAInputs EngineeringQA { get; set; }

        [DataMember]
        public WaterjetInputs Waterjet { get; set; }

        [DataMember]
        public RectangularBarStockInputs RectangularBarStock { get; set; }

        [DataMember]
        public CircularBarStockInputs CircularBarStock { get; set; }

        [DataMember]
        public HollowTubeInputs HollowTube { get; set; }

        [DataMember]
        public ForgingInputs Forging { get; set; }

        [DataMember]
        public WireFeedstockInputs WireFeedstock { get; set; }

        [DataMember]
        public NearNetAdditiveInputs NearNetAdditive { get; set; }

        public SearchInputs()
        {
            General = new GeneralInputs();
            Machining = new MachiningInputs();
            LFW = new LFWInputs();
            RFW = new RFWInputs();
            MaterialHandling = new MaterialHandlingInputs();
            FlashRemoval = new FlashRemovalInputs();
            WireFeed = new WireFeedInputs();
            EngineeringQA = new EngineeringQAInputs();
            Waterjet = new WaterjetInputs();
            HollowTube = new HollowTubeInputs();
            RectangularBarStock = new RectangularBarStockInputs();
            CircularBarStock = new CircularBarStockInputs();
            Forging = new ForgingInputs();
            WireFeedstock = new WireFeedstockInputs();
            NearNetAdditive = new NearNetAdditiveInputs();
            //Default user setting for the unit system
            UserSettingForUnitSystemIsMetric = false;
            UserInputGeometryUnits = TVGL.UnitType.inch;
        }

        public SearchInputs(GeneralInputs general, MachiningInputs machining, LFWInputs lfw, RFWInputs rfw, WireFeedInputs wireFeed,
            EngineeringQAInputs engineeringqa, WaterjetInputs waterjet, HollowTubeInputs hollowTube, 
            RectangularBarStockInputs rectangularBarStock, CircularBarStockInputs circularBarStock, ForgingInputs forging, 
			MaterialHandlingInputs materialhandling, FlashRemovalInputs flashremoval,
            WireFeedstockInputs wireFeedstock, NearNetAdditiveInputs nearNetAdditive,
            UnitType userInputGeometryUnits, bool userSettingForUnitSystemIsMetric = false) 
        {
            General = general;
            Machining = machining;
            LFW = lfw;
            RFW = rfw;
            MaterialHandling = materialhandling;
            FlashRemoval = flashremoval;
            WireFeed = wireFeed;
            EngineeringQA = engineeringqa;
            Waterjet = waterjet;
            HollowTube = hollowTube;
            RectangularBarStock = rectangularBarStock;
            CircularBarStock = circularBarStock;
            Forging = forging;
            WireFeedstock = wireFeedstock;
            NearNetAdditive = nearNetAdditive;
            //Default user setting for the unit system
            UserSettingForUnitSystemIsMetric = userSettingForUnitSystemIsMetric;
            UserInputGeometryUnits = userInputGeometryUnits;
        }

        public new object Clone()
        {
            return new SearchInputs(
                    general: General.Clone() as GeneralInputs,
                    machining: Machining.Clone() as MachiningInputs,
                    lfw: LFW.Clone() as LFWInputs,
                    rfw: RFW.Clone() as RFWInputs,
                    wireFeed: WireFeed.Clone() as WireFeedInputs,
                    engineeringqa: EngineeringQA.Clone() as EngineeringQAInputs,
                    waterjet: Waterjet.Clone() as WaterjetInputs,
                    hollowTube: HollowTube.Clone() as HollowTubeInputs,
                    rectangularBarStock: RectangularBarStock.Clone() as RectangularBarStockInputs, 
                    circularBarStock: CircularBarStock.Clone() as CircularBarStockInputs, 
                    forging: Forging.Clone() as ForgingInputs,
                    wireFeedstock: WireFeedstock.Clone() as WireFeedstockInputs,
                    nearNetAdditive: NearNetAdditive.Clone() as NearNetAdditiveInputs, 
					materialhandling: MaterialHandling.Clone() as MaterialHandlingInputs,
					flashremoval: FlashRemoval.Clone() as FlashRemovalInputs,
                    userInputGeometryUnits: UserInputGeometryUnits,
                    userSettingForUnitSystemIsMetric: UserSettingForUnitSystemIsMetric
                );
        }

        public static void Serialize(SearchInputs inputs)
        {
            using (var writer = new FileStream("User.SearchInputs.xml", FileMode.Create, FileAccess.Write))
            {
                var ser = new DataContractSerializer(typeof(SearchInputs));
                ser.WriteObject(writer, inputs);
            }
        }

        public static SearchInputs Deserialize()
        {
            using (var reader = new FileStream("User.SearchInputs.xml", FileMode.Open, FileAccess.Read))
            {
                var ser = new DataContractSerializer(typeof(SearchInputs));
                var output = (SearchInputs)ser.ReadObject(reader);
                return output;
            }
        }
    }
}
