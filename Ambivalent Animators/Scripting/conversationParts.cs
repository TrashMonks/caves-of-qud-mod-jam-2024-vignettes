using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using XRL;
using XRL.Rules;
using XRL.Language;
using XRL.World;
using XRL.World.Effects;
using XRL.World.Conversations;
using XRL.UI;
using XRL.World.Parts;
using Qud.API;
using NPRand = NPRandom.Sarc_Random;


namespace XRL.World.Conversations.Parts
{

    public class Sarcose_Safra_May_Recruit : IConversationPart
    {

        public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == EnteredElementEvent.ID;

        public override bool HandleEvent(EnteredElementEvent E){
            Popup.Show("You may now recruit one of {{dreamsmoke|Safra's}} animated allies to aid in the hunt!");
            return base.HandleEvent(E);
        }
    }

    public class Sarcose_Animate_Join : IConversationPart
    {
        public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == EnteredElementEvent.ID;

        public override bool HandleEvent(EnteredElementEvent E){
            The.Player.AddPart(new Sarcose_Has_Animate_Ally());
            The.Speaker.AddPart(new Sarcose_Has_Animate_Ally());
            Brain pBrain = The.Speaker.pBrain;
			pBrain.BecomeCompanionOf(The.Player);
			if (pBrain.GetFeeling(The.Player) < 0)
			{
				pBrain.SetFeeling(The.Player, 5);
			}
			if (The.Speaker.GetEffect("Lovesick") is Lovesick lovesick)
			{
				lovesick.PreviousLeader = The.Player;
			}
			Popup.Show(The.Speaker.Does("join", int.MaxValue, null, null, null, AsIfKnown: false, Single: false, NoConfusion: false, NoColor: false, Stripped: true) + " you!");

            return base.HandleEvent(E);
        }
    }

    public class Sarcose_Give_Kill_Neuroparents : IConversationPart
    {
        public override bool WantEvent(int id, int propagation){
            return 
            base.WantEvent(id, propagation) 
            || id == EnteredElementEvent.ID
            ;
        }
        public override bool HandleEvent(EnteredElementEvent E)
        {
            The.Player.AddPart(new Sarcose_Kill_Neuroparents());
            return base.HandleEvent(E);
        }
           
    }

    public class Sarcose_Give_Yurlfriend : IConversationPart
    {
        public override bool WantEvent(int id, int propagation){
            return 
            base.WantEvent(id, propagation) 
            || id == EnteredElementEvent.ID
            ;
        }
        public override bool HandleEvent(EnteredElementEvent E)
        {
            The.Player.AddPart(new Sarcose_Yurlfriend());
            return base.HandleEvent(E);
        }
        
    }

    public class Sarcose_Give_NNAReward : IConversationPart
    {
            public override bool WantEvent(int id, int propagation){
                return 
                base.WantEvent(id, propagation) 
                || id == EnterElementEvent.ID
                ;
            }
        public override bool HandleEvent(EnterElementEvent E)
        {
            XRL.Messages.MessageQueue.AddPlayerMessage("600 XP Awarded!", null, false);
            The.Player.AwardXP(600);
            return base.HandleEvent(E);
        }
            
    }
    public class Sarcose_Give_SprayReward : IConversationPart
    {
            public override bool WantEvent(int id, int propagation){
                return 
                base.WantEvent(id, propagation) 
                || id == EnterElementEvent.ID
                ;
            }
        public override bool HandleEvent(EnterElementEvent E)
        {
            XRL.Messages.MessageQueue.AddPlayerMessage("100 XP Awarded!", null, false);
            The.Player.AwardXP(100);
            return base.HandleEvent(E);
        }
        
    }

    


    public class Sarcose_GiveSecret : IConversationPart
    {
        public string secret = "";
        public Sarcose_GiveSecret()
        {
        }

        public Sarcose_GiveSecret(string secret)
        {
            this.secret = secret;
        }

                //"sarcose_safrasecret",sarcose_neurocampsecret,sarcose_picklesecret


        public override bool WantEvent(int id, int propagation){
                return 
                base.WantEvent(id, propagation) 
                || id == EnterElementEvent.ID
                ;
            }
        public override bool HandleEvent(EnterElementEvent E)
        {
            JournalMapNote mapNote = JournalAPI.GetMapNote(secret);
						if (mapNote != null && !mapNote.Revealed)
						{
							JournalAPI.RevealMapNote(mapNote);
						}
            return base.HandleEvent(E);
        }

    }


    

}