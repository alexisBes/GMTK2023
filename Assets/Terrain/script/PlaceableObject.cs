using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlaceableObject : MonoBehaviour
{
    [SerializeField]
    List<GameObject> objects = new List<GameObject>();

    public GameObject main;

    public Addterain addterain;

    public int width = 8;
    public int height = 7;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 position= new Vector3(0,0,0);
        for (int i = 0;i < width; i++) 
        {
            for(int j = 0;j < height; j++)
            {
                GameObject game =  Instantiate(main, position, main.transform.rotation);
                Addterain terrain = game.AddComponent<Addterain>();
                terrain.flags = 0;
                PlayerInput pi = game.GetComponent<PlayerInput>();
                pi.camera = Camera.main;
                objects.Add(game);
                position.x += 2;
            }
            position.z+= 2;
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
        Assert.AreEqual(1,0);
        return -1;
    }
}
