using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tile : MonoBehaviour
{
    private const int WATER_TILE = 0x01;
    private const int LAND_TITLE = 0x02;
    private const int SAND_TILE = 0x03;
    private const int TOWN_TILE = 0x04;

    public int flags = 0;
    public int x, y;
    public Our_Terrain our_terrain;

    public List<GameObject> prefabs;

    private GameObject currentPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Asset type :" + flags);
        SetPrefab();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void SetPrefab()
    {
        Destroy(currentPrefab);

        currentPrefab = Instantiate(prefabs[flags], transform.position, Quaternion.Euler(90f, 0f, 0f));
        currentPrefab.transform.parent = transform;
    }

    void OnClickedTerrain()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.GetInstanceID() != this.GetComponent<Collider>().GetInstanceID()) return;

            if (this.flags != 0)
            {
                return;
            }

            if (!this.our_terrain.isACaseValid(x, y))
                return;

            Debug.Log("Oi!"); // @ DEBUG.

            int what_we_should_do_on_this_tile = State.state;

            if (what_we_should_do_on_this_tile == State.SPAWN_WATER)
            {
                flags = WATER_TILE;
            }
            else if (what_we_should_do_on_this_tile == State.SPAWN_SAND)
            {
                flags = SAND_TILE;
            }
            else if (what_we_should_do_on_this_tile == State.SPAWN_LAND)
            {
                flags = LAND_TITLE;
            }
            else if (what_we_should_do_on_this_tile == State.SPAWN_TOWN)
            {
                flags = TOWN_TILE;
            }
            
            SetPrefab();
        }
    }
}
