using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Skolger.UI.InfoContainers
{
    public abstract class InfoContainer : MonoBehaviour
    {
        protected ColonistData colonist;
        [SerializeField] protected TextMeshProUGUI statusText;
        [SerializeField] protected Image fillUpBarImage;
        [SerializeField] protected Color emptyColor = Color.red;
        [SerializeField] protected Color fullColor = Color.green;

        void Awake()
        {
            ColonistBar colonistBar = GetComponentInParent<ColonistBar>();
            if (colonistBar != null)
                colonist = colonistBar.colonist;
            else
            {
                ColonistSelectionMenu selectionMenu = GetComponentInParent<ColonistSelectionMenu>();
                colonist = selectionMenu.colonist;
            }

        }
        protected float GetFillPercentage(float full, float empty, float current)
        {
            return Mathf.InverseLerp(full, empty, current);
        }
    }
}