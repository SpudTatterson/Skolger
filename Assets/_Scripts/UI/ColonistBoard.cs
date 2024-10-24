using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonistBoard : MonoBehaviour
{
    List<ColonistBar> colonistBars = new List<ColonistBar>();
    public ColonistBar CreateNewColonist(ColonistData colonist)
    {
        ColonistBar colonistBarInfo = MonoBehaviour.Instantiate(UIManager.Instance.colonistDataPrefab, UIManager.Instance.colonistBoard.transform).GetComponent<ColonistBar>();
        colonistBarInfo.gameObject.name = colonist.colonistName;
        colonistBarInfo.SetDataOnCreation(colonist);
        colonistBars.Add(colonistBarInfo);
        return colonistBarInfo;
    }

    public void TurnOffAllColonistTabs()
    {
        foreach (ColonistBar colonist in colonistBars)
        {
            colonist.TurnOffAllTabs();
        }
    }
    public void TurnOnInfoTab()
    {
        foreach (ColonistBar colonist in colonistBars)
        {
            colonist.TurnOnInfoTab();
        }
    }
    public void TurnOnPriorityTab()
    {
        foreach (ColonistBar colonist in colonistBars)
        {
            colonist.TurnOnPriorityTab();
        }
    }
}
