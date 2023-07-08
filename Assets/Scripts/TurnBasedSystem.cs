using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.Assertions;

public struct Map_Coords
{
    public int x;
    public int y;
}

public class TurnBasedSystem : MonoBehaviour
{
    static public Label turnText;
    static public VisualElement root;
    
    static Map_Coords coords_of_the_tile_that_is_being_colonised;



    private void Start()
    {
        coords_of_the_tile_that_is_being_colonised.x = -1;
        coords_of_the_tile_that_is_being_colonised.y = -1;
        
        turnText = GetComponent<UIDocument>().rootVisualElement.Q<Label>("label");
        turnText.text = "Player's Turn";

        root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("Watter").clicked  += PlayerButtonWaterClicked;
        root.Q<Button>("Air").clicked     += PlayerButtonSandClicked;
        root.Q<Button>("Land").clicked    += PlayerButtonLandClicked;
        root.Q<Button>("Tempest").clicked += PlayerButtonTempestClicked;
    }

    private void PlayerButtonWaterClicked()
    {
        State.state = State.SPAWN_WATER;
    }
    private void PlayerButtonLandClicked()
    {
        State.state = State.SPAWN_LAND;
    }
    private void PlayerButtonSandClicked()
    {
        State.state = State.SPAWN_SAND;
    }
    private void PlayerButtonTempestClicked()
    {
        State.state = State.SPAWN_TEMPEST;
    }

    static public void PerformEnemyAction()
    {
        turnText.text = "Enemy's Turn";
        
        if(coords_of_the_tile_that_is_being_colonised.x >= 0 && coords_of_the_tile_that_is_being_colonised.y >= 0)
        {
            // Finish colonising a tile we started colonising during the previous turn. START
            Tile tile_we_are_colonising = Our_Terrain.get_tile(coords_of_the_tile_that_is_being_colonised.x, coords_of_the_tile_that_is_being_colonised.y);
            if((tile_we_are_colonising.flags & Tile.SUBURB_TILE) != 0)
            {
                Debug.Assert((tile_we_are_colonising.flags & Tile.TOWN_TILE) == 0);
                tile_we_are_colonising.MixTile(State.SPAWN_TOWN);
                
                Debug.Assert((tile_we_are_colonising.flags & Tile.TOWN_TILE) != 0);
            }
            else Debug.LogError("qdfg");
            
            coords_of_the_tile_that_is_being_colonised.x = -1;
            coords_of_the_tile_that_is_being_colonised.y = -1;
            // Finish colonising a tile we started colonising during the previous turn. END
        }
        
        
        // Retrieve all colonisable tiles. START
        List<Tile> colonisable_tiles = new List<Tile>();
        
        for(int y = 0; y < Our_Terrain.height; y++)
        {
            for(int x = 0; x < Our_Terrain.width; x++)
            {
                Tile tile = Our_Terrain.get_tile(x, y);
                
                if((tile.flags & (Tile.TOWN_TILE | Tile.SUBURB_TILE)) == 0)
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
                    
                    if((nearby_flags & Tile.TOWN_TILE) != 0)
                    {
                        colonisable_tiles.Add(tile);
                    }
                }
            }
        }
        // Retrieve all colonisable tiles. END
        
        
        // Choose a tile to colonise at random and start working on it. START
        if(colonisable_tiles.Count != 0)
        {
            Debug.Log("COUNT: " + colonisable_tiles.Count);
            
            int index_to_choose_from = Random.Range(0, colonisable_tiles.Count);
            
            Tile tile_to_colonise = colonisable_tiles[index_to_choose_from];
            
            bool status = tile_to_colonise.MixTile(State.SPAWN_TOWN);
            Debug.Assert(status);
            
            coords_of_the_tile_that_is_being_colonised.x = tile_to_colonise.x;
            coords_of_the_tile_that_is_being_colonised.y = tile_to_colonise.y;
        }
        // Choose a tile to colonise at random and start working on it. END
        
        
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
