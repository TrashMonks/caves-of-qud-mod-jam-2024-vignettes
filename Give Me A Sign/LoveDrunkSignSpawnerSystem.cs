using System;
using System.Collections.Generic;
using XRL.Liquids;
using XRL.World;
using XRL.World.Effects;
using XRL.World.Parts;
using XRL.World.ZoneBuilders;

namespace XRL
{
  [Serializable]
  public class Gearlink_GIVEMEASIGN_LoveDrunkSignSpawnerSystem : IGameSystem
  {
    public bool spawned = false;

    public int chance = 5;

    [NonSerialized]
    /* Store a dictionary of the Zones the player visits while this system is
    active so we can avoid spawning the encounter in a Zone when the player returns
    to it. */
    public Dictionary<string, bool> Visited = new Dictionary<string, bool>();

    /* Override this virtual method of IGameSystem so this system will do something
    whenever a zone is activated. */
    public override void ZoneActivated(Zone zone) => this.LoveDrunkSignSpawn(zone);

    /* While string, int, and bool properties can just be marked as [Serializable]
    to persist across the game being saved and loaded, more complicated data
    types like a Dictionary need a custom serializer like this. These implementations
    of LoadGame and SaveGame are taken from XRL.PsychicHunterSystem. */
    public override void LoadGame(SerializationReader Reader) => this.Visited = Reader.ReadDictionary<string, bool>();

    public override void SaveGame(SerializationWriter Writer) => Writer.Write<string, bool>(this.Visited);

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
