using System;
using System.Linq;
using XRL.UI;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class Snakefangox_AstralMedusae_AxonemicSnare : BaseMutation
    {
        private const string CommandName = "CommandAxonemicSnare";

        public Snakefangox_AstralMedusae_AxonemicSnare()
        {
            DisplayName = "Axonemic Snare";
            Type = "Mental";
        }

        public override string GetDescription()
        {
            return "You snare a fragment of space and ingest it.";
        }

        public override string GetLevelText(int Level)
        {
            string text = "Pulls all creatures in an area into your guts.\n";
            int num = Math.Max(5, 550 - 10 * Level);
            text = text + "Cooldown: {{rules|" + num + "}} rounds\n";
            text = text + "Radius: {{rules|" + (Level + 1) + "}}\n";
            text = text + "Range: {{rules|" + (Level * 5) + "}}";
            return text;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            if (!base.WantEvent(ID, cascade) && ID != AIGetOffensiveAbilityListEvent.ID)
            {
                return ID == GetItemElementsEvent.ID;
            }
            return true;
        }

        public override bool HandleEvent(AIGetOffensiveAbilityListEvent E)
        {
            if (E.Distance <= 5 && IsMyActivatedAbilityAIUsable(ActivatedAbilityID))
            {
                E.Add(CommandName);
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetItemElementsEvent E)
        {
            if (E.IsRelevantCreature(ParentObject))
            {
                E.Add("travel", 1);
            }
            return base.HandleEvent(E);
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, CommandName);
            base.Register(Object);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == CommandName)
            {
                Zone currentZone = ParentObject.CurrentZone;
                if (currentZone == null || !ParentObject.HasPart<Interior>())
                {
                    return false;
                }
                if (currentZone.IsWorldMap())
                {
                    if (ParentObject.IsPlayer())
                    {
                        Popup.ShowFail("You may not use this mutation on the world map.");
                    }
                    return false;
                }

                var circle = PickCircle(Level + 1, Range: Level * 5, VisLevel: AllowVis.OnlyVisible, Label: DisplayName);
                if (circle == null)
                {
                    return false;
                }

                Interior interior = ParentObject.GetPart<Interior>();
                Event e = Event.New("InitiateRealityDistortionTransit", "Object", ParentObject, "Mutation", this, "Cell", circle.First());
                if (!ParentObject.FireEvent(e, E))
                {
                    return false;
                }

                foreach (var cell in circle)
                {
                    e.SetParameter("Cell", cell);
                    if (cell.FireEvent(e, E))
                    {
                        var objs = new GameObject[cell.Objects.Count];
                        cell.Objects.CopyTo(objs);

                        foreach (var obj in objs)
                        {
                            if (!obj.HasStat("Ego")) continue;

                            // 38, 12
                            obj.TeleportSwirl(Color: "&M", Voluntary: false, IsOut: true);
                            obj.CellTeleport(interior.Zone.GetCell(38, 12), Mutation: this);
                        }
                    }
                }
            }

            int turns = Math.Max(5, 550 - 10 * Level);
            CooldownMyActivatedAbility(ActivatedAbilityID, turns);
            UseEnergy(1000, "Mental Mutation AxonemicSnare");
            return base.FireEvent(E);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            ActivatedAbilityID = AddMyActivatedAbility(DisplayName, CommandName, "Mental Mutation", IsAttack: true, IsRealityDistortionBased: true);
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            RemoveMyActivatedAbility(ref ActivatedAbilityID);
            return base.Unmutate(GO);
        }
    }
}