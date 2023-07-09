using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using static TurnBasedSystem;

public class Tempest : MonoBehaviour
{
    public Vector3 target;
    [SerializeField]
    private float speed = 0.8f;
    //public GameObject row;
    void Start()
    {
        AudioSource audio_source = GetComponent<AudioSource>();
        audio_source.Play();
    }

    void Update()
    {
        Transform t = GetComponent<Transform>();
        
        t.position = Vector3.MoveTowards(t.position, target, speed * Time.deltaTime);
        // Check if the position of the cube and sphere are approximately equal.
        //Instantiate(row);
        
        Vector2 storm_position_2d  = new Vector2(t.position.x, t.position.z);
        Vector2 target_position_2d = new Vector2(target.x, target.z);
        
        if (Vector2.Distance(storm_position_2d, target_position_2d) < 0.001f)
        {
            there_is_an_active_tornado = false;
            
            CameraShaker.Invoke();
            // On casse tout.
            Destroy(gameObject);
        }
    }
}