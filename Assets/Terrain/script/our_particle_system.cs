using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public enum Particle_System_Type : byte
{
    Smoke,
    Rumble,
}

public class Our_Particle_System : MonoBehaviour
{
    public float radius;
    
    public GameObject base_object;
    public float lifetime_seconds;
    public int num_particles;
    public float lowest_speed;
    public float highest_speed;
    
    public Particle_System_Type type;
    
    float life_start;
    
    List<GameObject> particles;
    List<float> particles_speed;
    List<float> particles_offset;
    List<float> particles_theta;
    
    void Start()
    {
        particles       = new List<GameObject>(num_particles);
        particles_speed = new List<float>(num_particles);
        
        life_start = Time.timeSinceLevelLoad;
        
        // Spawn particles. START
        Transform t = GetComponent<Transform>();
        
        for(int i = 0; i < num_particles; i++)
        {
            Vector3 p = t.position;
            p.x = Random.Range(p.x - radius, p.x + radius);
            p.z = Random.Range(p.z - radius, p.z + radius);
            
            particles_speed.Add(Random.Range(lowest_speed, highest_speed));
            particles_offset.Add(Random.Range(-radius, radius));
            particles_theta.Add(0);
            
            GameObject particle = null;
            
            if(type == Particle_System_Type.Smoke)       particle = Instantiate(base_object, p, Quaternion.Euler(Random.Range(0, 360.0f), Random.Range(0, 360.0f), Random.Range(0, 360.0f)));
            else if(type == Particle_System_Type.Rumble) particle = Instantiate(base_object, t.position, Quaternion.Euler(Random.Range(0, 360.0f), Random.Range(0, 360.0f), Random.Range(0, 360.0f)));
            
            particles.Add(particle);
            
            Transform pt = particle.GetComponent<Transform>();
            pt.localScale = new Vector3(0, 0, 0);
        }
        // Spawn particles. END
    }
    
    void Update()
    {
        float life = Time.timeSinceLevelLoad - life_start;
        if(life > lifetime_seconds)
        {
            Destroy(gameObject);
            return;
        }
        
        
        if(type == Particle_System_Type.Smoke)
        {
            for(int i = 0; i < num_particles; i++)
            {
                GameObject particle = particles[i];
                
                float particle_speed = Time.deltaTime * particles_speed[i];
                
                Transform t = particle.GetComponent<Transform>();
                t.position = t.position + new Vector3(0, particle_speed, 0);
                
                float scale_factor = life / lifetime_seconds;
                t.localScale = new Vector3(scale_factor * 2.0f, scale_factor * 2.0f, scale_factor * 2.0f);
            }
        }
        else if(type == Particle_System_Type.Rumble)
        {
            Transform parent_transform = GetComponent<Transform>();
            
            for(int i = 0; i < num_particles; i++)
            {
                GameObject particle = particles[i];
                
                float particle_speed = Time.deltaTime * particles_speed[i];
                
                float theta = particles_theta[i];
                particles_theta[i] += particle_speed;
                
                Transform t = particle.GetComponent<Transform>();
                t.position = parent_transform.position + new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta)) * particles_offset[i];
                
                float scale_factor = life / lifetime_seconds;
                t.localScale = new Vector3(scale_factor * 2.0f, scale_factor * 2.0f, scale_factor * 2.0f);
            }
        }
    }
}