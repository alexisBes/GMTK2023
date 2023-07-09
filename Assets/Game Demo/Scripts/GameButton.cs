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

        Button button = root.Q<Button>("Watter");
        toolTexte(root.Q<Button>("Watter"), "Watter");
        root.Q<Button>("Watter").clicked += WaterButtonClicked;
        toolTexte(root.Q<Button>("Air"), "Sand");
        root.Q<Button>("Air").clicked    += AirButtonClicked;
        toolTexte(root.Q<Button>("Land"), "Land");
        root.Q<Button>("Land").clicked   += LandButtonClicked;
        toolTexte(root.Q<Button>("Exit"), "Exit");
        root.Q<Button>("Exit").clicked   += ExitButtonClicked;
        toolTexte(root.Q<Button>("Tempest"), "Tempest");
    }

    public void DisableButtons()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("Watter").clicked -= WaterButtonClicked;
        root.Q<Button>("Air").clicked    -= AirButtonClicked;
        root.Q<Button>("Land").clicked   -= LandButtonClicked;
        root.Q<Button>("Exit").clicked   -= ExitButtonClicked;
        
    }

    public void WaterButtonClicked()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
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

    private void toolTexte(Button button, string txt)
    {
        Label tooltipText = new Label();
        tooltipText.style.display = DisplayStyle.None;
        tooltipText.style.backgroundColor = new StyleColor(Color.black);
        tooltipText.style.color = new StyleColor(Color.white);
        tooltipText.style.alignSelf = Align.Center;  // Set the text alignment to center
        tooltipText.style.fontSize = 20;
        button.Add(tooltipText);

        button.RegisterCallback<MouseEnterEvent>((evt) =>
        {
            tooltipText.style.display = DisplayStyle.Flex;
            tooltipText.text = txt;
        });

        button.RegisterCallback<MouseLeaveEvent>((evt) =>
        {
            tooltipText.style.display = DisplayStyle.None;
        });
    }
}

