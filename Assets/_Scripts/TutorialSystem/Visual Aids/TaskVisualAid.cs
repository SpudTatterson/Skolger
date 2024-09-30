using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Skolger.Tutorial
{
    public class TaskVisualAid : VisualAid
    {
        GameObject taskPrefab;
        RectTransform tasksParent;

        [SerializeField] string taskName;

        UITask task;
        public override void Initialize()
        {
            taskPrefab = UIManager.Instance.taskPrefab;
            tasksParent = UIManager.Instance.taskParent;

            if (taskName == "") throw new System.Exception("taskName is empty");
            task = MonoBehaviour.Instantiate(taskPrefab, tasksParent).GetComponent<UITask>();
            task.UpdateText(taskName);
        }

        public override void Reset()
        {
            MonoBehaviour.Destroy(task.gameObject);
        }

        public override void Update()
        {

        }
    }
    public class TextBoxVisualAid : VisualAid
    {
        [SerializeField] GameObject textPrefab;
        RectTransform tasksParent;

        [SerializeField] string text;

        TextMeshProUGUI textObject;
        public override void Initialize()
        {
            tasksParent = UIManager.Instance.taskParent;

            if (text == "") throw new System.Exception("taskName is empty");
            textObject = MonoBehaviour.Instantiate(textPrefab, tasksParent).GetComponent<TextMeshProUGUI>();
            textObject.text = text;
        }

        public override void Reset()
        {
            MonoBehaviour.Destroy(textObject.gameObject);
        }

        public override void Update()
        {

        }
    }
    public class FinishTaskVisualAid : VisualAid
    {
        [SerializeField] string taskName;
        UITask task;
        public override void Initialize()
        {
            if (taskName == "") throw new System.Exception("taskName is empty");
            task = UITaskManager.GetTask(taskName);
        }

        public override void Reset()
        {
            task.Complete();
        }

        public override void Update()
        {

        }
    }
    public class UIFillBarVisualAid : VisualAid
    {
        GameObject fillbarPrefab;
        RectTransform tasksParent;

        UIFillbar fillbar;

        public override void Initialize()
        {
            fillbarPrefab = UIManager.Instance.fillbarPrefab;
            tasksParent = UIManager.Instance.taskParent;

            if (parentStep is INumberedStep numberedStep)
            {
                fillbar = MonoBehaviour.Instantiate(fillbarPrefab, tasksParent).GetComponent<UIFillbar>();
                fillbar.Initialize(numberedStep.current, numberedStep.max);
                numberedStep.OnNumberChange += fillbar.UpdateBar;
            }
            else
            {
                throw new Exception("Task is not compatible with this visual aid");
            }
        }

        public override void Reset()
        {
            MonoBehaviour.Destroy(fillbar.gameObject);
        }

        public override void Update()
        {
        }
    }
}
