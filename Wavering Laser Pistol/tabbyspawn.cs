using System;

namespace XRL.World.Parts
{
    [Serializable]
  public class ModTabbyspawn : IModification
  {
    public int Chance = 100;

    public ModTabbyspawn()
    {
    }

    public ModTabbyspawn(int Tier)
      : base(Tier)
    {
    }

    public override void Configure()
    {
      this.WorksOnHolder = true;
      this.ChargeUse = 2000;
      this.IsEMPSensitive = true;
      this.IsBootSensitive = true;
      this.IsTechScannable = true;
      this.NameForStatus = "AstralBeacon";
    }

    public override void TierConfigure() => this.ChargeUse = Math.Max(2000 - this.Tier * 125, 100);

    public override void ApplyModification(GameObject Object)
    {
      if (this.ChargeUse <= 0 || Object.IsCreature)
        return;
      Object.RequirePart<EnergyCellSocket>();
      this.IncreaseDifficultyAndComplexity(3, 2);
    }

    public override bool SameAs(IPart p)
    {
      return (p as ModTabbyspawn).Chance == this.Chance && base.SameAs(p);
    }

    public override bool WantEvent(int ID, int cascade)
    {
      return base.WantEvent(ID, cascade) || ID == GetShortDescriptionEvent.ID;
    }

    public override bool HandleEvent(GetShortDescriptionEvent E)
    {
      if (!this.ParentObject.HasTag("Creature"))
        E.Postfix.AppendRules("When powered, attracts a friend from the astral plane on hit. Drains cell power quickly.", this.GetEventSensitiveAddStatusSummary((IEvent) E));
      return base.HandleEvent(E);
    }

    public override bool AllowStaticRegistration() => true;

    public override void Register(GameObject Object, IEventRegistrar Registrar)
    {
      Registrar.Register("WeaponHit");
      Registrar.Register("AttackerAfterDamage");
      Registrar.Register("DealingMissileDamage");
      Registrar.Register("WeaponMissileWeaponHit");
      Registrar.Register("WeaponPseudoThrowHit");
      Registrar.Register("WeaponThrowHit");
      base.Register(Object, Registrar);
    }

    public override bool FireEvent(Event E)
    {
      if (E.ID == "WeaponHit" || E.ID == "AttackerAfterDamage" || E.ID == "DealingMissileDamage" || E.ID == "WeaponMissileWeaponHit" || E.ID == "WeaponPseudoThrowHit" || E.ID == "WeaponThrowHit")
      {
        GameObject gameObjectParameter1 = E.GetGameObjectParameter("Attacker");
        GameObject gameObjectParameter2 = E.GetGameObjectParameter("Defender");
        GameObject gameObjectParameter3 = E.GetGameObjectParameter("Weapon");
        GameObject gameObjectParameter4 = E.GetGameObjectParameter("Projectile");
        Cell currentCell = gameObjectParameter2.CurrentCell;
        if (currentCell != null)
        {
          Cell randomElement = currentCell.GetLocalEmptyAdjacentCells().GetRandomElement<Cell>((Random) null);
          if (randomElement != null && this.IsReady(true))
          {
            GameObject Actor = gameObjectParameter1;
            GameObject Object1 = gameObjectParameter3;
            GameObject gameObject1 = gameObjectParameter2;
            GameObject gameObject2 = gameObjectParameter4;
            int chance = this.Chance;
            GameObject Subject = gameObject1;
            GameObject Projectile = gameObject2;
            if (GetSpecialEffectChanceEvent.GetFor(Actor, Object1, "Modification ModTabbyspawn Activation", chance, Subject, Projectile).in100())
            {
              gameObjectParameter2.Bloodsplatter();
              GameObject Object2 = GameObject.Create("Astral Tabby");
              Object2.MakeActive();
              randomElement.AddObject(Object2);
              if (Object2.Brain != null)
              {
                Object2.Brain.PartyLeader = gameObjectParameter1;
                Object2.Brain.Target = gameObjectParameter2;
                Object2.IsTrifling = true;
              }
              if (!gameObjectParameter2.IsPlayer())
                gameObjectParameter2.Target = Object2;
            }
          }
        }
      }
      return base.FireEvent(E);
    }
  }
}
