using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelaunchAnimation : MonoBehaviour
{
    public string nameANimation;
    public Animation anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!anim.isPlaying)
        {
            anim.Play(nameANimation);
        }
    }
}
