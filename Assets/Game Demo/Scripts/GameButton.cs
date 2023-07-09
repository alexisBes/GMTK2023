using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
public class GameButton : MonoBehaviour
{
    public AudioSource audioSource;
    
    public Our_Terrain terrain;

    private void Awake()
    {
        EnableButtons();
    }
    private void Update() {
        //slider = uiDocument.rootVisualElement.Q<Slider>("slider");
        //slider.value = tile.bot_score;
    }
    public void EnableButtons()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("Watter").clicked += WaterButtonClicked;
        root.Q<Button>("Air").clicked += AirButtonClicked;
        root.Q<Button>("Land").clicked += LandButtonClicked;
        root.Q<Button>("Exit").clicked += ExitButtonClicked;
    }

    public void DisableButtons()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("Watter").clicked -= WaterButtonClicked;
        root.Q<Button>("Air").clicked -= AirButtonClicked;
        root.Q<Button>("Land").clicked -= LandButtonClicked;
        root.Q<Button>("Exit").clicked -= ExitButtonClicked;
        
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

    public void ExitButtonClicked()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnEscape()
    {
        SceneManager.LoadScene("Menu");
    }
}

