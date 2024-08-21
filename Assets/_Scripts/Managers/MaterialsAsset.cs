using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Skolger/MaterialsAsset")]
public class MaterialsAsset : SerializedScriptableObject
{
    public Dictionary<CellType, Material> cellMaterials = new Dictionary<CellType, Material>();
    public Material stockpileMaterial;
    public Material unfinishedBuildingMaterial;
    public Material SelectionMaterial;

    public Material GetMaterialForCellType(CellType cellType)
    {
        if (cellMaterials.ContainsKey(cellType))
        {
            return cellMaterials[cellType];
        }
        throw new Exception($"Need to reference a material for {cellType}");
    }
}