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

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Time.timeScale = 2;
            Debug.Log(Time.timeScale);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale = 1.5f;
            Debug.Log(Time.timeScale);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale = 1;
            Debug.Log(Time.timeScale);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Time.timeScale = 5;
            Debug.Log(Time.timeScale);
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
