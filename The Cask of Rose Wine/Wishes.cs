using XRL;
using XRL.UI;
using XRL.Wish;

[HasWishCommand]
public class Gearlink_CASKOFROSEWINE_WishHandler
{
	/* A wish that sets the chance of the Gearlink_CASKOFROSEWINE_FortunadoSpawnerSystem
	spawning the encounter to 100% for testing purposes.*/
	[WishCommand(Command = "CASKOFROSEWINEforcespawn")]
  public static bool ForceFortunadoSpawnerSystemWishHandler()
  {
		Gearlink_CASKOFROSEWINE_FortunadoSpawnerSystem fortunado_spawner_system = The.Game.GetSystem<Gearlink_CASKOFROSEWINE_FortunadoSpawnerSystem>();
		if( fortunado_spawner_system == null )
		{
			Popup.Show("Gearlink_CASKOFROSEWINE_FortunadoSpawnerSystem couldn't be found.");
			return true;
		}
		Popup.Show("CASKOFROSEWINEforcespawn ran");
		fortunado_spawner_system.chance = 100;
    return true;
  }
}
