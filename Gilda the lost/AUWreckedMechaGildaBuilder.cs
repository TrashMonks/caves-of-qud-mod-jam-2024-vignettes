
using System.Linq;
using Genkit;
using XRL.World;
using XRL;


namespace XRL.World.ZoneBuilders
{
    public class AUWreckedMechaGildaBuilder : ZoneBuilderSandbox
    {
        public bool BuildZone(Zone zone)
        {
            InfluenceMapRegion influenceMapRegion = ZoneBuilderSandbox.GenerateInfluenceMap(zone, null, InfluenceMapSeedStrategy.LargestRegion, 200).
                Regions.Where((InfluenceMapRegion r) => r.maxRect.Width >= 19 && r.maxRect.Height >= 19).FirstOrDefault();
            Cell cell = null;
            if (influenceMapRegion != null)
            {
                cell = zone.GetCell(influenceMapRegion.maxRect.x1, influenceMapRegion.maxRect.y1);
            }
            if (cell == null)
            {
                cell = (from c in zone.GetCells()
                        where c.X < 58 && c.Y < 19
                        select c).GetRandomElement();
            }
            for (int y = 0; y < 19; y++)
            {
                for (int x = 0; x < 19; x++)
                {
                    zone.GetCell(cell.X + x, cell.Y + y)?.Clear(null, Important: false, Combat: true);
                }
            }
            cell.AddObject("AUWreckedMecha-GildaLost");

            ZoneTemplateManager
                .Templates["AUWreckedMecha-LostEnclave"]
                .Execute(zone);

            return true;
        }
    }
}
