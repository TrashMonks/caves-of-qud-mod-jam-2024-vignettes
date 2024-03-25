using Qud.API;
using System;
using XRL.Language;
using XRL.UI;

namespace XRL.World.Parts
{
  /* This class is based off of the XRL.World.Parts.MerchantRevealer part which
  merchant advertisements.*/
  [Serializable]
  public class Gearlink_CASKOFROSEWINE_EtchedPottery : IPart
  {
    // The text that will precede the damning secret Fernado died for.
		public string bookText = "They killed their own.\n\nThey killed Fernado.\n\nThe Consortium doesn't want you to know.\n\n\n";

		public IBaseJournalEntry secret = null;

    /* Listen to the minimal events we want to use. MerchantRevealer also listens
    to events like IObjectCreationEvent and EnteredCellEvent because the
    name of the advertisements are based on the secret they reveal, but we don't
    need that here so they are excluded. */
		public override bool WantEvent(int ID, int cascade) {
	    return base.WantEvent(ID, cascade)
			|| ID == GetInventoryActionsEvent.ID
	    || ID == HasBeenReadEvent.ID
	    || ID == InventoryActionEvent.ID;
	  }

    /* Make it so the object can be read. Set the priority to 100 (high) if the
    pot hasn't been read so the UI will default to selecting read, or 15 (low) if
    it's already been read. */
		public override bool HandleEvent(GetInventoryActionsEvent E)
		{
			E.AddAction("Read", "read", "Read", Key: 'r', Default: (this.GetHasBeenRead() ? 15 : 100));
			return base.HandleEvent(E);
		}

		public override bool HandleEvent(HasBeenReadEvent E) => (E.Actor != The.Player || this.GetHasBeenRead()) && base.HandleEvent(E);

    /* Handle the pot being read. */
		public override bool HandleEvent(InventoryActionEvent E)
		{
			if (E.Command == "Read" && E.Actor.IsPlayer())
			{
        // If no secret has been generated, generate a new secret.
				if ( this.secret is null )
				{
          // Get a unique ID for the new secret.
					string newID = Guid.NewGuid().ToString();
          // Get a random faction for the Consortium to do something do.
					Faction randomFaction = Factions.GetRandomFaction();
          /* Create a gossip secret between the Consortium and the random faction
          with the new ID. Based off of how gossip in general in initialized in
          Qud.API.JournalAPI.InitializeGossip() */
					JournalAPI.AddObservation(Grammar.InitCap(Gossip.GenerateGossip_TwoFactions("Consortium", randomFaction.Name)), newID, "Gossip", newID, new string[3]
					{
						"gossip:" + "Consortium",
						"gossip:" + randomFaction.Name,
						"gossip"
					});
          /* AddObservation doesn't return the created secret, so we have to get
          it from the JournalAPI using the same ID we made it with. */
					this.secret = JournalAPI.GetObservation( newID );
				}

        // Show the contents of the book, appending the text of the secret.
				BookUI.ShowBook(this.bookText + this.secret.text, this.ParentObject.DisplayName);
        // Reveal the secret to the player.
				this.secret?.Reveal(false);
			}
			return base.HandleEvent(E);
		}

    /* When the book is read for the first time, a secret is generated, so use
    that to determine whether the book has been read. */
    public bool GetHasBeenRead() => this.secret != null;
	}
}
