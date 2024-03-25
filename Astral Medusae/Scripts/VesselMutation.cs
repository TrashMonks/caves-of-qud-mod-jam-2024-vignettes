using System;
using System.Collections.Generic;
using XRL.World.ZoneBuilders;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class Snakefangox_AstralMedusae_Vessel : BaseMutation
    {
        private const string DisembarkCommandName = "CommandDisembarkPassengers";

        public Snakefangox_AstralMedusae_Vessel()
        {
            DisplayName = "Vessal";
            Type = "Physical";
        }

        public override string GetDescription()
        {
            return "You are built immense for distant purpose.";
        }

        public override string GetLevelText(int Level)
        {
            return "+2 AV\n +50 Heat resistance\n +50 Cold resistance\nYou are immune to bleeding\nYour Travel mutations gain +3 levels";
        }

        public override bool WantEvent(int ID, int cascade)
        {
            if (!base.WantEvent(ID, cascade) && ID != AIGetOffensiveAbilityListEvent.ID && ID != CommandEvent.ID)
            {
                return false;
            }
            return true;
        }

        public override bool HandleEvent(AIGetOffensiveAbilityListEvent E)
        {
            if (E.Distance <= 8 && IsMyActivatedAbilityAIUsable(ActivatedAbilityID) && GameObject.Validate(E.Target) && E.Actor.HasLOSTo(E.Target, IncludeSolid: true, BlackoutStops: false, UseTargetability: true))
            {
                E.Add(DisembarkCommandName);
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(CommandEvent E)
        {
            if (E.Command == DisembarkCommandName)
            {
                if (!ParentObject.CheckFrozen())
                {
                    return false;
                }

                if (ParentObject.OnWorldMap())
                {
                    return ParentObject.Fail("You cannot do that on the world map.");
                }

                List<Cell> cells = PickCircle(1, 8, false, AllowVis.OnlyVisible, "Disembark");
                if (cells == null || cells.Count == 0)
                {
                    return false;
                }
                if (ParentObject.DistanceTo(cells[0]) > 8)
                {
                    return ParentObject.Fail("That is out of range! (" + 8.Things("square") + ")");
                }

                ParentObject.PlayWorldSound("Sounds/Abilities/sfx_ability_spitSlime_spit");
                SlimeGlands.SlimeAnimation("&R", ParentObject.CurrentCell, cells[0]);
                CooldownMyActivatedAbility(ActivatedAbilityID, 500);

                int i = 0;
                foreach (Cell cell in cells)
                {
                    if (i == 0 || 80.in100())
                    {
                        cell.AddObject("BloodPool");
                        GameObject obj = cell.AddObject("Snakefangox_AstralMedusae_Macrophage");
                        obj.pBrain.Goals.Clear();
                        obj.pBrain.PartyLeader = ParentObject;
                        obj.IsTrifling = true;
                    }
                    i++;
                }
                UseEnergy(1000, "Physical Mutation Disembark");
            }
            return base.HandleEvent(E);
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "CanApplyBleeding");
            Object.RegisterPartEvent(this, "ApplyBleeding");
            base.Register(Object);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "CanApplyBleeding" || E.ID == "ApplyBleeding")
            {
                return false;
            }
            return base.FireEvent(E);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            base.StatShifter.SetStatShift(GO, "AV", 2, baseValue: true);
            base.StatShifter.SetStatShift(GO, "HeatResistance", 50, baseValue: true);
            base.StatShifter.SetStatShift(GO, "ColdResistance", 50, baseValue: true);
            ActivatedAbilityID = AddMyActivatedAbility("Disembark Passengers", DisembarkCommandName, "Physical Mutation");
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            base.StatShifter.RemoveStatShifts(GO);
            RemoveMyActivatedAbility(ref ActivatedAbilityID);
            return base.Unmutate(GO);
        }
    }
}