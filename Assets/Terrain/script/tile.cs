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
    private const int SAND_TILE = 0x04;
    private const int TOWN_TILE= 0x08;

    public int flags = 0;
    
    // Start is called before the first frame update
    void Start()
    {
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
            
            int newState = State.state;
            if (newState == State.SPAWN_WATER)
            {
                flags = WATER_TILE;
            }
            else if (newState == State.SPAWN_SAND)
            {
                flags = SAND_TILE;
            }
            else if (newState == State.SPAWN_LAND)
            {
                flags = LAND_TITLE;
            }
            else if (newState == State.SPAWN_TOWN)
            {
                flags = TOWN_TILE;
            }
        }
    }
}
