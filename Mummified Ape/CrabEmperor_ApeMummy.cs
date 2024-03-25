using XRL;
using XRL.World;
using XRL.World.WorldBuilders;
using XRL.World.ZoneBuilders;

namespace ApeMummy.CrabEmperor
{
    [JoppaWorldBuilderExtension]
    public class CrabEmperor_ApeMummyJoppaWorldBuilderExtension : IJoppaWorldBuilderExtension
    {
        public override void OnAfterBuild(JoppaWorldBuilder builder)
        {
            var location = builder.popMutableLocationOfTerrain("Hills", centerOnly: false);
            string zoneID = builder.ZoneIDFromXY("JoppaWorld", location.X, location.Y);

            var secret = builder.AddSecret(
                zoneID,
                "the location of a mummified ape",
                new string[2] { "oddity", "ape" },
                "Oddities",
                "$CrabEmperor_ApeMummy_mummifiedApe"
            );
 
            var zoneManager = The.ZoneManager;
                 var item = GameObject.Create("Ape_Mummy");
            zoneManager.AddZonePostBuilder(zoneID, nameof(AddObjectBuilder), "Object", zoneManager.CacheObject(item));
        }
    }
}