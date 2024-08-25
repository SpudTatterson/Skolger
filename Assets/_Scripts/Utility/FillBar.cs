using NaughtyAttributes;
using UnityEngine;

public class FillBar : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float maxFillAmount = 100;
    float currentFill = 0f;
    [SerializeField] bool inverseFillBar = false;
    [SerializeField, Foldout("Advance Settings")] Vector3 emptyPosition = new Vector3(1, 0, 0);
    [SerializeField, Foldout("Advance Settings")] Vector3 fullPosition = new Vector3(0, 0, 0);

    [Header("References")]
    [SerializeField] SpriteRenderer barBGRenderer;
    [SerializeField] SpriteRenderer barRenderer;
    [SerializeField] Transform mask;

    [ContextMenu("UpdateHealth")]
    void test()
    {
        UpdateFillAmount(50f);
    }
    public void UpdateFillAmount(float newFillAmount)
    {
        currentFill = Mathf.Clamp(newFillAmount, 0, maxFillAmount);
        UpdateBar();
    }

    void UpdateBar()
    {
        float FillAmount;
        if (inverseFillBar)
            FillAmount = Mathf.InverseLerp(maxFillAmount, 0, currentFill);
        else
            FillAmount = Mathf.InverseLerp(0, maxFillAmount, currentFill);

        Vector3 maskPos = new Vector3(Mathf.InverseLerp(emptyPosition.x, fullPosition.x, FillAmount), 0 ,0);
        mask.localPosition = maskPos;
    }
    public void UpdateMaxFillAmount(float maxFillAmount)
    {
        this.maxFillAmount = maxFillAmount;
    }
}
