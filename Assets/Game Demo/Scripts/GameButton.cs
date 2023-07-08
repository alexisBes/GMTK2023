using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
public class GameButton : MonoBehaviour
{
    private void Awake()
    {
        EnableButtons();
    }

    public void EnableButtons()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("Watter").clicked += WaterButtonClicked;
        root.Q<Button>("Air").clicked += AirButtonClicked;
        root.Q<Button>("Land").clicked += LandButtonClicked;
    }

    public void DisableButtons()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("Watter").clicked -= WaterButtonClicked;
        root.Q<Button>("Air").clicked -= AirButtonClicked;
        root.Q<Button>("Land").clicked -= LandButtonClicked;
    }

    public void WaterButtonClicked()
    {
        Debug.Log("Water Clicked");
    }

    public void AirButtonClicked()
    {
        Debug.Log("Air Clicked");
    }

    public void LandButtonClicked()
    {
        Debug.Log("Land Clicked");
    }
}

