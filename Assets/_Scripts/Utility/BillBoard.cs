using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    [SerializeField] bool onlyRotateY;
    Transform cam;
    SpriteRenderer spriteRenderer;
    void Awake()
    {
        cam = Camera.main.transform;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    public void UpdateImage(Sprite sprite)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = sprite;
    }

    void LateUpdate()
    {
        Vector3 lookPos = transform.position - cam.forward;

        if (onlyRotateY)
            lookPos = VectorUtility.FlattenVector(lookPos);

        transform.LookAt(lookPos);
    }
}
