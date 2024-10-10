using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Skolger.Tutorial
{
    public class TutorialManager : MonoSingleton<TutorialManager>
    {
        [SerializeField] List<TutorialPart> tutorialParts = new List<TutorialPart>();

        [SerializeField] UnityEvent OnFinishedTutorial;
        bool tutorialFinished = false;

        void Start()
        {
            if (tutorialParts.Count > 0)
            {
                tutorialParts[0].Initialize();
            }
        }
        void Update()
        {
            if ((tutorialParts.Count > 0) && !tutorialParts[0].finished)
            {
                tutorialParts[0].Update();
            }
            else if (tutorialParts.Count > 0 && tutorialParts[0].finished)
            {
                tutorialParts.RemoveAt(0);
                if (tutorialParts.Count != 0)
                    tutorialParts[0].Initialize();
            }
            else if (tutorialParts.Count == 0 && !tutorialFinished)
            {
                tutorialFinished = true;
                OnFinishedTutorial?.Invoke();
            }
        }
    }
}