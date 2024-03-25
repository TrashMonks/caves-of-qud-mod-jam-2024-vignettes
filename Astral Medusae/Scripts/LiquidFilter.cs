

using System;

namespace XRL.World.Parts
{
    [Serializable]
    public class Snakefangox_AstralMedusae_LiquidFilter : IPart
    {
        public string FilterLiquid;
        public string Sound = "SplashStep1";

        public override bool WantTurnTick()
        {
            return true;
        }

        public override bool WantTenTurnTick()
        {
            return true;
        }

        public override bool WantHundredTurnTick()
        {
            return true;
        }

        public override void TurnTick(long TurnNumber)
        {
            Process();
        }

        public override void TenTurnTick(long TurnNumber)
        {
            Process();
        }

        public override void HundredTurnTick(long TurnNumber)
        {
            Process();
        }

        public void Process()
        {
            LiquidVolume volume = ParentObject.LiquidVolume;
            if (volume == null) { return; }

            if (volume.IsMixed() && volume.ContainsLiquid(FilterLiquid))
            {
                int amount = volume.Amount(FilterLiquid);
                volume.Empty();
                volume.Fill(FilterLiquid, amount);
                SoundManager.PlaySound(Sound);
            }
        }
    }
}