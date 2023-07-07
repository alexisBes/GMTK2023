using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music_Player : MonoBehaviour
{
    public AudioClip[] tracks = new AudioClip[] {};
    [System.NonSerialized] public int current_track_index;
    [System.NonSerialized] public AudioSource audio_source;
    [System.NonSerialized] public float time_elapsed_since_we_stopped_playing_the_last_track;
    public float time_between_tracks_seconds = 5;
    
    
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // NOTE: the goal is to play all the tracks forever unti we exit the game. We want to wait for a little bit between each track for more smoothness.
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    
    void Start()
    {
        audio_source = GetComponent<AudioSource>();
        
        // Start playing the first track. START
        current_track_index = 0;
        time_elapsed_since_we_stopped_playing_the_last_track = 0;
        
        if(tracks.Length > 0)
        {
            audio_source.clip = tracks[0];
            audio_source.Play(0);
        }
        // Start playing the first track. END
    }

    void Update()
    {
        if(audio_source.isPlaying == false)
        {
            time_elapsed_since_we_stopped_playing_the_last_track += Time.deltaTime;
            
            if(time_elapsed_since_we_stopped_playing_the_last_track > time_between_tracks_seconds)
            {
                // Play the next track. START
                time_elapsed_since_we_stopped_playing_the_last_track = 0;
                
                current_track_index++;
                current_track_index %= tracks.Length;
                
                audio_source.clip = tracks[current_track_index];
                audio_source.Play(0);
                // Play the next track. END
            }
        }
    }
}
