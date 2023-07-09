using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static TurnBasedSystem;

public class Tempest : MonoBehaviour
{
    public Vector3 target;
    [SerializeField]
    private float speed = 0.8f;
    
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
        if (Vector3.Distance(t.position, target) < 0.001f)
        {
            there_is_an_active_tornado = false;
            
            CameraShaker.Invoke();
            // On casse tout.
            Destroy(gameObject);
        }
    }
}
