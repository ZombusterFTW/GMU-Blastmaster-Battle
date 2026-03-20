using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AlternateGameModeChanceSlider : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] Slider slider;
    [SerializeField] AudioSource audioSource;
    void Start()
    {
        slider.value = PlayerPrefs.GetInt("AlternateGamemodeChance", 1);
    }

    // Update is called once per frame
    public void SetLevel()
    {
        int sliderValue = (int)slider.value;
        PlayerPrefs.SetInt("AlternateGamemodeChance", sliderValue);
        if (SceneTransitionerManager.instance.timeSinceSceneTransition > 0.125f) audioSource.Play();
        Debug.Log(PlayerPrefs.GetInt("AlternateGamemodeChance", 1));
    }
}
