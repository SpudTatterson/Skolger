using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class ColonistData : MonoBehaviour, ISelectable
{

    [SerializeField, ChildGameObjectsOnly, BoxGroup("References")] GameObject visual;
    [field: SerializeField, ChildGameObjectsOnly, BoxGroup("References")] public ColonistMoodManager moodManager { get; private set; }
    [field: SerializeField, ChildGameObjectsOnly, BoxGroup("References")] public HungerManager hungerManager { get; private set; }
    [field: SerializeField, ChildGameObjectsOnly, BoxGroup("References")] public RestManger restManger { get; private set; }
    [field: SerializeField, ChildGameObjectsOnly, BoxGroup("References")] public NavMeshAgent agent { get; private set; }
    [field: SerializeField, ChildGameObjectsOnly, BoxGroup("References")] public ColonistInventory inventory { get; private set; }

    [field: SerializeField] public BrainState brainState { get; private set; } = BrainState.Unrestricted;

    [Header("Portrait")]
    [SerializeField] int width = 256;
    [SerializeField] int height = 256;
    public Sprite faceSprite { get; private set; }

    public event Action<string> OnActivityChanged;
    [HideInInspector] public string colonistName { get; private set; }
    private string _colonistActivity;
    [HideInInspector]
    public string colonistActivity
    {
        get => _colonistActivity;
        set
        {
            if (_colonistActivity != value)
            {
                _colonistActivity = value;
                OnActivityChanged?.Invoke(_colonistActivity);
            }
        }
    }

    [Header("Selection")]
    [SerializeField] Outline outline;
    public bool IsSelected { get; private set; }
    void Awake()
    {
        restManger.OnSleep += Sleep;
        restManger.OnWakeUp += WakeUp;

        colonistName = ColonistUtility.SetRandomName();
    }

    void WakeUp()
    {
        visual.SetActive(true);
        SetBrainState(BrainState.Unrestricted);   //return to other brain state
    }

    void Sleep()
    {
        visual.SetActive(false);
        SetBrainState(BrainState.Sleeping);
    }

    void Start()
    {
        faceSprite = ColonistUtility.CaptureFace(gameObject, 1.75f, new Vector3(0, 1.75f, 1.15f), width, height, 1.5f);
        UIManager.Instance.AddColonistToBoard(colonistName, this);
    }

    void Update()
    {
        hungerManager.GetHungry(Time.deltaTime);
       // restManger.UpdateRest();
        moodManager.UpdateMood();
    }

    public void SetBrainState(BrainState state)
    {
        brainState = state;
    }

    public void ChangeActivity(string activity)
    {
        colonistActivity = activity;
    }


    #region ISelectable

    public SelectionType GetSelectionType()
    {
        return SelectionType.Colonist;
    }

    public ISelectionStrategy GetSelectionStrategy()
    {
        return new ColonistSelectionStrategy();
    }

    public string GetMultipleSelectionString(out int amount)
    {
        amount = 1;
        return colonistName;
    }

    public bool HasActiveCancelableAction()
    {
        return false;
    }

    public void OnSelect()
    {
        SelectionManager manager = SelectionManager.Instance;
        manager.AddToCurrentSelected(this);
        IsSelected = true;

        outline.Enable();
    }
    public void OnDeselect()
    {
        SelectionManager manager = SelectionManager.Instance;
        manager.RemoveFromCurrentSelected(this);
        if (IsSelected)
            manager.UpdateSelection();

        outline.Disable();
        IsSelected = false;
    }

    public void OnHover()
    {
        outline.Enable();
    }

    public void OnHoverEnd()
    {
        outline.Disable();
    }

    #endregion

}