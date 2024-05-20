using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshUtility : MonoBehaviour
{
    public static GameObject CreateGridMesh(int width, int height, Vector3 startPosition, string objectName, Material material, Transform parent = null, int cellSize = 1)
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

}
