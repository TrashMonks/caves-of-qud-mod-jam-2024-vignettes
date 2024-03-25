using System;

namespace XRL.World.Parts
{
    [Serializable]
    public class Snakefangox_AstralMedusae_CopyPilotMutations : IPart
    {
        public override bool WantEvent(int ID, int cascade)
        {
            if (!base.WantEvent(ID, cascade))
            {
                return ID == AfterPilotChangeEvent.ID;
            }
            return true;
        }

        public override bool HandleEvent(AfterPilotChangeEvent E)
        {
            if (E.OldPilot != null)
            {
                Mutations mutations = ParentObject.GetPart<Mutations>();
                foreach (var mutation in E.OldPilot.GetMentalMutations())
                {
                    foreach (var m in mutations.MutationList)
                    {
                        if (m.Name == mutation.Name)
                            mutations.RemoveMutation(m);
                    }
                }
            }

            if (E.NewPilot != null)
            {
                Mutations mutations = ParentObject.GetPart<Mutations>();
                foreach (var mutation in E.NewPilot.GetMentalMutations())
                {
                    mutations.AddMutation(mutation.GetMutationClass());
                }
            }

            return true;
        }
    }
}