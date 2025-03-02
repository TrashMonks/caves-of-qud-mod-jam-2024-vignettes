using XRL;
using XRL.World;
using XRL.World.WorldBuilders;
using XRL.World.ZoneBuilders;

namespace NeuroParent
{
    [JoppaWorldBuilderExtension]
    public partial class NeuroParentWorldBuilderExtension : IJoppaWorldBuilderExtension
    {
        public static void AddSecretCampLocation(JoppaWorldBuilder builder)
        {
            var location = builder.popMutableLocationOfTerrain("Saltdunes", centerOnly: false);
            var zoneID = builder.ZoneIDFromXY("JoppaWorld", location.X, location.Y);
            var secret = builder.AddSecret(
                zoneID,
                "A Neuroparent Camp",
                new string[2] { "newly sentient beings", "trolls" },
                "Oddities",
                "sarcose_neurocampsecret"
            );
            var zoneManager = The.ZoneManager;
            // Add some roads to the north and south
            zoneManager.AddZoneBuilder(zoneID, ZoneBuilderPriority.LATE, nameof(OverlandRuins));

            // Add an object to the zone
            // Replace "Oboroqoru" with the object ID of the creature you want to add
            var creature = GameObject.Create("Neuroparent_Camper");
            zoneManager.AddZonePostBuilder(zoneID, nameof(AddObjectBuilder), "Object", zoneManager.CacheObject(creature));

            // You can also set various properties on the zone, if you wish.
            zoneManager.SetZoneName(zoneID, "the location of a Neuroparent Camp", Article: "the", Proper: true);
            zoneManager.SetZoneIncludeStratumInZoneDisplay(zoneID, false);
            zoneManager.SetZoneProperty(zoneID, "NoBiomes", "Yes");
        }

        public static void AddSecretSafraLocation(JoppaWorldBuilder builder)
        {
            var location = builder.popMutableLocationOfTerrain("Ruins", centerOnly: false);
            var zoneID = builder.ZoneIDFromXY("JoppaWorld", location.X, location.Y);

            var secret = builder.AddSecret(
                zoneID,
                "The Hideout of Safra",
                new string[2] { "newly sentient beings", "trolls" },
                "Oddities",
                "sarcose_safrasecret"
            );
            var zoneManager = The.ZoneManager;
            var creature = GameObject.Create("Sarcose_FugitiveBasket");
            zoneManager.AddZonePostBuilder(zoneID, nameof(AddObjectBuilder), "Object", zoneManager.CacheObject(creature));
            zoneManager.SetZoneName(zoneID, "The Hideout of Safra", Article: "the", Proper: true);
            zoneManager.SetZoneIncludeStratumInZoneDisplay(zoneID, false);
            zoneManager.SetZoneProperty(zoneID, "NoBiomes", "Yes");
        }

        public static void AddPickleRuinLocation(JoppaWorldBuilder builder)
        {
            var location = builder.popMutableLocationOfTerrain("Jungle", centerOnly: false);
            var zoneID = builder.ZoneIDFromXY("JoppaWorld", location.X, location.Y);

            var secret = builder.AddSecret(
                zoneID,
                "The Last Seen Location of an Animate Rebel",
                new string[2] { "newly sentient beings", "trolls" },
                "Oddities",
                "sarcose_picklesecret"
            );
            var zoneManager = The.ZoneManager;
            zoneManager.AddZoneBuilder(zoneID, ZoneBuilderPriority.LATE, nameof(OverlandRuins));
            var creature = GameObject.Create("Safra_Pickle_Remnant");
            zoneManager.AddZonePostBuilder(zoneID, nameof(AddObjectBuilder), "Object", zoneManager.CacheObject(creature));
            zoneManager.SetZoneName(zoneID, "A Long Ago Meeting", Article: "the", Proper: true);
            zoneManager.SetZoneIncludeStratumInZoneDisplay(zoneID, false);
            zoneManager.SetZoneProperty(zoneID, "NoBiomes", "Yes");
        }
        public override void OnAfterBuild(JoppaWorldBuilder builder)
        {

            AddSecretSafraLocation(builder);
            AddSecretCampLocation(builder);
            AddPickleRuinLocation(builder);

        }
    }

}
