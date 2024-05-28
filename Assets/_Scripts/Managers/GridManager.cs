using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance { get; private set; }
    public List<GridObject> grids { get; private set; }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.Log("More then one grid manager");
            Destroy(this);
        }

        grids = FindObjectsOfType<GridObject>().ToList();

    }
    public GridObject GetGridFromPosition(Vector3 position)
    {
        foreach (GridObject grid in grids)
        {
            if (Mathf.Round(grid.transform.position.y) == Mathf.Round(position.y))
                return grid;
        }
        return null;
    }

}
