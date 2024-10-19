using UnityEngine;

public class DayNightTimeManager : MonoSingleton<DayNightTimeManager>
{
    public bool IsDayCycleOn = true;
    public bool RandomStartTime;
    [field: SerializeField, Range(0, 24)] public float TimeOfDay { get; private set; } = 12f;
    [SerializeField, Range(0, 24)] float StartTime = 12f;
    [field: SerializeField, Range(1, 1200)] public float CycleDuration { get; private set; } = 360f;
    public float TimePercent => TimeOfDay / 24f;
    public float adjustedDeltaTime { get; private set; }

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (IsDayCycleOn)
        {
            UpdateTime();
        }
    }
    void LateUpdate()
    {
        adjustedDeltaTime = 0;
    }

    void Initialize()
    {
        if (IsDayCycleOn)
        {
            TimeOfDay = RandomStartTime ? Random.Range(0f, 24f) : StartTime % 24;
        }
    }

    void UpdateTime()
    {
        adjustedDeltaTime = (Time.deltaTime / CycleDuration) * 24f;
        TimeOfDay += adjustedDeltaTime;
        TimeOfDay %= 24;
    }

    public void ToggleDayCycle(bool value)
    {
        IsDayCycleOn = value;
    }
}
