using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneUtility
{
    public static void RestartScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public static void LoadScene(int buildIndex)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(buildIndex);
    }
    public static void Exit()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
        #endif
    }
    public static void NextScene()
    {
        Time.timeScale = 1;
        LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
