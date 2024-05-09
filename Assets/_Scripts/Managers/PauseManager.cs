using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseManager : MonoBehaviour
{
    public UnityEvent OnPause;
    public UnityEvent OnResume;
    bool paused;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Toggle();
        }
    }

    void Toggle()
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

}
