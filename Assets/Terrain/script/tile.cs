using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    public const int WATER_TILE = 0x01;
    public const int LAND_TILE = 0x02;
    public const int SAND_TILE = 0x04;
    public const int TOWN_TILE = 0x08;

    public const int NO_MIX_TILE = 0x00;
    public const int SWAMP_TILE = 0x10;
    public const int QUICKSAND_TILE = 0x20;
    public const int DUNE_TILE = 0x40;
    public const int SUBURB_TILE = 0x80;

    public AudioSource audioSource;

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

        if ((prefab_flags & TOWN_TILE)           != 0) prefab_index = 4;
        else if ((prefab_flags & SUBURB_TILE)    != 0) prefab_index = 4 + 4;
        else if ((prefab_flags & QUICKSAND_TILE) != 0) prefab_index = 4 + 2;
        else if ((prefab_flags & SWAMP_TILE)     != 0) prefab_index = 4 + 1;
        else if ((prefab_flags & DUNE_TILE)      != 0) prefab_index = 4 + 3;
        else if ((prefab_flags & WATER_TILE)     != 0) prefab_index = 1;
        else if ((prefab_flags & SAND_TILE)      != 0) prefab_index = 3;
        else if ((prefab_flags & LAND_TILE)      != 0) prefab_index = 2;

        currentPrefab = Instantiate(prefabs[prefab_index], transform.position, Quaternion.Euler(0f, 0f, 0f));
        currentPrefab.transform.parent = transform;
    }

    void OnClickedTerrain()
    {
        if(State.do_not_raytrace_this_frame == true)
        {
            State.do_not_raytrace_this_frame = false;
            return;
        }
        
        
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            audioSource.Play();
            
            if (hit.collider.GetInstanceID() != this.GetComponent<Collider>().GetInstanceID()) return;

            bool play_enemy_turn = true;

            if (this.flags != 0 && State.state == State.SPAWN_TEMPEST)
            {
                if ((this.flags & (TOWN_TILE | SUBURB_TILE)) == 0)
                {
                    Debug.Log("Setting target");
                    State.originTile = this;

                    play_enemy_turn = false;
                }
                else
                {
                    Debug.Log("Target acquired");
                    LaunchTempest();
                }
            }
            else
            {
                if (!Our_Terrain.isACaseValid(x, y)) return;

                if (!MixTile(State.state)) return; // This action did nothing so we do not want to run a turn.
            }
            
            if(play_enemy_turn)
            {
                TurnBasedSystem.PerformEnemyAction(); // Play enemy's turn.
                State.state = State.EMPTY;
                
                // Check for a game over state. START
                ///////////////////////////////////////////////////////////////////
                // NOTE: we consider the game to be over when all tiles are filled.
                ///////////////////////////////////////////////////////////////////
                
                int player_score = 0;
                int bot_score    = 0;
                
                bool all_tiles_are_filled = true;
                
                for(int y = 0; y < Our_Terrain.height; y++)
                {
                    if(all_tiles_are_filled == false) break;
                    
                    for(int x = 0; x < Our_Terrain.width; x++)
                    {
                        if(all_tiles_are_filled == false) break;
                        
                        Tile tile = Our_Terrain.get_tile(x, y);
                        if(tile.flags == 0) all_tiles_are_filled = false;
                        
                        if((tile.flags & (TOWN_TILE | SUBURB_TILE)) != 0) bot_score++;
                        else                                              player_score++;
                    }
                }
                
                
                if(all_tiles_are_filled)
                {
                    // @ Transition to a game over screen.
                    if(player_score >= bot_score)
                    {
                        SceneManager.LoadScene("Win");
                    }
                    else
                    {
                        SceneManager.LoadScene("Lost");
                    }
                }
                // Check for a game over state. END
            }
        }
    }

    public bool MixTile(int state)
    {
        ///////////////////////////////////////////////////////
        // NOTE: when this action does nothing we return false.
        ///////////////////////////////////////////////////////

        Debug.Log("Mixing from " + state + " case X:" + x + " y : " + y);

        bool result = true;

        if (flags == 0)
        {
            if (state == State.SPAWN_WATER) flags = WATER_TILE;
            else if (state == State.SPAWN_SAND) flags = SAND_TILE;
            else if (state == State.SPAWN_LAND) flags = LAND_TILE;
            //else if (state == State.SPAWN_TOWN) flags = TOWN_TILE;
            else if (state == State.SPAWN_TEMPEST) result = false;
            else Debug.LogError("INVALID STATE " + state);
        }
        else if ((flags & WATER_TILE) != 0 && state == State.SPAWN_LAND || (flags & LAND_TILE) != 0 && state == State.SPAWN_WATER)
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
            if ((flags & SUBURB_TILE) != 0)
            {
                flags &= ~SUBURB_TILE;
                flags |=  TOWN_TILE;
            }
            else
            {
                flags |= SUBURB_TILE;
                Debug.Assert((flags & TOWN_TILE) == 0);
            }
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
        GameObject saracePrefab = Instantiate(tempestPrefab, vector, Quaternion.Euler(90f, 0f, 0f));
        Tempest tempest = saracePrefab.GetComponentInChildren<Tempest>();
        tempest.target = gameObject.transform;
        if ((this.flags & Tile.SUBURB_TILE) != 0)
        {
            // Si c'est en cours de transformation.
            Debug.Log("dead goblin");
            this.flags &= ~Tile.SUBURB_TILE;
            this.SetPrefab(flags);
        }
        else if ((this.flags & Tile.TOWN_TILE) != 0)
        {
            Debug.Log("Dead catto");
            // Si c'est une ville.
        }
    }

}