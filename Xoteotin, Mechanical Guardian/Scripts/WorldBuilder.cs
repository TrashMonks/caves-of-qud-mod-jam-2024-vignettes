using System.Collections.Generic;
using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.WorldBuilders;
using XRL.World.ZoneBuilders;

namespace Kernelmethod.Vignettes2024 {
    [JoppaWorldBuilderExtension]
    public class WorldBuilder : IJoppaWorldBuilderExtension {
        /// <summary>
        /// Place the location of the mech after world generation has completed.
        /// </summary>
        public override void OnAfterBuild(JoppaWorldBuilder builder) {
            var zoneManager = The.ZoneManager;

            // Find a random mountains tile to place the location.
            var location = builder.popMutableLocationOfTerrain("Mountains", centerOnly: false);
            var zoneID = builder.ZoneIDFromXY("JoppaWorld", location.X, location.Y);

            var secret = builder.AddSecret(
                zoneID,
                "the lair of Xoteotin, Mechanical Guardian",
                new string[4] { "lair", "robot", "tech", "xoteotin" },
                "Lairs",
                "$kernelmethod_vignettes2024_mechlair"
            );

            zoneManager.AddZoneBuilder(zoneID, ZoneBuilderPriority.EARLY, nameof(ClearAll));
            zoneManager.AddZoneBuilder(zoneID, ZoneBuilderPriority.MID, nameof(Hills));
            zoneManager.AddZoneBuilder(zoneID, ZoneBuilderPriority.LATE, nameof(RoadNorthMouth));
            zoneManager.AddZoneBuilder(zoneID, ZoneBuilderPriority.LATE, nameof(RoadSouthMouth));
            zoneManager.AddZoneBuilder(zoneID, ZoneBuilderPriority.LATE, nameof(RoadEastMouth));
            zoneManager.AddZoneBuilder(zoneID, ZoneBuilderPriority.LATE, nameof(RoadWestMouth));
            zoneManager.AddZoneBuilder(zoneID, ZoneBuilderPriority.AFTER_TERRAIN, nameof(Kernelmethod_Vignettes2024_MechLair));

            zoneManager.AddZonePostBuilder(zoneID, nameof(XRL.World.ZoneBuilders.Music), "Track", "Overworld1");
            zoneManager.SetZoneName(zoneID, "lair of Xoteotin, Mechanical Guardian", Article: "the", Proper: true);
            zoneManager.SetZoneIncludeStratumInZoneDisplay(zoneID, false);
            zoneManager.SetZoneProperty(zoneID, "NoBiomes", "Yes");
            zoneManager.SetZoneProperty(zoneID, "SkipTerrainBuilders", true);

            builder.game.SetStringGameState("Kernelmethod_Vignettes2024_MechLairZoneID", zoneID);
            Factions.Get("Robots")
                .HolyPlaces
                .Add(zoneID);
        }
    }
}
