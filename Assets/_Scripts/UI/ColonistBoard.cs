using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonistBoard : MonoBehaviour
{
    List<ColonistBar> colonistBars = new List<ColonistBar>();
    public ColonistBar CreateNewColonist(ColonistData colonist)
    {
        ColonistBar colonistBar = MonoBehaviour.Instantiate(UIManager.Instance.colonistDataPrefab, UIManager.Instance.colonistBoard.transform).GetComponent<ColonistBar>();
        colonistBar.gameObject.name = colonist.colonistName;
        colonistBar.SetDataOnCreation(colonist);
        colonistBars.Add(colonistBar);
        return colonistBar;
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
