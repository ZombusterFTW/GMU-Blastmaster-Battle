using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SceneTransitionerManager : MonoBehaviour
{
    //This class allows for each scene transition to have a unique image and text!



    [SerializeField] GameObject parentGameObject;
    public string[] gameHints;
    public string[] gameHintAnimations;

    public string[] game153Hints;
    public string game153Animation;




    public Animator transitionAnimator;
    public Animator transitionShowcaseAnimator;
    [SerializeField] private TextMeshProUGUI gameHintsText;
    private int lastScreen = 0;
    [SerializeField] private Canvas canvas;
    public float timeSinceSceneTransition = 0f;



    public static SceneTransitionerManager instance { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(parentGameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            gameHintsText.text = "";
            canvas.enabled = true;
        }
        else if (instance != null && instance != this)
        {
            Debug.Log("Scene transitioner already exists. Deleting clone");
            DestroyImmediate(gameObject);
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        //Play animator animation
        if(this != null && transitionAnimator != null) transitionAnimator.Play("BoxWipe_End");
        timeSinceSceneTransition = 0;
        //gameHintsText.text = gameHints[Random.Range(0, gameHints.Length)];
    }

    public void StartTransition()
    {
        if (!SceneTransitionerManager.instance.transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("BoxWipe_Start"))
        {
            //Added to prevent player from selecting again during scene transition.
            FirstJoinPrivelege fJoinScript = GameObject.FindObjectOfType<FirstJoinPrivelege>();
            if(fJoinScript != null) fJoinScript.enabled = false;

            //Time.timeScale = 1;
            //For demonstration purposes a text hint is directly linked to an animation that would play with it.
            //Screens are random but they cannot be the same twice in a row

            if (DateTime.Now.Minute == 53 && DateTime.Now.Hour == 1 && Random.Range(0,2) == 1)
           {
                int index = Random.Range(0, game153Hints.Length);
                while (index == lastScreen)
                {
                    index = Random.Range(0, game153Hints.Length);
                }
                lastScreen = index;
                transitionAnimator.SetTrigger("Transition");
                gameHintsText.text = game153Hints[index];
                transitionShowcaseAnimator.Play(game153Animation);
            }
           else
           {
                int index = Random.Range(0, gameHints.Length);
                while (index == lastScreen)
                {
                    index = Random.Range(0, gameHints.Length);
                }
                lastScreen = index;
                transitionAnimator.SetTrigger("Transition");
                gameHintsText.text = gameHints[index];
                if (gameHintAnimations[index] != null) transitionShowcaseAnimator.Play(gameHintAnimations[index]);
            } 
        }
    }




    public void LoadGameLevelByIndex(int index, bool fast = false)
    {
        StartCoroutine(LevelTransition(index, fast));
    }

    IEnumerator LevelTransition(int levelID, bool fast)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelID, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;
        if(!fast) yield return new WaitForSeconds(3f);
        if (GameObject.FindObjectOfType<GameMusicMenu>() != null) GameObject.FindObjectOfType<GameMusicMenu>().BeginMusic(levelID);
        SceneTransitionerManager.instance.StartTransition();
        while (!asyncLoad.isDone && asyncLoad.progress < 0.9f) yield return null;
        yield return new WaitForSecondsRealtime(1f);
        asyncLoad.allowSceneActivation = true;

    }
    // Update is called once per frame
    void Update()
    {
        timeSinceSceneTransition += Time.unscaledDeltaTime;
    }
}
