using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class RestManger : MonoBehaviour
{
    ColonistData colonist;
    BedBuilding assignedBed;

    const float Max_Rest_Capacity = 100f;
    [BoxGroup("Thresholds"), SerializeField] float wellRestedThreshold = 70;
    [BoxGroup("Thresholds"), SerializeField] float tiredThreshold = 40;
    [BoxGroup("Thresholds"), SerializeField] float sleepDeprivedThreshold = 20;
    [field: SerializeField, ReadOnly] public float RestLevel { get; private set; } = 50;
    [SerializeField] float tirednessSpeed = 0.1f;
    [SerializeField] float restGainSpeed = 0.3f;

    bool sleeping = false;
    float sleepEffectivenessModifier = 1;
    public event Action OnSleep;
    public event Action OnWakeUp;

    RestStatus restStatus;
    public event Action<RestStatus> OnStatusChange;


    void Awake()
    {
        colonist = GetComponent<ColonistData>();
    }
    void Start()
    {
        OnStatusChange?.Invoke(restStatus);
    }

    public void Sleep(float bedQuality)
    {
        sleeping = true;
        sleepEffectivenessModifier = bedQuality;
        OnSleep?.Invoke();
    }
    public void WakeUp()
    {
        sleeping = false;
        OnWakeUp?.Invoke();
    }

    public void UpdateRest()
    {
        if (!sleeping)
            RestLevel -= tirednessSpeed * Time.deltaTime;
        else
            RestLevel += restGainSpeed * Time.deltaTime * sleepEffectivenessModifier;

        RestLevel = Mathf.Clamp(RestLevel, 0, Max_Rest_Capacity);
        RestStatus restStatus = GetRestStatus();
        if (restStatus != this.restStatus)
        {
            HandleStatusChanged(restStatus);
        }
    }
    void HandleStatusChanged(RestStatus restStatus)
    {
        this.restStatus = restStatus;
        OnStatusChange?.Invoke(restStatus);
    }

    RestStatus GetRestStatus()
    {
        if (RestLevel >= wellRestedThreshold)
            return RestStatus.WellRested;
        else if (RestLevel < wellRestedThreshold && RestLevel >= tiredThreshold)
            return RestStatus.Rested;
        else if (RestLevel < tiredThreshold)
            return RestStatus.Tired;
        else if (RestLevel < sleepDeprivedThreshold)
            return RestStatus.SleepDeprived;
        else
            return RestStatus.Insomniac;
    }
}

public enum RestStatus
{
    Insomniac,
    SleepDeprived,
    Tired,
    Rested,
    WellRested

}