using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.ZoneBuilders;

namespace XRL
{
    [Serializable]
  public class Gearlink_CASKOFROSEWINE_FortunadoSpawnerSystem : IGameSystem
  {
    /* Because this IGameSystem is Serializable, spawned will persist across
    the game being saved and loaded. */
    public bool spawned = false;

    public int chance = 1;

    [NonSerialized]
    /* Store a dictionary of the Zones the player visits while this system is
    active so we can avoid spawning the encounter in a Zone when the player returns
    to it. */
    public Dictionary<string, bool> Visited = new Dictionary<string, bool>();

    /* Register the ZoneActivatedEvent with this system so it can do something
    when a new zone activates. */
    public override void Register(XRLGame Game, IEventRegistrar Registrar)
    {
      /* Only register if the system hasn't yet spawned Fortunado's remains. */
      if ( !this.spawned )
        Registrar.Register(ZoneActivatedEvent.ID);
    }

    /* Check if Fortunado's remains should be spawned each time a new zone
    activates. */
    public override bool HandleEvent(ZoneActivatedEvent E)
    {
      this.CheckFortunadoSpawn(E.Zone);
      return base.HandleEvent(E);
    }

    /* While string, int, and bool properties can just be marked as [Serializable]
    to persist across the game being saved and loaded, more complicated data
    types like a Dictionary need a custom serializer like this. These implementations
    of LoadGame and SaveGame are taken from XRL.PsychicHunterSystem. */
    public override void Read(SerializationReader Reader) => this.Visited = Reader.ReadDictionary<string, bool>();

    public override void Write(SerializationWriter Writer) => Writer.Write<string, bool>(this.Visited);

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
        /* Since we don't plan on spawning anymore, unregister from the
        ZoneActivatedEvent so this check doesn't run. */
        this.UnregisterEvent(ZoneActivatedEvent.ID);
        /* Still need to set spawned to true so the system knows it's already
        spawned next time the game is loaded. */
        this.spawned = true;
      }
    }
	}
}
