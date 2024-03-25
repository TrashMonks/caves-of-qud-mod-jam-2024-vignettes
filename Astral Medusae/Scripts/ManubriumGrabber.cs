using System;
using XRL.World.Effects;

namespace XRL.World.Parts
{
    [Serializable]
    public class Snakefangox_AstralMedusae_ManubriumGrabber : IPart
    {
        public int Difficulty = 15;

        public override bool WantEvent(int ID, int cascade)
        {
            if (base.WantEvent(ID, cascade) || ID == GetNavigationWeightEvent.ID || ID == EndTurnEvent.ID)
            {
                return true;
            }

            return false;
        }

        public override bool HandleEvent(GetNavigationWeightEvent E)
        {
            E.MinWeight(99);
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(EndTurnEvent E)
        {
            GameObject obj = null;
            foreach (var o in ParentObject.CurrentCell.Objects)
            {
                if (!o.HasPart(GetType()))
                {
                    obj = o;
                }
            }

            if (obj == null)
            {
                return base.HandleEvent(E);
            }

            if (obj.HasEffect("Stun"))
            {
                Cell destCell = ParentObject.CurrentCell.GetCellFromDirection("U", BuiltOnly: false);
                obj.SystemMoveTo(destCell, forced: true);

                if (obj.IsPlayer())
                {
                    ZoneManager.ZoneTransitionCount -= 1;
                    AddPlayerMessage("You are grabbed and pulled upwards!");
                }
            }
            else if (!obj.MakeSave("Willpower", Difficulty, Attacker: ParentObject))
            {
                obj.ApplyEffect(new Stun(5, Difficulty));
            }

            return base.HandleEvent(E);
        }
    }
}