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

        public abstract void Initialize();
        public abstract void Update();
        public virtual void Finish()
        {
            finished = true;
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
        [SerializeField] UI.Tabs.TabButton tab;
        [SerializeField] Color blinkColor;

        Color initialColor;

        public override void Initialize()
        {
            initialColor = tab.image.color;
            tab.OnSelected.AddListener(Finish);
        }

        public override void Finish()
        {
            base.Finish();
            tab.image.color = initialColor;
        }

        public override void Update()
        {
            float t = Mathf.Sin(Time.time * 3f) * 0.5f + 0.5f; // Sine wave oscillating between 0 and 1
            tab.image.color = Color.Lerp(initialColor, blinkColor, t);
        }
    }
    [System.Serializable]
    public class PressButtonStep : BaseStep
    {
        [SerializeField] Button button;
        [SerializeField] Color blinkColor;

        Color initialColor;

        public override void Initialize()
        {
            initialColor = button.image.color;
            button.onClick.AddListener(Finish);
        }

        public override void Finish()
        {
            base.Finish();
            button.image.color = initialColor;
        }

        public override void Update()
        {
            float t = Mathf.Sin(Time.time * 3f) * 0.5f + 0.5f; // Sine wave oscillating between 0 and 1
            button.image.color = Color.Lerp(initialColor, blinkColor, t);
        }
    }
}
