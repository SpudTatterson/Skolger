using System.Collections.Generic;
using Skolger.UI.InfoContainers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ColonistBar : MonoBehaviour
{
    public ColonistData colonist { get; private set; }
    [SerializeField] Image colonistFace;
    [SerializeField] TextMeshProUGUI colonistName;
    [SerializeField] TextMeshProUGUI activity;
    [SerializeField] Button colonistButton;

    [Header("Extended info tab")]
    [SerializeField] GameObject infoTab;
    [SerializeField] InfoContainer[] infoContainers;

    [Header("Priority tab")]
    [SerializeField] GameObject priorities;


    private void Update()
    {
        if (colonist == null)
        {
            Destroy(gameObject);
        }
    }

    public void SetDataOnCreation(ColonistData colonist)
    {
        this.colonistName.text = colonist.colonistName;
        this.colonist = colonist;
        colonistFace.sprite = colonist.faceSprite;

        if (this.colonist != null)
        {
            this.colonist.OnActivityChanged += UpdateActivity;
            colonistButton.onClick.AddListener(FocusOnColonist);
        }

        foreach (InfoContainer infoContainer in infoContainers)
        {
            infoContainer.Initialize(colonist);
        }
    }
    void FocusOnColonist()
    {
        StartCoroutine(CameraController.Instance.SendCameraToTarget(colonist.transform.position));
    }
    private void UpdateActivity(string activity)
    {
        this.activity.text = activity;
    }

    private void OnDestroy()
    {
        if (colonist != null)
        {
            colonist.OnActivityChanged -= UpdateActivity;
        }
    }

    public void TurnOnInfoTab()
    {
        infoTab.SetActive(true);
    }

    public void TurnOnPriorityTab()
    {
        priorities.SetActive(true);
    }
    public void TurnOffAllTabs()
    {
        infoTab.SetActive(false);
        priorities.SetActive(false);
    }
}
