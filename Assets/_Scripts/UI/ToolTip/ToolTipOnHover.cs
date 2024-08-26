using UnityEngine;
using UnityEngine.EventSystems;
namespace Skolger.UI.ToolTip
{
    public class ToolTipOnHover : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] string textOnHover;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTip.ShowToolTip_Static(textOnHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTip.HideToolTip_Static();
    }
}
}
