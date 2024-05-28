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
    public GameObject multipleSelection;
    public Transform multipleSelectionContent;
    public TextMeshProUGUI defaultTextAsset;

    [Header("Selection Action Buttons")]
    public GameObject harvestButton;
    public GameObject allowButton;
    public GameObject disallowButton;
    public GameObject cancelButton;
    public GameObject deconstructButton;


    public static UIManager instance { get; private set; }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.Log("More then 1 UIManager Exists");
    }

    public void SetAllSelectionUIInactive()
    {
        itemSelection.gameObject.SetActive(false);
        harvestableSelection.gameObject.SetActive(false);
        multipleSelection.SetActive(false);
        constructableSelection.gameObject.SetActive(false);
        buildingSelection.gameObject.SetActive(false);
    }
    public void SetAllActionButtonsInactive()
    {
        disallowButton.SetActive(false);
        allowButton.SetActive(false);
        harvestButton.SetActive(false);
        cancelButton.SetActive(false);
        deconstructButton.SetActive(false);
    }
    public void EnableAllowDisallowButton(bool allowed)
    {
        allowButton.SetActive(!allowed);
        disallowButton.SetActive(allowed);
    }

    public void EnableItemButtons(bool allowed)
    {
        SetAllActionButtonsInactive();
        EnableAllowDisallowButton(allowed);
        
    }
    public void EnableCancelButton()
    {
        SetAllActionButtonsInactive();
        cancelButton.SetActive(true);
    }

    public void EnableConstructableButtons(bool allowed)
    {
        SetAllActionButtonsInactive();
        EnableAllowDisallowButton(allowed);
        cancelButton.SetActive(true);
    }

    public void EnableBuildingButtons()
    {
        SetAllActionButtonsInactive();
        deconstructButton.SetActive(true);
    }

    public void EnableHarvestableButtons()
    {
        SetAllActionButtonsInactive();
        harvestButton.SetActive(true);
    }

    public void EnableColonistButtons()
    {
        throw new NotImplementedException();
    }

    public void EnableButtons(SelectionType type, bool allowed)
    {
        if (type == SelectionType.Colonist)
            EnableColonistButtons();
        else if (type == SelectionType.Harvestable)
            EnableHarvestableButtons();
        else if (type == SelectionType.Item)
            EnableItemButtons(allowed);
        else if (type == SelectionType.Constructable)
            EnableConstructableButtons(allowed);
        else if (type == SelectionType.Building)
            EnableBuildingButtons();

        SelectionManager.instance.CheckForCancelableAction();
    }
}
