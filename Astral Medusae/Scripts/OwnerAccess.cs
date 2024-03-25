
using System;
using XRL.UI;

namespace XRL.World.Parts
{
    [Serializable]
    public class Snakefangox_AstralMedusae_OwnerAccess : IPart
    {
        public override bool HandleEvent(ObjectLeavingCellEvent E) {
            if (CanPassThrough(E.Object) && !ParentObject.pPhysics.Solid) {
                ParentObject.pPhysics.Solid = true;
            }

            return base.HandleEvent(E);
        }

        public bool CanPathThrough(GameObject who)
        {
            return CanPassThrough(who) || !ParentObject.pPhysics.Solid;
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "BeforePhysicsRejectObjectEntringCell");
            base.Register(Object);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeforePhysicsRejectObjectEntringCell" && ParentObject.pPhysics.Solid)
            {
                GameObject obj = E.GetGameObjectParameter("Object");
                if (CanPassThrough(obj)) {
                    ParentObject.pPhysics.Solid = false;
                }
            }

            return base.FireEvent(E);
        }

        public bool CanPassThrough(GameObject who)
        {
            if (ParentObject.CurrentZone is InteriorZone interiorZone && interiorZone.ParentObject.HasPart<Vehicle>())
            {
                Vehicle vehicle = interiorZone.ParentObject.GetPart<Vehicle>();
                return who.ID == vehicle.OwnerID || (vehicle.OwnerID == "Player" && who.IsPlayer());
            }

            return false;
        }
    }
}