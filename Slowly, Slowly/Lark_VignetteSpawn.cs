using XRL;
using XRL.World;
using XRL.World.WorldBuilders;
using XRL.World.ZoneBuilders;

namespace Lark_Vignette.Lark
{
    [JoppaWorldBuilderExtension]
    public class YourJoppaWorldBuilderExtension : IJoppaWorldBuilderExtension
    {
        public override void OnAfterBuild(JoppaWorldBuilder builder)
        {
            var location = builder.popMutableLocationOfTerrain("Saltdunes", centerOnly: false);
            var zoneID = builder.ZoneIDFromXY("JoppaWorld", location.X, location.Y);

            var secret = builder.AddSecret(
                zoneID,
                "The site of a chance encounter",
                new string[3] { "encounter", "special", "oddity" },
                "Oddities",
                "$lark_vignette_senpat"
            );

            var zoneManager = The.ZoneManager;

            zoneManager.AddZoneBuilder(zoneID, ZoneBuilderPriority.LATE, nameof(RoadNorthMouth));
            zoneManager.AddZoneBuilder(zoneID, ZoneBuilderPriority.LATE, nameof(RoadSouthMouth));

            var creature = GameObject.Create("Lark_Senpat");
            zoneManager.AddZonePostBuilder(zoneID, nameof(AddObjectBuilder), "Object", zoneManager.CacheObject(creature));

            zoneManager.SetZoneIncludeStratumInZoneDisplay(zoneID, false);
            zoneManager.SetZoneProperty(zoneID, "NoBiomes", "Yes");
        }
    }
}