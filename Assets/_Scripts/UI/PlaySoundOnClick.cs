using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaySoundOnClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] AudioClip clip;

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundsFXManager.instance?.PlaySoundFXClip(clip, Camera.main.transform, 100);
    }
}
