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
    private const int SAND_TILE  = 0x03;
    private const int TOWN_TILE  = 0x04;

    public int flags = 0;

    public List<Material> materials;
    
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Renderer>().material = materials[flags];
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    void OnClickedTerrain()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {            
            if (hit.collider.GetInstanceID() != this.GetComponent<Collider>().GetInstanceID()) return;
            
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
            this.GetComponent<Renderer>().material = materials[flags];
        }
    }
}
