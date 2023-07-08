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
            if ((targetTile.flags & Tile.SUBURB_TILE) != 0)
            {
                // Si c'est en cours de transformation.
                Debug.Log("dead goblin");
                targetTile.flags &= ~Tile.SUBURB_TILE;
            }
            else if ((targetTile.flags & Tile.TOWN_TILE) != 0)
            {
                Debug.Log("Dead catto");
                // Si c'est une ville.
                Destroy(gameObject);
            }

        }
    }
}
