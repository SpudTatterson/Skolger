using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class LayerManager : MonoSingleton<LayerManager>
{

    [Layer] public int draggableLayer = 6;
    [Layer] public int groundLayer = 7;
    [Layer] public int itemLayer = 8;
    [Layer] public int buildableLayer = 12;

    public LayerMask GroundLayerMask;
    public LayerMask ItemLayerMask;
    public LayerMask SelectableLayerMask;
    public LayerMask buildableLayerMask;


}
