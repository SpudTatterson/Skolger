using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject newCanvas; // Assign the new canvas in the inspector

    // Call this method to start a new scene
    public void StartNewScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Call this method to open a new canvas
    public void OpenNewCanvas()
    {
        if (newCanvas != null)
        {
            newCanvas.SetActive(true);
        }
    }

    public void CloseNewCanvas()
    {
        if (newCanvas != null)
        {
            newCanvas.SetActive(false);
        }
    }

    // Call this method to quit the game
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor
#else
            Application.Quit(); // Quit the application
#endif
    }
}
