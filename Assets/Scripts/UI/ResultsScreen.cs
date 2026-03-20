using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;


public class ResultsScreen : MonoBehaviour
{
    public TextMeshProUGUI[] playersText;
    public TextMeshProUGUI[] losingPlayersElims;
    public TextMeshProUGUI winningPlayerWins;
    public TextMeshProUGUI winningPlayerElims;
    public TextMeshProUGUI winningSelfElims;

    public TextMeshProUGUI rematchCountdown;
    public GameObject[] victoryPortraits;
    public GameObject[] victoryBanners;
    public GameObject[] losingPortraits;
    public Sprite[] losingPortraitImages;

    public float resultsScreenTime = 7.5f;
    public float rematchVoteTime = 30f;

    public GameObject rematchCanvas;
    public GameObject[] rematchButtons;
    public GameObject[] rematchButtonsBG;

    private int votesNeeded;
    private int currentVotes = 0;
    private int noVotes;
    private PlayerController[] loadedPlayers;

    //New speechbubble feature
    public Image winningCharSpeechBubble;
    public Sprite[] winnerSpeechBubble;
    public string[] patriotWinningLines;
    public string[] centaurshipWinningLines;
    public string[] graduateWinningLines;
    public string[] profWinningLines;
    public TextMeshProUGUI speechBubbleText;
    private List<string[]> characterSpeechBubbleLines = new List<string[]>();
    private List<int> playersToVote = new List<int>() { 1, 2, 3, 4 };

    // Start is called before the first frame update
    void Start()
    {
        //Speech bubble feature. Adding all arrays to list for easy access.
        characterSpeechBubbleLines.Add(patriotWinningLines);
        characterSpeechBubbleLines.Add(centaurshipWinningLines);
        characterSpeechBubbleLines.Add(graduateWinningLines);
        characterSpeechBubbleLines.Add(profWinningLines);
        loadedPlayers = GameObject.FindObjectsOfType<PlayerController>();
        ResultsScreenPopulate();
        ResetPlayerStats();
        StartCoroutine(ResultsScreenTimer());
        GameMusicMenu.instance.PlayLevelMusic(SceneManager.GetActiveScene().buildIndex);
        votesNeeded = loadedPlayers.Length;
        noVotes = loadedPlayers.Length;
    }

    public void VoteForRematch(int playerID)
    {
        //By defualt all players vote no for a rematch. When a player presses the button we add 1 to the yes votes and subtract 1 from the no votes.
        currentVotes++;
        noVotes--;
        playersToVote.Remove(playerID);
    }

    private void ResetPlayerStats()
    {
        //Stats are reset once the player reaches the victory screen to prevent them from winning instantly if they enter another match.
        foreach (PlayerController player in loadedPlayers) 
        {
            player.ResetPlayerStats();
        }
    }


    private void ResultsScreenPopulate()
    {
        PlayerController winner = null;
        if(loadedPlayers.Length > 0) winner = loadedPlayers[0];
        if (winner == null) return;
        foreach (PlayerController player in loadedPlayers) 
        {
            if(player.GetPlayerWins() > winner.GetPlayerWins())
            {
                winner = player;
            }
        }
        StartCoroutine(DelayPlayerVictoryLine(winner));
        playersText[0].text = "P" + winner.GetPlayerID().ToString();
        winningPlayerWins.text = "Round Wins: " + winner.GetPlayerWins();
        winningPlayerElims.text = "Eliminations: " + winner.GetPlayerEliminations();
        winningSelfElims.text = "Self Eliminations: " + winner.GetPlayerSelfElims();
        victoryPortraits[winner.GetPlayerCharacter()].SetActive(true);
        victoryBanners[winner.GetPlayerCharacter()].SetActive(true);
        List<PlayerController> tempList = loadedPlayers.ToList();

        //Speech bubble winner feature
        winningCharSpeechBubble.sprite = winnerSpeechBubble[winner.GetPlayerCharacter()];
        speechBubbleText.text = characterSpeechBubbleLines[winner.GetPlayerCharacter()][Random.Range(0, characterSpeechBubbleLines[winner.GetPlayerCharacter()].Length)];
        tempList.Remove(winner);
        //Sort list so it is in winner order
        tempList = tempList.OrderByDescending(player => player.GetPlayerWins()).ToList();   
        int i = 0;
        foreach (PlayerController player in tempList)
        {
            playersText[i+1].text = "P" + tempList[i].GetPlayerID().ToString();
            losingPlayersElims[i].text = "Wins: " + tempList[i].GetPlayerWins();
            losingPortraits[i].GetComponent<Image>().sprite = losingPortraitImages[tempList[i].GetPlayerCharacter()];
            playersText[i+1].enabled = true;
            losingPlayersElims[i].enabled = true;
            losingPortraits[i].SetActive(true);
            i++;
        }
    }


    IEnumerator ResultsScreenTimer()
    {
        yield return new WaitForSeconds(resultsScreenTime);
        rematchCanvas.SetActive(true);
        CanvasGroup canvas = rematchCanvas.GetComponent<CanvasGroup>();
        var shieldTween = DOTween.To(() => canvas.alpha, x => canvas.alpha = x, 1, 1);
        foreach (PlayerController player in loadedPlayers)
        {
            rematchButtonsBG[player.GetPlayerID() - 1].GetComponent<Image>().color = Color.white;
        }
        yield return new WaitForSeconds(1);
        //Assign each players input system to their button here. If a player doesnt exist we show their button but it is greyed out.
        //Players can join on the results screen but they don't get a vote.
        foreach(PlayerController player in loadedPlayers)
        {
            //Assign players their buttons so they may select and set the color of the button white, so it doesn't have a gray appearance
            player.playerUIAssigner.mpEventSystem.playerRoot = rematchCanvas;
            player.playerUIAssigner.mpEventSystem.SetSelectedGameObject(rematchButtons[player.GetPlayerID()-1]);
        }

        float currenttime = rematchVoteTime;
        float elapsedtime = 0;  
        bool rematch = false;
        while (elapsedtime < rematchVoteTime) 
        {
            if (votesNeeded == currentVotes)
            {
                //return to level select
                //StartCoroutine(LevelTransition(3));
                rematch = true;
                break;
            }
            yield return null;
            elapsedtime += Time.deltaTime;
            currenttime -= Time.deltaTime;
            if(rematchVoteTime > 0) rematchCountdown.text = ((int)currenttime).ToString();
        }
        foreach (PlayerController player in loadedPlayers)
        {
            //Assign players their buttons so they may select and set the color of the button white, so it doesn't have a gray appearance
            //Issue where player UI could be active over others. Likely related to this! Bug showed in class
            player.playerUIAssigner.mpEventSystem.playerRoot = null;
            player.playerUIAssigner.mpEventSystem.SetSelectedGameObject(null);
            if(playersToVote.Contains(player.playerID))
            {
                Destroy(player.gameObject);
            }
        }
        //Figure out tie. In the case of a tie the game will always return to the menu
        if (currentVotes == noVotes || currentVotes < noVotes)
        {
            //if a tie occurs return to the main menu
            rematch = false;
        }
        //if enough players have voted for a match to begin we continue
        if (currentVotes >= 2) rematch = true;
        if (rematch)
        {
            //return to level select if a tie isn't made
            rematch = true;
            StartCoroutine(LevelTransition(3));
        }
        else
        {
            StartCoroutine(LevelTransition(0));
        }
        

        //Change this to pop up that appears after 7.5 seconds then players can hit B1 to vote to rematch. If a vote times out players return to lobby. If rematch is chosen return players to level select.
    }

    IEnumerator DelayPlayerVictoryLine(PlayerController player)
    {
        //Announcer says win then player says their line
        yield return new WaitForSeconds(Announcer.instance.PlayerPlayerVictory(player).length);
        player.PlayPlayerVoiceline(CharacterVoiceLine.Victory);
    }


    IEnumerator LevelTransition(int sceneID)
    {
        yield return new WaitForSeconds(1);
        GameMusicMenu.instance.BeginMusic(sceneID);
        GameMusicMenu.instance.levelMusicPlayer.Stop();
        SceneTransitionerManager.instance.StartTransition();
        AsyncOperation asyncLoad = null;
        yield return new WaitForSecondsRealtime(1);
        asyncLoad = SceneManager.LoadSceneAsync(sceneID, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;
        asyncLoad.allowSceneActivation = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnDestroy()
    {
        
    }
}
