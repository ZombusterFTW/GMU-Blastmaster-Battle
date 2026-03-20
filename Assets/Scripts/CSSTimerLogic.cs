using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CSSTimerLogic : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public int minWaitTime = 15;
    public int waitTimeAdd = 7;
    private int timeRemaining;
    private float timeRemainingf;
    private List<PlayerController> joinedPlayers = new List<PlayerController>();
    private List<PlayerController> lockedInPlayers = new List<PlayerController>();
    private Coroutine timer;

    private void Awake()
    {
        timerText.enabled = false;
        joinedPlayers = GameObject.FindObjectsOfType<PlayerController>().ToList<PlayerController>();
        timer = StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        bool moreThanOnePlayer = false;
        while (!moreThanOnePlayer) 
        {
            if(joinedPlayers.Count > 1) moreThanOnePlayer = true;
            yield return null;
        }
        timeRemaining = minWaitTime;
        timeRemainingf = minWaitTime;
        timerText.enabled = true;
        timerText.text = timeRemaining.ToString();

        int joinedPlayersIG = 0;
        while (joinedPlayers.Count != lockedInPlayers.Count)
        {
            joinedPlayersIG = joinedPlayers.Count;
            yield return new WaitForEndOfFrame();
            if (joinedPlayersIG < joinedPlayers.Count) timeRemainingf += waitTimeAdd;
            else if(joinedPlayers.Count == 1)
            {
                ResetTimer();
                yield break;
            }
            timeRemainingf -= Time.deltaTime;
            timeRemaining = (int)timeRemainingf;
            if(timeRemaining < 0) timeRemaining = 0;    
            timerText.text = timeRemaining.ToString();
            if (joinedPlayers.Count == lockedInPlayers.Count || timeRemaining == 0)
            {
                Debug.Log("Start game");
                break;
            }
        }


        //Select all characters that are hovered here. 


        //Before the game finish logic must be changed on a vote. Random level selection must not have priority over a level vote, so the only true way to vote random is a max player random players vote.
        //Not going to be in this script but caden asked for player select to be static even if a player leaves. Player character choices could be saved on another object like the PlayerManager_Menu

        foreach (var player in joinedPlayers) 
        {
            if(!lockedInPlayers.Contains(player))
            {
                player.playerUIAssigner.ForceButtonLockIn();
            }
        }
        timerText.text = timeRemaining.ToString();







        timeRemaining = 0;
        timeRemainingf = 0;
        while(true)
        {
            if (timeRemaining == 0) break;
            timerText.text = timeRemaining.ToString();
            yield return new WaitForEndOfFrame();
            timeRemainingf -= Time.deltaTime;
            timeRemaining = (int)timeRemainingf;
            if (timeRemaining < 0) timeRemaining = 0;
        }




        //Load Level select
        StartCoroutine(LevelTransition(3));

        yield return null;
    }



    IEnumerator LevelTransition(int levelID)
    {

        SceneTransitionerManager.instance.StartTransition();
        if (GameObject.FindObjectOfType<GameMusicMenu>() != null) GameObject.FindObjectOfType<GameMusicMenu>().BeginMusic(levelID);
        AsyncOperation asyncLoad = null;
        asyncLoad = SceneManager.LoadSceneAsync(levelID, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;
        yield return new WaitForSeconds(1.5f);
        asyncLoad.allowSceneActivation = true;
    }





    private void ResetTimer()
    {
        StopCoroutine(timer);
        StartCoroutine(Timer());
    }




    public void JoinPlayer(PlayerController player)
    {
        joinedPlayers.Add(player);
    }

    public void LeavePlayer(PlayerController player)
    {
       joinedPlayers.Remove(player);
       lockedInPlayers.Remove(player);
    }

    public void LockInPlayer(PlayerController player)
    {
        lockedInPlayers.Add(player);
    }


}
