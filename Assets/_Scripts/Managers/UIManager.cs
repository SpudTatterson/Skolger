using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.Log("More then 1 UIManager Exists");
    }
}
