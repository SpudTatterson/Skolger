using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ColonistBar : MonoBehaviour
{
    private ColonistData colonist;
    [SerializeField] UnityEngine.UI.Image colonistFace;
    [SerializeField] TextMeshProUGUI colonistName;
    [SerializeField] TextMeshProUGUI activity;

    private void Update()
    {
        if(colonist == null)
        {
            Destroy(gameObject);
        }
    }

    public void SetDataOnCreation(string colonistName, ColonistData colonist)
    {
        this.colonistName.text = colonistName;
        this.colonist = colonist;
        colonistFace.sprite = colonist.faceSprite;

        if(this.colonist != null)
        {
            this.colonist.OnActivityChanged += UpdateActivity;
        }
    }

    private void UpdateActivity(string activity)
    {
        this.activity.text = activity;
    }

    private void OnDestroy()
    {
        if (colonist != null)
        {
            colonist.OnActivityChanged -= UpdateActivity;
        }
    }
}
