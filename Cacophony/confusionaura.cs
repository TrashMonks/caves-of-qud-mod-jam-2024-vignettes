using System;
using System.Collections.Generic;
using XRL.Rules;
using XRL.World.Capabilities;
using XRL.World.Effects;

namespace XRL.World.Parts
{
  [Serializable]
  public class ConfusionAura : IActivePart
  {
	public static readonly int ICON_COLOR_PRIORITY = 150;
    public static readonly string COMMAND_NAME = "CommandFearAura";
    public int Chance = 25;
    public int EnergyCost = 1000;
    public int SpinTimer;
    public int Cooldown;
	public string ColorString = "&y";
	public string DetailColor = "R";

    //public ConfusionAura() => this.DisplayName = "Confusion Aura";

    //public override string GetLevelText(int Level) => "You're really scary.\n";

    public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == EndTurnEvent.ID || ID == CommandEvent.ID;

    public override bool HandleEvent(EndTurnEvent E)
    {
		//XRL.Messages.MessageQueue.AddPlayerMessage("event is firing");
      --this.Cooldown;
      if (this.Cooldown < 0 /*&& this.IsMyActivatedAbilityVoluntarilyUsable(this.ActivatedAbilityID) */&& this.Chance.in100())
      {
		this.DetailColor = RandomScreenColor();
        if (!ConfusionAura.PulseAura(this.ParentObject))
          return false;
        this.Cooldown = Stat.Random(21, 26);
      }
      if (this.EnergyCost <= 0 || this.ParentObject.IsPlayer())
        return base.HandleEvent(E);
      //this.UseEnergy(this.EnergyCost);
      return false;
    }

    public override bool HandleEvent(CommandEvent E)
    {
      if (E.Command == ConfusionAura.COMMAND_NAME && this.Cooldown < 0 /*&& this.IsMyActivatedAbilityVoluntarilyUsable(this.ActivatedAbilityID)*/)
      {
        this.Cooldown = Stat.Random(34, 39);
        /*if (ConfusionAura.PulseAura(this.ParentObject) && this.EnergyCost > 0 && !this.ParentObject.IsPlayer())
          this.UseEnergy(this.EnergyCost);*/
      }
      return base.HandleEvent(E);
    }
	
	public override bool Render(RenderEvent E)
    {
      if (!this.ColorString.IsNullOrEmpty() || !this.DetailColor.IsNullOrEmpty())
        E.ApplyColors(this.ColorString, this.DetailColor, ConfusionAura.ICON_COLOR_PRIORITY, ConfusionAura.ICON_COLOR_PRIORITY);
      return true;
    }

    public static bool PulseAura(GameObject Actor)
    {
      List<Cell> localAdjacentCells = Actor?.CurrentCell?.GetLocalAdjacentCells();
      if (localAdjacentCells == null)
        return false;
      bool flag = false;
      foreach (Cell cell in localAdjacentCells)
      {
        foreach (GameObject Defender in cell.Objects)
        {
          if (Defender.pBrain != null)
          {
            Mental.PerformAttack(new Mental.Attack(ConfusionAura.ApplyFear), Actor, Defender, Command: "Confuse Aura", Dice: "1d8+4", Type: 8388609, Magnitude: "1d3".RollCached());
            flag = true;
          }
        }
      }
      if (flag)
        Actor.ParticleBlip("&W!");
      return flag;
    }

    public override bool AllowStaticRegistration() => true;

    public static bool ApplyFear(MentalAttackEvent E)
    {
      if (!E.Defender.FireEvent("CanApplyFear") || !CanApplyEffectEvent.Check(E.Defender, "Confused"))
        return false;
      if (E.Penetrations > 0)
      {
        Confused E1 = new Confused(E.Magnitude+10, 5,5/*E.Attacker, true*/);
        if (E.Defender.ApplyEffect((Effect) E1))
          return true;
      }
      if (E.Defender.IsPlayer())
        IComponent<GameObject>.AddPlayerMessage("You feel uneasy.", 'K');
      return false;
    }

    /*public override bool Mutate(GameObject GO, int Level)
    {
      this.ActivatedAbilityID = this.AddMyActivatedAbility("Fear Aura", ConfusionAura.COMMAND_NAME, "Physical Mutation", Icon: "®");
      return base.Mutate(GO, Level);
    }

    public override bool Unmutate(GameObject GO)
    {
      this.RemoveMyActivatedAbility(ref this.ActivatedAbilityID);
      return base.Unmutate(GO);
    }*/
		
	public static string RandomScreenColor()
	{
		int ColorIndex = Stat.RandomCosmetic(0, 5); 
		string[] ColorList = new string[6]{"R","W","G","C","B","M"};
		return ColorList[ColorIndex];
	}
  }
}
