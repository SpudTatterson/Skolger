using UnityEngine;
using UnityEngine.EventSystems;
namespace Skolger.UI.ToolTip
{
    public class ToolTipOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        const string IgnoreClicksTooltip = "If this is causing issues with image on top of tab make sure to turn of raycast target on specific child";

        [SerializeField] string textOnHover;
        [SerializeField, Tooltip(IgnoreClicksTooltip)] bool ignoreClicksOnChildGameObjects = true;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (ignoreClicksOnChildGameObjects && eventData.pointerEnter == gameObject)
                ToolTip.ShowToolTip_Static(textOnHover);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ToolTip.HideToolTip_Static();
        }

        public void SetHoverText(string hoverText)
        {
            textOnHover = hoverText;
        }
    }
}
