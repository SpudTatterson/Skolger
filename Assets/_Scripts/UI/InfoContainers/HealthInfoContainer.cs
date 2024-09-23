using UnityEngine;

namespace Skolger.UI.InfoContainers
{
    public class HealthInfoContainer : InfoContainer
    {
        public void UpdateInfo(float current, HealthStatus status)
        {
            float fill = GetFillPercentage(0, 100, current);
            fillUpBarImage.fillAmount = fill;
            fillUpBarImage.color = Color.Lerp(emptyColor, fullColor, fill + 0.2f);

            statusText.text = status.ToString();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            colonist.healthManager.OnHealthChanged += UpdateInfo;
            colonist.healthManager.UpdateHealth();

        }
        void OnDisable()
        {
            colonist.healthManager.OnHealthChanged -= UpdateInfo;
        }
    }
}