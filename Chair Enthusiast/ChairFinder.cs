using System;
using XRL;
using XRL.World;

namespace XRL.World.Parts
{
    [PlayerMutator]
    public class Gnarf_ChairFinder : IPlayerPart, IPlayerMutator
    {
        public void mutate(GameObject player)
        {
            // modify the player object when a New Game begins
            // for example, add a custom part to the player:
            player.AddPart<Gnarf_ChairFinder>();
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return
                base.WantEvent(ID, cascade)
                || ID == GetPointsOfInterestEvent.ID
                ;
        }

        public override bool HandleEvent(GetPointsOfInterestEvent E)
        {
            Chair closestChair = null, comfiestChair = null;
            int chairContestents = 0;
            foreach (var chairObject in E.Zone.FindObjectsWithPart("Chair"))
            {
                var chair = chairObject.GetPart<Chair>();
                if (!E.StandardChecks(chair, E.Actor)) // have we seen the chair, etc???
                {
                    continue;
                }
                chairContestents++;
                if (comfiestChair == null || chair.Level > comfiestChair.Level)
                {
                    comfiestChair = chair;
                }
                else if (chair.Level == comfiestChair.Level)
                {
                    if (comfiestChair.ParentObject.DistanceTo(E.Actor) > chairObject.DistanceTo(E.Actor))
                    {
                        comfiestChair = chair;
                    }
                }
                if (closestChair == null || closestChair.ParentObject.DistanceTo(E.Actor) > chairObject.DistanceTo(E.Actor))
                {
                    closestChair = chair;
                }
            }

            if (closestChair != null && closestChair == comfiestChair)
            {
                var useExplanation = chairContestents > 1 ? "comfiest, nearest" : "";
                E.Add(
                    Object: closestChair.ParentObject,
                    DisplayName: closestChair.ParentObject.GetReferenceDisplayName(),
                    Explanation: useExplanation,
                    Key: "chair",
                    Radius: 1
                );
            }
            else if (closestChair != null)
            {
                E.Add(
                    Object: closestChair.ParentObject,
                    DisplayName: closestChair.ParentObject.GetReferenceDisplayName(),
                    Explanation: "nearest",
                    Key: "chair",
                    Radius: 1
                );
                E.Add(
                    Object: comfiestChair.ParentObject,
                    DisplayName: comfiestChair.ParentObject.GetReferenceDisplayName(),
                    Explanation: "comfiest",
                    Key: "chair",
                    Radius: 1
                );

            }

            return base.HandleEvent(E);
        }

    }
}
