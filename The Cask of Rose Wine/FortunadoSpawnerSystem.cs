using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;
using XRL.World.ZoneBuilders;

namespace XRL
{
  [Serializable]
  public class Gearlink_CASKOFROSEWINE_FortunadoSpawnerSystem : IGameSystem
  {
    public bool spawned = false;

    public int chance = 1;

    [NonSerialized]
    /* Store a dictionary of the Zones the player visits while this system is
    active so we can avoid spawning the encounter in a Zone when the player returns
    to it. */
    public Dictionary<string, bool> Visited = new Dictionary<string, bool>();

    /* Override this virtual method of IGameSystem so this system will do something
    whenever a zone is activated. */
    public override void ZoneActivated(Zone zone) => this.CheckFortunadoSpawn(zone);

    /* While string, int, and bool properties can just be marked as [Serializable]
    to persist across the game being saved and loaded, more complicated data
    types like a Dictionary need a custom serializer like this. These implementations
    of LoadGame and SaveGame are taken from XRL.PsychicHunterSystem. */
    public override void LoadGame(SerializationReader Reader) => this.Visited = Reader.ReadDictionary<string, bool>();

    public override void SaveGame(SerializationWriter Writer) => Writer.Write<string, bool>(this.Visited);

    public void CheckFortunadoSpawn(Zone zone)
    {
      /* Don't spawn if above ground. Z = 10 is the surface of the world and
      Z < 10 is in the sky. */
      if ( this.spawned || zone.IsWorldMap() || zone.Z <= 10 || this.Visited.ContainsKey(zone.ZoneID))
        return;

      this.Visited.Add(zone.ZoneID, true);

      if (chance.in100())
      {
        new Gearlink_CASKOFROSEWINE_FortunadoSpawnerBuilder().BuildZone( zone );
        this.spawned = true;
      }
    }
	}
}
