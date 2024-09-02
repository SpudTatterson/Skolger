using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Skolger.UI.Tabs
{
    [RequireComponent(typeof(Image))]
    public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Events")]
        public UnityEvent OnSelected;
        public UnityEvent OnDeselected;
        [Header("Visual Settings")]
        [SerializeField] Color idleColor = new Color(0.9f, 0.9f, 0.9f);
        [SerializeField] Color hoverColor = Color.white;
        [SerializeField] Color selectedColor = new Color(0.7f, 0.7f, 0.7f);

        [Header("Refs")]
        [SerializeField] TabGroup tabGroup;
        public Image image;

        bool selected;

        void Awake()
        {
            tabGroup.Subscribe(this);
            image = GetComponent<Image>();
            image.color = idleColor;
        }

        #region Pointer Events

        public void OnPointerClick(PointerEventData eventData)
        {
            tabGroup.OnTabSelected(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tabGroup.OnTabEnter(this);
            if (!selected)
                image.color = hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tabGroup.OnTabExit(this);
        }

        #endregion

        public void ResetTab()
        {
            if (!selected)
                image.color = idleColor;
        }
        public void ResetSelected()
        {
            selected = false;
        }

        public void OnSelect()
        {
            selected = true;
            image.color = selectedColor;
            OnSelected.Invoke();
        }
        public void OnDeselect()
        {
            ResetSelected();
            OnDeselected.Invoke();
        }
    }
}
