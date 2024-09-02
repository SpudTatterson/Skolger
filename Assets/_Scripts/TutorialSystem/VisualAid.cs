using UnityEngine;
using UnityEngine.UI;

namespace Skolger.Tutorial
{
    [System.Serializable]
    public abstract class VisualAid
    {
        public abstract void Initialize();
        public abstract void Update();
        public abstract void Reset();
    }

    [System.Serializable]
    public class BlinkingVisualAid : VisualAid
    {
        [SerializeField] Graphic targetGraphic;  // Can be a Button, Image, etc.
        [SerializeField] Color blinkColor = Color.white;
        [SerializeField] float frequency = 3;

        Color initialColor;

        public override void Initialize()
        {
            if (targetGraphic != null)
            {
                initialColor = targetGraphic.color;
            }
        }

        public override void Update()
        {
            if (targetGraphic != null)
            {
                float t = Mathf.Sin(Time.time * frequency) * 0.5f + 0.5f; // Sine wave oscillating between 0 and 1
                targetGraphic.color = Color.Lerp(initialColor, blinkColor, t);
            }
        }

        public override void Reset()
        {
            if (targetGraphic != null)
            {
                targetGraphic.color = initialColor;
            }
        }
    }
    [System.Serializable]
    public class ScalingVisualAid : VisualAid
    {
        [SerializeField] Transform targetTransform;
        [SerializeField] Vector3 targetScale = Vector3.one;
        [SerializeField] float frequency = 3;
        Vector3 initialScale;

        public override void Initialize()
        {
            if (targetTransform != null)
            {
                initialScale = targetTransform.localScale;
            }
        }

        public override void Update()
        {
            if (targetTransform != null)
            {
                float t = Mathf.Sin(Time.time * frequency) * 0.5f + 0.5f; // Sine wave oscillating between 0 and 1
                targetTransform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            }
        }

        public override void Reset()
        {
            if (targetTransform != null)
            {
                targetTransform.localScale = initialScale;
            }
        }
    }
}
