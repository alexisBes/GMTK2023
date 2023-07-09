using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

using static Tile;
using static State;

public class TurnBasedSystem : MonoBehaviour
{
    static public Label turnText;
    static public Label player_score_text;
    static public Label bot_score_text;
    static public VisualElement root;
    
    static public bool  there_is_an_active_tornado = false;
    static public bool  bot_must_play              = false;
    static public float bot_turn_delay_start       = 0;
    static public float bot_turn_delay_seconds     = 2;
    
    static Tile tile_to_colonise       = null;
    static Tile mixed_tile_to_colonise = null;
    
    private static int CountTurn = 0;
    
    public static int pop_count(int value)
    {
        int result = 0;
        for(int i = 0; i < 32; i++)
        {
            if((value & 1) != 0) result++;
            value = value >> 1;
        }
        
        return result;
    }
    
    private void Start()
    {
        turnText = GetComponent<UIDocument>().rootVisualElement.Q<Label>("label");
        turnText.text = "Player's Turn";
        
        // @ Set score labels.
                
        root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("Watter").clicked  += PlayerButtonWaterClicked;
        root.Q<Button>("Air").clicked     += PlayerButtonSandClicked;
        root.Q<Button>("Land").clicked    += PlayerButtonLandClicked;
        root.Q<Button>("Tempest").clicked += PlayerButtonTempestClicked;
    }
    
    void Update()
    {
        if(bot_must_play) turnText = "Enemy's turn";
        else              turnText = "Player's turn";
        
        float time = Time.realtimeSinceStartup;
        
        if(bot_must_play == true && (time - bot_turn_delay_start) > bot_turn_delay_seconds && there_is_an_active_tornado == false)
        {
            PerformEnemyAction(false);
            action_to_perform = EMPTY;
            
            // Check for a game over state. START
            ////////////////////////////////////////////////////////////////
            // NOTE: we consider the game is over when all tiles are filled.
            ////////////////////////////////////////////////////////////////
            
            bool all_tiles_are_filled = true;
            
            int player_score = 0;
            int bot_score    = 0;
            
            for(int i = 0; i < Our_Terrain.tiles.Count; i++)
            {
                Tile tile = Our_Terrain.tiles[i];
                
                if(tile.flags == 0) all_tiles_are_filled = false;
                
                if((tile.flags & (TOWN_TILE | SUBURB_TILE)) != 0) bot_score++;
                else                                              player_score++;
            }
            
            // @ Set score labels.
            
            if(all_tiles_are_filled)
            {
                // Transition to a game over screen. START
                if(player_score >= bot_score)
                {
                    SceneManager.LoadScene("Win");
                }
                else
                {
                    SceneManager.LoadScene("Lost");
                }
                // Transition to a game over screen. END
            }
            // Check for a game over state. END
            
            bot_must_play = false;
        }
    }
    
    private void PlayerButtonWaterClicked()
    {
        action_to_perform = SPAWN_WATER;
    }
    private void PlayerButtonLandClicked()
    {
        action_to_perform = SPAWN_LAND;
    }
    private void PlayerButtonSandClicked()
    {
        action_to_perform = SPAWN_SAND;
    }
    private void PlayerButtonTempestClicked()
    {
        action_to_perform = SPAWN_TEMPEST;
    }
    
    static public void PerformEnemyAction(bool only_do_the_automatic_stuff)
    {
        Debug.Assert(bot_must_play == true);
        
        if(tile_to_colonise != null && mixed_tile_to_colonise == null)
        {
            // Finish colonising a tile we started colonising during the previous turn. START
            if((tile_to_colonise.flags & SUBURB_TILE) != 0)
            {
                Debug.Assert((tile_to_colonise.flags & TOWN_TILE) == 0);
                tile_to_colonise.MixTile(SPAWN_TOWN);
                Debug.Assert((tile_to_colonise.flags & TOWN_TILE) != 0);
            }
            // Finish colonising a tile we started colonising during the previous turn. END
        }
        
        tile_to_colonise = null;
        
        if(only_do_the_automatic_stuff) return;
        
        
        if(mixed_tile_to_colonise == null)
        {
            // Retrieve all colonisable tiles. START
            List<Tile> colonisable_tiles = new List<Tile>();
            
            for(int y = 0; y < Our_Terrain.height; y++)
            {
                for(int x = 0; x < Our_Terrain.width; x++)
                {
                    Tile tile = Our_Terrain.get_tile(x, y);
                    
                    if(tile.flags == 0) continue;
                    if(tile.num_turns_left_until_usable > 0) continue;
                    
                    if((tile.flags & (TOWN_TILE | SUBURB_TILE)) == 0)
                    {
                        int nearby_flags = 0;
                        
                        if(x > 0)
                        {
                            Tile adjacent = Our_Terrain.get_tile(x - 1, y);
                            nearby_flags |= adjacent.flags;
                        }
                        if(x + 1 < Our_Terrain.width)
                        {
                            Tile adjacent = Our_Terrain.get_tile(x + 1, y);
                            nearby_flags |= adjacent.flags;
                        }
                        if(y > 0)
                        {
                            Tile adjacent = Our_Terrain.get_tile(x, y - 1);
                            nearby_flags |= adjacent.flags;
                        }
                        if(y + 1 < Our_Terrain.height)
                        {
                            Tile adjacent = Our_Terrain.get_tile(x, y + 1);
                            nearby_flags |= adjacent.flags;
                        }
                        
                        if((nearby_flags & TOWN_TILE) != 0)
                        {
                            colonisable_tiles.Add(tile);
                        }
                    }
                }
            }
            // Retrieve all colonisable tiles. END
            if(colonisable_tiles.Count != 0)
            { // Choose a tile to colonise at random.
                int index_to_choose_from = Random.Range(0, colonisable_tiles.Count);
                
                tile_to_colonise = colonisable_tiles[index_to_choose_from];
                Debug.Assert(tile_to_colonise.flags != 0x00);
            }
        }
        else
        { // We are already working on a tile to colonise.
            tile_to_colonise       = mixed_tile_to_colonise;
            mixed_tile_to_colonise = null;
            Debug.Assert(tile_to_colonise.flags != 0x00);
        }
        
        
        if(tile_to_colonise && tile_to_colonise.original_terrain != 0x00)
        {
            // Work on the tile to colonise. START
            if((tile_to_colonise.flags & BASIC_TERRAIN) != tile_to_colonise.original_terrain)
            { // This is a mixed tile so we need to unmix it first and then later we will build a castle on it.
                tile_to_colonise.flags &= ~(BASIC_TERRAIN | QUICKSAND_TILE | SWAMP_TILE | DUNE_TILE);
                tile_to_colonise.flags |=  tile_to_colonise.original_terrain;
                tile_to_colonise.SetPrefab();
                mixed_tile_to_colonise = tile_to_colonise;
            }
            else
            {
                Debug.Assert(pop_count(tile_to_colonise.flags & BASIC_TERRAIN) == 1);
                
                mixed_tile_to_colonise = null;
                
                Debug.Assert((tile_to_colonise.flags & SUBURB_TILE) == 0);
                Debug.Assert(tile_to_colonise.flags != 0);
                bool status = tile_to_colonise.MixTile(SPAWN_TOWN);
                Debug.Assert(status);
                Debug.Assert((tile_to_colonise.flags & (SUBURB_TILE | TOWN_TILE)) == SUBURB_TILE);
            }
            // Work on the tile to colonise. END
        }
        
        
        Debug.Log("Enemy action performed");
    }

    private void EnableButtons()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("Watter").SetEnabled(true);
        root.Q<Button>("Air").SetEnabled(true);
        root.Q<Button>("Land").SetEnabled(true);
    }

    private void DisableButtons()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("Watter").SetEnabled(false);
        root.Q<Button>("Air").SetEnabled(false);
        root.Q<Button>("Land").SetEnabled(false);
    }
}
