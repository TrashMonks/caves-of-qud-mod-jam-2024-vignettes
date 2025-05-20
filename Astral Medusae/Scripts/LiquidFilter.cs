

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

        public override void TurnTick(long TurnNumber, int Amount)
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
