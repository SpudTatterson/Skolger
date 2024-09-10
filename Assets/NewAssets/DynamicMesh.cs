using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class DynamicMesh : MonoBehaviour
{
    Transform small, medium, large;
    [SerializeField] float width = 4;
    [SerializeField] Vector2 breakPoints = new Vector2(3, 6);
    Size size = Size.Small;
    float actualSize;
    Dictionary<Size, float> actualSizes = new Dictionary<Size, float>();
    // Start is called before the first frame update
    void Start()
    {
        small = transform.Find("small");
        medium = transform.Find("medium");
        large = transform.Find("large");

        actualSizes.Add(Size.Small, small.GetComponentInChildren<MeshRenderer>().bounds.extents.z * 2);
        actualSizes.Add(Size.Medium, medium.GetComponentInChildren<MeshRenderer>().bounds.extents.z * 2);
        actualSizes.Add(Size.Large, large.GetComponentInChildren<MeshRenderer>().bounds.extents.z * 2);
    }

    [Button]
    public void SetWidth()
    {
        if (width < breakPoints.x)
        {
            size = Size.Small;
            actualSize = actualSizes[size];
            small.localScale = new Vector3(small.localScale.x, small.localScale.y, width / actualSize);
            UpdateTiling(small.GetComponentInChildren<MeshRenderer>().materials);
        }
        else if (width < breakPoints.y)
        {
            size = Size.Medium;
            actualSize = actualSizes[size];
            medium.localScale = new Vector3(medium.localScale.x, medium.localScale.y, width / actualSize);
            UpdateTiling(medium.GetComponentInChildren<MeshRenderer>().materials);
        }
        else
        {
            size = Size.Large;
            actualSize = actualSizes[size];
            large.localScale = new Vector3(large.localScale.x, large.localScale.y, width / actualSize);
            UpdateTiling(large.GetComponentInChildren<MeshRenderer>().materials);
        }
        Debug.Log(actualSize);
        ShowCorrectMesh();
    }

    void ShowCorrectMesh()
    {
        small.gameObject.SetActive(size == Size.Small);
        medium.gameObject.SetActive(size == Size.Medium);
        large.gameObject.SetActive(size == Size.Large);

    }
    void UpdateTiling(Material[] materials)
    {
        float scaleFactor = width / actualSize;

        // TODO: improve this scaling doesnt look that good might want to wait for actual model and textures but not sure
        foreach (var material in materials)
        {
            material.mainTextureScale = new Vector2(scaleFactor, scaleFactor / 2);
        }
    }
}

public enum Size
{
    Small,
    Medium,
    Large
}