using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    private const int WATER_TILE = 0x01;
    private const int LAND_TITLE = 0x02;
    private const int SAND_TILE = 0x03;
    private const int TOWN_TILE = 0x04;

    private const int NO_MIX_TILE = 0x00;
    private const int SWAMP_TILE = 0x01;
    private const int QUICKSAND_TILE = 0x02;
    private const int DUNE_TILE = 0x03;
    private const int SUBURG_TILE = 0x04;


    public int flags = 0;
    public int mix_flags = 0;
    public int x, y;
    public Our_Terrain our_terrain;

    public List<GameObject> prefabs;
    public GameObject tempestPrefab;

    private GameObject currentPrefab;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Asset type :" + flags);
        SetPrefab(flags);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void SetPrefab(int position)
    {
        Destroy(currentPrefab);

        currentPrefab = Instantiate(prefabs[position], transform.position, Quaternion.Euler(75f, 0f, 0f));
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
                if (State.state != State.SPAWN_TEMPEST)
                {
                    MixTile(State.state);
                    return;
                }
                else
                {
                    if (this.flags != 4)
                    {
                        Debug.Log("Setting target");
                        State.originTile = this;
                        return;
                    }
                    else
                    {
                        LaunchTempest();
                        return;
                    }
                }
                
            }

            if (!this.our_terrain.isACaseValid(x, y))
                return;

            AddNewTile(State.state);
        }
    }
    public void AddNewTile(int state)
    {
        Debug.Log("Oi!"); // @ DEBUG.

        if (state == State.SPAWN_WATER)
        {
            flags = WATER_TILE;
        }
        else if (state == State.SPAWN_SAND)
        {
            flags = SAND_TILE;
        }
        else if (state == State.SPAWN_LAND)
        {
            flags = LAND_TITLE;
        }
        else if (state == State.SPAWN_TOWN)
        {
            flags = TOWN_TILE;
        }

        SetPrefab(flags);
    }

    private void MixTile(int state)
    {
        Debug.Log("Mixing");
        if (mix_flags == NO_MIX_TILE)
        {
            if (flags == WATER_TILE && state == State.SPAWN_LAND || flags == LAND_TITLE && state == State.SPAWN_WATER)
            {
                mix_flags = SWAMP_TILE;
            }
            else if (flags == WATER_TILE && state == State.SPAWN_SAND || flags == SAND_TILE && state == State.SPAWN_WATER)
            {
                mix_flags = QUICKSAND_TILE;
            }
            else if (flags == LAND_TITLE && state == State.SPAWN_SAND || flags == SAND_TILE && state == State.SPAWN_LAND)
            {
                mix_flags = DUNE_TILE;
            }
            else if (flags != 0x00 && state == State.SPAWN_TOWN)
            {
                if (flags == TOWN_TILE)
                {
                    flags = TOWN_TILE;
                    SetPrefab(flags);
                    return;
                }
                else mix_flags = SUBURG_TILE;
            }
            if (mix_flags != NO_MIX_TILE)
            {
                SetPrefab(TOWN_TILE + mix_flags);
            }
        }
        return;
    }

    private void LaunchTempest()
    {
        Debug.Log("FULL PAWAR");
        Vector3 vector = State.originTile.gameObject.transform.position;
        vector.z += 6;
        currentPrefab = Instantiate(tempestPrefab, vector, Quaternion.Euler(90f, 0f, 0f));
        Tempest tempest =  currentPrefab.GetComponentInChildren<Tempest>();
        tempest.target = gameObject.transform;
    }

}