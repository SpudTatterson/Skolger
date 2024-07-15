using UnityEngine;

public class VectorUtility
{
    static public Vector3 FlattenVector(Vector3 vectorToFlatten)
    {
        return new Vector3(vectorToFlatten.x, 0f, vectorToFlatten.z);
    }
    static public Vector3 FlattenVector(Vector3 vectorToFlatten, float desiredYValue)
    {
        return new Vector3(vectorToFlatten.x, desiredYValue, vectorToFlatten.z);
    }
    static public Vector3 GetDirection(Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }
    static public Vector3 Round(Vector3 vector3, int decimalPlaces)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            Mathf.Round(vector3.z * multiplier) / multiplier);
    }
    static public Vector3 Round(Vector3 vector3)
    {
        return new Vector3(
            Mathf.Round(vector3.x),
            Mathf.Round(vector3.y),
            Mathf.Round(vector3.z));
    }
    static public Vector3 FromColor(Color color)
    {
        return new Vector3(color.r, color.g, color.b);
    }
    static public Color ToColor(Vector3 color)
    {
        return new Color(color.x, color.y, color.z);
    }
    public static Vector3 ScreeToWorldPosition(Vector3 screenPosition, LayerMask layerMask = default)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            return hit.point;
        }
        else
        {
            Plane plane = new Plane();
            plane.Raycast(ray, out float distance);
            return ray.GetPoint(distance);
        }
    }

    public static Vector3 ScreenBoxToWorldBox(Vector3 mouseStartPos, Vector3 mouseEndPos, out Vector3 center)
    {
        Vector3 firstCorner = ScreeToWorldPosition(mouseStartPos);
        Debug.DrawLine(firstCorner, firstCorner + Vector3.up);
        Vector3 secondCorner = ScreeToWorldPosition(mouseEndPos);
        Debug.DrawLine(secondCorner, secondCorner + Vector3.up);

        center = (firstCorner + secondCorner) / 2;

        Vector3 size = new Vector3(Mathf.Abs(firstCorner.x - secondCorner.x), 3f, Mathf.Abs(firstCorner.z - secondCorner.z));

        return size / 2;
    }

    public static Vector3 ScreenBoxToWorldBoxGridAligned(Vector3 mouseStartPos, Vector3 mouseEndPos, float cellSize, LayerMask layerMask, out Vector3 center)
    {
        Vector3 firstCorner = ScreeToWorldPosition(mouseStartPos, layerMask);
        Debug.DrawLine(firstCorner, firstCorner + Vector3.up);
        firstCorner = GridManager.instance.GetCellFromPosition(firstCorner).position - new Vector3(cellSize / 2, 0, cellSize / 2);

        Vector3 secondCorner = ScreeToWorldPosition(mouseEndPos, layerMask);
        Debug.DrawLine(secondCorner, secondCorner + Vector3.up);
        secondCorner = GridManager.instance.GetCellFromPosition(secondCorner).position + new Vector3(cellSize / 2, 0, cellSize / 2);

        center = (firstCorner + secondCorner) / 2;

        Vector3 size = new Vector3(Mathf.Abs(firstCorner.x - secondCorner.x), 3f, Mathf.Abs(firstCorner.z - secondCorner.z));

        return size / 2;
    }

}

