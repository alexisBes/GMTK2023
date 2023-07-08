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

    public int width = 8;
    public int height = 7;
    
    const float TILE_STEP = 2;
    
    // Start is called before the first frame update
    void Start()
    {
        Vector3 position = new Vector3(0,0,0);
        
        for (int y = 0; y < height; y++) 
        {
            for(int x = 0; x < width; x++)
            {
                GameObject tile = Instantiate(tile_prefab, position, tile_prefab.transform.rotation);
                Tile tile_info = tile.GetComponent<Tile>();
                tile_info.flags = 0;
                
                PlayerInput pi = tile.GetComponent<PlayerInput>();
                pi.camera = Camera.main;
                
                position.x += TILE_STEP;
            }
            
            position.y += TILE_STEP;
            position.x  = 0;
        }
        
        // Set the camera so that the whole terrain is in view. START
        Vector3 top_left_corner    = new Vector3(-TILE_STEP * 0.5f, -TILE_STEP * 0.5f, 0);
        Vector3 bottom_left_corner = new Vector3(top_left_corner.x + (float)width * TILE_STEP, top_left_corner.y + (float)height * TILE_STEP, 0);
        
        Transform camera_transform = Camera.main.GetComponent<Transform>();
        camera_transform.position = new Vector3(
            top_left_corner.x + (bottom_left_corner.x - top_left_corner.x) * 0.5f,
            top_left_corner.y + (bottom_left_corner.y - top_left_corner.y) * 0.5f,
            camera_transform.position.z
        );
        // Set the camera so that the whole terrain is in view. END
    }

    // Update is called once per frame
    void Update()
    {
    }
}
