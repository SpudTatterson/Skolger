using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Skolger.UI.InfoContainers
{
    public class HungerInfoContainer : InfoContainer
    {
        
        public void UpdateInfo(float current, HungerStatus status)
        {
            float fill = GetFillPercentage(0, 100, current);
            fillUpBarImage.fillAmount = fill;
            fillUpBarImage.color = Color.Lerp(emptyColor, fullColor, fill + 0.2f);

            statusText.text = status.ToString();
        }

        void Update()
        {
            UpdateInfo(colonist.hungerManager.HungerLevel, colonist.hungerManager.hungerStatus);
        }
    }
}