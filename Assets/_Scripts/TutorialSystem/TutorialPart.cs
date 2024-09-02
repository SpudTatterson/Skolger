using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Skolger.Tutorial
{
    [System.Serializable]
    public abstract class BaseStep
    {
        public string name;
        [HideInInspector] protected TutorialPart Part;
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


    [System.Serializable]
    public class PressTabStep : BaseStep
    {
        [SerializeField] private UI.Tabs.TabButton tab;

        public override void Initialize()
        {
            base.Initialize();
            tab.OnSelected.AddListener(Finish);
        }
    }

    [System.Serializable]
    public class PressButtonStep : BaseStep
    {
        [SerializeField] private Button button;

        public override void Initialize()
        {
            base.Initialize();
            button.onClick.AddListener(Finish);
        }
    }
}

