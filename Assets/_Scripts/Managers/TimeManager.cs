using System;
using System.Collections;
using System.Collections.Generic;
using Skolger.UI.Tabs;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    [SerializeField] TabGroup timeManagerTabGroup;
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            timeManagerTabGroup.TriggerTab(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            timeManagerTabGroup.TriggerTab(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            timeManagerTabGroup.TriggerTab(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            timeManagerTabGroup.TriggerTab(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetNewTimeScale(5);
        }
    }
    public void SetNewTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
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
