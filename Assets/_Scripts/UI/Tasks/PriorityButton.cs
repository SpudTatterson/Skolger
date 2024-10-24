using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PriorityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    enum TaskType
    {
        Haul,
        Harvest,
        Construct
    }
    ColonistData colonist;
    [SerializeField] TaskType taskType;
    [SerializeField] int maxPriority = 5;
    int priority;
    [SerializeField] TextMeshProUGUI text;
    Image image;


    [Header("Visual Settings")]
    [SerializeField] Color idleColor = new Color(0.9f, 0.9f, 0.9f);
    [SerializeField] Color hoverColor = Color.white;
    [SerializeField] Color clickColor = new Color(0.7f, 0.7f, 0.7f);


    void Awake()
    {
        image = GetComponent<Image>();
        image.color = idleColor;
    }
    int GetRelevantTaskPriority()
    {
        switch (taskType)
        {
            case TaskType.Haul:
                return colonist.brain.taskHaul;
            case TaskType.Harvest:
                return colonist.brain.taskHarvest;
            case TaskType.Construct:
                return colonist.brain.taskConstruct;
        }
        throw new System.NotImplementedException();
    }

    void SetRelevantTaskPriority(int priority)
    {
        switch (taskType)
        {
            case TaskType.Haul:
                colonist.brain.taskHaul = priority;
                return;
            case TaskType.Harvest:
                colonist.brain.taskHarvest = priority;
                return;
            case TaskType.Construct:
                colonist.brain.taskConstruct = priority;
                return;
        }
        throw new System.NotImplementedException();
    }

    public void Initialize(ColonistData colonist)
    {
        this.colonist = colonist;
        priority = GetRelevantTaskPriority();
        text.text = priority.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = idleColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        image.color = clickColor;
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                RaisePriority();
                break;
            case PointerEventData.InputButton.Right:
                LowerPriority();
                break;
        }
    }

    void RaisePriority()
    {
        priority++;
        priority %= maxPriority;
        UpdatePriority();
    }

    void UpdatePriority()
    {
        SetRelevantTaskPriority(priority);
        text.text = priority.ToString();
        colonist.brain.rearrangeTree = true;
    }

    void LowerPriority()
    {
        priority--;
        if (priority < 0)
        {
            priority = maxPriority - 1;
        }
        UpdatePriority();
    }

}

