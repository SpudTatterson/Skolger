using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConstructable
{
    bool beingConstructed { get; }
    bool finishedConstruction { get; }
    ItemCost GetNextCost();
    List<ItemCost> GetAllCosts();
    void AddItem(InventoryItem item);

    bool CheckIfCostsFulfilled();
    IEnumerator ConstructBuilding();
    Cell GetPosition();

    void CancelConstruction();
    bool SetForCancellation { get; }
}
