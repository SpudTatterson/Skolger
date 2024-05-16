using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    [SerializeField] SerializableDictionary<ItemData, int> materials = new SerializableDictionary<ItemData, int>{
        // { MaterialType.Iron, 100 },
        // { MaterialType.Wood, 100 },
    };
        
    void Awake()
    {
        instance = this;
    }
    public void AddMaterial(ItemData item, int amount)
    {
        materials[item] = amount;
    }

    public bool HasMaterial(ItemCost materialCost)
    {
        if(!materials.ContainsKey(materialCost.item)) return false;
        if(materials[materialCost.item] >= materialCost.cost) return true;
        else return false;
    }
    public bool HasMaterials(List<ItemCost> materialCosts)
    {
        foreach(ItemCost Cost in materialCosts)
        {
            bool has;
            has = HasMaterial(Cost);
            if(has == false)
            {
                Debug.Log("missing: " + Cost.ToString());
                return false;
            } 
        }
        return true;
    }
    public void UseMaterial(ItemCost materialCost)
    {
        materials[materialCost.item] -= materialCost.cost;
    }
    public void UseMaterials(List<ItemCost> materialCosts)
    {
        foreach (ItemCost cost in materialCosts)
        {
            materials[cost.item] -= cost.cost;
        }
    }
    public int CheckAmount(ItemData type)
    {
        return materials[type];
    }
    [ContextMenu("CheckInv")]
    void CheckInv()
    {
        foreach(KeyValuePair<ItemData, int> material in materials)
        {
            Debug.Log(material.Key + ": " + material.Value);
        }
    }
}
