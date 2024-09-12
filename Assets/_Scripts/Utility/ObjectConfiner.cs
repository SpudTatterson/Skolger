using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectConfiner : MonoBehaviour
{
    [SerializeField] Collider boundingVolume;

    // Update is called once per frame
    void LateUpdate()
    {
        if (boundingVolume != null)
            Confine3D();
    }

    void Confine3D()
    {
        Vector3 closestPoint = boundingVolume.ClosestPoint(transform.position);
        if (transform.position != closestPoint)
        {
            transform.position = closestPoint; 
        }
    }
}
