//#define DO_THE_3D_THING

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class Our_Terrain : MonoBehaviour
{
    public GameObject tile_prefab;


    public static int width = 16;
    public static int height = 16;
    const float TILE_STEP = 1;

    static public List<Tile> tiles = new List<Tile>();
    
    // Start is called before the first frame update
    void Start()
    {
        Vector3 position = new Vector3(0, 0, 0);
        int centreW = (int)(UnityEngine.Random.value * width);
        int centreH = (int)(UnityEngine.Random.value * height);
        if (centreW == 0) centreW++;
        if (centreH == 0) centreH++;
        if (centreW == width -1 ) centreW--;
        if (centreH == height -1 ) centreH--;
        Debug.Log("X : " + centreW + ", Y : " + centreH);
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int newFlags = 0x00;
                
                // Set the goblins colony.
                if ((x >= centreW - 1 && x <= centreW + 1) && (y <= centreH + 1 && y >= centreH - 1)) newFlags = 1 << UnityEngine.Random.Range(0, 3);
                if (y == centreH && x == centreW)                                                     newFlags = Tile.TOWN_TILE;
                //////////////////////////
                
                GameObject tile = Instantiate(tile_prefab, position, tile_prefab.transform.rotation);
                Tile terrain = tile.GetComponent<Tile>();
                terrain.flags       = newFlags;
                //terrain.our_terrain = this;
                terrain.x           = x;
                terrain.y           = y;

                PlayerInput pi = tile.GetComponent<PlayerInput>();
                pi.camera = Camera.main;
                position.x += TILE_STEP;
                
                tiles.Add(terrain);
            }
            
            #if DO_THE_3D_THING
            position.z += TILE_STEP;
            #else
            position.y += TILE_STEP;
            #endif
            
            position.x = 0;
        }

        // Set the camera so that the whole terrain is in view. START
        #if !DO_THE_3D_THING
        Vector3 top_left_corner    = new Vector3(-TILE_STEP * 0.5f, -TILE_STEP * 0.5f, 0);
        Vector3 bottom_left_corner = new Vector3(top_left_corner.x + (float)width * TILE_STEP, top_left_corner.y + (float)height * TILE_STEP, 0);

        Transform camera_transform = Camera.main.GetComponent<Transform>();
        camera_transform.position = new Vector3(
            top_left_corner.x + (bottom_left_corner.x - top_left_corner.x) * 0.5f,
            top_left_corner.y + (bottom_left_corner.y - top_left_corner.y) * 0.5f,
            camera_transform.position.z
        );
        #else
        Vector3 top_left_corner    = new Vector3(-TILE_STEP * 0.5f, 0, -TILE_STEP * 0.5f);
        Vector3 bottom_left_corner = new Vector3(top_left_corner.x + (float)width * TILE_STEP, 0, top_left_corner.z + (float)height * TILE_STEP);
        
        Transform camera_transform = Camera.main.GetComponent<Transform>();
        camera_transform.position = new Vector3(
            bottom_left_corner.x,
            5,
            bottom_left_corner.z
        );
        
        Vector3 where_to_look_at = new Vector3(
            top_left_corner.x + (bottom_left_corner.x - top_left_corner.x) * 0.5f,
            0,
            top_left_corner.z + (bottom_left_corner.z - top_left_corner.z) * 0.5f
        );
        
        Vector3 direction_to_look_at = where_to_look_at - camera_transform.position;
        
        Quaternion orientation = Quaternion.LookRotation(direction_to_look_at.normalized, new Vector3(0, 1, 0));
        
        camera_transform.rotation = orientation;
        
        Camera.main.orthographicSize = 7.2f;
        #endif
        // Set the camera so that the whole terrain is in view. END
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    static public Tile get_tile(int x, int y)
    {
        return tiles[y * width + x];
    }
    
    static public bool isACaseValid(int x, int y)
    {
        Debug.Log("X: " + x + ", Y:" + y);
        
        int flags_from_surrounding_tiles = 0;
        
        if (x > 0)          flags_from_surrounding_tiles |= get_tile(x - 1, y).flags;
        if (x + 1 < width)  flags_from_surrounding_tiles |= get_tile(x + 1, y).flags;
        if (y > 0)          flags_from_surrounding_tiles |= get_tile(x, y - 1).flags;
        if (y + 1 < height) flags_from_surrounding_tiles |= get_tile(x, y + 1).flags;
        
        if(flags_from_surrounding_tiles == 0)
        {
            Debug.Log("This tile cannot be used!");
            return false;
        }
        
        return true;
    }
}
