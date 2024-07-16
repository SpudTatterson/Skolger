using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Grid/WorldSettings")]
public class WorldSettings : ScriptableObject
{
    public int gridXSize = 200;
    public int gridYSize = 200;
    public float cellSize = 1f;
    public float cellHeight = 3f;
    [Layer] public int groundLayer = 7;

    public int aboveGroundLayers = 2;
    public int belowGroundLayers = 2;

    public Material material;
    // use grid manager to spawn world using numbers above

}