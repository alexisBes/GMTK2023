using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    public const int WATER_TILE = 0x01;
    public const int LAND_TILE  = 0x02;
    public const int SAND_TILE  = 0x04;
    public const int TOWN_TILE  = 0x08;

    public const int NO_MIX_TILE    = 0x00;
    public const int SWAMP_TILE     = 0x01;
    public const int QUICKSAND_TILE = 0x20;
    public const int DUNE_TILE      = 0x40;
    public const int SUBURG_TILE    = 0x80;


    public int flags = 0;
    public int mix_flags = 0;
    public int x, y;
    //public Our_Terrain our_terrain;

    public List<GameObject> prefabs;
    public GameObject tempestPrefab;

    private GameObject currentPrefab;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Asset type :" + flags);
        SetPrefab(flags);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void SetPrefab(int prefab_flags)
    {
        Destroy(currentPrefab);
        
        int prefab_index = 0;
        
        if     ((prefab_flags & TOWN_TILE)      != 0) prefab_index = 4;
        else if((prefab_flags & SUBURG_TILE)    != 0) prefab_index = 4 + 4;
        else if((prefab_flags & QUICKSAND_TILE) != 0) prefab_index = 4 + 2;
        else if((prefab_flags & SWAMP_TILE)     != 0) prefab_index = 4 + 1;
        else if((prefab_flags & DUNE_TILE)      != 0) prefab_index = 4 + 3;
        else if((prefab_flags & WATER_TILE)     != 0) prefab_index = 1;
        else if((prefab_flags & SAND_TILE)      != 0) prefab_index = 3;
        else if((prefab_flags & LAND_TILE)      != 0) prefab_index = 2;

        currentPrefab = Instantiate(prefabs[prefab_index], transform.position, Quaternion.Euler(75f, 0f, 0f));
        currentPrefab.transform.parent = transform;
    }

    void OnClickedTerrain()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.GetInstanceID() != this.GetComponent<Collider>().GetInstanceID()) return;
            
            bool play_enemy_turn = true;
            
            if (this.flags != 0)
            {
                if ((this.flags & TOWN_TILE) == 0)
                {
                    Debug.Log("Setting target");
                    State.originTile = this;
                    
                    play_enemy_turn = false;
                }
                else
                {
                    LaunchTempest();
                }
            }
            else
            {
                if (!Our_Terrain.isACaseValid(x, y)) return;
                
                if(!MixTile(State.state)) return; // This action did nothing so we do not want to run a turn.
            }
            
            
            // Play enemy's turn.
            if(play_enemy_turn) TurnBasedSystem.PerformEnemyAction();
            /////////////////////
        }
    }

    public bool MixTile(int state)
    {
        ///////////////////////////////////////////////////////
        // NOTE: when this action does nothing we return false.
        ///////////////////////////////////////////////////////
        
        Debug.Log("Mixing from " + state);
        
        bool result = true;
        
        if(flags == 0)
        {
            if     (state == State.SPAWN_WATER) flags = WATER_TILE;
            else if(state == State.SPAWN_SAND)  flags = SAND_TILE;
            else if(state == State.SPAWN_LAND)  flags = LAND_TILE;
            else Debug.LogError("INVALID STATE");
        }
        if ((flags & WATER_TILE) != 0 && state == State.SPAWN_LAND || (flags & LAND_TILE) != 0 && state == State.SPAWN_WATER)
        {
            flags |= SWAMP_TILE;
        }
        else if ((flags & WATER_TILE) != 0 && state == State.SPAWN_SAND || (flags & SAND_TILE) != 0 && state == State.SPAWN_WATER)
        {
            flags |= QUICKSAND_TILE;
        }
        else if ((flags & LAND_TILE) != 0 && state == State.SPAWN_SAND || (flags & SAND_TILE) != 0 && state == State.SPAWN_LAND)
        {
            flags |= DUNE_TILE;
        }
        else if (state == State.SPAWN_TOWN)
        {
            if ((flags & TOWN_TILE) != 0)
            {
                flags &= ~SUBURG_TILE;
                flags |=  TOWN_TILE;
            }
            else flags |= SUBURG_TILE;
        }
        else result = false;
        
        SetPrefab(flags);
        
        return result;
    }

    public void LaunchTempest()
    {
        Debug.Log("FULL PAWAR");
        Vector3 vector = State.originTile.gameObject.transform.position;
        vector.z += 6;
        currentPrefab = Instantiate(tempestPrefab, vector, Quaternion.Euler(90f, 0f, 0f));
        Tempest tempest =  currentPrefab.GetComponentInChildren<Tempest>();
        tempest.target = gameObject.transform;
    }

}