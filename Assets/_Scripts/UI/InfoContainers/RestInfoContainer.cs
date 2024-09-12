using UnityEngine;

namespace Skolger.UI.InfoContainers
{
    public class RestInfoContainer : InfoContainer
    {
        public void UpdateInfo(float current, RestStatus status)
        {
            float fill = GetFillPercentage(0, 100, current);
            fillUpBarImage.fillAmount = fill;
            fillUpBarImage.color = Color.Lerp(emptyColor, fullColor, fill + 0.2f);

            statusText.text = status.ToString();
        }

        void Update()
        {
            UpdateInfo(colonist.restManger.RestLevel, colonist.restManger.restStatus);
        }
    }
}