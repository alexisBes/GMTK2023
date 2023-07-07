using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
public class GameButton : MonoBehaviour
{
    private void Awake() {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("watter").clicked += WatterButtonClicked;
        root.Q<Button>("air").clicked += AirButtonClicked;
        root.Q<Button>("land").clicked += LandButtonClicked;
    }


    public void WatterButtonClicked()
    {
        Debug.Log("Watter Clicked");
    }

    void AirButtonClicked()
    {
        Debug.Log("Air Clicked");
    }

    void LandButtonClicked()
    {
        Debug.Log("Land Clicked");
    }
}
