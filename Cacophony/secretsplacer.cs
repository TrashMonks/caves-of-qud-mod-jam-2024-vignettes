using System.Collections.Generic;
using XRL;
using XRL.World;
using XRL.World.WorldBuilders;
using XRL.World.ZoneBuilders;

namespace Sophie.Vignettes2024 {
    [JoppaWorldBuilderExtension]
    public class WorldBuilder : IJoppaWorldBuilderExtension {
        public override void OnAfterBuild(JoppaWorldBuilder builder) {
            var zoneManager = The.ZoneManager;
			//UnityEngine.Debug.Log("the script ran");
            // Find a random mountains tile to place the location.
            var location = builder.popMutableLocationOfTerrain("Jungle", centerOnly: false);
            var zoneID = builder.ZoneIDFromXY("JoppaWorld", location.X, location.Y);

            // Replace MyMapName.rpm with the name of your map file
            zoneManager.AddZonePostBuilder(zoneID, nameof(MapBuilder), "FileName", "casino1.rpm");

            zoneManager.SetZoneProperty(zoneID, "NoBiomes", "Yes");
            zoneManager.SetZoneProperty(zoneID, "SkipTerrainBuilders", true);
            zoneManager.SetZoneIncludeStratumInZoneDisplay(zoneID, false);

            // Customize the following as you see fit
            zoneManager.AddZonePostBuilder(zoneID, nameof(XRL.World.ZoneBuilders.Music), "Track", "Overworld1");
            zoneManager.SetZoneName(zoneID, "loud ruin", Article: "a", Proper: true);

            var secret = builder.AddSecret(
                zoneID,
                "a loud ruin",
                new string[3] { "lair", "robot", "tech" },
                "Ruins",
                "$sophie_vignettes2024"
            );
        }
    }
}