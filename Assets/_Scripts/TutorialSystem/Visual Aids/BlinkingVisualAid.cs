using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Skolger.Tutorial
{
    [System.Serializable]
    public class BlinkingVisualAid : VisualAid
    {
        [SerializeField] Graphic targetGraphic;  // Can be a Button, Image, etc.
        [SerializeField] Color blinkColor = Color.white;
        [SerializeField] float time = 3;
        [SerializeField] Ease ease = Ease.InOutElastic;

        Color initialColor;
        Tween tween;
        public override void Initialize()
        {
            if (targetGraphic != null)
            {
                initialColor = targetGraphic.color;
                tween = targetGraphic.DOColor(blinkColor, time).SetEase(ease).SetLoops(-1, LoopType.Yoyo);
            }
        }

        public override void Update()
        {
        }

        public override void Reset()
        {
            if (targetGraphic != null)
            {
                tween.Kill();
                targetGraphic.color = initialColor;
            }
        }
    }
}
