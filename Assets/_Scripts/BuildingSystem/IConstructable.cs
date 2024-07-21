using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConstructable 
{
    ItemCost GetNextCost();
    List<ItemCost> GetAllCosts();
    void AddItem(IItem item);

    void CheckIfCanConstruct();
    void ConstructBuilding();
    Cell GetPosition();
    
    void CancelConstruction();
}
