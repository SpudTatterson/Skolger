using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [Header("Selection")]
    public GameObject selectionPanel;
    public HarvestableSelectionMenu harvestableSelection;
    public ItemSelectionMenu itemSelection;
    public ConstructableSelectionMenu constructableSelection;
    public BuildingSelectionMenu buildingSelection; 
    public StockpileSelectionMenu stockpileSelection;
    public GameObject multipleSelection;
    public Transform multipleSelectionContent;
    public TextMeshProUGUI defaultTextAsset;
    public RectTransform selectionBoxImage;

    [Header("Selection Action Buttons")]
    public GameObject harvestButton;
    public GameObject allowButton;
    public GameObject disallowButton;
    public GameObject cancelButton;
    public GameObject deconstructButton;
    public GameObject growZoneButton;
    public GameObject shrinkZoneButton;

    [Header("Colonist Info Panel")]
    public GameObject colonistInfoPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI activityText;
    private ColonistData currentColonist;

    public static UIManager instance { get; private set; }

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

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.Log("More then 1 UIManager Exists");

            colonistInfoPanel.SetActive(false);
    }

    #region SelectionUI
    public void SetAllSelectionUIInactive()
    {
        itemSelection.gameObject.SetActive(false);
        harvestableSelection.gameObject.SetActive(false);
        multipleSelection.SetActive(false);
        constructableSelection.gameObject.SetActive(false);
        buildingSelection.gameObject.SetActive(false);
        stockpileSelection.gameObject.SetActive(false);
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
        if(!selectionBoxImage.gameObject.activeSelf)
            selectionBoxImage.gameObject.SetActive(true);

        float width = mouseEnd.x - mouseStart.x;
        float height = mouseEnd.y - mouseStart.y;

        selectionBoxImage.anchoredPosition = mouseStart + new Vector3(width/2, height/2);
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
        SetAllActionButtonsInactive();
        cancelButton.SetActive(true);
    }
    #endregion

    #region  Colonist Info Panel
    public void ShowColonistPanel(string name, string activity)
    {
        nameText.text = name;
        activityText.text = activity;
        colonistInfoPanel.SetActive(true);
    }

    public void HideColonistPanel()
    {
        colonistInfoPanel.SetActive(false);
    }
    #endregion
}
