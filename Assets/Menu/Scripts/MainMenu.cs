using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    VisualElement root;
    
    private void Awake() {
        root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("Start").clicked += StartButtonClicked;
        root.Q<Button>("Tuto").clicked += TutoButtonClicked;
        root.Q<Button>("Credit").clicked += CreditButtonClicked;
        root.Q<Button>("Quit").clicked += QuitButtonClicked;
    }


    public void StartButtonClicked()
    {
        SceneManager.LoadScene("Annimation");
    }
    
    public void TutoButtonClicked()
    {
        SceneManager.LoadScene("Tuto");
    }

    public void CreditButtonClicked()
    {
        SceneManager.LoadScene("Thanks");
    }

    void QuitButtonClicked()
    {
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
