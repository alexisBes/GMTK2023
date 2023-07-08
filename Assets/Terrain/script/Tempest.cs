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
    }

    private void OnTriggerEnter(Collider other)
    {
        Tile targetTile = other.gameObject.GetComponent<Tile>();
        if (targetTile != null)
        {
            if (targetTile.mix_flags == 0x04)
            {
                // si c'est en cours de transformation
                Debug.Log("dead goblin");
                targetTile.AddNewTile(targetTile.flags);
            }
            else if (targetTile.flags == 0x04)
            {
                Debug.Log("Dead catto");
                // si c'est une ville
                Destroy(gameObject);
            }

        }
    }
}
