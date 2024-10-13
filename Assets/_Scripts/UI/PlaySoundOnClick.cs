using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaySoundOnClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] AudioClip clip;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clip != null)
            SoundsFXManager.Instance?.PlaySoundFXClip(clip, 100);
        else
            Debug.LogWarning("Sound missing on " + gameObject.name);
    }
}
