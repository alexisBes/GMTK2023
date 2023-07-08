using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class Terrain : MonoBehaviour
{
    [SerializeField]
    List<GameObject> objects = new List<GameObject>();

    public GameObject tile_prefab;

    public Material material;

    public int width = 8;
    public int height = 7;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 position = new Vector3(0, 0, 0);
        int centreW = (int)(Random.value * width);
        int centreH = (int)(Random.value * height);
        if (centreW == 0) centreW++;
        if (centreH == 0) centreH++;
        if (centreW == width) centreW--;
        if (centreH == height) centreH--;
        Debug.Log("X : " + centreW + ", Y : " + centreH);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int newFlags = 0x00;
                if ((i >= centreW-1 && i <= centreW+1) && (j <= centreH +1  && j >= centreH -1))
                    newFlags = Random.Range(1, 0x05);
                if (i == centreW && j == centreH) { newFlags = State.SPAWN_TOWN; }
                GameObject game = Instantiate(tile_prefab, position, tile_prefab.transform.rotation);
                Tile terrain = game.GetComponent<Tile>();
                terrain.flags = newFlags;

                PlayerInput pi = game.GetComponent<PlayerInput>();
                pi.camera = Camera.main;
                if(newFlags != 0x00)
                {
                    game.SetActive(false);
                }
                objects.Add(game);
                position.x += 1;
            }
            position.z += 1;
            position.x = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int GetIndex(int uid)
    {
        int index = 0;
        foreach (GameObject obj in objects)
        {
            if (obj.GetInstanceID() == uid)
            {
                return index;
            }
            index++;
        }
        Assert.AreEqual(1, 0);
        return -1;
    }
}
