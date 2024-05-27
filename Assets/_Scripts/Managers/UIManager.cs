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
    public GameObject multipleSelection;
    public Transform multipleSelectionContent;
    public TextMeshProUGUI defaultTextAsset;

    [Header("Selection Action Buttons")]
    public GameObject harvestButton;
    public GameObject allowButton;
    public GameObject cancelButton;


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
    }
    public void SetAllActionButtonsInactive()
    {

        allowButton.SetActive(false);
        harvestButton.SetActive(false);
        cancelButton.SetActive(false);
    }

    public void EnableItemButtons()
    {
        SetAllActionButtonsInactive();
        allowButton.SetActive(true);
    }
    public void EnableCancelButton()
    {
        SetAllActionButtonsInactive();
        cancelButton.SetActive(true);
    }

    public void EnableConstructableButtons()
    {
        SetAllActionButtonsInactive();
        allowButton.SetActive(true);
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

        public void EnableButtons(SelectionType type)
    {
        if (type == SelectionType.Colonist)
            EnableColonistButtons();
        else if (type == SelectionType.Harvestable)
            EnableHarvestableButtons();
        else if (type == SelectionType.Item)
            EnableItemButtons();
        else if (type == SelectionType.Constructable)
            EnableConstructableButtons();

        SelectionManager.instance.CheckForCancelableAction();
    }
}
