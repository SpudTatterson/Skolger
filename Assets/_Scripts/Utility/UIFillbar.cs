using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFillbar : MonoBehaviour
{
    [SerializeField] Image fillbar;
    [SerializeField] TextMeshProUGUI currentValueText;
    [SerializeField] TextMeshProUGUI maxValueText;

    float maxValue;

    public void Initialize(float currentValue, float maxValue)
    {
        this.maxValue = maxValue;
        currentValueText.text = currentValue.ToString();
        maxValueText.text = maxValue.ToString();
        UpdateBar(currentValue);
    }
    
    public void UpdateBar(float value)
    {
        fillbar.fillAmount =  Mathf.InverseLerp(0, maxValue, value);
    }
}
