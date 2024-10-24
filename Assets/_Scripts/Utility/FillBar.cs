using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class FillBar : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float maxFillAmount = 100;
    [ShowInInspector, ProgressBar(0, "maxFillAmount"), ReadOnly] float currentFill = 0f;
    [SerializeField] bool inverseFillBar = false;
    [SerializeField, FoldoutGroup("Advance Settings")] bool easeMove = true;
    [SerializeField, FoldoutGroup("Advance Settings"), ShowIf(nameof(easeMove))] float moveDuration = 0.5f;
    [SerializeField, FoldoutGroup("Advance Settings"), ShowIf(nameof(easeMove))] Ease easing = Ease.InOutSine;
    [SerializeField, FoldoutGroup("Advance Settings")] Vector3 emptyPosition = new Vector3(1, 0, 0);
    [SerializeField, FoldoutGroup("Advance Settings")] Vector3 fullPosition = new Vector3(0, 0, 0);

    [Header("References")]
    [SerializeField, ChildGameObjectsOnly] SpriteRenderer barBGRenderer;
    [SerializeField, ChildGameObjectsOnly] SpriteRenderer barRenderer;
    [SerializeField, ChildGameObjectsOnly] Transform mask;

    [ContextMenu("Test")]
    void test()
    {
        UpdateFillAmount(50f);
    }
    public void UpdateFillAmount(float newFillAmount)
    {
        currentFill = Mathf.Clamp(newFillAmount, 0, maxFillAmount);
        UpdateBar(newFillAmount);
    }

    void UpdateBar(float newFillAmount)
    {
        float FillAmount;
        if (inverseFillBar)
            FillAmount = Mathf.InverseLerp(maxFillAmount, 0, currentFill);
        else
            FillAmount = Mathf.InverseLerp(0, maxFillAmount, currentFill);

        Vector3 maskPos = new Vector3(Mathf.InverseLerp(emptyPosition.x, fullPosition.x, FillAmount), 0, 0);
        if (easeMove)
            mask.DOLocalMove(maskPos, moveDuration).SetEase(easing);
        else
            mask.localPosition = maskPos;
    }
    public void UpdateMaxFillAmount(float maxFillAmount)
    {
        this.maxFillAmount = maxFillAmount;
    }
}
