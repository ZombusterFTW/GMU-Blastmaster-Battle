using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class ButtonHolder : MonoBehaviour, ISelectHandler
{
    public Button[] buttons;
    public Image[] arrows;
    public AudioClip scrollAudio;
    public AudioClip lockInAudio;
    public MultiplayerEventSystem multiplayerEventSystem;
    public AudioSource selectionSounds;
    public PlayerController playerController;
    public GameObject lockedInAnimator;
    public GameObject matchWinsTracker;
    // Start is called before the first frame update

    private void Awake()
    {
        switch(playerController.GetPlayerID()) 
        {
            case 1:
                {
                    selectionSounds.panStereo = -.6f;
                    break;
                }
            case 2:
                {
                    selectionSounds.panStereo = -.15f;
                    break;
                }
            case 3:
                {
                    selectionSounds.panStereo = .15f;
                    break;
                }
            case 4:
                {
                    selectionSounds.panStereo = .6f;
                    break;
                }




        }

    }


    public void OnSelect(BaseEventData eventData)
    {

    }

    public void BeginArrowBlinking()
    {
        StartCoroutine(BlinkArrows());     
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void StopBlinking()
    {
        StopAllCoroutines();
        foreach(var arrow in arrows)
        {
            arrow.color = Color.black;
        }
        lockedInAnimator.GetComponent<Animator>().Play("Lockedinanimation");


    }

    IEnumerator BlinkArrows() 
    {
        while(true) 
        {
            foreach (var arrow in arrows)
            {
                arrow.color = Color.white;
            }
            yield return new WaitForSeconds(1f);
            foreach (var arrow in arrows)
            {
                arrow.color = Color.gray;
            }
        }
    }


    public void EnableButtons(bool enable)
    {
        foreach (var button in buttons) 
        {
            if(button.interactable!= enable) button.interactable = enable;
        }
        lockedInAnimator.GetComponent<Animator>().Play("ResetAnim");


    }

    public void PlaySound(int soundInt)
    {
        if (soundInt == 0)
        {
            selectionSounds.clip = scrollAudio;
            selectionSounds.Play();
            //AudioSource.PlayClipAtPoint(scrollAudio, Camera.main.transform.position, 2f);
        }
        else
        {
            selectionSounds.clip = lockInAudio;
            selectionSounds.Play();
            ///AudioSource.PlayClipAtPoint(lockInAudio, Camera.main.transform.position, 2f);
        }
    }

    
}
