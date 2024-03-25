using System;
using System.Text;
using XRL.Core;
using XRL.World;
using XRL.World.Parts;

namespace XRL.Liquids
{
    [Serializable]
    [IsLiquid]
    public class LiquidEgo : BaseLiquid
    {
        public new const string ID = "rawego";

        public LiquidEgo() : base(ID)
        {
            VaporTemperature = 1000;
            Weight = 0.01;
            Glows = true;
        }

        public override string GetSmearedAdjective(LiquidVolume Liquid)
        {
            return "{{snakefangox_astralmedusae_egotistic|egotistic}}";
        }

        public override string GetSmearedName(LiquidVolume Liquid)
        {
            return "{{snakefangox_astralmedusae_egotistic|egotistical}}";
        }

        public override string GetName(LiquidVolume Liquid)
        {
            return "{{snakefangox_astralmedusae_egotistic|raw ego}}";
        }

        public override string GetWaterRitualName()
        {
            return "raw ego";
        }

        public override string GetStainedName(LiquidVolume Liquid)
        {
            return "{{snakefangox_astralmedusae_egotistic|raw ego}}";
        }

        public override string GetAdjective(LiquidVolume Liquid)
        {
            return "{{snakefangox_astralmedusae_egotistic|egotistic}}";
        }

        public override string GetPreparedCookingIngredient()
        {
            return "dissociation";
        }

        public override float GetValuePerDram()
        {
            return 500f;
        }

        public override bool Drank(LiquidVolume Liquid, int Volume, GameObject Target, StringBuilder Message, ref bool ExitInterface)
        {
            if (Liquid.IsPure() && "1d10".Roll() == 10)
            {
                Message.Compound("You taste a hint of greatness and your ego {{snakefangox_astralmedusae_egotistic|swells}}");
                Target.AddBaseStat("Ego", 1);
            }
            else
            {
                Message.Compound("You taste a hint of greatness, but nothing more.");
            }

            ExitInterface = true;
            return true;
        }

        public override void RenderBackgroundPrimary(LiquidVolume Liquid, RenderEvent eRender)
        {
            if (eRender.ColorsVisible)
            {
                eRender.ColorString = "^m" + eRender.ColorString;
            }
        }

        public override void BaseRenderPrimary(LiquidVolume Liquid)
        {
            Liquid.ParentObject.pRender.ColorString = "&M^C";
            Liquid.ParentObject.pRender.TileColor = "&M";
            Liquid.ParentObject.pRender.DetailColor = "C";
        }

        public override void BaseRenderSecondary(LiquidVolume Liquid)
        {
            Liquid.ParentObject.pRender.ColorString += "&C";
        }

        public override void RenderSmearPrimary(LiquidVolume Liquid, RenderEvent eRender, GameObject obj)
        {
            if (eRender.ColorsVisible)
            {
                int num = XRLCore.CurrentFrame % 60;
                if (num > 15 && num < 35)
                {
                    eRender.ColorString = "&C";
                }
            }

            base.RenderSmearPrimary(Liquid, eRender, obj);
        }

        public override void RenderSecondary(LiquidVolume Liquid, RenderEvent eRender)
        {
            if (eRender.ColorsVisible)
            {
                eRender.ColorString += "&M";
            }
        }
    }
}