using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Skolger.UI
{
    public class MaskBox : MonoBehaviour
    {
        [SerializeField] RectTransform mask;
        [SerializeField] TextMeshProUGUI textBox;
        [SerializeField] Image bgImage;

        public void UpdateMask(Vector2 position, Vector2 sizeDelta, string text, bool blockRayCast)
        {
            mask.localPosition = position;
            mask.sizeDelta = sizeDelta;

            textBox.text = text;

            RectTransform textRect = textBox.transform as RectTransform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(textRect);
            textRect.localPosition = new Vector2(position.x, position.y + sizeDelta.y);
            // textRect.localPosition = VectorUtility.ClampPositionToScreen(textRect.localPosition, textRect.sizeDelta, sizeDelta);
            textRect.localPosition = VectorUtility.OffsetPositionToScreen(textRect.localPosition, sizeDelta);
            textRect.localPosition = VectorUtility.ClampPositionToScreen(textRect.localPosition, textRect.sizeDelta);

            bgImage.raycastTarget = blockRayCast;
        }
    }
}
