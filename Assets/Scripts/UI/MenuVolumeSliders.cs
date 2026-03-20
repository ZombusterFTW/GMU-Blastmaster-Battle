using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class MenuVolumeSliders : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] string mixerFloat;
    [SerializeField] string prefsString;
    [SerializeField] Slider slider;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;



    // Start is called before the first frame update
    void Start()
    {
        slider.value = PlayerPrefs.GetFloat(prefsString, 0.75f);
    }

   public void SetLevel()
    {
        float sliderValue = slider.value;
        audioMixer.SetFloat(mixerFloat, Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat(prefsString, sliderValue);
        if(audioClip != null) audioSource.PlayOneShot(audioClip);
    }
}
