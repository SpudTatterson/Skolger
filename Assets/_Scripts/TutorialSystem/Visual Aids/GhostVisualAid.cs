using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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

        [SerializeField, InlineButton("StartSelecting")] List<Vector3> positions;
        List<GameObject> tempObjects = new List<GameObject>();
        [SerializeField] bool deleteObjectsInUsedCells = false;
        Dictionary<Cell, GameObject> objects = new Dictionary<Cell, GameObject>();
        public override void Initialize()
        {
            foreach (var position in positions)
            {
                Cell cell = GridManager.Instance.GetCellFromPosition(position);
                if ((!isFloor && !cell.inUse) || (isFloor && !cell.hasFloor))
                {
                    GameObject temp = MonoBehaviour.Instantiate(model, cell.position, Quaternion.identity);
                    tempObjects.Add(temp);
                    objects.Add(cell, temp);
                }
            }
        }

        public override void Reset()
        {
            foreach (var tempObject in tempObjects)
                MonoBehaviour.Destroy(tempObject);
        }

        public override void Update()
        {
            if (deleteObjectsInUsedCells)
            {
                List<Cell> inUseCells = new List<Cell>();
                foreach (var entry in objects)
                {
                    if (entry.Key.inUse)
                    {
                        MonoBehaviour.Destroy(entry.Value);
                        tempObjects.Remove(entry.Value);
                        inUseCells.Add(entry.Key);
                    }
                }
                foreach (var cell in inUseCells)
                {
                    objects.Remove(cell);
                }
            }
        }

        void StartSelecting()
        {
            RaycastCellHelper.StartEditModeRaycast(this);
        }

        public void SetCells(Cell[] cells)
        {

            for (int i = 0; i < cells.Length; i++)
            {
                positions.Add(cells[i].position);
                // positions[i] = cells[i].position;
            }

            ShowCells();
        }

        [Button]
        void ShowCells()
        {
            List<Cell> cells = new List<Cell>();
            positions.ForEach(p => cells.Add(GridManager.Instance.GetCellFromPosition(p)));
            foreach (var cell in cells)
                if ((!isFloor && !cell.inUse) || (isFloor && !cell.hasFloor))
                    Debug.DrawLine(cell.position, cell.position + Vector3.up, Color.blue, 5);
        }

        [Button]
        void ClearDuplicates()
        {
            positions.Distinct();
        }
    }

}
