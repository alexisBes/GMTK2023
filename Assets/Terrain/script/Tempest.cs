using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tempest : MonoBehaviour
{
    public Transform target;
    [SerializeField]
    private float speed = 0.8f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(gameObject.transform.position, target.position, speed * Time.deltaTime);
        // Check if the position of the cube and sphere are approximately equal.
        if (Vector3.Distance(transform.position, target.position) < 0.001f)
        {
            // on casse tout
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
    }
}
