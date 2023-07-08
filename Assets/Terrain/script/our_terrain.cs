#define DO_THE_3D_THING

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


    public static int width  = 16;
    public static int height = 16;
    const float TILE_STEP = 1;
    
    public static float default_camera_zoom  = 7.2f;
    public static float furthest_camera_zoom = 10.0f;
    public static float closest_camera_zoom  = 2.0f;
    
    Vector3 camera_look_at_direction;

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
        camera_look_at_direction = direction_to_look_at.normalized;
        
        camera_transform.position = camera_transform.position - camera_look_at_direction * 100.0f;
        
        Quaternion orientation = Quaternion.LookRotation(camera_look_at_direction, new Vector3(0, 1, 0));
        
        camera_transform.rotation = orientation;
        
        Camera.main.orthographicSize = default_camera_zoom;
        #endif
        // Set the camera so that the whole terrain is in view. END
    }

    // Update is called once per frame
    void Update()
    {
        Camera camera = Camera.main;
        
        // Handle camera zoom. START
        float camera_zoom = camera.orthographicSize;
        
        camera_zoom -= Input.mouseScrollDelta.y;
        camera_zoom  = Mathf.Clamp(camera_zoom, closest_camera_zoom, furthest_camera_zoom);
        
        camera.orthographicSize = camera_zoom;
        
        // @ Zoom around the mouse position.
        // Handle camera zoom. END
        
        
        // @ Handle camera rotation.
        
        
        // Handle camera panning. START
        float horizontal_pan = Input.GetAxisRaw("Horizontal");
        float vertical_pan   = Input.GetAxisRaw("Vertical");
        
        float horizontal_pan_speed = Time.deltaTime * 10.0f; // @ Hardcoded.
        float vertical_pan_speed   = horizontal_pan_speed * 2.0f;
        
        float zoom_extent = furthest_camera_zoom - closest_camera_zoom;
        float zoom_factor = Mathf.Lerp(0.5f, 1, camera.orthographicSize / zoom_extent);
        
        Vector3 camera_right   = Vector3.Cross(new Vector3(0, 1, 0), camera_look_at_direction).normalized;
        Vector3 camera_forward = Vector3.Cross(camera_right, new Vector3(0, 1, 0)).normalized;
        
        // @ Right now we are not preventing the camera from going too far!!!
        camera.transform.position = camera.transform.position + (camera_right * horizontal_pan * horizontal_pan_speed + camera_forward * vertical_pan * vertical_pan_speed) * zoom_factor;
        // Handle camera panning. END
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
