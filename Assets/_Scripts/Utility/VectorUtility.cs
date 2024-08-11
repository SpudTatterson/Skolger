using System.Collections.Generic;
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

    public static Box ScreenBoxToWorldBox(Vector3 mouseStartPos, Vector3 mouseEndPos)
    {
        Vector3 firstCorner = ScreeToWorldPosition(mouseStartPos);

        Vector3 secondCorner = ScreeToWorldPosition(mouseEndPos);

        return CalculateBoxSize(firstCorner, secondCorner);
    }

    public static Box ScreenBoxToWorldBoxGridAligned(Vector3 mouseStartPos, Vector3 mouseEndPos, float cellSize, LayerMask layerMask)
    {
        Vector3 firstCorner = ScreeToWorldPosition(mouseStartPos, layerMask);
        firstCorner = GridManager.instance.GetCellFromPosition(firstCorner).position - new Vector3(cellSize / 2, 0, cellSize / 2);

        Vector3 secondCorner = ScreeToWorldPosition(mouseEndPos, layerMask);
        secondCorner = GridManager.instance.GetCellFromPosition(secondCorner).position + new Vector3(cellSize / 2, 0, cellSize / 2);

        return CalculateBoxSize(firstCorner, secondCorner);
    }

    public static Box CalculateBoxSizeGridAligned(Vector3 firstCorner, Vector3 secondCorner, float cellSize)
    {
        firstCorner = GridManager.instance.GetCellFromPosition(firstCorner).position - new Vector3(cellSize / 2, 0, cellSize / 2);

        secondCorner = GridManager.instance.GetCellFromPosition(secondCorner).position + new Vector3(cellSize / 2, 0, cellSize / 2);

        return CalculateBoxSize(firstCorner, secondCorner);
    }
    public static Box CalculateBoxSizeGridAligned(Cell firstCorner, Cell secondCorner, float cellSize)
    {
        return CalculateBoxSize(firstCorner.position - new Vector3(cellSize / 2, 0, cellSize / 2), secondCorner.position + new Vector3(cellSize / 2, 0, cellSize / 2));
    }

    public static Box CalculateBoxSize(Vector3 firstCorner, Vector3 secondCorner)
    {
        float cellHeight = GridManager.instance.worldSettings.cellHeight;

        Vector3 center = (firstCorner + secondCorner) / 2;
        center.y += cellHeight / 2;

        Vector3 size = new Vector3(Mathf.Abs(firstCorner.x - secondCorner.x), cellHeight, Mathf.Abs(firstCorner.z - secondCorner.z));

        return new Box(center, size / 2);
    }

    public static Vector3 CalculateCenter(List<Vector3> positions)
    {
        if (positions == null || positions.Count == 0)
        {
            throw new System.Exception("Provided list was empty or null");
        }

        Vector3 sum = Vector3.zero;

        foreach (Vector3 pos in positions)
        {
            sum += pos;
        }

        Vector3 center = sum / positions.Count;

        return center;
    }

}
