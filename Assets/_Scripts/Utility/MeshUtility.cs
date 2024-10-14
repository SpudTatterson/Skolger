using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshUtility : MonoBehaviour
{
    private static Dictionary<Cell, Vector3[]> cellVerticesCache = new Dictionary<Cell, Vector3[]>();

    private static Vector3[] GetCellVertices(Cell cell, float cellSize)
    {
        if (cellVerticesCache.TryGetValue(cell, out Vector3[] cellVertices))
        {
            return cellVertices;
        }

        cellVertices = new Vector3[4];
        Vector3 cornerPosition = cell.position - new Vector3(cellSize / 2, 0, cellSize / 2);
        Vector3 basePosition = cornerPosition + new Vector3(0, 0.01f, 0);

        cellVertices[0] = basePosition + new Vector3(0, 0, 0);
        cellVertices[1] = basePosition + new Vector3(0, 0, cellSize);
        cellVertices[2] = basePosition + new Vector3(cellSize, 0, cellSize);
        cellVertices[3] = basePosition + new Vector3(cellSize, 0, 0);

        cellVerticesCache[cell] = cellVertices;
        return cellVertices;
    }
    public static GameObject CreateGridMesh(int width, int height, Vector3 startPosition, string objectName, Material material, Transform parent = null, float cellSize = 1)
    {
        GameObject MeshObject = new GameObject(objectName);
        MeshFilter meshFilter = MeshObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = MeshObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[width * height * 4];
        int[] triangles = new int[width * height * 6];
        Vector2[] uv = new Vector2[width * height * 4];

        int vertIndex = 0;
        int triIndex = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 basePosition = startPosition + new Vector3(x * cellSize, 0, y * cellSize);

                vertices[vertIndex + 0] = basePosition + new Vector3(0, 0, 0);
                vertices[vertIndex + 1] = basePosition + new Vector3(0, 0, cellSize);
                vertices[vertIndex + 2] = basePosition + new Vector3(cellSize, 0, cellSize);
                vertices[vertIndex + 3] = basePosition + new Vector3(cellSize, 0, 0);

                triangles[triIndex + 0] = vertIndex + 0;
                triangles[triIndex + 1] = vertIndex + 1;
                triangles[triIndex + 2] = vertIndex + 2;
                triangles[triIndex + 3] = vertIndex + 2;
                triangles[triIndex + 4] = vertIndex + 3;
                triangles[triIndex + 5] = vertIndex + 0;

                uv[vertIndex + 0] = new Vector2(0, 0);
                uv[vertIndex + 1] = new Vector2(0, 1);
                uv[vertIndex + 2] = new Vector2(1, 1);
                uv[vertIndex + 3] = new Vector2(1, 0);

                vertIndex += 4;
                triIndex += 6;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        MeshObject.AddComponent<MeshCollider>();
        MeshObject.transform.parent = parent;

        return MeshObject;
    }
    public static GameObject CreateGridMesh(List<Cell> cells, string objectName, Material material, Transform parent = null, float cellSize = 1)
    {
        if (cells == null || cells.Count == 0)
            return null;

        GameObject MeshObject = new GameObject(objectName);
        MeshFilter meshFilter = MeshObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = MeshObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[cells.Count * 4];
        int[] triangles = new int[cells.Count * 6];
        Vector2[] uv = new Vector2[cells.Count * 4];

        int vertIndex = 0;
        int triIndex = 0;

        // Calculate the offset based on the position of the first cell
        Vector3 offset = cells[0].position;

        foreach (Cell cell in cells)
        {
            Vector3[] cellVertices = GetCellVertices(cell, cellSize);

            // Apply offset to move vertices relative to the first cell's position
            vertices[vertIndex + 0] = cellVertices[0] - offset;
            vertices[vertIndex + 1] = cellVertices[1] - offset;
            vertices[vertIndex + 2] = cellVertices[2] - offset;
            vertices[vertIndex + 3] = cellVertices[3] - offset;

            triangles[triIndex + 0] = vertIndex + 0;
            triangles[triIndex + 1] = vertIndex + 1;
            triangles[triIndex + 2] = vertIndex + 2;
            triangles[triIndex + 3] = vertIndex + 2;
            triangles[triIndex + 4] = vertIndex + 3;
            triangles[triIndex + 5] = vertIndex + 0;

            uv[vertIndex + 0] = new Vector2(0, 0);
            uv[vertIndex + 1] = new Vector2(0, 1);
            uv[vertIndex + 2] = new Vector2(1, 1);
            uv[vertIndex + 3] = new Vector2(1, 0);

            vertIndex += 4;
            triIndex += 6;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        MeshObject.AddComponent<MeshCollider>();

        // Set the mesh object position to the first cell's position
        MeshObject.transform.position = offset;

        MeshObject.transform.parent = parent;

        return MeshObject;
    }

    public static void UpdateGridMesh(List<Cell> cells, MeshFilter meshFilter, float cellSize = 1)
    {

        Mesh mesh = meshFilter.mesh;
        mesh.Clear();
        Vector3[] vertices = new Vector3[cells.Count * 4];
        int[] triangles = new int[cells.Count * 6];
        Vector2[] uv = new Vector2[cells.Count * 4];

        int vertIndex = 0;
        int triIndex = 0;

        Vector3 offset = cells[0].position;

        foreach (Cell cell in cells)
        {
            Vector3[] cellVertices = GetCellVertices(cell, cellSize);

            // Apply offset to move vertices relative to the first cell's position
            vertices[vertIndex + 0] = cellVertices[0] - offset;
            vertices[vertIndex + 1] = cellVertices[1] - offset;
            vertices[vertIndex + 2] = cellVertices[2] - offset;
            vertices[vertIndex + 3] = cellVertices[3] - offset;

            triangles[triIndex + 0] = vertIndex + 0;
            triangles[triIndex + 1] = vertIndex + 1;
            triangles[triIndex + 2] = vertIndex + 2;
            triangles[triIndex + 3] = vertIndex + 2;
            triangles[triIndex + 4] = vertIndex + 3;
            triangles[triIndex + 5] = vertIndex + 0;

            uv[vertIndex + 0] = new Vector2(0, 0);
            uv[vertIndex + 1] = new Vector2(0, 1);
            uv[vertIndex + 2] = new Vector2(1, 1);
            uv[vertIndex + 3] = new Vector2(1, 0);

            vertIndex += 4;
            triIndex += 6;
        }


        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        meshFilter.gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
    }


}
