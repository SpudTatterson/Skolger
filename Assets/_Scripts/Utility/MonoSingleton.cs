using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour
where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null || Instance == this)
            Instance = this as T;
        else
        {
            Debug.Log($"More Than one {nameof(T)} exists");
            Destroy(this);
        }
    }

    void OnValidate()
    {
        Instance = this as T;
    }
}