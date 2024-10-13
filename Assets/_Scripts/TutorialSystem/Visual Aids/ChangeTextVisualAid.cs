using TMPro;
using UnityEngine;

namespace Skolger.Tutorial
{
    public class ChangeTextVisualAid : VisualAid
    {
        [SerializeField] TextMeshProUGUI textObject;
        [SerializeField] string text;

        public override void Initialize()
        {
            textObject.text = text;
        }

        public override void Reset()
        {
        }

        public override void Update()
        {
        }
    }
}
