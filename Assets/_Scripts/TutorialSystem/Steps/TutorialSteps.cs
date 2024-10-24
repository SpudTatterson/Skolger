using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Skolger.Tutorial
{
    [System.Serializable]
    public abstract class BaseStep
    {
        public string name;
        public bool finished { get; protected set; }

        [SerializeReference] List<VisualAid> visualAids = new List<VisualAid>();

        public virtual void Initialize()
        {
            foreach (var visualAid in visualAids)
            {
                visualAid.SetParent(this);
                visualAid.Initialize();
            }
        }

        public virtual void Update()
        {
            foreach (var visualAid in visualAids)
            {
                if (!visualAid.finished)
                    visualAid.Update();
            }
        }

        public virtual void Finish()
        {
            finished = true;
            foreach (var visualAid in visualAids)
            {
                visualAid.Reset();
            }
        }
    }

    [System.Serializable]
    public class TutorialPart : BaseStep
    {
        [SerializeReference] public List<BaseStep> Steps = new List<BaseStep>();

        public override void Initialize()
        {
            base.Initialize();
            if (Steps.Count != 0)
                Steps[0].Initialize();
        }
        public override void Update()
        {
            base.Update();
            if ((Steps.Count > 0) && !Steps[0].finished)
            {
                Steps[0].Update();
            }
            else if (Steps.Count > 0 && Steps[0].finished)
            {
                Steps.RemoveAt(0);
                if (Steps.Count != 0)
                    Steps[0].Initialize();
            }
            else if (Steps.Count == 0)
            {
                Finish();
            }
        }
    }


    public class PressTabStep : BaseStep
    {
        [SerializeField] UI.Tabs.TabButton tab;

        public override void Initialize()
        {
            base.Initialize();
            tab.OnSelected.AddListener(Finish);
        }
    }

    public class PressButtonStep : BaseStep
    {
        [SerializeField] Button button;

        public override void Initialize()
        {
            base.Initialize();
            button.onClick.AddListener(Finish);
        }
    }

    public class StockpilePlacementStep : BaseStep
    {
        public override void Initialize()
        {
            base.Initialize();

            StockpilePlacer.Instance.onStockpilePlaced.AddListener(OnStockpilePlaced);
        }

        void OnStockpilePlaced()
        {
            Finish();
        }

        public override void Finish()
        {
            StockpilePlacer.Instance.onStockpilePlaced.RemoveListener(OnStockpilePlaced);

            base.Finish();
        }
    }
    public class CompositeStep : BaseStep
    {
        [SerializeReference] List<BaseStep> Steps = new List<BaseStep>();

        public override void Initialize()
        {
            base.Initialize();
            foreach (var step in Steps)
            {
                step.Initialize();
            }
        }

        public override void Update()
        {
            base.Update();
            List<BaseStep> finishedSteps = new List<BaseStep>();
            foreach (var step in Steps)
            {
                if (!step.finished)
                    step.Update();
                else
                    finishedSteps.Add(step);
            }
            foreach (var step in finishedSteps)
            {
                Steps.Remove(step);
            }
            if (Steps.Count == 0)
                Finish();
        }
    }

    public class HarvestStep : BaseStep, INumberedStep
    {
        [SerializeField] BaseHarvestable[] harvestables;

        public event Action<float> OnNumberChange;

        public float max => needToHarvest;

        public float current => harvested;

        int needToHarvest;
        int harvested;
        public override void Initialize()
        {
            needToHarvest = harvestables.Length;
            base.Initialize();
            foreach (var harvestable in harvestables)
            {
                if (harvestable != null)
                    harvestable.OnHarvested += OnHarvested;
                else
                    OnHarvested();
            }
        }

        void OnHarvested()
        {
            harvested++;
            OnNumberChange?.Invoke(harvested);

            if (harvested >= needToHarvest)
            {
                Finish();
            }
        }
    }
    public class GetItemStep : BaseStep
    {
        [SerializeField] ItemCost itemNeed;

        public override void Initialize()
        {
            base.Initialize();
            InventoryManager.Instance.OnInventoryUpdated += CheckForItem;
        }

        void CheckForItem(ItemData data, int amount)
        {
            if (InventoryManager.Instance.HasItem(itemNeed))
                Finish();
        }

        public override void Finish()
        {
            base.Finish();

            InventoryManager.Instance.OnInventoryUpdated -= CheckForItem;
        }

    }

    public class WaitForTimeStep : BaseStep
    {
        [SerializeField, Range(0, 24)] float timeOfDay;
        public override void Initialize()
        {
            base.Initialize();

            DayNightEventManager.Instance.GetEvent(timeOfDay).AddListener(Finish);
        }
        public override void Finish()
        {
            base.Finish();

            DayNightEventManager.Instance.GetEvent(timeOfDay).RemoveListener(Finish);
        }
    }
    public class PressKeyStep : BaseStep, INumberedStep
    {
        [SerializeField] List<KeyCode> keyCodes = new List<KeyCode>();
        [SerializeField] int neededNumberOfPresses = 4;

        int numberOfPresses;

        public float max => neededNumberOfPresses;

        public float current => numberOfPresses;

        public event Action<float> OnNumberChange;

        public override void Update()
        {
            base.Update();

            foreach (var key in keyCodes)
            {
                if (Input.GetKeyDown(key))
                {
                    numberOfPresses++;
                    OnNumberChange?.Invoke(numberOfPresses);
                }
            }

            if (numberOfPresses >= neededNumberOfPresses)
            {
                Finish();
            }
        }
    }

    public class PressAnyKeyStep : BaseStep
    {
        [SerializeField] float timeBeforeCanClick = 1;

        float time;

        public override void Update()
        {
            base.Update();

            time += Time.unscaledDeltaTime;
            if (time > timeBeforeCanClick && Input.anyKeyDown)
            {
                Finish();
            }
        }
    }

    public class AllowStep : BaseStep, INumberedStep
    {
        [SerializeField, ValidateInput("ValidateAllowables", "All GameObjects must have an IAllowable component")] List<GameObject> allowableObjects;

        public float max => amountToAllow;

        public float current => allowed;

        public event Action<float> OnNumberChange;

        int amountToAllow;
        int allowed;
        List<IAllowable> allowables = new List<IAllowable>();
        public override void Initialize()
        {
            allowableObjects.ForEach(gameObject => allowables.Add(gameObject.GetComponent<IAllowable>()));
            amountToAllow = allowables.Count;

            base.Initialize();
        }
        public override void Update()
        {
            base.Update();
            int allowed = 0;
            foreach (var allowable in allowables)
            {
                if (allowable.IsAllowed())
                    allowed++;
            }

            if (allowed >= amountToAllow)
                Finish();

            if (this.allowed != allowed)
            {
                this.allowed = allowed;
                OnNumberChange?.Invoke(this.allowed);
            }
        }

        bool ValidateAllowables(List<GameObject> gameObjects)
        {
            foreach (var go in gameObjects)
            {
                if (go == null || go.GetComponent<IAllowable>() == null)
                {
                    return false;
                }
            }
            return true;
        }
    }
    public class FinishConstructionStep : BaseStep 
    {
        public override void Update()
        {
            base.Update();

            if(!TaskManager.Instance.CheckIfConstructionTaskExists())
            {
                Finish();
            }
        }
    }
}

