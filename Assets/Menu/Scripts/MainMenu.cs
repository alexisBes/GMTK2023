using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Awake() {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("Start").clicked += StartButtonClicked;
        root.Q<Button>("Settings").clicked += () => Debug.Log("Settings Clicked");
        root.Q<Button>("Quit").clicked += QuitButtonClicked;
    }


    public void StartButtonClicked()
    {
        SceneManager.LoadScene("GameScene");
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
