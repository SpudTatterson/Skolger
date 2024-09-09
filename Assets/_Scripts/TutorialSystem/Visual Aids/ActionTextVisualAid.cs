using UnityEngine;

namespace Skolger.Tutorial
{
    public class ActionTextVisualAid : VisualAid
    {
        [SerializeField] string text;
        public override void Initialize()
        {
            UIManager.Instance.actionText.text = text;
            UIManager.Instance.SelectionActionCanvas.SetActive(true);
        }

        public override void Reset()
        {
            UIManager.Instance.SelectionActionCanvas.SetActive(false);
        }

        public override void Update()
        {

        }
    }
}
