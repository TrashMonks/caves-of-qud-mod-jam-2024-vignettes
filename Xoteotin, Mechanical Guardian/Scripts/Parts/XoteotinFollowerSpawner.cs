using Qud.API;
using System;
using XRL.Rules;

namespace XRL.World.Parts {
    [Serializable]
    public class Kernelmethod_Vignettes2024_XoteotinFollowerSpawner : IPart {

        public bool Uplift = false;

        public override bool WantEvent(int ID, int cascade) {
            return base.WantEvent(ID, cascade)
                || ID == BeforeObjectCreatedEvent.ID;
        }

        public override bool HandleEvent(BeforeObjectCreatedEvent E) {
            GameObject gameObject;
            int num = Stat.Roll(1, 100);

            if (num <= 90) {
                // Select a random robotic creature
                gameObject = EncountersAPI
                    .GetALegendaryEligibleCreature(
                        (GameObjectBlueprint o) => DefaultObjectFilter(o) && o.HasTag("Robot")
                    );
            }
            else {
                // Select a random creature, roboticize it
                gameObject = EncountersAPI
                    .GetALegendaryEligibleCreature(
                        (GameObjectBlueprint o) => DefaultObjectFilter(o) && !o.HasTag("Robot")
                    );

                Roboticized.Roboticize(gameObject);
            }

            gameObject.SetStringProperty("SpawnedFrom", ParentObject.Blueprint);
            E.ReplacementObject = gameObject;

            gameObject.pBrain.FactionMembership.Clear();
            gameObject.pBrain.FactionMembership.Add("Robots", 100);
            gameObject
                .RequirePart<SocialRoles>()
                .RequireRole("follower of Xoteotin");

            if (Uplift)
                HeroMaker.MakeHero(gameObject);

            // Give custom conversation options particular to mech followers
            if (gameObject.TryGetPart<ConversationScript>(out var part))
                part.Append = "\n\nXoteotin guides you, wayfarer.~\n\nMay Xoteotin serve as an exemplar to all robotfolk.~\n\nMay It lead the way.";

            return base.HandleEvent(E);
        }

        public static bool DefaultObjectFilter(GameObjectBlueprint go) =>
            !go.HasTag("ExcludeFromVillagePopulations") &&
            !go.HasTag("Gigantic");
    }
}
