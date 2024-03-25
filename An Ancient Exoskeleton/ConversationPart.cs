using XRL;
using XRL.World.Conversations;

namespace XRL.World.Conversations.Parts{
    public class ReplaceWithItem : IConversationPart
    {
        //public bool BroadcastForHelp;


        public override bool WantEvent(int ID, int Propagation)
        {
            MetricsManager.LogInfo("Replace with item exists");
            return base.WantEvent(ID, Propagation) || ID == PrepareTextEvent.ID;
        }

        public override bool HandleEvent(PrepareTextEvent E)
        {
            //currentCell = The.Speaker.GetCurrentCell();
            The.Speaker.CurrentCell.AddObject("c67f_ExoVignette_Special Exo Item");
            MetricsManager.LogInfo("Replace with item running");
            //MetricsManager.LogInfo(The.Speaker.CurrentCell);
            The.Speaker.Obliterate();
            return base.HandleEvent(E);
	    }
    }
}