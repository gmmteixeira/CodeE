using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public ScriptableRendererFeature deathEffectFeature;
    public Material psxMaterial;
    public Canvas settingsCanvas;
    private AsyncOperation asyncLoad;

    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    IEnumerator LoadSceneCoroutine(string sceneName)
    {
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Optional: Don't let the scene activate immediately
        asyncLoad.allowSceneActivation = false;

        // Wait until the load is almost done
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    void Start()
    {
        settingsCanvas.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        deathEffectFeature.SetActive(false);
        LoadSceneAsync("GameScene");
    }
    public void PlayGame()
    {
        if (asyncLoad != null && asyncLoad.isDone)
        {
            asyncLoad.allowSceneActivation = true;
        }
        else
        {
            SceneManager.LoadScene("GameScene");
        }
    }
    public void OpenSettings()
    {
        settingsCanvas.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    public void QuitGame()
    {
        
        Application.Quit();
    }
}
