using System.Runtime.Serialization;
using KatanaObjects.BaseClasses;
using UnitsNet;

namespace KatanaObjects.Blanks
{
    [DataContract]
    public class ForgingBlank : Blank
    {  
        [DataMember]
        public Volume ToolingExcess { get; set; }

        [DataMember]
        public Volume TestingCoupon { get; set; }

        [DataMember]
        public Volume YieldLoss { get; set; }

        internal ForgingBlank(SubVolume subVolume) : base(subVolume)
        {
            //Set these parameters regardless if this is the seed.
            Type = BlankType.ClosedDieForging;
            StockVolume = VolumeIsUnderestimate ? SubVolume.ForgingVolumeUnderestimate : SubVolume.ForgingVolume;
            WasteVolume = StockVolume - SubVolume.SolidVolume;
            FinishVolume = SubVolume.SolidVolume;
            IsFeasible = SubVolume.ForgingIsFeasible;
            TestingCoupon = SubVolume.ForgingTestingCoupon;
            ToolingExcess = SubVolume.ForgingToolingExcess;
            YieldLoss = SubVolume.ForgingYieldLoss;

            if (subVolume.IsSeed) return;

            //Only set these parameters if the the forging blank is not the seed (beam search depth > 0)
            AreaIsCircular = false;

            //Skip parameters that cannot be set without an accurate volume estimation
            if (VolumeIsUnderestimate) return;
            SubVolume._minForgingBlankData = null;
        }
    }
}
