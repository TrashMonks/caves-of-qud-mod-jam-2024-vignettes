using Genkit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XRL.World.ZoneBuilders
{
  /* This class is basically copied from the XRL.World.Parts.RecorporealizationBoothSpawnerBuilder
  part which is used to spawn the recoming nook Gyl somewhere in Lake Hinnom. */
  public class Gearlink_CASKOFROSEWINE_FortunadoSpawnerBuilder : ZoneBuilderSandbox
  {
    public bool BuildZone(Zone zone)
    {
      InfluenceMapRegion influenceMapRegion = ZoneBuilderSandbox.GenerateInfluenceMap(
        zone,
        (List<Point>) null,
        InfluenceMapSeedStrategy.LargestRegion,
        200
      ).Regions.Where<InfluenceMapRegion>(
        (Func<InfluenceMapRegion, bool>) (r => r.maxRect.Width >= 12 && r.maxRect.Height >= 10)
      ).FirstOrDefault<InfluenceMapRegion>();

      // Find the cell that is going to be the top left corner of the 3x3
      Cell cell = (Cell) null;
      // If an influenceMapRegion is available, just use that.
      if (influenceMapRegion != null)
        cell = zone.GetCell(influenceMapRegion.maxRect.x1, influenceMapRegion.maxRect.y1);
      if (cell == null)
        /* If no influenceMapRegion is available, instead pick a random cell.
        We need to make sure the cell picked is at least 3 cells away from the
        zone edges, and zones are 80 cells wide by 25 cells tall so find a cell
        within 77 cells from the left and 22 cells from the top. */
        cell = zone.GetCells().Where<Cell>((Func<Cell, bool>) (c => c.X < 77 && c.Y < 22)).GetRandomElement<Cell>();
      /* Clear the cells in a 3x3 area to make room for the Fortunado3x3.rpm map
      chunk placement widget. */
      for (int index1 = 0; index1 < 2; ++index1)
      {
        for (int index2 = 0; index2 < 2; ++index2)
          zone.GetCell(cell.X + index2, cell.Y + index1)?.Clear(Combat: true);
      }
      // Add the MapChunkPlacement widget.
      cell.AddObject("Gearlink_CASKOFROSEWINE_Fortunado3x3");

      return true;
    }
  }
}
