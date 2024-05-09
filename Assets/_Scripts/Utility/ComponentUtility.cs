using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class ComponentUtility : MonoBehaviour
{
    public static List<T> GetComponentsInBox<T>(Vector3 position, Vector3 halfExtents)
    {
        List<T> hitObjects = new List<T>();
        Collider[] hits = Physics.OverlapBox(position, halfExtents);
        for (int i = 0; i < hits.Length; i++)
        {
            GameObject hitObject = hits[i].gameObject;
            T t = hitObject.GetComponentInParent<T>();
            if (t == null) continue;
            hitObjects.Add(t);
        }
        List<T> distinctHitObjects = hitObjects.Distinct().ToList();
        return distinctHitObjects;
    } 
    public static List<T> GetComponentsInRadius<T>(Vector3 position, float explosionRadius)
    {
        List<T> hitObjects = new List<T>();
        Collider[] hits = Physics.OverlapSphere(position, explosionRadius);
        for (int i = 0; i < hits.Length; i++)
        {
            GameObject hitObject = hits[i].gameObject;
            T t = hitObject.GetComponentInParent<T>();
            if (t == null) continue;
            hitObjects.Add(t);
        }
        List<T> distinctHitObjects = hitObjects.Distinct().ToList();
        return distinctHitObjects;
    }
    public static T GetComponentInRadius<T>(Vector3 position, float explosionRadius)
    {
        List<T> hitObjects = new List<T>();
        Collider[] hits = Physics.OverlapSphere(position, explosionRadius);
        for (int i = 0; i < hits.Length; i++)
        {
            GameObject hitObject = hits[i].gameObject;
            T t = hitObject.GetComponentInParent<T>();
            if (t == null) continue;
            hitObjects.Add(t);
        }
        List<T> distinctHitObjects = hitObjects.Distinct().ToList();
        if(distinctHitObjects.Count != 0) return distinctHitObjects[0];
        else return default;
    }
}
