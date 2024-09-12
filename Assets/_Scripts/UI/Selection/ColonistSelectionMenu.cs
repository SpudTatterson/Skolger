using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColonistSelectionMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI colonistName;
    [SerializeField] TextMeshProUGUI activeTask;
    [SerializeField] Image colonistImage;

    public ColonistData colonist { get; private set; }

    public void UpdateMenu(ColonistData colonist)
    {
        this.colonist = colonist;
        this.colonist.OnActivityChanged += UpdateActiveTask;

        colonistName.text = colonist.colonistName;
        activeTask.text = colonist.colonistActivity;
        colonistImage.sprite = colonist.faceSprite;
    }
    void UpdateActiveTask(string activity)
    {
        activeTask.text = activity;
    }
    void OnDisable()
    {
        colonist.OnActivityChanged -= UpdateActiveTask;
    }
}