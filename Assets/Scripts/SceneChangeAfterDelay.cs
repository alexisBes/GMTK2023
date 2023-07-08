using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeAfterDelay : MonoBehaviour
{
    public string sceneName;
    public float delay = 5f;

    private float timer = 0f;
    private bool sceneLoaded = false;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= delay && !sceneLoaded)
        {
            sceneLoaded = true;
            ChangeScene();
        }
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
