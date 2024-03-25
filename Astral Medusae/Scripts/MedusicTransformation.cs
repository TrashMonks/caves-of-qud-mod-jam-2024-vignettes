using System;
using Qud.UI;
using XRL.UI;

namespace XRL.World.Parts
{
    [Serializable]
    public class Snakefangox_AstralMedusae_MedusicTransformation : IPart
    {
        public override bool WantEvent(int ID, int cascade)
        {
            if (!base.WantEvent(ID, cascade) && ID != GetInventoryActionsEvent.ID)
            {
                return ID == InventoryActionEvent.ID;
            }

            return true;
        }

        public override bool HandleEvent(GetInventoryActionsEvent E)
        {
            E.AddAction("Eat", "eat", "Eat", Key: 'a');
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(InventoryActionEvent E)
        {
            if (E.Command == "Eat")
            {
                SoundManager.PlaySound("Sounds/Abilities/sfx_ability_recoiler_use");
                Popup.Show("You sense renewed purpose. You support multitudes, the pattern matches.");
                Popup.Show("Flesh-steel knits itself through your core, your skin pulled to perfect edges.");
                Popup.Show("Your insides grow gigantic into countersunk space, your passengers are made safe for travel.");

                var mutations = E.Actor.GetPart<Mutations>();
                mutations.AddMutation("Snakefangox_AstralMedusae_Vessel");
            }

            return base.HandleEvent(E);
        }
    }
}