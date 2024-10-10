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
        firstCorner = GridManager.Instance.GetCellFromPosition(firstCorner).position - new Vector3(cellSize / 2, 0, cellSize / 2);

        Vector3 secondCorner = ScreeToWorldPosition(mouseEndPos, layerMask);
        secondCorner = GridManager.Instance.GetCellFromPosition(secondCorner).position + new Vector3(cellSize / 2, 0, cellSize / 2);

        return CalculateBoxSize(firstCorner, secondCorner);
    }

    public static Box CalculateBoxSizeGridAligned(Vector3 firstCorner, Vector3 secondCorner, float cellSize)
    {
        firstCorner = GridManager.Instance.GetCellFromPosition(firstCorner).position - new Vector3(cellSize / 2, 0, cellSize / 2);

        secondCorner = GridManager.Instance.GetCellFromPosition(secondCorner).position + new Vector3(cellSize / 2, 0, cellSize / 2);

        return CalculateBoxSize(firstCorner, secondCorner);
    }
    public static Box CalculateBoxSizeGridAligned(Cell firstCorner, Cell secondCorner, float cellSize)
    {
        if (firstCorner == null || secondCorner == null) throw new MissingReferenceException("One of the cells are null");
        return CalculateBoxSize(firstCorner.position - new Vector3(cellSize / 2, 0, cellSize / 2), secondCorner.position + new Vector3(cellSize / 2, 0, cellSize / 2));
    }

    public static Box CalculateBoxSize(Vector3 firstCorner, Vector3 secondCorner)
    {
        float cellHeight = GridManager.Instance.worldSettings.cellHeight;

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
    public static Vector3 CalculateCenter(List<Transform> transforms)
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (Transform t in transforms)
        {
            if(t != null) positions.Add(t.position);
        }
        
        return CalculateCenter(positions);
    }
    public static Vector3 RoundVector3ToHalf(Vector3 vector)
    {
        return new Vector3(
            Mathf.Round(vector.x * 2f) / 2f,
            Mathf.Round(vector.y * 2f) / 2f,
            Mathf.Round(vector.z * 2f) / 2f
        );
    }

    public static Vector2 OffsetPositionToScreen(Vector2 position, Vector2 sizeDelta, Vector2 offset = default)
    {
        Vector2 screenBounds = new Vector2(Screen.width, Screen.height);

        Vector2 clampedPosition = position + offset;
        bool outsideTop = false;

        // Check if exceeds top or bottom bounds
        if (clampedPosition.y + sizeDelta.y > screenBounds.y) // Top
        {
            clampedPosition.y = position.y - sizeDelta.y;
            outsideTop = true;
        }
        if (clampedPosition.y < 0) // Bottom
        {
            clampedPosition.y = position.y + sizeDelta.y;
        }

        // Check if exceeds right or left bounds
        if (clampedPosition.x + sizeDelta.x > screenBounds.x || outsideTop) // Right side
        {
            clampedPosition.x = position.x - sizeDelta.x;
        }
        if (clampedPosition.x < 0) // Left side
        {
            clampedPosition.x = position.x + sizeDelta.x;
        }


        return clampedPosition;
    }
    public static Vector2 ClampPositionToScreen(Vector2 position, Vector2 sizeDelta, Vector2 offset = default)
    {
        Vector2 screenBounds = new Vector2(Screen.width, Screen.height);

        Vector2 clampedPosition = position + offset;


        // Check if exceeds top or bottom bounds
        if (clampedPosition.y + sizeDelta.y > screenBounds.y) // Top
        {
            clampedPosition.y = screenBounds.y - sizeDelta.y;
        }
        if (clampedPosition.y < 0) // Bottom
        {
            clampedPosition.y = 0;
        }

        // Check if exceeds right or left bounds
        if (clampedPosition.x + sizeDelta.x > screenBounds.x) // Right side
        {
            clampedPosition.x = screenBounds.x - sizeDelta.x;
        }
        if (clampedPosition.x < 0) // Left side
        {
            clampedPosition.x = 0;
        }


        return clampedPosition;
    }
}
