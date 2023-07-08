using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementIA : MonoBehaviour
{
    int width = 16;
    int height = 16;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveRandomly();
    }

    public void MoveRandomly()
    {
        int startX = UnityEngine.Random.Range(0, width);
        int startY = UnityEngine.Random.Range(0, height);

        int currentX = startX;
        int currentY = startY;

        while (true)
        {
            int randomDirection = UnityEngine.Random.Range(0, 4); // 0: Up, 1: Down, 2: Left, 3: Right

            switch (randomDirection)
            {
                case 0: // Up
                    currentY++;
                    break;
                case 1: // Down
                    currentY--;
                    break;
                case 2: // Left
                    currentX--;
                    break;
                case 3: // Right
                    currentX++;
                    break;
            }
                Debug.Log("Moving to: X: " + currentX + ", Y: " + currentY);
                break;
        }
    }
}
