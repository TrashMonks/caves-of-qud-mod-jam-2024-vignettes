using XRL;
using XRL.UI;
using XRL.Wish;

[HasWishCommand]
public class Gearlink_GIVEMEASIGN_WishHandler
{
	/* A wish that sets the chance of the Gearlink_GIVEMEASIGN_LoveDrunkSignSpawnerSystem
	spawning the encounter to 100% for testing purposes.*/
	[WishCommand(Command = "GIVEMEASIGNforcespawn")]
  public static bool ForceLoveDrunkSignSpawnerSystemWishHandler()
  {
		Gearlink_GIVEMEASIGN_LoveDrunkSignSpawnerSystem love_drunk_sign_spawner_system = The.Game.GetSystem<Gearlink_GIVEMEASIGN_LoveDrunkSignSpawnerSystem>();
		if( love_drunk_sign_spawner_system == null )
		{
			Popup.Show("Gearlink_GIVEMEASIGN_LoveDrunkSignSpawnerSystem couldn't be found.");
			return true;
		}
		Popup.Show("GIVEMEASIGNforcespawn ran");
		love_drunk_sign_spawner_system.chance = 100;
    return true;
  }
}
