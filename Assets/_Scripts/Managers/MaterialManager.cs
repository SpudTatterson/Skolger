using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public static MaterialManager instance;

    public Material gridMaterial;
    public Material stockpileMaterial;
    public Material unfinishedBuildingMaterial;

    void Awake()
    {
        if(instance == null)
        instance = this;
        else
        {
            Debug.Log("More then one material manager");
            Destroy(this);
        }
    }
}
