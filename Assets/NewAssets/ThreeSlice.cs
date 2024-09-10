using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeSlice : MonoBehaviour
{
    Transform start, middle, end;
    [SerializeField] float width = 4;
    // Start is called before the first frame update
    void Start()
    {
        start = transform.Find("Start");
        middle = transform.Find("Middle");
        end = transform.Find("End");

        SetWidth(width);
    }
    void Update()
    {

        SetWidth(Random.Range(1, 15));
    }
    public void SetWidth(float width)
    {
        this.width = width;
        UpdatedMesh(width);
    }

    private void UpdatedMesh(float width)
    {
        start.localPosition = Vector3.zero;
        middle.localPosition = new Vector3(0, 0, width * 0.5f);
        middle.localScale = new Vector3(middle.localScale.x, middle.localScale.y, width);
        end.localPosition = new Vector3(0, 0, width);
    }
}
