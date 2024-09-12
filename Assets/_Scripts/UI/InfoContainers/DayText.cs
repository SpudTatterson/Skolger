using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayText : MonoBehaviour
{
    int day;
    TextMeshProUGUI text;
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        DayNightEventManager.Instance.GetEvent(0).AddListener(IncrementDay);
        text.text = $"Day {day}";
    }

    void IncrementDay()
    {
        day++;
        text.text = $"Day {day}";
    }
}
