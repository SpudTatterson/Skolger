using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    [SerializeField] Dictionary<MaterialType, int> materials = new Dictionary<MaterialType, int>{
        { MaterialType.Iron, 100 },
        { MaterialType.Wood, 100 },
    };
        
    void Awake()
    {
        instance = this;
    }
    public void AddMaterial(MaterialType type, int amount)
    {
        materials[type] = amount;
    }

    public bool HasMaterial(MaterialCost materialCost)
    {
        if(!materials.ContainsKey(materialCost.type)) return false;
        if(materials[materialCost.type] >= materialCost.cost) return true;
        else return false;
    }
    public bool HasMaterials(List<MaterialCost> materialCosts)
    {
        foreach(MaterialCost Cost in materialCosts)
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
    public void UseMaterial(MaterialCost materialCost)
    {
        materials[materialCost.type] -= materialCost.cost;
    }
    public void UseMaterials(List<MaterialCost> materialCosts)
    {
        foreach (MaterialCost cost in materialCosts)
        {
            materials[cost.type] -= cost.cost;
        }
    }
}
