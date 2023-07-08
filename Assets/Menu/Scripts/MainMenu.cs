using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    VisualElement root;
    public AudioSource audioSource;
    
    private void Awake() {
        root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("Start").clicked += StartButtonClicked;
        root.Q<Button>("Tuto").clicked += TutoButtonClicked;
        root.Q<Button>("Credit").clicked += CreditButtonClicked;
        root.Q<Button>("Quit").clicked += QuitButtonClicked;
    }
    
    public void StartButtonClicked()
    {
        StartCoroutine(PlaySoundAndWait());
    }

    private IEnumerator PlaySoundAndWait()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Annimation");
    }
    
    private IEnumerator PlaySoundAndWaitTuto()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Tuto");
    }

    public void TutoButtonClicked()
    {
        StartCoroutine(PlaySoundAndWaitTuto());
    }

    public void CreditButtonClicked()
    {
        SceneManager.LoadScene("Thanks");
    }

    void QuitButtonClicked()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}