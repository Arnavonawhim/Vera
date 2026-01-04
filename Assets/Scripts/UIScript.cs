using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIScript : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mainMenuUI;      // The panel with Start/Settings/Quit buttons
    public GameObject loadingScreenUI; // The panel with your Slider
    public Slider progressBar;
    public void StartGame(string sceneName)
    {
        StartCoroutine(LoadAsynchronously(sceneName));
    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        
        // This prevents the scene from switching automatically when it's done
        operation.allowSceneActivation = false;

        mainMenuUI.SetActive(false);
        loadingScreenUI.SetActive(true);

        float targetProgress = 0f;

        
        while (progressBar.value < 1f)
        {
            
            targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            
            progressBar.value = Mathf.MoveTowards(progressBar.value, targetProgress, 0.5f * Time.deltaTime);

            
            if (progressBar.value >= 0.99f && operation.progress >= 0.9f)
            {
                
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    public void Settings()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    public void QuitGame()
    {
        Debug.Log("Game is exiting...");
        Application.Quit();
    }
}
