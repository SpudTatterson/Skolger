using System.Collections.Generic;
using Skolger.UI.InfoContainers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColonistSelectionMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI colonistName;
    [SerializeField] TextMeshProUGUI activeTask;
    [SerializeField] Image colonistImage;
    [SerializeField] InfoContainer[] infoContainers;

    public ColonistData colonist { get; private set; }

    public void UpdateMenu(ColonistData colonist)
    {
        this.colonist = colonist;
        this.colonist.OnActivityChanged += UpdateActiveTask;

        foreach (InfoContainer infoContainer in infoContainers)
        {
            infoContainer.Initialize(colonist);
        }

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