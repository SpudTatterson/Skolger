using System;
using System.Collections;
using System.Collections.Generic;
using Skolger.UI.Tabs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class PauseManager : MonoSingleton<PauseManager>
{
    public UnityEvent OnPause;
    public UnityEvent OnResume;
    public UnityEvent OnPauseMenu;
    public UnityEvent OnResumeMenu;
    bool paused;
    bool pauseMenuOpen = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetNewTimeScale(10);
        }
    }
    public void SetNewTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }
    public void TogglePause()
    {
        if (paused)
        {
            UnPause();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        OnPause?.Invoke();
        Time.timeScale = 0;
        paused = true;
    }

    public void UnPause()
    {
        OnResume?.Invoke();
        Time.timeScale = 1;
        paused = false;
    }
    public void TogglePauseMenu()
    {
        if (pauseMenuOpen)
        {
            UnPause();
            OnResumeMenu?.Invoke();
        }
        else
        {
            Pause();
            OnPauseMenu?.Invoke();
        }
        pauseMenuOpen = !pauseMenuOpen;
    }

    public void LoadScene(int index)
    {
        SceneUtility.LoadScene(index);
    }
    public void Exit()
    {
        SceneUtility.Exit();
    }
    public void Restart()
    {
        SceneUtility.RestartScene();
    }
}
