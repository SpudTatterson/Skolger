using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Skolger.Tutorial
{
    public class GhostVisualAid : VisualAid, IRaycastCellHelperUser
    {
        [SerializeField] GameObject model;
        [SerializeField] bool isFloor = false;

        [SerializeField, InlineButton("StartSelecting")] Cell[] cells;
        List<GameObject> tempObjects = new List<GameObject>();
        public override void Initialize()
        {
            foreach (var c in cells)
            {
                Cell cell = GridManager.Instance.GetCellFromPosition(c.position);
                if ((!isFloor && !cell.inUse) || (isFloor && !cell.hasFloor))
                    tempObjects.Add(MonoBehaviour.Instantiate(model, cell.position, Quaternion.identity));
            }
        }

        public override void Reset()
        {
            foreach (var tempObject in tempObjects)
                MonoBehaviour.Destroy(tempObject);
        }

        public override void Update()
        {

        }

        void StartSelecting()
        {
            RaycastCellHelper.StartEditModeRaycast(this);
        }

        public void SetCells(Cell[] cells)
        {
            this.cells = cells;
            ShowCells();
        }

        [Button]
        void ShowCells()
        {
            foreach (var cell in cells)
                if ((!isFloor && !cell.inUse) || (isFloor && !cell.hasFloor))
                    Debug.DrawLine(cell.position, cell.position + Vector3.up, Color.blue, 5);
        }
    }

}
