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
        [ChildGameObjectsOnly] public Transform dropDownObjectsParent;
        // [Header("Visual Settings")]
        // [SerializeField] Sprite openDropDown;
        // [SerializeField] Sprite closeDropDown;
        bool active = true;
        ContentSizeFitter contentSizeFitter;

        void Awake()
        {
            contentSizeFitter = GetComponentInParent<ContentSizeFitter>();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            foreach (Transform child in dropDownObjectsParent)
            {
                if (child.GetSiblingIndex() == 0) continue;
                child.gameObject.SetActive(!active);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(UIManager.Instance.inventoryPanel as RectTransform);
            active = !active;
            if (active)
            {
                (dropDownStatus.transform as RectTransform).Rotate(0, 0, 180);
            }
            else
            {
                (dropDownStatus.transform as RectTransform).Rotate(0, 0, 180);
            }
        }
    }
}
