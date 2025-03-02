using Qud.API;
using System;

namespace XRL.World.Parts
{
    [Serializable]
    public class Kernelmethod_Vignettes2024_XoteotinStatue : IPart {
        public override void Register(GameObject Object, IEventRegistrar Registrar) {
            Registrar.Register("AfterLookedAt");
            base.Register(Object, Registrar);
        }

        public override bool FireEvent(Event E) {
            if (E.ID == "AfterLookedAt") {
                var mapNote = JournalAPI.GetMapNote("$kernelmethod_vignettes2024_mechlair");
                JournalAPI.RevealMapNote(mapNote);
            }

            return base.FireEvent(E);
        }
    }
}
