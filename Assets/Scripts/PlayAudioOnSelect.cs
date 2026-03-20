using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayAudioOnSelect : MonoBehaviour, ISelectHandler
{

    public AudioClip buttonSelect;
    public AudioSource audioSource;



    public void OnSelect(BaseEventData eventData)
    {
        if (audioSource == null)
        {
            return;
            //AudioSource.PlayClipAtPoint(buttonSelect, Camera.main.transform.position, 2f);
        }
        else
        {
            //Don't the first button select sound unless the character object was just created. This will prevent the select sound from stacking after each scene transition that changes the UI
            if (SceneTransitionerManager.instance.timeSinceSceneTransition > 0.125f)
            {

                audioSource.PlayOneShot(buttonSelect);
            }
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
