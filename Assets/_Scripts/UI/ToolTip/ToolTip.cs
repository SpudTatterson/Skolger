using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Skolger.UI.ToolTip
{
    public class ToolTip : MonoBehaviour
    {
        static ToolTip instance;
        [SerializeField, ChildGameObjectsOnly, Required] TextMeshProUGUI text;
        [SerializeField, ChildGameObjectsOnly, Required] RectTransform background;
        [SerializeField] float textPaddingSize = 5f;
        [SerializeField] Vector2 offset = new Vector2(10f, 10f);  // Offset to avoid overlapping with the mouse cursor

        void Awake()
        {
            instance = this;
            HideToolTip();
        }

        void LateUpdate()
        {
            transform.position = VectorUtility.OffsetPositionToScreen(Input.mousePosition, background.sizeDelta, offset);
        }

        void ShowToolTip(string tooltipString)
        {
            gameObject.SetActive(true);
            text.text = tooltipString;

            // Update background size
            Vector2 backgroundSize = new Vector2(text.preferredWidth + textPaddingSize * 2, text.preferredHeight + textPaddingSize * 2);
            background.sizeDelta = backgroundSize;

            // Adjust position immediately to prevent flickering
            transform.position = VectorUtility.OffsetPositionToScreen(Input.mousePosition, background.sizeDelta, offset);
        }

        void HideToolTip()
        {
            gameObject.SetActive(false);
        }

        public static void ShowToolTip_Static(string tooltipString)
        {
            instance.ShowToolTip(tooltipString);
        }

        public static void HideToolTip_Static()
        {
            instance.HideToolTip();
        }
    }
}
