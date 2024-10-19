using Skolger.UI;
using UnityEngine;

namespace Skolger.Tutorial
{
    public class MaskBoxVisualAid : VisualAid
    {
        [SerializeField] MaskBox maskBox;

        [SerializeField] Vector2 position;
        [SerializeField] Vector2 sizeDelta;
        [SerializeField, Multiline] string text;
        [SerializeField] bool blockRayCasts = true;

        public override void Initialize()
        {
            maskBox.gameObject.SetActive(true);
            maskBox.UpdateMask(position, sizeDelta, text, blockRayCasts);
        }

        public override void Reset()
        {
            maskBox.gameObject.SetActive(false);
        }

        public override void Update()
        {
        }
    }
}