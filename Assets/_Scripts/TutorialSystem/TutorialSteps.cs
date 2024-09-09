using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Skolger.Tutorial
{
    [System.Serializable]
    public abstract class BaseStep
    {
        public string name;
        public bool finished { get; protected set; }

        [SerializeReference] private List<VisualAid> visualAids = new List<VisualAid>();

        public virtual void Initialize()
        {
            foreach (var visualAid in visualAids)
            {
                visualAid.Initialize();
            }
        }

        public virtual void Update()
        {
            foreach (var visualAid in visualAids)
            {
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
            if (Steps.Count != 0)
                Steps[0].Initialize();
        }
        public override void Update()
        {
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
        [SerializeField] private UI.Tabs.TabButton tab;

        public override void Initialize()
        {
            base.Initialize();
            tab.OnSelected.AddListener(Finish);
        }
    }

    public class PressButtonStep : BaseStep
    {
        [SerializeField] private Button button;

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

        private void OnStockpilePlaced()
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

    public class HarvestStep : BaseStep
    {
        [SerializeField] BaseHarvestable harvestable;

        public override void Initialize()
        {
            base.Initialize();
            if (harvestable != null)
                harvestable.OnHarvested += Finish;
            else
                Finish();
        }

        public override void Finish()
        {
            base.Finish();
            harvestable.OnHarvested -= Finish;
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
}

