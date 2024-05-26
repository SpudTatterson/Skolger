using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public static LayerManager instance;

    public int draggableLayer = 6;
    public int groundLayer = 7;
    public int itemLayer = 8;

    public LayerMask GroundLayerMask;
    public LayerMask ItemLayerMask;
    public LayerMask SelectableLayerMask;


    void Awake()
    {
        if(instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("More the one layerMask manager");
            Destroy(this);
        }
    }
}
