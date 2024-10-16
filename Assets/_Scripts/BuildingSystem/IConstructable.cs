using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConstructable
{
    ItemCost GetNextCost();
    List<ItemCost> GetAllCosts();
    void AddItem(InventoryItem item);

    bool CheckIfCostsFulfilled();
    void ConstructBuilding();
    Cell GetPosition();

    void CancelConstruction();
    bool SetForCancellation { get; }
}
