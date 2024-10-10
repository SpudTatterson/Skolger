using UnityEngine;
public class Box
{
    public Vector3 center { get; private set; }
    public Vector3 halfExtents { get; private set; }

    public Box(Vector3 center, Vector3 halfExtents)
    {
        this.center = center;
        this.halfExtents = halfExtents;
    }

    public Box ShrinkBoxNoY(float amountToShrink)
    {
        Vector3 shrunkSize = new Vector3 (
            halfExtents.x - amountToShrink,
            halfExtents.y ,
            halfExtents.z - amountToShrink
        );

        halfExtents = shrunkSize;
        return this;
    }

    public static Box RoundBoxToHalf(Box box)
    {
        Vector3 roundedCenter = VectorUtility.RoundVector3ToHalf(box.center);
        Vector3 roundedSize = VectorUtility.RoundVector3ToHalf(box.halfExtents);

        return new Box(roundedCenter, roundedSize);
    }
    
    public static Box GetBoxSize(GameObject visual)
    {
        Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

        foreach (Renderer renderer in renderers)
        {
            if (renderer is SpriteRenderer) continue;
            bounds.Encapsulate(renderer.bounds);
        }

        return new Box(bounds.center, bounds.size);
    }
}