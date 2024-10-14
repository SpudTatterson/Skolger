using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Skolger.UI.Tabs
{
    public class TabGroup : MonoBehaviour
    {
        [System.Serializable]
        private class TabShortCut
        {
            public TabButton tab;
            public KeyCode shortcut;
        }
        const string closeAllTabsTooltip = "Set to None if you dont want a shortcut to close all tabs";
        [SerializeField] List<TabButton> tabButtons = new List<TabButton>();
        TabButton selectedTab;
        [SerializeField] bool requireSelectedTab;

        [SerializeField, HideIf(nameof(requireSelectedTab)), Tooltip(closeAllTabsTooltip)] KeyCode closeAllTabs = KeyCode.Mouse1;
        [SerializeField] List<TabShortCut> tabShortCuts = new List<TabShortCut>();

        void Update()
        {
            if (!requireSelectedTab && closeAllTabs != KeyCode.None && Input.GetKeyDown(closeAllTabs))
                CloseAllTabs();

            foreach (var shortCut in tabShortCuts)
            {
                if (Input.GetKeyDown(shortCut.shortcut))
                    OnTabSelected(shortCut.tab);
            }
        }
        public void Subscribe(TabButton button)
        {
            if (!tabButtons.Contains(button))
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
                CloseAllTabs();
                return;
            }
            selectedTab?.OnDeselect();

            selectedTab = button;
            selectedTab.OnSelect();

            ResetTabVisuals();
        }

        private void CloseAllTabs()
        {
            selectedTab?.OnDeselect();
            selectedTab = null;
            ResetTabVisuals();
        }

        public void ResetTabVisuals()
        {
            foreach (TabButton button in tabButtons)
            {
                button.ResetTab();
            }
        }

        public void TriggerTab(int index)
        {
            OnTabSelected(tabButtons[index]);
        }

    }
}
