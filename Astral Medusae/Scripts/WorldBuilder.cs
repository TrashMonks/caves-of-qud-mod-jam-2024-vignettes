using System;
using XRL.World.WorldBuilders;
using Genkit;
using XRL;
using XRL.World;
using System.Collections.Generic;

namespace SnakefangoxAstralMedusae
{
    [JoppaWorldBuilderExtension]
    public class AstralMedusaeJoppaWorldBuilderExtension : IJoppaWorldBuilderExtension
    {
        private const string JoppaWorldID = "JoppaWorld";

        public override void OnAfterBuild(JoppaWorldBuilder builder)
        {
            Random rand = Utilities.GameRandom();
            Location2D location2D = PopMutableLocationOfTerrain(builder, "Mountains", rand);
            string zoneID = Zone.XYToID(JoppaWorldID, location2D.X, location2D.Y, 10);
            The.ZoneManager.AddZonePostBuilder(zoneID, "AddWidgetBuilder", "Blueprint", "Snakefangox_AstralMedusae_ManubriumSpawner");

            string lowSkyZoneID = The.ZoneManager.GetZoneFromIDAndDirection(zoneID, "U");
            BuildManubriumTrail(lowSkyZoneID);

            string highSkyZoneID = The.ZoneManager.GetZoneFromIDAndDirection(lowSkyZoneID, "U");
            BuildManubriumTrail(highSkyZoneID);

            string medusaeZoneID = highSkyZoneID;
            for (int i = 1; i <= 2; i++)
            {
                medusaeZoneID = The.ZoneManager.GetZoneFromIDAndDirection(medusaeZoneID, "U");
                BuildMedusaeBody(medusaeZoneID, i);
            }

            builder.AddSecret(zoneID, "trailing manubrium", new string[3] { "encounter", "special", "oddity" }, "Oddities", "$snakefangox_astralmedusae_trailingmanubrium");
        }

        private static void BuildMedusaeBody(string medusaeZoneID, int i)
        {
            The.ZoneManager.AddZoneBuilder(medusaeZoneID, 6000, "MapBuilder", "FileName", $"Snakefangox_AstralMedusae_MedusaeBodyLv{i}.rpm", "ClearChasms", true);
            The.ZoneManager.SetZoneProperty(medusaeZoneID, "DisableForcedConnections", "Yes");
            The.ZoneManager.SetZoneName(medusaeZoneID, "sky", "medusae", Article: "the");
        }

        private static void BuildManubriumTrail(string lowSkyZoneID)
        {
            The.ZoneManager.AddZonePostBuilder(lowSkyZoneID, "AddWidgetBuilder", "Blueprint", "Snakefangox_AstralMedusae_ManubriumSpawner");
            The.ZoneManager.SetZoneName(lowSkyZoneID, "sky", "medusae underbelly", Article: "the");
        }

        private Location2D PopMutableLocationOfTerrain(JoppaWorldBuilder builder, string Terrain, Random random)
        {
            List<Location2D> list = new();
            foreach (Location2D item in builder.worldInfo.terrainLocations[Terrain].Shuffle(random))
            {
                list.Clear();
                for (int x = 0; x <= 2; x++)
                {
                    for (int y = 0; y <= 2; y++)
                    {
                        if (builder.mutableMap.GetMutable(item.X * 3 + x, item.Y * 3 + y) > 0)
                        {
                            list.Add(Location2D.Get(item.X * 3 + x, item.Y * 3 + y));
                        }
                    }
                }
                if (list.Count > 0)
                {
                    Location2D randomElement = list.GetRandomElement(random);
                    builder.mutableMap.RemoveMutableLocation(randomElement);
                    return randomElement;
                }
            }
            return null;
        }
    }
}
