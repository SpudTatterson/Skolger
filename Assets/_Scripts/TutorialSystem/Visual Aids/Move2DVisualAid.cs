using System.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Skolger.Tutorial
{
    public class Move2DVisualAid : VisualAid
    {
        enum PointType
        {
            WorldPoint,
            ScreenPoint
        }

        [SerializeField] RectTransform rectTransform;
        [SerializeField] float time = 1;
        [SerializeField, ShowIf("startType", PointType.WorldPoint)] Transform startWorldPoint;
        [SerializeField, ShowIf("startType", PointType.ScreenPoint)] Vector3 startScreenPoint;
        [SerializeField, Tooltip("Set to world point if you need the point to be converted to screen space")] PointType startType;
        [SerializeField, ShowIf("endType", PointType.WorldPoint)] Transform endWorldPoint;
        [SerializeField, ShowIf("endType", PointType.ScreenPoint)] Vector3 endScreenPoint;
        [SerializeField, Tooltip("Set to world point if you need the point to be converted to screen space")] PointType endType;
        [SerializeField] Ease ease = Ease.Linear;

        [SerializeField] bool loop = true;
        [SerializeField, ShowIf("loop")] bool snapBack = true;
        [SerializeField, ShowIf("snapBack")] float snapBackTime = 1f;

        Tween tween;
        float elapsedTime = 0f;
        bool reverse = false;
        bool active = true;
        public override void Initialize()
        {
            rectTransform.gameObject.SetActive(true);


            if (startType == PointType.WorldPoint)
                startScreenPoint = Camera.main.WorldToScreenPoint(startWorldPoint.position);

            if (endType == PointType.WorldPoint)
                endScreenPoint = Camera.main.WorldToScreenPoint(endWorldPoint.position);

            // Set initial position
            rectTransform.anchoredPosition = startScreenPoint;

        }

        public override void Reset()
        {
            rectTransform.gameObject.SetActive(false);
            tween?.Kill();  // Ensure any running tween is stopped
        }

        public override void Update()
        {
            if (!active) return;
            if (startType == PointType.WorldPoint)
                startScreenPoint = Camera.main.WorldToScreenPoint(startWorldPoint.position);

            if (endType == PointType.WorldPoint)
                endScreenPoint = Camera.main.WorldToScreenPoint(endWorldPoint.position);

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / time);

            if (!reverse)
            {
                t = DOVirtual.EasedValue(0, 1, t, ease);
                rectTransform.anchoredPosition = Vector2.Lerp(startScreenPoint, endScreenPoint, t);
            }
            else
            {
                t = DOVirtual.EasedValue(0, 1, t, Ease.InExpo);
                rectTransform.anchoredPosition = Vector2.Lerp(endScreenPoint, startScreenPoint, t);
            }

            // Handle looping
            if (loop && t >= 1f)
            {
                if (!snapBack)
                    reverse = !reverse;
                else
                {
                    Debug.Log("Working");
                    Debug.Log(snapBackTime);
                    active = false;
                    DOVirtual.DelayedCall(snapBackTime, RestartPosition);
                }
                elapsedTime = 0f;
            }
        }

        public void RestartPosition()
        {
            Debug.Log("Restarted");
            active = true;
            rectTransform.anchoredPosition = startScreenPoint;
        }
    }
}
