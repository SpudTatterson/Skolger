using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Skolger.UI.InfoContainers
{
    public class MoodInfoContainer : InfoContainer
    {
        public void UpdateInfo(int current, MoodStatus status)
        {
            float fill = GetFillPercentage(0, 100, current);
            fillUpBarImage.fillAmount = fill;
            fillUpBarImage.color = Color.Lerp(emptyColor, fullColor, fill + 0.2f);

            statusText.text = status.ToString();
        }

        void OnEnable()
        {
            colonist.colonistMood.onMoodChange += UpdateInfo;
            colonist.colonistMood.ForceMoodUpdate();
        }
        void OnDisable()
        {
            colonist.colonistMood.onMoodChange -= UpdateInfo;
        }
    }
}