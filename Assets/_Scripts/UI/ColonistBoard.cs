using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;

public class ColonistBoard : MonoBehaviour
{
    private ColonistData colonist;
    [SerializeField] GameObject colonistFace;
    [SerializeField] TextMeshProUGUI colonistName;
    [SerializeField] TextMeshProUGUI activity;

    public void SetDataOnCreation(string colonistName, ColonistData colonist)
    {
        this.colonistName.text = colonistName;
        this.colonist = colonist;

        if(this.colonist != null)
        {
            this.colonist.OnActivityChanged += UpdateActivity;
        }
    }

    public void UpdateData(string activity)
    {
        this.activity.text = activity;
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
