using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SceneUtility : MonoBehaviour
{
    [SerializeField] UnityEvent StartEvents;
    void Start()
    {
        StartEvents.Invoke();
    }
    public void RestartScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadScene(int buildIndex)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(buildIndex);
    }
    public void Exit()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
        #endif
    }
    public void NextScene()
    {
        Time.timeScale = 1;
        LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
