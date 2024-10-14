using Skolger.UI.Tabs;
using UnityEngine;
using UnityEngine.Events;

public class PauseManager : MonoSingleton<PauseManager>
{
    [SerializeField] TabGroup timeControlTabGroup;
    [SerializeField] Skolger.UI.Tabs.TabButton regularSpeedTab;
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
        if (timeScale != 0 && paused)
            UnPause();
        Time.timeScale = timeScale;
    }
    public void TogglePause()
    {
        if (paused)
        {
            UnPause();
            timeControlTabGroup?.OnTabSelected(regularSpeedTab);
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
