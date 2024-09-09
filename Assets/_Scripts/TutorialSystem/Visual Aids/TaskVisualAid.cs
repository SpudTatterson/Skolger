using TMPro;
using UnityEngine;
namespace Skolger.Tutorial
{
    public class TaskVisualAid : VisualAid
    {
        [SerializeField] GameObject taskPrefab;
        [SerializeField] RectTransform tasksParent;

        [SerializeField] string taskName;

        GameObject task;
        public override void Initialize()
        {
            task = MonoBehaviour.Instantiate(taskPrefab, tasksParent);
            task.GetComponent<TextMeshProUGUI>().text = taskName;
        }

        public override void Reset()
        {
            MonoBehaviour.Destroy(task);
        }

        public override void Update()
        {
        }
    }
}
