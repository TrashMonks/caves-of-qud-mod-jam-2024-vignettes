using XRL;
using XRL.World;

[PlayerMutator]
public class Gearlink_CASKOFROSEWINE_AddFortunadoSpawnerSystem : IPlayerMutator
{
	/* Add the FortunadoSpawnerSystem to the game. Doing it here because this
	IPlayerMutator will run when the game starts regardless of which type of game
	it is. */
	public void mutate(GameObject player)
	{
		The.Game.AddSystem( new Gearlink_CASKOFROSEWINE_FortunadoSpawnerSystem() );
	}
}
