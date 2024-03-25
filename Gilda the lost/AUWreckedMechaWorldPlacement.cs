using System;
using XRL.World;
using XRL.World.Parts;
using XRL.World.ZoneBuilders;
using Genkit;

namespace XRL.World.WorldBuilders
{
    [JoppaWorldBuilderExtension]
    public class AUWreckedMechaWorldPlacement : IJoppaWorldBuilderExtension
    {
        public override void OnAfterBuild(JoppaWorldBuilder builder)
        {
            Location2D location = builder.mutableMap.popMutableLocationInArea(228, 6, 233, 11);

            string zoneID = Zone.XYToID("JoppaWorld", location.X, location.Y, 10);

            The.ZoneManager.AddZoneBuilder(zoneID, ZoneBuilderPriority.AFTER_TERRAIN, nameof(AUWreckedMechaGildaBuilder));

            string secret = builder.AddSecret(zoneID, "a lost enclave of Barathrumites", new string[4] { "encounter", "special", "settlement", "barathrumites" }, "Settlements", "$auwreckedmechalost");

            if (The.ZoneManager.GetZone("JoppaWorld").GetCell(location.X / 3, location.Y / 3).GetFirstObjectWithPart("TerrainTravel")
                .TryGetPart<TerrainTravel>(out var Part))
            {
                Part.AddEncounter(new EncounterEntry("You encounter strewn battle wreckage, Urshiib tinkers picking at the pieces. Would you like to investigate?", zoneID, "", secret, Optional: true));
            }

            
            GameObject gameObject = GameObjectFactory.Factory.CreateObject("SecretRevealWidget");
            SecretRevealer part = gameObject.GetPart<SecretRevealer>();
            part.message = "You discover a seperated enclave of Barathrumites.";
            part.id = secret;
            part.adjectives = "encounter,special,settlement,barathrumites";
            part.category = "Settlements";
            part.text = "Lost enclave";
            The.ZoneManager.AddZonePostBuilder(zoneID, "AddWidgetBuilder", "Object", The.ZoneManager.CacheObject(gameObject));


        }
    }
}