using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

using static TurnBasedSystem;
using static State;

public class Tile : MonoBehaviour
{
    public const int WATER_TILE = 0x01;
    public const int LAND_TILE  = 0x02;
    public const int SAND_TILE  = 0x04;
    public const int TOWN_TILE  = 0x08;

    public const int NO_MIX_TILE    = 0x00;
    public const int SWAMP_TILE     = 0x10;
    public const int QUICKSAND_TILE = 0x20;
    public const int DUNE_TILE      = 0x40;
    public const int SUBURB_TILE    = 0x80;
    
    public const int TILE_IS_UNUSABLE = 0x100;
    
    public const int BASIC_TERRAIN = WATER_TILE | LAND_TILE | SAND_TILE;

    public AudioSource audioSource;

    public AudioSource audioSourceDenied;
    private Slider slider;
    private UIDocument uiDocument;
    public string uiDocumentName;

    [NonSerialized] public AudioSource earthquake_audio_source;
    public AudioClip earthquake_audio_clip;
    
    public int flags_from_last_time_we_set_the_prefab = -1;
    public int flags = 0;
    public int original_terrain = 0;
    public int x, y;
    public int num_turns_left_until_usable = 0;

    public List<GameObject> prefabs;
    public GameObject tempestPrefab;
    public GameObject camp_prefab_to_instantiate_from;
    public GameObject camp_prefab = null;
    public GameObject unusable_tile_prefab;
    
    public GameObject ui_disk_to_instantiate_from;
    public GameObject ui_disk;

    public GameObject currentPrefab { get; private set; }
    
    Vector3 camp_target_position;
    
    bool playing_earthquake_sound = true;

    private bool notClickedThrough = false;

    public int player_score = 0;
    public int bot_score    = 0;

    // Start is called before the first frame update
    void Start()
    {
        Transform t = GetComponent<Transform>();
        
        Vector3 spawn_position = t.position;
        spawn_position.y += 0.3f;
        
        Quaternion orientation = new Quaternion();
        orientation.eulerAngles = new Vector3(-90, 0, 0);
        ui_disk = Instantiate(ui_disk_to_instantiate_from, spawn_position, orientation);
        
        Renderer renderer = ui_disk.GetComponent<Renderer>();
        renderer.enabled = false;
        
        earthquake_audio_source = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        earthquake_audio_source.clip = earthquake_audio_clip;
    }

    // Update is called once per frame
    void Update()
    {
        Camera camera = Camera.main;
        Vector3 mouse_position = Input.mousePosition;
        
        bool we_can_do_something_on_this_tile = Our_Terrain.isACaseValid(this.x, this.y);
        
        
        // Update the mouse over highlight thing. START
        Renderer renderer = ui_disk.GetComponent<Renderer>();
        
        if(bot_must_play == false && (flags & TOWN_TILE) == 0 && there_is_an_active_tornado == false && we_can_do_something_on_this_tile)
        {
            Ray ray = camera.ScreenPointToRay(mouse_position);
            RaycastHit hit;
            if(Physics.Raycast(ray.origin, ray.direction, out hit, 500))
            {
                if (hit.collider.GetInstanceID() == this.GetComponent<Collider>().GetInstanceID()) renderer.enabled = true;
                else                                                                               renderer.enabled = false;
            }
        }
        else renderer.enabled = false;
        // Update the mouse over highlight thing. END
        
        
        if(camp_prefab)
        {
            // Update the camp. START
            if(there_is_an_active_tornado == true) return; // We do not want to make castles appear out of the blue when there is a tornado about.
            
            if(playing_earthquake_sound == false)
            {
                earthquake_audio_source.Play();
                playing_earthquake_sound = true;
            }
            
            Transform t = camp_prefab.GetComponent<Transform>();
            
            t.position = Vector3.MoveTowards(t.position, camp_target_position, Time.deltaTime * 0.3f);
            // Update the camp. END
        }
        
        // Update usability flag. START
        if(we_can_do_something_on_this_tile == true) flags &= ~TILE_IS_UNUSABLE;
        else if(flags == 0)                          flags |=  TILE_IS_UNUSABLE;
        // Update usability flag. END
        
        
        if(flags != flags_from_last_time_we_set_the_prefab)
        {
            // Update prefab. START
            Destroy(currentPrefab);
            
            int prefab_index = 0;
            
            if ((flags & TOWN_TILE)           != 0) prefab_index = 4;
            //else if ((flags & SUBURB_TILE)    != 0) prefab_index = 4 + 4;
            else if ((flags & QUICKSAND_TILE) != 0) prefab_index = 4 + 2;
            else if ((flags & SWAMP_TILE)     != 0) prefab_index = 4 + 1;
            else if ((flags & DUNE_TILE)      != 0) prefab_index = 4 + 3;
            else if ((flags & WATER_TILE)     != 0) prefab_index = 1;
            else if ((flags & SAND_TILE)      != 0) prefab_index = 3;
            else if ((flags & LAND_TILE)      != 0) prefab_index = 2;
            
            GameObject prefab_to_instanciate = prefabs[prefab_index];
            
            if((flags & TILE_IS_UNUSABLE) != 0) prefab_to_instanciate = unusable_tile_prefab;
            
            currentPrefab = Instantiate(prefab_to_instanciate, transform.position, Quaternion.Euler(0, 0, 0));
            currentPrefab.transform.parent = transform;
            
            if((flags & SUBURB_TILE) != 0)
            {
                if(camp_prefab) Destroy(camp_prefab);
                
                
                camp_target_position = transform.position;
                Vector3 camp_start_position = camp_target_position - new Vector3(0, 1, 0);
                
                camp_prefab = Instantiate(camp_prefab_to_instantiate_from, camp_start_position, Quaternion.Euler(0, 0, 0));
                camp_prefab.transform.parent = transform;
                
                if(there_is_an_active_tornado == false)
                {
                    playing_earthquake_sound = true;
                    earthquake_audio_source.Play();
                }
                else playing_earthquake_sound = false;
            }
            else if(camp_prefab ) Destroy(camp_prefab);
            
            flags_from_last_time_we_set_the_prefab = flags;
            // Update prefab. END
        }
        
        
        notClickedThrough = EventSystem.current.IsPointerOverGameObject();
    }


    void OnClickedTerrain()
    {
        if(bot_must_play == true)              return; // The bot is "still" playing its turn
        if(there_is_an_active_tornado == true) return; // We do not want to play while there is an active tornado about.

        if (notClickedThrough) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            audioSource.Play();
            
            if (hit.collider.GetInstanceID() != this.GetComponent<Collider>().GetInstanceID()) return;

            bool play_enemy_turn = true;

            if (this.flags != 0 && action_to_perform == SPAWN_TEMPEST)
            {
                if ((this.flags & (TOWN_TILE | SUBURB_TILE)) == 0)
                {
                    if(storm_start_tile != null)
                    {
                        Debug.Log("Erase old origin");
                        storm_start_tile.GetComponentInChildren<Renderer>().enabled = false;
                    }
                    Debug.Log("Setting target");
                    storm_start_tile = this;
                    this.GetComponentInChildren<MeshRenderer>().enabled = true;
                    play_enemy_turn = false;
                }
                else if(storm_start_tile != null && (this.x == storm_start_tile.x) != (this.y == storm_start_tile.y))
                {
                    if((this.flags & SUBURB_TILE) == 0)
                    {
                        audioSourceDenied.Play();
                        Debug.Log("This target is not a suburb.");
                        return; // We did not target a suburb so no need to proceed.
                    }
                    
                    
                    // Unleash a storm. START
                    Debug.Log("Target acquired");
                    
                    there_is_an_active_tornado = true;
                    
                    int step_x = 0;
                    int step_y = 0;
                    
                    if(this.x == storm_start_tile.x) step_y = storm_start_tile.y < this.y ? 1 : -1;
                    else                             step_x = storm_start_tile.x < this.x ? 1 : -1;
                    
                    
                    Tile last_tile = storm_start_tile;
                    
                    int x_coord = storm_start_tile.x + step_x;
                    int y_coord = storm_start_tile.y + step_y;
                    
                    Tile end_tile = last_tile;

                    last_tile.GetComponentInChildren<MeshRenderer>().enabled = false;
                    while (true)
                    {
                        Tile tile = Our_Terrain.get_tile(x_coord, y_coord);
                        end_tile = tile;
                        
                        bool it_works = Our_Terrain.get_priority_between_tiles(last_tile, tile);
                        if (!it_works)
                        {
                            audioSourceDenied.Play();
                            Debug.Log("You lost at rock-paper-scissors.");
                           break;
                        }
                        
                        
                        if((tile.flags & SUBURB_TILE) != 0)
                        { // We found a target.
                            Debug.Log("Found a target.");
                            
                            if((last_tile.flags & BASIC_TERRAIN) != (tile.flags & BASIC_TERRAIN))
                            {
                                tile.flags &= ~SUBURB_TILE;
                                tile.num_turns_left_until_usable = Our_Terrain.num_turns_a_tile_is_denied_when_the_bot_gets_its_castle_destroyed;
                            }
                            
                            break;
                        }
                        
                        if((tile.flags & TOWN_TILE) != 0)
                        {
                            break; // Cities block storms.
                        }
                        
                        if(x_coord == this.x && y_coord == this.y)
                        {
                            Debug.Log("We reached the last tile.");
                            break; // We reached the last tile.
                        }

                        Animation[] animations = tile.currentPrefab.gameObject.GetComponentsInChildren<Animation>();
                        for(int i = 0;i < animations.Length; i++)
                        {
                            Animation animation = animations[i];
                            if(animation != null)
                            {
                                String name = animation.clip.name; // @ BUG: sometimes something goes wrong here.
                                if (animation.clip.name.IndexOf('-') == -1)
                                    name += "-Tempest";
                                bool isPLay = animation.Play(name, PlayMode.StopAll);
                                Debug.Log("Trying play " + name + " return " + isPLay);
                            }
                        }

                        last_tile = tile;
                        x_coord += step_x;
                        y_coord += step_y;
                    }
                    
                    
                    // Spawn an object for visual effects.
                    Transform origin_transform = storm_start_tile.GetComponent<Transform>();
                    Vector3 storm_spawn_site = origin_transform.position;
                    
                    Transform end_transform = end_tile.GetComponent<Transform>();
                    
                    GameObject saracePrefab = Instantiate(tempestPrefab, storm_spawn_site, Quaternion.Euler(0, 0, 0));
                    Tempest tempest = saracePrefab.GetComponentInChildren<Tempest>();
                    tempest.target  = end_transform.position;
                    //////////////////////////////////////
                    // Unleash a storm. END
                }
            }
            else
            {
                if (!Our_Terrain.isACaseValid(x, y)) return;

                if (!MixTile(action_to_perform)) return; // This action did nothing so we do not want to run a turn.
            }
            
            if(play_enemy_turn)
            {
                for(int i = 0; i < Our_Terrain.tiles.Count; i++)
                {
                    Tile tile = Our_Terrain.tiles[i];
                    if(tile.num_turns_left_until_usable != 0) tile.num_turns_left_until_usable--;
                }
                
                bot_must_play        = true;
                bot_turn_delay_start = Time.realtimeSinceStartup;
                
                PerformEnemyAction(true); // Do a light turn where only basic stuff happens (like basic castles becoming full castles, drains, etc).
            }
        }
    }
    
    static public int action_to_tile_type(int action)
    {
        switch(action)
        {
            case SPAWN_SAND:  return SAND_TILE;
            case SPAWN_WATER: return WATER_TILE;
            case SPAWN_LAND:  return LAND_TILE;
        }
        
        return 0;
    }

    public int getBot_score() {
        return bot_score;
    }

    public int getPlayer_score() {
        Debug.Log("getPlayer_score -> " + player_score);
        return player_score;
    }

    public bool MixTile(int action_to_perform)
    {
        ///////////////////////////////////////////////////////
        // NOTE: when this action does nothing we return false.
        ///////////////////////////////////////////////////////

        //Debug.Log("Mixing from " + action_to_perform + " case X:" + x + " y : " + y);

        bool result = true;

        if (flags == 0)
        {
            if (action_to_perform == SPAWN_WATER)        flags  = WATER_TILE;
            else if (action_to_perform == SPAWN_SAND)    flags  = SAND_TILE;
            else if (action_to_perform == SPAWN_LAND)    flags  = LAND_TILE;
            else if (action_to_perform == SPAWN_TEMPEST) return false;
            else if (action_to_perform == EMPTY)         return false;
            else Debug.LogError("INVALID STATE " + action_to_perform);
            
            original_terrain = flags;
        }
        else if ((flags & WATER_TILE) != 0 && action_to_perform == SPAWN_LAND || (flags & LAND_TILE) != 0 && action_to_perform == SPAWN_WATER)
        {
            flags |= SWAMP_TILE | action_to_tile_type(action_to_perform);
        }
        else if ((flags & WATER_TILE) != 0 && action_to_perform == SPAWN_SAND || (flags & SAND_TILE) != 0 && action_to_perform == SPAWN_WATER)
        {
            flags |= QUICKSAND_TILE | action_to_tile_type(action_to_perform);
        }
        else if ((flags & LAND_TILE) != 0 && action_to_perform == SPAWN_SAND || (flags & SAND_TILE) != 0 && action_to_perform == SPAWN_LAND)
        {
            flags |= DUNE_TILE | action_to_tile_type(action_to_perform);
        }
        else if (action_to_perform == SPAWN_TOWN)
        {
            Debug.Assert(pop_count(flags & BASIC_TERRAIN) == 1);
            
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

        return result;
    }
}
