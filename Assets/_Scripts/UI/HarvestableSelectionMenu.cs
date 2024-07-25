using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HarvestableSelectionMenu : MonoBehaviour
{
    [SerializeField] List<IconAndAmount> dropsVisual;
    public TextMeshProUGUI harvestableName;


    public void SetDrops(List<ItemDrop> drops)
    {
        foreach(IconAndAmount visual in dropsVisual) visual.gameObject.SetActive(false);

        for (int i = 0; i < drops.Count; i++)
        {
            dropsVisual[i].gameObject.SetActive(true);
            dropsVisual[i].text.text = $"{drops[i].minDropAmount} - {drops[i].maxDropAmount}";
            dropsVisual[i].image.sprite = drops[i].itemData.icon;
        }
    }
}
