using TMPro;
using UnityEngine;

public class TimeText : MonoBehaviour
{
    const int HoursInDay = 24;
    int hour;
    TextMeshProUGUI text;
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        for (int i = 0; i < HoursInDay; i++)
        {
            DayNightEventManager.Instance.GetEvent(i).AddListener(GetTime);
        }
    }

    void GetTime()
    {
        text.text = $"{Mathf.Round(DayNightTimeManager.Instance.TimeOfDay)}h";
    }
}