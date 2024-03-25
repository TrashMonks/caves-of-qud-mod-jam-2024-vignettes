using Genkit;
using System;
using System.Collections.Generic;
using XRL;
using XRL.Rules;
using XRL.World.Parts;

namespace XRL.World.ZoneBuilders {
    /// <summary>
    /// Custom zone builder for the mech lair.
    /// </summary>
    public class Kernelmethod_Vignettes2024_MechLair : ZoneBuilderSandbox {
        public bool BuildZone(Zone zone) {
            var centerX = zone.Width / 2;
            var centerY = zone.Height / 2;
            var center = Location2D.Get(centerX, centerY).Point;
            var nearbyCells = new List<Cell>();

            // Paint floor
            zone.GetCell(0, 0).AddObject("Dirty");

            foreach (Cell C in zone.GetCells()) {
                var distance = C.CosmeticDistanceTo(center);
                if (distance > 6)
                    continue;

                C.Clear();

                int chanceOneIn = Math.Max(distance, 1);
                if (Stat.Random(1, chanceOneIn) == 1)
                    C.AddObject("Crystal Flowers");

                if (distance > 0)
                    nearbyCells.Add(C);
            }

            float angle = Stat.Random((float) 0.0, (float) (2.0 * Math.PI));

            for (int i = 0; i < 3; i++) {
                var distance = Stat.Random(4, 5);
                int deltaX = (int) Math.Floor(distance * Math.Cos(angle));
                int deltaY = (int) Math.Floor(distance * Math.Sin(angle));

                var widgetLocation = Location2D.Get(centerX + deltaX, centerY + deltaY);
                var gameObject = PopulationManager.CreateOneFrom("Kernelmethod_Vignettes2024_MechLairShrineChunks");
                if (gameObject.TryGetPart<MapChunkPlacement>(out var part)) {
                    part.Rotation = Stat.Random(0, 3);
                }

                zone.GetCell(widgetLocation)
                    .AddObject(gameObject);

                angle += (float) (2 * Math.PI / 3);
            }

            if (nearbyCells.Count > 0) {
                var index = Stat.Random(0, nearbyCells.Count - 1);
                var seatCell = nearbyCells[index];

                seatCell.Clear();
                seatCell.AddObject("Kernelmethod_Vignettes2024_MechPilotSeat");
            }

            zone.GetCell(center)
                .AddObject("Kernelmethod_Vignettes2024_Xoteotin");

            ZoneTemplateManager
                .Templates["Kernelmethod_Vignettes2024_MechLair"]
                .Execute(zone);

            return true;
        }
    }
}
