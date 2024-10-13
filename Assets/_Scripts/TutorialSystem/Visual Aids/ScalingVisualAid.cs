using DG.Tweening;
using UnityEngine;

namespace Skolger.Tutorial
{
    [System.Serializable]
    public class ScalingVisualAid : VisualAid
    {
        [SerializeField] Transform[] targetTransforms;
        [SerializeField] Vector3 targetScale = Vector3.one;
        [SerializeField] float time = 1;
        [SerializeField] Ease easeType = Ease.InOutElastic;
        Vector3[] initialScales;
        Tween[] tweens;

        public override void Initialize()
        {
            initialScales = new Vector3[targetTransforms.Length];
            tweens = new Tween[targetTransforms.Length];

            for (int i = 0; i < targetTransforms.Length; i++)
            {
                Transform targetTransform = targetTransforms[i];
                if (targetTransform != null)
                {
                    initialScales[i] = targetTransform.localScale;
                    tweens[i] = targetTransform.DOScale(targetScale, time).SetEase(easeType).SetLoops(-1, LoopType.Yoyo);
                }
            }
        }

        public override void Update()
        {
            for (int i = 0; i < targetTransforms.Length; i++)
            {
                if (targetTransforms[i] == null)
                {
                    tweens[i].Kill(false);
                }
            }
        }

        public override void Reset()
        {
            for (int i = 0; i < targetTransforms.Length; i++)
            {
                Transform targetTransform = targetTransforms[i];
                if (targetTransform != null)
                {
                    targetTransform.localScale = initialScales[i];
                }
            }
            foreach (Tween tween in tweens)
                tween.Kill(true);

        }
    }
}
