using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour
{
    public int sceneToLoad;
    public AudioSource buttonAudioSource;
    private bool pressed = false;
    private EscMenu playerManagerClass;
    [SerializeField] TextMeshProUGUI buttonText;
    public string activeText;
    public string unActiveText;
    /// <summary>
    /// Addition of these settings is very messy due to having to work with some previous code choices.
    /// </summary>
    public string buttonTypeSaveData = "";

    private void Start()
    {
        playerManagerClass = GameObject.FindGameObjectWithTag("PlayerInputManager").GetComponent<EscMenu>();
        if (playerManagerClass != null)
        {
            bool tempBool;


            switch(buttonTypeSaveData)
            {
                //If we dont have the additional "tag" data we return
                default:
                    return;
                case "eastereggs":
                    {
                        tempBool = playerManagerClass.easterEggsEnabled;
                        break;
                    }
                case "squeakysteps":
                    {
                        tempBool = playerManagerClass.squeakyStepsOnLvl1;
                        break;
                    }
                case "altmodes":
                    {
                        tempBool = playerManagerClass.alternateGamemodesEnabled;
                        break;
                    }

            }
            if (tempBool)
            {
                if (buttonText != null)
                {
                    buttonText.text = activeText;
                }
            }
            else
            {
                if (buttonText != null)
                {
                    buttonText.text = unActiveText;
                }
            }
        }
    }


    public void OnButtonClick()
    {
        if(buttonAudioSource != null) buttonAudioSource.Play();
        // Load the specified scene
        StartCoroutine(LevelTransition());
    }

    public void PlayOnlyButtonSound()
    {
        buttonAudioSource.Play();
    }

    //Using this class also to handle sneaker squeak toggle
    public void ToggleSneakerSqueaks()
    {
        if (playerManagerClass != null)
        {
            playerManagerClass.ToggleSqueakySteps();

            if (playerManagerClass.squeakyStepsOnLvl1)
            {
                if (buttonText != null)
                {
                    buttonText.text = activeText;
                }
            }
            else
            {
                if (buttonText != null)
                {
                    buttonText.text = unActiveText;
                }
            }

        }

    }
    public void ToggleEasterEggs()
    {
        if (playerManagerClass != null)
        {
            playerManagerClass.ToggleEasterEggsEnabled();

            if (playerManagerClass.easterEggsEnabled)
            {
                if (buttonText != null)
                {
                    buttonText.text = activeText;
                }
            }
            else
            {
                if (buttonText != null)
                {
                    buttonText.text = unActiveText;
                }
            }

        }

    }
    public void ToggleAlternateGamemodes()
    {
        if (playerManagerClass != null)
        {
            playerManagerClass.ToggleAlternateGamemodesEnabled();

            if (playerManagerClass.alternateGamemodesEnabled)
            {
                if (buttonText != null)
                {
                    buttonText.text = activeText;
                }
            }
            else
            {
                if (buttonText != null)
                {
                    buttonText.text = unActiveText;
                }
            }

        }

    }


    IEnumerator LevelTransition()
    {
        if(GameObject.FindObjectOfType<GameMusicMenu>() != null) GameObject.FindObjectOfType<GameMusicMenu>().BeginMusic(sceneToLoad);
        SceneTransitionerManager.instance.StartTransition();
        AsyncOperation asyncLoad = null;
        asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;
        yield return new WaitForSeconds(1.5f);
        asyncLoad.allowSceneActivation = true;
    }


}
