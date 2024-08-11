using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    [SerializeField] bool onlyRotateY;
    Transform cam;
    void Awake()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        Vector3 lookPos = transform.position - cam.forward;

        if (onlyRotateY)
            lookPos = VectorUtility.FlattenVector(lookPos);

        transform.LookAt(lookPos);
    }
}
