using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Effects;
using XRL.World.Parts;

namespace XRL
{
    [Serializable]
  public class Gearlink_GIVEMEASIGN_LoveDrunkSignSpawnerSystem : IGameSystem
  {
    /* Because this IGameSystem is Serializable, spawned will persist across
    the game being saved and loaded. */
    public bool spawned = false;

    public int chance = 5;

    [NonSerialized]
    /* Store a dictionary of the Zones the player visits while this system is
    active so we can avoid spawning the encounter in a Zone when the player returns
    to it. */
    public Dictionary<string, bool> Visited = new Dictionary<string, bool>();

    /* Register the ZoneActivatedEvent with this system so it can do something
    when a new zone activates. */
    public override void Register(XRLGame Game, IEventRegistrar Registrar)
    {
      /* Only register if the system hasn't yet spawned the items. */
      if ( !this.spawned )
        Registrar.Register(ZoneActivatedEvent.ID);
    }

    /* Check if the items should be spawned each time a new zone activates. */
    public override bool HandleEvent(ZoneActivatedEvent E)
    {
      this.LoveDrunkSignSpawn(E.Zone);
      return base.HandleEvent(E);
    }

    /* While string, int, and bool properties can just be marked as [Serializable]
    to persist across the game being saved and loaded, more complicated data
    types like a Dictionary need a custom serializer like this. These implementations
    of LoadGame and SaveGame are taken from XRL.PsychicHunterSystem. */
    public override void Read(SerializationReader Reader) => this.Visited = Reader.ReadDictionary<string, bool>();

    public override void Write(SerializationWriter Writer) => Writer.Write<string, bool>(this.Visited);

    public void LoveDrunkSignSpawn(Zone zone)
    {
      if (this.spawned || zone.IsWorldMap() || this.Visited.ContainsKey(zone.ZoneID))
        return;

      this.Visited.Add(zone.ZoneID, true);

      // Look for the tag added to the base sign object
      List<GameObject> signs = zone.FindObjectsWithTagOrProperty("Gearlink_GIVEMEASIGN_LovableSign");
      // Only do something if zone has at least one loveable sign
      if ( signs.IsNullOrEmpty() )
        return;

      if (chance.in100())
      {
        Cell signCell = signs.GetRandomElement<GameObject>().CurrentCell;
        /* Spawn the paraphernalia around the sign. Try to pick a new empty cell
        each time and spawn the item on the sign itself if no empty cell can be
        found */
        Cell adjacentCell = signCell.GetEmptyConnectedAdjacentCells( 1 ).RemoveRandomElement<Cell>();
        if( adjacentCell == null )
          AddLoveNote( signCell );
        else
          AddLoveNote( adjacentCell );

        adjacentCell = signCell.GetEmptyConnectedAdjacentCells( 1 ).RemoveRandomElement<Cell>();
        if( adjacentCell == null )
          AddEmptyBooster( signCell );
        else
          AddEmptyBooster( adjacentCell );

        adjacentCell = signCell.GetEmptyConnectedAdjacentCells( 1 ).RemoveRandomElement<Cell>();
        if( adjacentCell == null )
          AddGlassBottle( signCell );
        else
          AddGlassBottle( adjacentCell );

        /* Since we don't plan on spawning anymore, unregister from the
        ZoneActivatedEvent so this check doesn't run. */
        this.UnregisterEvent(ZoneActivatedEvent.ID);
        /* Still need to set spawned to true so the system knows it's already
        spawned next time the game is loaded. */
        this.spawned = true;
      }
    }

    public void AddLoveNote( Cell cell )
    {
      cell.AddObject( "Gearlink_GIVEMEASIGN_SignLoveNote" );
    }

    public void AddEmptyBooster( Cell cell )
    {
      GameObject emptyBooster = cell.AddObject( "Empty Booster" );
      emptyBooster.ForceApplyEffect((Effect) new LiquidStained(new LiquidVolume("wine", 1)));
    }

    public void AddGlassBottle( Cell cell )
    {
      GameObject glassBottle = cell.AddObject( "Bottle" );
      glassBottle.ForceApplyEffect((Effect) new LiquidStained(new LiquidVolume("wine", 1)));
      LiquidVolume liquidVolume = glassBottle.LiquidVolume;
      liquidVolume.ComponentLiquids.Clear();
      liquidVolume.ComponentLiquids.Add("wine", 1000);
      liquidVolume.Volume = 1;
      liquidVolume.Update();
    }
	}
}
