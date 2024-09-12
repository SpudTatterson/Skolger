using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MultipleSelectionText : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TextMeshProUGUI text;
    List<ISelectable> selectables = new List<ISelectable>();

    public void Init(string text, List<ISelectable> selectables)
    {
        this.text.text = text;
        this.selectables = selectables;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetKey(KeyCode.LeftControl))
            DeselectAllAcceptThis();
        else
            DeselectAll();
    }

    void DeselectAll()
    {
        foreach (var selectable in selectables)
        {
            selectable.OnDeselect();
        }
    }
    void DeselectAllAcceptThis()
    {
        SelectionManager.Instance.ResetSelection();

        foreach (var selectable in selectables)
        {
            selectable.OnSelect();
        }
    }
}
