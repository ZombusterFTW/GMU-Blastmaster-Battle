using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameMusicMenu : MonoBehaviour
{
    public AudioMixerSnapshot allSilent;
    public AudioMixerSnapshot mainMenu;
    public AudioMixerSnapshot characterSelect;
    public AudioMixerSnapshot levelSelect;
    public AudioMixerSnapshot resultsScreenMix;

    public AudioClip level1BGM;
    public AudioClip level2BGM;
    public AudioClip level3BGM;
    public AudioClip level1BGMAlt;
    public AudioClip level2BGMAlt;
    public AudioClip level3BGMAlt;
    public AudioClip resultsScreen;
    public AudioClip drawTheme;
    public AudioClip speedUpAlert;
    public AudioClip speedUpAlertLv2;
    public AudioClip speedUpAlertLv3;

    public AudioClip level1AMB;
    public AudioClip level1Cheer;
    public AudioClip level2AMB;
    public AudioClip level3AMB;
    

    public AudioClip professorWin;
    public AudioClip tophWin;
    public AudioClip patriotWin;
    public AudioClip centaurWin;
    public AudioClip levelTransition;

    public AudioSource levelMusicPlayer;
    public AudioSource victoryMusicPlayer;
    public AudioSource transitionSoundPlayer;
    public AudioSource resultsScreenSource;
    public AudioSource ambientSoundPlayer;
    public AudioSource lvl1CheerSoundPlayer;
    public AudioSource suddenDeathPlayer;

    public float transitionTime = 0f;
    public float localTransitionTime = 0.15f;
    public float minTransitionPitch = 0.7f;
    public float maxTransitionPitch = 1.5f;


    private RoundManager roundManager;

    public static GameMusicMenu instance { get; private set; }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null && instance != this)
        {
            Debug.Log("Menu music already exists. Deleting clone");
            DestroyImmediate(gameObject);
        }

        roundManager = GameObject.FindObjectOfType<RoundManager>(); 
        //BeginMusic(SceneManager.GetActiveScene().buildIndex);
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
    }

    private void Start()
    {
        BeginMusicInstant(SceneManager.GetActiveScene().buildIndex);
    }



    public void BeginMusicInstant(int sceneID)
    {
        Debug.Log(sceneID);
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            transitionSoundPlayer.pitch = Random.Range(minTransitionPitch, maxTransitionPitch);
            transitionSoundPlayer.Play();
            levelMusicPlayer.Stop();
            victoryMusicPlayer.Stop();
            ambientSoundPlayer.Stop();
        }
        
        //Change to audio mixer snapshot depending on scene index.
        switch (sceneID)
        {
            default:
                {
                    allSilent.TransitionTo(localTransitionTime);
                    break;
                }
            case 0:
                {
                    mainMenu.TransitionTo(localTransitionTime);
                    break;
                }
            case 1:
                {
                    mainMenu.TransitionTo(localTransitionTime);
                    break;
                }
            case 2:
                {
                    characterSelect.TransitionTo(localTransitionTime);
                    break;
                }
            case 3:
                {
                    levelSelect.TransitionTo(localTransitionTime);
                    break;
                }
            case 9:
                {
                    mainMenu.TransitionTo(transitionTime);
                    break;
                }
            case 10:
                {
                    mainMenu.TransitionTo(transitionTime);
                    break;
                }
        }
    }



    public void BeginMusic(int sceneID)
    {
        Debug.Log(sceneID);
        transitionSoundPlayer.pitch = Random.Range(minTransitionPitch, maxTransitionPitch);
        transitionSoundPlayer.Play();
        levelMusicPlayer.Stop();
        victoryMusicPlayer.Stop();
        ambientSoundPlayer.Stop();
        //Change to audio mixer snapshot depending on scene index.
        switch (sceneID) 
        {
            default:
            {
                    allSilent.TransitionTo(transitionTime);
                    break;
            }
            case 0:
            {
                    mainMenu.TransitionTo(transitionTime);
                    break;
            }
            case 1:
            {
                    mainMenu.TransitionTo(transitionTime);
                    break;
            }
            case 2:
            {
                    characterSelect.TransitionTo(transitionTime);
                    break;
            }
            case 3:
            {
                    levelSelect.TransitionTo(transitionTime);
                    break;
            }
            case 9:
                {
                    mainMenu.TransitionTo(transitionTime);
                    break;
                }
            case 10:
                {
                    mainMenu.TransitionTo(transitionTime);
                    break;
                }
        }    
    }

    public void FadeOutMusic(float time)
    {
        //allSilent.TransitionTo(3);
    }


    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (this != null && instance == this)
        {
            StopAllCoroutines();
        }
        if (levelMusicPlayer != null)levelMusicPlayer.Stop();
        if(victoryMusicPlayer != null)victoryMusicPlayer.Stop();
        if (ambientSoundPlayer != null) ambientSoundPlayer.Stop();
        //BeginMusic(SceneManager.GetActiveScene().buildIndex);
        if (victoryMusicPlayer != null) transitionSoundPlayer.pitch = Random.Range(minTransitionPitch, maxTransitionPitch);
        if (victoryMusicPlayer != null) transitionSoundPlayer.Play();
    }

    void Update()
    {
        
    }




    public void PlayLevelMusic(int sceneID, bool speedUp = false)
    {
        switch(sceneID) 
        {
            default:
                Debug.Log("Something went wrong. Line 173 Game Music Menu");
                if(speedUp) levelMusicPlayer.clip = level1BGM;
                else levelMusicPlayer.clip = level1BGMAlt;
                levelMusicPlayer.Play();
                break;
            case 4:
                if (speedUp) levelMusicPlayer.clip = level1BGMAlt;
                else levelMusicPlayer.clip = level1BGM;
                levelMusicPlayer.Play();
                break;
            case 5:
                if (speedUp) levelMusicPlayer.clip = level2BGMAlt;
                else levelMusicPlayer.clip = level2BGM;
                levelMusicPlayer.Play();
                break;
            case 6:
                if (speedUp) levelMusicPlayer.clip = level3BGMAlt;
                else levelMusicPlayer.clip = level3BGM;
                levelMusicPlayer.Play();
                break;
            case 8:
                levelMusicPlayer.clip = resultsScreen;
                levelMusicPlayer.Play();
                allSilent.TransitionTo(0);
                break;
        }
    }

    public float PlayVictoryTheme(int charID) 
    {
        switch (charID)
        {
            default:
                return 1;
            case 0:
                victoryMusicPlayer.PlayOneShot(patriotWin);
                return patriotWin.length;
            case 1:
                victoryMusicPlayer.PlayOneShot(centaurWin);
                return centaurWin.length;   
            case 2:
                victoryMusicPlayer.PlayOneShot(tophWin);
                return tophWin.length;
            case 3:
                victoryMusicPlayer.PlayOneShot(professorWin);
                return professorWin.length;
            case 4:
                victoryMusicPlayer.PlayOneShot(resultsScreen);
                return resultsScreen.length;
            case 5:
                victoryMusicPlayer.PlayOneShot(drawTheme);
                return resultsScreen.length;
        }
    }

    public void PlaySpeedUpLevelTheme()
    {
        int sceneID = SceneManager.GetActiveScene().buildIndex;
        //Speed up music is contexual to the scene.
        levelMusicPlayer.Stop();
        switch (sceneID)
        {
            default:
                levelMusicPlayer.clip = level1BGMAlt;
                suddenDeathPlayer.clip = speedUpAlert;
                break;
            case 4:
                levelMusicPlayer.clip = level1BGMAlt;
                suddenDeathPlayer.clip = speedUpAlert;
                break;
            case 5:
                levelMusicPlayer.clip = level2BGMAlt;
                suddenDeathPlayer.clip = speedUpAlertLv2;
                break;
            case 6:
                levelMusicPlayer.clip = level3BGMAlt;
                suddenDeathPlayer.clip = speedUpAlertLv3;
                break;
        }
        suddenDeathPlayer.Play();
        //Start faster theme where the slower one ended
        float tempTime = levelMusicPlayer.time;
        levelMusicPlayer.PlayScheduled(tempTime);
    }

    public void PlayLevelAmbience(int sceneID)
    {
        switch (sceneID)
        {
            default:
                Debug.Log("Something went wrong. Line 173 Game Music Menu");
                ambientSoundPlayer.clip = level1AMB;
                ambientSoundPlayer.Play();
                break;
            case 4:
                ambientSoundPlayer.clip = level1AMB;
                ambientSoundPlayer.Play();
                break;
            case 5:
                ambientSoundPlayer.clip = level2AMB;
                ambientSoundPlayer.Play();

                break;
            case 6:
                ambientSoundPlayer.clip = level3AMB;
                ambientSoundPlayer.Play();

                break;
            
        }
    }

    public void PlayCheer()
    {
        lvl1CheerSoundPlayer.PlayOneShot(level1Cheer);
        
        

    }
}
