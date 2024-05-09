using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendedDebug : MonoBehaviour
{
    public static void DrawBox(Vector3 center, Vector3 size, Quaternion rotation)
{
    Vector3 halfExtents = size / 2;
    var points = new Vector3[8];

    // Calculate the local space corners of the box
    points[0] = center + rotation * new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
    points[1] = center + rotation * new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);
    points[2] = center + rotation * new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z);
    points[3] = center + rotation * new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z);
    points[4] = center + rotation * new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
    points[5] = center + rotation * new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
    points[6] = center + rotation * new Vector3(halfExtents.x, halfExtents.y, halfExtents.z);
    points[7] = center + rotation * new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z);

    // Draw bottom square
    Debug.DrawLine(points[0], points[1], Color.red);
    Debug.DrawLine(points[1], points[2], Color.red);
    Debug.DrawLine(points[2], points[3], Color.red);
    Debug.DrawLine(points[3], points[0], Color.red);

    // Draw top square
    Debug.DrawLine(points[4], points[5], Color.red);
    Debug.DrawLine(points[5], points[6], Color.red);
    Debug.DrawLine(points[6], points[7], Color.red);
    Debug.DrawLine(points[7], points[4], Color.red);

    // Draw vertical lines
    Debug.DrawLine(points[0], points[4], Color.red);
    Debug.DrawLine(points[1], points[5], Color.red);
    Debug.DrawLine(points[2], points[6], Color.red);
    Debug.DrawLine(points[3], points[7], Color.red);
}
}
