using DG.Tweening;
using UnityEngine;

namespace Skolger.Tutorial
{
    [System.Serializable]
    public class ScalingVisualAid : VisualAid
    {
        [SerializeField] Transform targetTransform;
        [SerializeField] Vector3 targetScale = Vector3.one;
        [SerializeField] float time = 1;
        [SerializeField] Ease easeType = Ease.InOutElastic;
        Vector3 initialScale;
        Tween tween;

        public override void Initialize()
        {
            if (targetTransform != null)
            {
                initialScale = targetTransform.localScale;
                tween = targetTransform.DOScale(targetScale, time).SetEase(easeType).SetLoops(-1, LoopType.Yoyo);
            }
        }

        public override void Update()
        {
        }

        public override void Reset()
        {
            if (targetTransform != null)
            {
                tween.Kill(true);
                targetTransform.localScale = initialScale;
            }
        }
    }
}
