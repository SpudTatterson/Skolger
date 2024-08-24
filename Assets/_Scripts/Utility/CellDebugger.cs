using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellDebugger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Cell cell = GridManager.Instance.GetCellFromPosition(hit.point);
                    Debug.Log($"{cell}, {cell.cellType} \n in use {cell.inUse} \n has floor {cell.hasFloor} \n hit point: {hit.point}");
                }
            }
        }
    }
}
