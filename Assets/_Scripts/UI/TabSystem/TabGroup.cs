using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skolger.UI.Tabs
{
    public class TabGroup : MonoBehaviour
    {
        List<TabButton> tabButtons = new List<TabButton>();
        TabButton selectedTab;
        [SerializeField] bool requireSelectedTab;

        public void Subscribe(TabButton button)
        {
            tabButtons.Add(button);
        }

        public void UnSubscribe(TabButton button)
        {
            tabButtons.Remove(button);
        }

        public void OnTabEnter(TabButton button)
        {
            ResetTabVisuals();
        }
        public void OnTabExit(TabButton button)
        {
            ResetTabVisuals();
        }
        public void OnTabSelected(TabButton button)
        {
            if (selectedTab != null && button == selectedTab && !requireSelectedTab)
            {
                selectedTab.OnDeselect();
                selectedTab = null;
                ResetTabVisuals();
                return;
            }
            selectedTab?.OnDeselect();

            selectedTab = button;
            selectedTab.OnSelect();

            ResetTabVisuals();
        }
        public void ResetTabVisuals()
        {
            foreach (TabButton button in tabButtons)
            {
                button.ResetTab();
            }
        }
    }

}
