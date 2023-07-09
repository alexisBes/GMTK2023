using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UpdateSliderValue : MonoBehaviour
{
    public Our_Terrain terrain;

    public UIDocument uiDocument;  // Reference to the UI document

    private Slider slider;  // Reference to the Slider component

    void Update()
    {
        if (slider.value == 100) {
            SceneManager.LoadScene("Lost");
        }
    }

    public float increaseAmount = 0.1f;
    public float increaseInterval = 1f;

    private void Start()
    {
        slider = uiDocument.rootVisualElement.Q<Slider>("slider");
        StartCoroutine(IncreaseSliderValue());
    }

    private System.Collections.IEnumerator IncreaseSliderValue()
    {
        while (true)
        {
            slider.value += increaseAmount;
            yield return new WaitForSeconds(increaseInterval);
        }
    }
}
