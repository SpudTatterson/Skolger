using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Skolger.UI
{

    public class DropDown : MonoBehaviour, IPointerClickHandler
    {
        [Header("Refs")]
        [SerializeField] Image dropDownStatus;
        [SerializeField, ChildGameObjectsOnly] Transform dropDownObjectsParent;
        [SerializeField] bool rebuildLayout = true;
        [SerializeField, ShowIf("rebuildLayout")] RectTransform layoutToRebuild;
        // [Header("Visual Settings")]
        // [SerializeField] Sprite openDropDown;
        // [SerializeField] Sprite closeDropDown;
        bool active = true;

        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.pointerEnter != gameObject) return;
            foreach (Transform child in dropDownObjectsParent)
            {
                if (child.GetSiblingIndex() == 0) continue;
                child.gameObject.SetActive(!active);
            }
            if (rebuildLayout)
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutToRebuild);

            active = !active;
            if (active)
                (dropDownStatus.transform as RectTransform).Rotate(0, 0, 180);
            else
                (dropDownStatus.transform as RectTransform).Rotate(0, 0, 180);

            ToolTip.ToolTip.HideToolTip_Static();
        }

        public void SetLayoutToRebuild(RectTransform layout)
        {
            rebuildLayout = true;
            layoutToRebuild = layout;
        }

    }
}
