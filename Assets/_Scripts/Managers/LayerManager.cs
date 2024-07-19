using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public static LayerManager instance;

    [Layer] public int draggableLayer = 6;
    [Layer] public int groundLayer = 7;
    [Layer] public int itemLayer = 8;

    public LayerMask GroundLayerMask;
    public LayerMask ItemLayerMask;
    public LayerMask SelectableLayerMask;
    public LayerMask buildableLayerMask;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("More the one layerMask manager");
            Destroy(this);
        }
    }
}
