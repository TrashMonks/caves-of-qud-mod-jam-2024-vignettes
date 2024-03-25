using System;
using Genkit;
using SnakefangoxAstralMedusae;

namespace XRL.World.Parts
{
    [Serializable]
    public class Snakefangox_AstralMedusae_ManubriumSpawner : IPart
    {
        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "EnteredCell");
            base.Register(Object);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "EnteredCell")
            {
                Zone zone = E.GetParameter<Cell>("Cell").ParentZone;
                Random rand = Utilities.GameRandom();

                Point2D center = zone.GetCenterCell().location.Point;
                Point2D[] cardinals = new Point2D[] { center + new Point2D(8, 0), center + new Point2D(-8, 0), center + new Point2D(0, 7), center + new Point2D(0, -7), };

                foreach (Cell c in zone.GetCells())
                {
                    int dist = c.CosmeticDistanceTo(center);
                    if (dist > 13)
                    {
                        continue;
                    }

                    c.Clear();

                    int minCardinalDist = 12;
                    foreach (var cardinal in cardinals)
                    {
                        int cDist = c.CosmeticDistanceTo(cardinal);
                        if (cDist < minCardinalDist)
                        {
                            minCardinalDist = cDist;
                        }
                    }

                    if (minCardinalDist < 2 && rand.Next(0, 3) == 0)
                    {
                        if (zone.Z == 10)
                        {
                            c.AddObject("Snakefangox_AstralMedusae_TrailingManubrium");
                        }
                        else
                        {
                            c.AddObject("Snakefangox_AstralMedusae_ManubriumMidsection");
                        }
                    }
                }

                ParentObject.Destroy();
            }
            return base.FireEvent(E);
        }
    }
}