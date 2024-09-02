using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Skolger.Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] List<TutorialPart> tutorialParts = new List<TutorialPart>();

        void Start()
        {
            if (tutorialParts.Count > 0 )
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
            }
            else if(tutorialParts.Count < 0)
            {
                Debug.Log("Finished Tutorial");
            }
        }
    }
}