using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Sirenix.OdinInspector;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("Selection")]
    [TabGroup("Selection Panels")] public GameObject selectionPanel;
    [TabGroup("Selection Panels")] public ColonistSelectionMenu colonistSelection;
    [TabGroup("Selection Panels")] public HarvestableSelectionMenu harvestableSelection;
    [TabGroup("Selection Panels")] public ItemSelectionMenu itemSelection;
    [TabGroup("Selection Panels")] public ConstructableSelectionMenu constructableSelection;
    [TabGroup("Selection Panels")] public BuildingSelectionMenu buildingSelection;
    [TabGroup("Selection Panels")] public StockpileSelectionMenu stockpileSelection;
    [TabGroup("Selection Panels")] public GameObject multipleSelection;
    [TabGroup("Selection Assets")] public Transform multipleSelectionContent;
    [TabGroup("Selection Assets")] public GameObject multipleSelectionTextAsset; 
    [TabGroup("Selection Assets")] public RectTransform selectionBoxImage;

    [Header("Selection Action Buttons")]
    [TabGroup("Selection Action")] public GameObject harvestButton;
    [TabGroup("Selection Action")] public GameObject allowButton;
    [TabGroup("Selection Action")] public GameObject disallowButton;
    [TabGroup("Selection Action")] public GameObject cancelButton;
    [TabGroup("Selection Action")] public GameObject deconstructButton;
    [TabGroup("Selection Action")] public GameObject growZoneButton;
    [TabGroup("Selection Action")] public GameObject shrinkZoneButton;
    [Space(10f)]
    [Header("Selection Action UI")]
    [TabGroup("Selection Action")] public GameObject SelectionActionCanvas;
    [TabGroup("Selection Action")] public TextMeshProUGUI actionText;

    [Header("Colonist Info Panel")]
    [TabGroup("Colonist Info")] public GameObject colonistInfoPanel;
    [TabGroup("Colonist Info")] public TextMeshProUGUI nameText;
    [TabGroup("Colonist Info")] public TextMeshProUGUI activityText;
    [TabGroup("Colonist Info")]
    [Space]
    [TabGroup("Colonist Info")] public GameObject colonistsBoard;
    [TabGroup("Colonist Info")] public GameObject colonistDataPrefab;
    [TabGroup("Colonist Info")] private ColonistData currentColonist;

    [Space(5f), Header("Inventory")]
    [TabGroup("Inventory")] public GameObject defaultInventoryUIPrefab;
    [TabGroup("Inventory")] public GameObject itemTypeGroupPrefab;
    [TabGroup("Inventory")] public Transform inventoryPanel;


    #region SelectionUI
    public void SetAllSelectionUIInactive()
    {
        itemSelection.gameObject.SetActive(false);
        harvestableSelection.gameObject.SetActive(false);
        multipleSelection.SetActive(false);
        constructableSelection.gameObject.SetActive(false);
        buildingSelection.gameObject.SetActive(false);
        stockpileSelection.gameObject.SetActive(false);
        colonistSelection.gameObject.SetActive(false);
    }
    public void SetAllActionButtonsInactive()
    {
        disallowButton.SetActive(false);
        allowButton.SetActive(false);
        harvestButton.SetActive(false);
        cancelButton.SetActive(false);
        deconstructButton.SetActive(false);
        growZoneButton.SetActive(false);
        shrinkZoneButton.SetActive(false);
    }
    public void ResizeSelectionBox(Vector3 mouseStart, Vector3 mouseEnd)
    {
        if (!selectionBoxImage.gameObject.activeSelf)
            selectionBoxImage.gameObject.SetActive(true);

        float width = mouseEnd.x - mouseStart.x;
        float height = mouseEnd.y - mouseStart.y;

        selectionBoxImage.anchoredPosition = mouseStart + new Vector3(width / 2, height / 2);
        selectionBoxImage.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
    }
    #endregion

    #region SelectionUIButtons
    public void EnableAllowDisallowButton(bool allowed)
    {
        allowButton.SetActive(!allowed);
        disallowButton.SetActive(allowed);
    }

    public void EnableCancelButton()
    {
        cancelButton.SetActive(true);
    }
    #endregion

    #region  Colonist Info Panel
    public void ShowColonistWindow(string name, string activity)
    {
        nameText.text = name;
        activityText.text = activity;
        colonistInfoPanel.SetActive(true);
    }

    public void HideColonistWindow()
    {
        colonistInfoPanel.SetActive(false);
    }

    public void SetCurrentColonist(ColonistData colonist)
    {
        if (currentColonist != null)
        {
            currentColonist.OnActivityChanged -= UpdateActivityDisplay;
        }

        currentColonist = colonist;
        currentColonist.OnActivityChanged += UpdateActivityDisplay;

        UpdateActivityDisplay(currentColonist.colonistActivity);
    }

    private void UpdateActivityDisplay(string activity)
    {
        activityText.text = activity;
        if (!colonistInfoPanel.activeSelf)
        {
            colonistInfoPanel.SetActive(true);
        }
    }

    public void AddColonistToBoard(string name, ColonistData colonist)
    {
        var colonistsDataBar = Instantiate(colonistDataPrefab, colonistsBoard.transform);
        var data = colonistsDataBar.GetComponent<ColonistBar>();
        data.SetDataOnCreation(name, colonist);
    }
    #endregion
}
