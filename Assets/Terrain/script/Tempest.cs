using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using static TurnBasedSystem;

public class Tempest : MonoBehaviour
{
    public Vector3 target;
    Vector2 start_position;
    
    float distance_to_do;
    
    [SerializeField]
    private float speed = 0.8f;
    //public GameObject row;
    
    void Start()
    {
        AudioSource audio_source = GetComponent<AudioSource>();
        audio_source.volume = 0;
        audio_source.Play();
        
        Transform t = GetComponent<Transform>();
        
        start_position = new Vector2(t.position.x, t.position.z);
        Vector2 target_position_2d = new Vector2(target.x, target.z);
        
        distance_to_do = Vector2.Distance(start_position, target_position_2d);
    }

    void Update()
    {
        Transform t = GetComponent<Transform>();
        
        t.position = Vector3.MoveTowards(t.position, target, speed * Time.deltaTime);
        // Check if the position of the cube and sphere are approximately equal.
        //Instantiate(row);
        
        Vector2 storm_position_2d  = new Vector2(t.position.x, t.position.z);
        Vector2 target_position_2d = new Vector2(target.x, target.z);
        
        float distance_done = Vector2.Distance(start_position, storm_position_2d);
        float distance_ratio = distance_done / distance_to_do;
        
        Debug.Log(distance_ratio);
        
        const float POINT_BEFORE_WHICH_WE_FADE_THE_VOLUME = 0.3f;
        const float POINT_AFTER_WHICH_WE_FADE_THE_VOLUME  = 0.9f;
        
        AudioSource audio_source = GetComponent<AudioSource>();
        
        if(distance_ratio < POINT_BEFORE_WHICH_WE_FADE_THE_VOLUME)
        {
            audio_source.volume = distance_ratio / POINT_BEFORE_WHICH_WE_FADE_THE_VOLUME;
        }
        else if(distance_ratio > POINT_AFTER_WHICH_WE_FADE_THE_VOLUME)
        { // Fade the tornado sound out.
            audio_source.volume = 1.0f - (distance_ratio - POINT_AFTER_WHICH_WE_FADE_THE_VOLUME) / (1.0f - POINT_AFTER_WHICH_WE_FADE_THE_VOLUME);
        }
        
        
        if (Vector2.Distance(storm_position_2d, target_position_2d) < 0.001f)
        {
            there_is_an_active_tornado = false;
            
            CameraShaker.Invoke();
            // On casse tout.
            Destroy(gameObject);
        }
    }
}