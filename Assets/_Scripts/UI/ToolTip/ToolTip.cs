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
            Vector2 mousePosition = Input.mousePosition;
            AdjustPosition(mousePosition);
        }

        void AdjustPosition(Vector2 mousePosition)
        {
            // Set the tooltip position to follow the mouse, adding the offset
            transform.position = mousePosition + offset;

            // Get the screen boundaries
            Vector2 screenBounds = new Vector2(Screen.width, Screen.height);

            // Get the tooltip size
            Vector2 backgroundSize = background.sizeDelta;

            // Clamp the position to ensure the tooltip stays within screen bounds
            Vector2 clampedPosition = transform.position;

            // Check if tooltip exceeds right or left bounds
            if (clampedPosition.x + backgroundSize.x > screenBounds.x) // Right side
            {
                clampedPosition.x = screenBounds.x - backgroundSize.x;
            }
            if (clampedPosition.x < 0) // Left side
            {
                clampedPosition.x = 0;
            }

            // Check if tooltip exceeds top or bottom bounds
            if (clampedPosition.y + backgroundSize.y > screenBounds.y) // Top
            {
                clampedPosition.y = screenBounds.y - backgroundSize.y;
            }
            if (clampedPosition.y < 0) // Bottom
            {
                clampedPosition.y = 0;
            }

            // Set the tooltip position to the clamped position
            transform.position = clampedPosition;
        }

        void ShowToolTip(string tooltipString)
        {
            gameObject.SetActive(true);
            text.text = tooltipString;

            // Update background size
            Vector2 backgroundSize = new Vector2(text.preferredWidth + textPaddingSize * 2, text.preferredHeight + textPaddingSize * 2);
            background.sizeDelta = backgroundSize;

            // Adjust position immediately to prevent flickering
            AdjustPosition(Input.mousePosition);
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
