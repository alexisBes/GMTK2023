using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UpdateSliderValue : MonoBehaviour
{
    public Our_Terrain terrain;

    public UIDocument uiDocument;  // Reference to the UI document

    private Slider slider;  // Reference to the Slider component


    // Start is called before the first frame update
    void Start()
    {
        slider = uiDocument.rootVisualElement.Q<Slider>("slider");   
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("here");
        Tile tile = terrain.GetComponent<Tile>();
        slider.value += (tile.player_score + 20);
    }
}
