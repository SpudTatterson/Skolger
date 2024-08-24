using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ConstructableSelectionMenu : MonoBehaviour
{
    [SerializeField] List<IconAndAmount> costsVisual;
    public TextMeshProUGUI constructableName;


    public void SetCosts(List<ItemCost> costs)
    {
        foreach(IconAndAmount visual in costsVisual) visual.gameObject.SetActive(false);

        for (int i = 0; i < costs.Count; i++)
        {
            costsVisual[i].gameObject.SetActive(true);
            costsVisual[i].text.text = $"{costs[i].cost}";
            costsVisual[i].image.sprite = costs[i].item.icon;
        }
    }
}