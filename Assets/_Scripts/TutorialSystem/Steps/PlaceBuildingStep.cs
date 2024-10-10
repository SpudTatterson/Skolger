using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Skolger.Tutorial
{
    public class PlaceBuildingStep : BaseStep, IRaycastCellHelperUser, INumberedStep
    {
        [SerializeField, InlineButton("StartSelecting")] List<Vector3> positions; // need to figure out way to select from editor preferably with a raycast or something similar
        [SerializeField] BuildingData buildingData;

        [SerializeField] bool needToPlaceOnAllCells = true;
        [SerializeField, HideIf(nameof(needToPlaceOnAllCells))] int neededBuildingsPlaced;
        [SerializeField, ShowIf(nameof(needToPlaceOnAllCells))] bool ignoreInUseCells = true;

        public event Action<float> OnNumberChange;

        public float max => neededBuildingsPlaced;

        public float current => buildingsPlaced;

        int buildingsPlaced;
        Cell[] cells = new Cell[0];
        public override void Initialize()
        {
            base.Initialize();

            cells = new Cell[positions.Count];
            for (int i = 0; i < positions.Count; i++)
            {
                Cell cell = GridManager.Instance.GetCellFromPosition(positions[i]);
                if (!cell.inUse && ignoreInUseCells)
                    cells[i] = cell;
            }

            BuildingPlacer.Instance.OnBuildingPlaced += CheckIfPlaced;

            if (needToPlaceOnAllCells)
                neededBuildingsPlaced = cells.Length;
        }
        public override void Finish()
        {
            BuildingPlacer.Instance.OnBuildingPlaced -= CheckIfPlaced;

            base.Finish();
        }

        void CheckIfPlaced(BuildingData data, Cell cell, ConstructionSiteObject constructionSite)
        {
            foreach (var c in cells)
            {
                if (cell == c)
                {
                    if (data == buildingData)
                    {
                        buildingsPlaced++;
                        OnNumberChange?.Invoke(buildingsPlaced);
                        break;
                    }
                    else
                    {
                        constructionSite.CancelConstruction();
                        break;
                    }
                }
            }
            if (buildingsPlaced >= neededBuildingsPlaced)
                Finish();
        }

        void StartSelecting()
        {
            RaycastCellHelper.StartEditModeRaycast(this);
        }
        public void SetCells(Cell[] cells)
        {
            this.cells = cells;
            foreach (var cell in cells)
            {
                positions.Add(cell.position);
            }
            ShowCells();
        }
        [Button]
        void ShowCells()
        {
            foreach (var cell in positions)
                Debug.DrawLine(cell, cell + Vector3.up, Color.blue, 5);
        }
    }
}