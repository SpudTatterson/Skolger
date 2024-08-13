using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class VisualLayerManager : MonoSingleton<VisualLayerManager>
{
    [Header("Settings")]
    [SerializeField] float maxYHeight = 12f;
    [SerializeField] float minYHeight = -3f;

    [Header("Y Plane")]
    [SerializeField, OnValueChanged("UpdateYClip")] float yClip = 6f;
    [SerializeField] float offset = 0.2f;
    bool needsUpdate = true;


    public Action<float> OnYPlaneChange;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                GoUp();
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                GoDown();
            }
        }

        if (needsUpdate)
        {
            UpdateYClip();
            needsUpdate = false;
        }
    }
    void UpdateYClip()
    {
        yClip = Mathf.Clamp(yClip, minYHeight, maxYHeight);
        Shader.SetGlobalFloat("_GlobalYClip", yClip + offset);
        OnYPlaneChange.Invoke(yClip + offset);
    }
    [Button]
    public void GoUp()
    {
        yClip += 1.5f;
        needsUpdate = true;
    }
    [Button]
    public void GoDown()
    {
        yClip -= 1.5f;
        needsUpdate = true;
    }
    public float GetYPlane()
    {
        return yClip;
    }
}
