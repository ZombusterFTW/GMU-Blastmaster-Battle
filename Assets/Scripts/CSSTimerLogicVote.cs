using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class CSSTimerLogicVote : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI winnerText;
    public int minWaitTime = 15;
    public int waitTimeAdd = 7;
    private int timeRemaining;
    private float timeRemainingf;
    private List<PlayerController> joinedPlayers = new List<PlayerController>();
    private List<PlayerController> lockedInPlayers = new List<PlayerController>();
    private Coroutine timer;
    private int[] mapVoteArray = new int[4];

    private void Awake()
    {
        for(int i = 0; i<mapVoteArray.Length; i++)
        {
            mapVoteArray[i] = -1;
        }
        timerText.enabled = false;
        joinedPlayers = GameObject.FindObjectsOfType<PlayerController>().ToList<PlayerController>();
        timer = StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        bool moreThanOnePlayer = false;
        while (!moreThanOnePlayer) 
        {
            Debug.Log("Need another player to begin countdown");
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
                foreach (var player in joinedPlayers)
                {
                    if (!lockedInPlayers.Contains(player))
                    {
                        player.playerUIAssigner.ForceButtonLockIn();
                    }
                }
                Debug.Log("Start game");
                break;
            }
        }
        //Force lock in logic.

        int mapWinner = 0;
        int map1Votes = 0;
        int map2Votes = 0;
        int map3Votes = 0;
        int randomVotes = 0;
        int noVotes = 0;
        bool isRandom = false;


        for(int i = 0; i < mapVoteArray.Length; i++) 
        {
            if (mapVoteArray[i] == 0) map1Votes++;
            else if (mapVoteArray[i] == 1) map2Votes++; 
            else if (mapVoteArray[i] == 2) map3Votes++;
            else if (mapVoteArray[i] == 3) randomVotes++;
            else noVotes++;
        }

        if (noVotes == 4 || (map1Votes == 1 && map2Votes == 1 && map3Votes == 1 && randomVotes == 1))
        {
            mapWinner = Random.Range(1, 4);
            isRandom =  true;
        }
        else
        {
            int winner = Mathf.Max(map1Votes, map2Votes, map3Votes, randomVotes);
            if (winner == map1Votes) { mapWinner = 1; }
            if (winner == map2Votes) { mapWinner = 2; }
            if (winner == map3Votes) { mapWinner = 3; }
            if (winner == randomVotes)
            {
                mapWinner = Random.Range(1, 4);
                isRandom = true;
            }
        }

        if(isRandom) winnerText.text = "Random Level Wins!";
        else winnerText.text = "Level " + (mapWinner) + " Wins!";

        //Level loading logic has been moved to the SceneTransitionerManager class as it isn't destroyed on load. 
        //Start map load during countdown
        //StartCoroutine(LevelTransition(mapWinner + SceneManager.GetActiveScene().buildIndex));
        SceneTransitionerManager.instance.LoadGameLevelByIndex(mapWinner + SceneManager.GetActiveScene().buildIndex);
        timeRemaining = 4;
        timeRemainingf = 4;
        while(true)
        {
            timerText.text = timeRemaining.ToString();
            yield return null;
            timeRemainingf -= Time.deltaTime;
            timeRemaining = (int)timeRemainingf;
            if (timeRemaining < 0) timeRemaining = 0;
            if (timeRemaining == 0) break;
        }
        yield return null;
    }


    /*
    IEnumerator LevelTransition(int levelID)
    {
        if (GameObject.FindObjectOfType<GameMusicMenu>() != null) GameObject.FindObjectOfType<GameMusicMenu>().BeginMusic(levelID);
        SceneTransitionerManager.instance.StartTransition();
        yield return new WaitForSeconds(1);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelID, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;
        while(!asyncLoad.isDone) yield return null;
        asyncLoad.allowSceneActivation = true;

    }
    */





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
       mapVoteArray[player.GetPlayerID() - 1] = -1;
    }

    public void LockInPlayer(PlayerController player, int mapID)
    {
        lockedInPlayers.Add(player);
        mapVoteArray[player.GetPlayerID()-1] = mapID;
    }


}
