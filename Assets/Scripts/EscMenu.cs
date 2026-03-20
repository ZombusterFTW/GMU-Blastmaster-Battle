using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class EscMenu : MonoBehaviour
{
  //This class manages the behavior of pressing the esc key while in different scenes. Add pause as a feature. Should only work for the first player to press it and the first player to interact with the game will have control(like the menus). 
    private bool transitioning = false;
    public AudioSource backSoundPlayer;
    //Piggy backing off this class as it persists
    public bool squeakyStepsOnLvl1 = true;
    public bool alternateGamemodesEnabled = true;
    public bool easterEggsEnabled = true;
    public bool gamePaused = false;
    private RoundManager roundManager;
    public static EscMenu instance;
    [SerializeField] private Canvas pauseMenuCanvas;
    [SerializeField] private FirstJoinPrivelege pauseMenuManager;
    public AudioMixer menuMusic;
    public AudioMixer gameAudio;
    [SerializeField] GameObject pauseFailHint;
    GameObject activePauseObj;
    public float savedTimeScale {  get; private set; }

    public void SetSavedTimeScale(float savedTimeScaleIn)
    {
        savedTimeScale = savedTimeScaleIn;  
    }

    private void Awake()
    {
        if(instance == null) 
        {
            instance = this;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Debug.Log("Another ESC menu object exists in the scene.");
            return;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;

        //Load saved player data from unity prefs so menu options stay. 
        squeakyStepsOnLvl1 = (PlayerPrefs.GetInt("SqueakyStepsLv1", 1) != 0);
        alternateGamemodesEnabled = (PlayerPrefs.GetInt("AlternateGamemodesEnabled", 1) != 0);
        easterEggsEnabled = (PlayerPrefs.GetInt("EasterEggsEnabled", 1) != 0);
        menuMusic.SetFloat("MenuMusicVol", Mathf.Log10(PlayerPrefs.GetFloat("MenuMusicVolume", 0.75f))*20);
        gameAudio.SetFloat("GameAudioVol", Mathf.Log10(PlayerPrefs.GetFloat("GameAudioVolume", 0.75f))*20);
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(pauseMenuCanvas != null)
        {
            pauseMenuCanvas.gameObject.SetActive(false);
        }
        roundManager = GameObject.FindObjectOfType<RoundManager>();
        transitioning = false;
        gamePaused = false;
    }

    private void Start()
    {
        roundManager = GameObject.FindObjectOfType<RoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !transitioning)
        {
            //ensure the scene isn't already transitioning. If it is we disable
            if (!GameObject.FindGameObjectWithTag("SceneTransitioner").GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("BoxWipe_Start"))
            {

                //Pausing is disallowed during the intro, after blocks begin to fall, and if a win is in-progress.
                if ((SceneManager.GetActiveScene().buildIndex == 4 || SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 6) && roundManager != null && (!roundManager.IsStartingCountdownActive() && !roundManager.IsWinInProgress() && !roundManager.blocksActive && !gamePaused))
                {
                    //pause game here. WE are currently within a match.
                    //Game cannot be paused during the countdown or a win
                    //pause feature WIP.
                    //If there is no set canvas we don't pause to prevent errors.
                    if (pauseMenuCanvas == null) return;
                    roundManager.roundStartCountdownActive = true;
                    savedTimeScale = Time.timeScale;    
                    Time.timeScale = 0;
                    foreach (AudioSource audioSource in FindObjectsOfType<AudioSource>())
                    {
                        audioSource.pitch = Time.timeScale;
                    }
                    gamePaused = true;
                    //Unhide a canvas here and do something with the sound system. 
                    pauseMenuCanvas.gameObject.SetActive(true);
                    pauseMenuManager.OverwriteControllingPlayer(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>());



                    //321 countdown needs to happen before gameplay resumes.
                    Debug.Log("Pause");
                    return;
                }
                else if((SceneManager.GetActiveScene().buildIndex == 4 || SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 6) && roundManager != null && (roundManager.IsStartingCountdownActive() || roundManager.IsWinInProgress() || roundManager.blocksActive) && !gamePaused)
                {
                    //Pause attempt failed
                   if(activePauseObj == null) activePauseObj = Instantiate(pauseFailHint);
                   return;
                }
                if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 9 || SceneManager.GetActiveScene().buildIndex == 10)
                {
                    transitioning = true;
                    Debug.Log("Back");
                    backSoundPlayer.Play();
                    StartCoroutine(LevelTransition(0));

                }
                
                else if (SceneManager.GetActiveScene().buildIndex == 3)
                {
                    StartCoroutine(LevelTransition(2));
                    transitioning = true;
                    Debug.Log("Back");
                    backSoundPlayer.Play();
                }
                else if (SceneManager.GetActiveScene().buildIndex == 0)
                {
                    FirstJoinPrivelege menuUIManager;
                    if (GameObject.FindObjectOfType<FirstJoinPrivelege>() != null) 
                    {
                        transitioning = true;
                        Debug.Log("Back");
                        backSoundPlayer.Play();
                        menuUIManager = GameObject.FindObjectOfType<FirstJoinPrivelege>();
                        menuUIManager.EnableQuitCanvas(true);
                    }
                    else
                    {
                        Debug.Log("Scene does not have a SceneES object!");
                        transitioning = false;
                    }
                }
            }
            else Debug.Log("YOU CANNOT");
        }
    }

    public void ToggleSqueakySteps()
    {
        //Toggle squeaky steps on and off.
        squeakyStepsOnLvl1 = !squeakyStepsOnLvl1;
        PlayerPrefs.SetInt("SqueakyStepsLv1", squeakyStepsOnLvl1 ? 1 : 0);
    }

    public void ToggleEasterEggsEnabled()
    {
        easterEggsEnabled = !easterEggsEnabled;
        PlayerPrefs.SetInt("EasterEggsEnabled", easterEggsEnabled ? 1 : 0);
    }

    public void ToggleAlternateGamemodesEnabled()
    {
        alternateGamemodesEnabled = !alternateGamemodesEnabled;
        PlayerPrefs.SetInt("AlternateGamemodesEnabled", alternateGamemodesEnabled ? 1 : 0);
    }



    public void CallEscBack(PlayerController callBackPlayer)
    {
        if (!transitioning)
        {
            //ensure the scene isn't already transitioning. If it is we disable
            if (!SceneTransitionerManager.instance.transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("BoxWipe_Start"))
            {




                //Pausing is disallowed during the intro, after blocks begin to fall, and if a win is in-progress.
                if ((SceneManager.GetActiveScene().buildIndex == 4 || SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 6) && roundManager != null && (!roundManager.IsStartingCountdownActive() && !roundManager.IsWinInProgress() && !roundManager.blocksActive && !gamePaused))
                {
                    Debug.Log("pause");
                    backSoundPlayer.Play();
                    //pause game here. WE are currently within a match.
                    //Game cannot be paused during the countdown or a win
                    //pause feature WIP.
                    //If there is no set canvas we don't pause to prevent errors.
                    if (pauseMenuCanvas == null) return;
                    roundManager.roundStartCountdownActive = true;
                    savedTimeScale = Time.timeScale;
                    Time.timeScale = 0;
                    foreach (AudioSource audioSource in FindObjectsOfType<AudioSource>())
                    {
                        audioSource.pitch = Time.timeScale;
                    }
                    gamePaused = true;
                    //Unhide a canvas here and do something with the sound system. 
                    pauseMenuCanvas.gameObject.SetActive(true);
                    //Set menu control to the pausing player
                    pauseMenuManager.OverwriteControllingPlayer(callBackPlayer);
                    //321 countdown needs to happen before gameplay resumes.
                    return;
                }

                else if ((SceneManager.GetActiveScene().buildIndex == 4 || SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 6) && roundManager != null && (roundManager.IsStartingCountdownActive() || roundManager.IsWinInProgress() || roundManager.blocksActive) && !gamePaused)
                {
                    //Pause attempt failed
                    if (activePauseObj == null) activePauseObj = Instantiate(pauseFailHint);
                    return;
                }

                if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 9 || SceneManager.GetActiveScene().buildIndex == 10)
                {
                    StartCoroutine(LevelTransition(0));
                    transitioning = true;
                    Debug.Log("Back");
                    backSoundPlayer.Play();

                }
                
                else if (SceneManager.GetActiveScene().buildIndex == 3)
                {
                    StartCoroutine(LevelTransition(2));
                    transitioning = true;
                    Debug.Log("Back");
                    backSoundPlayer.Play();
                }
                else if (SceneManager.GetActiveScene().buildIndex == 0)
                {
                    FirstJoinPrivelege menuUIManager;
                    if (GameObject.FindObjectOfType<FirstJoinPrivelege>() != null)
                    {
                        transitioning = true;
                        Debug.Log("quit menu");
                        backSoundPlayer.Play();
                        menuUIManager = GameObject.FindObjectOfType<FirstJoinPrivelege>();
                        menuUIManager.EnableQuitCanvas(true);
                    }
                    else
                    {
                        Debug.Log("Scene does not have a SceneES object!");
                        transitioning = false;
                    }
                }
            }
            else
            {
                Debug.Log("YOU CANNOT");
            }

        }
    }

    public void SetIsTransitioning(bool isTransitioning)
    {
        transitioning = isTransitioning;
    }

    IEnumerator LevelTransition(int sceneID)
    {
        if (GameObject.FindObjectOfType<GameMusicMenu>() != null) GameObject.FindObjectOfType<GameMusicMenu>().BeginMusic(sceneID);
        SceneTransitionerManager.instance.StartTransition();
        AsyncOperation asyncLoad = null;
        asyncLoad = SceneManager.LoadSceneAsync(sceneID, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;
        yield return new WaitForSeconds(1.5f);
        asyncLoad.allowSceneActivation = true;
        transitioning = false;
        //SceneManager.LoadScene(sceneID);
    }


    public void ResumeGameplay()
    {
        //This function calls the round manager to unpause the game and start the resume "countdown"
        roundManager.ResumeRoundAfterGamePause();
        gamePaused = false;
        pauseMenuCanvas.gameObject.SetActive(false);
    }
}
