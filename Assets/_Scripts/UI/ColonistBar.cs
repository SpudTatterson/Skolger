using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;

public class ColonistBar : MonoBehaviour
{
    public ColonistData colonist;
    [SerializeField] GameObject colonistFace;
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
