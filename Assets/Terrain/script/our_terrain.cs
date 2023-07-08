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

    Tile[,] tiles = new Tile[width, height];
    // Start is called before the first frame update
    void Start()
    {
        Vector3 position = new Vector3(0, 0, 0);
        int centreW = (int)(UnityEngine.Random.value * width);
        int centreH = (int)(UnityEngine.Random.value * height);
        if (centreW == 0) centreW++;
        if (centreH == 0) centreH++;
        if (centreW == width) centreW--;
        if (centreH == height) centreH--;
        Debug.Log("X : " + centreW + ", Y : " + centreH);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int newFlags = 0x00;
                if ((x >= centreW - 1 && x <= centreW + 1) && (y <= centreH + 1 && y >= centreH - 1))
                    newFlags = UnityEngine.Random.Range(1, 0x04);
                if (y == centreH && x == centreW) { newFlags = State.SPAWN_TOWN; }
                GameObject tile = Instantiate(tile_prefab, position, tile_prefab.transform.rotation);
                Tile terrain = tile.GetComponent<Tile>();
                terrain.flags = newFlags;
                terrain.our_terrain = this;
                terrain.x = x; terrain.y = y;

                PlayerInput pi = tile.GetComponent<PlayerInput>();
                pi.camera = Camera.main;
                position.x += TILE_STEP;
                tiles[x, y] = terrain;
            }
            position.y += TILE_STEP;
            position.x = 0;
        }

        // Set the camera so that the whole terrain is in view. START
        Vector3 top_left_corner = new Vector3(-TILE_STEP * 0.5f, -TILE_STEP * 0.5f, 0);
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

    public bool isACaseValid(int x, int y)
    {
        Debug.Log("X: " + x + ", Y:" + y);
        if (x >= 0 && tiles[x - 1, y].flags != 0x00)
            return true;
        else if (x < width && tiles[x + 1, y].flags != 0x00)
            return true;
        else if (y >= 0 && tiles[x, y - 1].flags != 0x00)
            return true;
        else if (y < height && tiles[x, y + 1].flags != 0x00)
            return true;
        else return false;
    }
}
