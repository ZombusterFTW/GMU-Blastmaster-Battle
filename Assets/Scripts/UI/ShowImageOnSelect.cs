using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowImageOnSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private GameObject controlsImage;
    private AudioSource buttonAudioSource;

    private void Start()
    {
        buttonAudioSource = GetComponent<AudioSource>();
        controlsImage.SetActive(false);
    }

    public void HideImageToggle()
    {
        if(!GameObject.ReferenceEquals(buttonAudioSource, null))
        {
            buttonAudioSource.Play();
        }
        if(controlsImage.activeSelf) controlsImage.SetActive(false);
        else controlsImage.SetActive(true);
    }

    public void OnSelect(BaseEventData eventData)
    {
        controlsImage.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        controlsImage.SetActive(false);
    }
}
