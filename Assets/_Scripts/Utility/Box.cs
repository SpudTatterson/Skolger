using UnityEngine;
using UnityEngine.Video;

public class Box
{
    public Vector3 center { get; private set; }
    public Vector3 halfExtents { get; private set; }

    public Box(Vector3 center, Vector3 halfExtents)
    {
        this.center = center;
        this.halfExtents = halfExtents;
    }
}