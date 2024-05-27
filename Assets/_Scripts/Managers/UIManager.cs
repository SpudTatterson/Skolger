using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Selection")]
    public GameObject selectionPanel;
    public HarvestableSelectionMenu harvestableSelection;
    public ItemSelectionMenu itemSelection;
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
}
