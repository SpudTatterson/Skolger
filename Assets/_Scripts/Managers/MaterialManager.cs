using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoSingleton<MaterialManager>
{
    public Material grassMaterial;
    public Material rockMaterial;
    public Material stockpileMaterial;
    public Material unfinishedBuildingMaterial;
    public Material SelectionMaterial;

    internal Material GetMaterialForCellType(CellType cellType)
    {
       if(cellType == CellType.Grass) return grassMaterial;
       else if (cellType == CellType.Rock) return rockMaterial;

       return grassMaterial;
    }
}
