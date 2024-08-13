using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipPlaneHandler : MonoBehaviour
{
    Renderer objectRenderer;
    Collider objectCollider;

    [SerializeField, Range(0.0f, 1.0f)] float disableThreshold = 0.25f; // The threshold at which the collider will be disabled

    void Start()
    {
        if (objectRenderer == null)
            objectRenderer = GetComponentInChildren<Renderer>();

        if (objectCollider == null)
            objectCollider = GetComponent<Collider>();
        VisualLayerManager.Instance.OnYPlaneChange += CheckAgainstClipPlane;
    }

    void CheckAgainstClipPlane(float yClipPlane)
    {
        if (objectRenderer == null || objectCollider == null) return;

        Bounds bounds = objectRenderer.bounds;
        float objectHeight = bounds.size.y;
        float objectTopY = bounds.max.y;

        // Calculate visibility factor based on how much of the object is above the clip plane
        float visibilityFactor = Mathf.Clamp01((objectTopY - yClipPlane) / objectHeight);

        if (visibilityFactor >= disableThreshold)
        {
            objectCollider.enabled = false;
        }
        else
        {
            objectCollider.enabled = true;
        }
    }
}
