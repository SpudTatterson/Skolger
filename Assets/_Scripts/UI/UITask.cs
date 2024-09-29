using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITask : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image image;
    [SerializeField] Sprite completedSprite;
    [SerializeField] Color completedColor = Color.gray;

    public string taskName { get; private set; }

    public void UpdateText(string text)
    {
        this.text.text = text;
        taskName = text;
        UITaskManager.Subscribe(this);
    }

    public void Complete()
    {
        image.sprite = completedSprite;
        text.color = completedColor;
        text.fontStyle = FontStyles.Strikethrough;
    }

    void OnDisable()
    {
        UITaskManager.UnSubscribe(this);
    }
}

public class UITaskManager
{
    static List<UITask> tasks = new List<UITask>();
    static Dictionary<string, UITask> taskNames = new Dictionary<string, UITask>();

    static public void Subscribe(UITask task)
    {
        if (!tasks.Contains(task))
        {
            tasks.Add(task);
            taskNames.Add(task.taskName, task);
        }
    }
    static public void UnSubscribe(UITask task)
    {
        tasks.Remove(task);
        taskNames.Remove(task.taskName);
    }

    static public UITask GetTask(string taskName)
    {
        Debug.Log(taskNames.Count);
        if (taskNames.ContainsKey(taskName))
            return taskNames[taskName];
        throw new System.Exception("Task not found " + taskName);
    }
}