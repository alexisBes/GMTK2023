using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
public class GameButton : MonoBehaviour
{
    public AudioSource audioSource;

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
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        Debug.Log("Water Clicked");
        
    }

    public void AirButtonClicked()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        Debug.Log("Air Clicked");
    }

    public void LandButtonClicked()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        Debug.Log("Land Clicked");
    }
}

