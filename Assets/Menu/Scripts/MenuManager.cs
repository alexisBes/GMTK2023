using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public AudioSource audioSource;

    public void OnMenuClick()
    {
        StartCoroutine(PlaySoundAndWait());
    }
      private IEnumerator PlaySoundAndWait()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Menu");
    }
}
