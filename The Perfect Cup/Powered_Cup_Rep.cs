using System;
using XRL.Rules;
using XRL.World.Parts.Skill;

namespace XRL.World.Parts
{

    [Serializable]
    public class PoweredCupReputation : IPoweredPart
    {
        public PoweredCupReputation()
        {
            WorksOnEquipper = true;
            ChargeUse = 50;
            NameForStatus = "FriendlyCup";
        }

        public override bool SameAs(IPart p)
        {
            return false;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return
                base.WantEvent(ID, cascade)

                || ID == GetWaterRitualReputationAmountEvent.ID
                || ID == GetTinkeringBonusEvent.ID
            ;
        }
        
        public override bool HandleEvent(GetWaterRitualReputationAmountEvent E)
        {
            if (E.Actor == ParentObject.Equipped
                && !E.SpeakingWith.HasIntProperty("PerfectCupReputation")
                && E.Actor.IsPlayer()
                && E.Faction == E.Record.faction
                && IsReady(UseCharge:true)
                )
            {
                E.SpeakingWith.SetIntProperty("PerfectCupReputation", 1);
                E.Amount += 50;
            }
            return base.HandleEvent(E);
        }
        
        public override bool HandleEvent(GetTinkeringBonusEvent E)
        {
            if (
            (E.Type == "Inspect" || E.Type == "Disassemble")
            && E.Actor == ParentObject.Equipped
            && IsReady(UseCharge:false)
            )
            {
                E.Bonus += 2;
                
            }
            return base.HandleEvent(E);
        }

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

        public override void TurnTick(Int64 TurnNumber)
        {
            if (ConsumeChargeIfOperational(ChargeUse:1)
                && ParentObject.TryGetPart(out LiquidFueledPowerPlant plant)
                && plant.ChargeCounter == 0)
                {
                    XDidYToZ(
                        Actor:ParentObject.Equipped,
                        Verb: "take", 
                        Preposition: "a sip from",
                        Object:ParentObject);
                }

        }

        public override void TenTurnTick(Int64 TurnNumber)
        {
            ConsumeChargeIfOperational(ChargeUse: 10);
        }

        public override void HundredTurnTick(Int64 TurnNumber)
        {
            ConsumeChargeIfOperational(ChargeUse: 100);
        }


    }

}
