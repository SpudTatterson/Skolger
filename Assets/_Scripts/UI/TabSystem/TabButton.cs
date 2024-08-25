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
        [SerializeField] UnityEvent OnSelected;
        [SerializeField] UnityEvent OnDeselected;
        [Header("Visual Settings")]
        [SerializeField] Color idleColor = new Color(0.9f, 0.9f, 0.9f);
        [SerializeField] Color hoverColor = Color.white;
        [SerializeField] Color selectedColor = new Color(0.7f, 0.7f, 0.7f);

        [Header("Refs")]
        [SerializeField] TabGroup tabGroup;
        Image background;

        bool selected;

        void Awake()
        {
            tabGroup.Subscribe(this);
            background = GetComponent<Image>();
            background.color = idleColor;
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
                background.color = hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tabGroup.OnTabExit(this);
        }

        #endregion

        public void ResetTab()
        {
            if (!selected)
                background.color = idleColor;
        }
        public void ResetSelected()
        {
            selected = false;
        }

        public void OnSelect()
        {
            selected = true;
            background.color = selectedColor;
            OnSelected.Invoke();
        }
        public void OnDeselect()
        {
            ResetSelected();
            OnDeselected.Invoke();
        }
    }
}
