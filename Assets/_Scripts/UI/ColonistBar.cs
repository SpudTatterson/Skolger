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
    [SerializeField] InfoContainer[] infoContainers;

    private void Update()
    {
        if (colonist == null)
        {
            Destroy(gameObject);
        }
    }

    public void SetDataOnCreation(string colonistName, ColonistData colonist)
    {
        this.colonistName.text = colonistName;
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
}
