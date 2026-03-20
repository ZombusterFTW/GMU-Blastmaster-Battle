using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static KeyboardSplitter;

public class PlayerUIManager : MonoBehaviour
{
    public PlayerUIContainer[] playerUIHints;

    public TextMeshProUGUI roundCounter;
    public TextMeshProUGUI timeRemaining;
    public TextMeshProUGUI countdown;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI whoDisText;

    public RoundManager roundManager;
    public GameObject winBanner;

    public Sprite defaultBomb;
    public Sprite pierceBomb;
    public Sprite lightningBomb;
    public Sprite remoteBomb;
    public Sprite slideBomb;
    public Sprite wingsImage;
    public Sprite glovesImage;


    public Sprite[] characterPortraits;

    // Start is called before the first frame update
    void Start()
    {
        roundManager = GameObject.FindObjectOfType<RoundManager>();
       // InvokeRepeating("UpdatePlayerJoinHint", 0, 0.05f);
    }

    // Update is called once per frame
    void Update()
    {
        if (roundManager.IsStartingCountdownActive()) UpdateRoundStartCountdown();
        else countdown.enabled = false;
        if (roundManager.IsRoundActive()) UpdateRoundInGameCountdown();
        else timeRemaining.enabled = false;
        if (roundManager.GetIntroActive()) UpdateWhoDisText();
        else whoDisText.enabled = false;



        UpdateWinner();
        //UpdateRoundCounter();
        UpdatePlayerJoinHint();
    }

    public void ShakeClock(bool shake)
    {
        if (shake) timeRemaining.transform.DOShakePosition(roundManager.pressureBlockSpawnTime, 5, 15);
        else timeRemaining.transform.DOComplete();
    }

    public void PlayWinBanner(bool reverse)
    {
        //take in bool to determine which direction to play the animation
        if(reverse) 
        {
            winBanner.GetComponent<Animator>().Play("WinBanner");
        }
        else 
        {
            winBanner.GetComponent<Animator>().Play("ReverseWinBanner");
        }
        GameMusicMenu.instance.transitionSoundPlayer.pitch = Random.Range(1.2f, 1.8f);
        GameMusicMenu.instance.transitionSoundPlayer.Play();
    }

    void UpdateWhoDisText()
    {
        whoDisText.enabled = true;
        whoDisText.text = roundManager.GetWhoDis();
    }


    void UpdateWinner()
    {
        if(roundManager.IsWinInProgress())
        {
            winnerText.enabled = true;
            timeRemaining.enabled = false;
            countdown.enabled = false;
            winnerText.text = roundManager.GetWhoWon();
        }
        else
        {
            winnerText.enabled = false;
        }
    }


    void UpdateRoundStartCountdown()
    {
       // timeRemaining.enabled = false;
        countdown.enabled = true;

        if (roundManager.GetStartingCountdownTime() > 0)
        {
            countdown.text = roundManager.GetStartingCountdownTime().ToString();
        }
        else if (roundManager.GetStartingCountdownTime() == -1)
        {
            countdown.text = "Go!";
        }
        else if(roundManager.GetStartingCountdownTime() <= 0) countdown.text = "";
    }

    void UpdateRoundInGameCountdown()
    {
       // countdown.enabled = false;
        timeRemaining.enabled = true;
        int minutes = (int)(roundManager.GetRemainingRoundTime() / 60);

        int seconds = (int)roundManager.GetRemainingRoundTime() % 60;

        int seconds2 = (int)roundManager.GetRemainingRoundTime();

        //Debug.Log("Time Remaining: " + roundManager.GetRemainingRoundTime().ToString() + "s");
        //Debug.Log(minutes);

        if (!roundManager.GetIsOvertimeActive())
        {
            if (minutes < 1)
            {
                timeRemaining.text = string.Format("{0:00}:{1:00}", minutes, seconds2);
                if(seconds2 <= roundManager.pressureBlockSpawnTime)
                {
                    timeRemaining.text = string.Format("<color=red>{0:00}:{1:00}</color>", minutes, seconds2);
                }
            }
            else
            {
                timeRemaining.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }
        else timeRemaining.text = "<color=red>Overtime</color>";

    }

    public void UpdateRoundCounter()
    {
        roundCounter.text = "Round: " + roundManager.GetRound();
    }

    void UpdatePlayerJoinHint()
    {

        //Change player UI depending on if a player has joined, left, or about to join.

        //Grab the list of joining players and the list of players that have joined
        List<PlayerController> playerArray = roundManager.GetJoinedPlayers();
        List<PlayerController> joiningPlayerList = roundManager.GetJoiningPlayers();
        List<PlayerController> eliminatedPlayerList = roundManager.GetEliminatedPlayers();
        //Loop over all of the player specfic text UI hints

        
        for (int i = 0; i < playerUIHints.Length; i++)
        {
            playerUIHints[i].playerUIHint.text = "P" + (i+1).ToString() + " Tap To Join";
            //playerUIHints[i].shieldCountText.text = "";
            playerUIHints[i].bombCapText.text = "";
            playerUIHints[i].bombFireLevelText.text = "";
            playerUIHints[i].moveSpeedText.text = "";
            playerUIHints[i].playerWinsText.text = "";
            playerUIHints[i].bombTypeImage.color = Color.black;
            playerUIHints[i].bombFireLevelImage.color = Color.black;
            playerUIHints[i].bombCapImage.color = Color.black;
            playerUIHints[i].playerPowerupImage.color = Color.black;
            playerUIHints[i].skatesImage.color = Color.black;
            playerUIHints[i].shieldTypeImage.color = Color.black;
            playerUIHints[i].playerCharacterPortrait.color = Color.black;
            playerUIHints[i].playerCharacterPortrait.sprite = characterPortraits[i];
        }
        
        
        
        foreach (PlayerController player in playerArray.ToList()) 
        {
            if (player != null)
            {
                playerUIHints[player.GetPlayerID() - 1].playerUIHint.text = "Player " + player.GetPlayerID();
                playerUIHints[player.GetPlayerID() - 1].playerCharacterPortrait.sprite = characterPortraits[player.GetPlayerCharacter()];
            }

        }

        foreach (PlayerController player in joiningPlayerList.ToList())
        {
            if (player != null)
            {
                playerUIHints[player.GetPlayerID() - 1].playerUIHint.text = "P" + player.GetPlayerID() + " Will Join Next Round!";
                playerUIHints[player.GetPlayerID() - 1].bombTypeImage.color = Color.black;
                playerUIHints[player.GetPlayerID() - 1].bombFireLevelImage.color = Color.black;
                playerUIHints[player.GetPlayerID() - 1].bombCapImage.color = Color.black;
                playerUIHints[player.GetPlayerID() - 1].playerPowerupImage.color = Color.black;
                playerUIHints[player.GetPlayerID() - 1].skatesImage.color = Color.black;
                playerUIHints[player.GetPlayerID() - 1].shieldTypeImage.color = Color.black;
                playerUIHints[player.GetPlayerID() - 1].playerCharacterPortrait.sprite = characterPortraits[player.GetPlayerCharacter()];
                playerUIHints[player.GetPlayerID() - 1].playerCharacterPortrait.color = Color.white;
            }
        }
    


        foreach (PlayerController player in playerArray.ToList())
        {
            if (player != null)
            {
                //This UI element has hidden
               // playerUIHints[player.GetPlayerID() - 1].shieldCountText.text = player.GetShieldCount().ToString();
                playerUIHints[player.GetPlayerID() - 1].bombCapText.text = player.GetBombCap().ToString();
                playerUIHints[player.GetPlayerID() - 1].bombFireLevelText.text = player.GetBombFireLevel().ToString();
                playerUIHints[player.GetPlayerID() - 1].moveSpeedText.text = player.GetPlayerMoveSpeed().ToString("F1") + ""; // Format float value to 2 decimal places
                playerUIHints[player.GetPlayerID() - 1].playerWinsText.text = "Wins: " + player.GetPlayerWins().ToString();


                playerUIHints[player.GetPlayerID() - 1].bombFireLevelImage.color = Color.white;
                playerUIHints[player.GetPlayerID() - 1].bombCapImage.color = Color.white;
                playerUIHints[player.GetPlayerID() - 1].bombTypeImage.color = Color.white;
                playerUIHints[player.GetPlayerID() - 1].bombTypeImage.sprite = GetBombTypeImage(player.GetPlayerBombType());
                playerUIHints[player.GetPlayerID() - 1].playerCharacterPortrait.color = Color.white;


                if (player.GetHasShield()) playerUIHints[player.GetPlayerID() - 1].shieldTypeImage.color = Color.white;
                else playerUIHints[player.GetPlayerID() - 1].shieldTypeImage.color = Color.black;


                if (player.GetPlayerMoveSpeed() > 1) playerUIHints[player.GetPlayerID() - 1].skatesImage.color = Color.white;
                else playerUIHints[player.GetPlayerID() - 1].skatesImage.color = Color.black;

                if (player.GetPlayerPowerup() != PlayerPowerup.None)
                {
                    playerUIHints[player.GetPlayerID() - 1].playerPowerupImage.color = Color.white;
                    playerUIHints[player.GetPlayerID() - 1].playerPowerupImage.sprite = GetPlayerPowerupImage(player.GetPlayerPowerup());
                }
                else playerUIHints[player.GetPlayerID() - 1].playerPowerupImage.color = Color.black;
            }

        }
        foreach (PlayerController player in eliminatedPlayerList.ToList())
        {
            if (player != null) playerUIHints[player.GetPlayerID() - 1].playerUIHint.text = "P" + player.GetPlayerID() + " Out!";
            //playerUIHints[player.GetPlayerID() - 1].shieldCountText.text = "";
            playerUIHints[player.GetPlayerID() - 1].bombCapText.text = "";
            playerUIHints[player.GetPlayerID() - 1].bombFireLevelText.text = "";
            playerUIHints[player.GetPlayerID() - 1].moveSpeedText.text = "";
            playerUIHints[player.GetPlayerID() - 1].playerWinsText.text = "Wins: " + player.GetPlayerWins().ToString();
            playerUIHints[player.GetPlayerID() - 1].bombTypeImage.color = Color.black;
            playerUIHints[player.GetPlayerID() - 1].bombFireLevelImage.color = Color.black;
            playerUIHints[player.GetPlayerID() - 1].bombCapImage.color = Color.black;
            playerUIHints[player.GetPlayerID() - 1].playerPowerupImage.color = Color.black;
            playerUIHints[player.GetPlayerID() - 1].skatesImage.color = Color.black;
            playerUIHints[player.GetPlayerID() - 1].shieldTypeImage.color = Color.black;
            playerUIHints[player.GetPlayerID() - 1].playerCharacterPortrait.color = Color.gray;
        }


    }

    public void PlayerEnterMatchUIChange(int playerID, PlayerController player)
    {
        /*
        playerUIHints[player.GetPlayerID() - 1].playerUIHint.text = "Player " + player.GetPlayerID();
        playerUIHints[player.GetPlayerID() - 1].playerCharacterPortrait.sprite = characterPortraits[player.GetPlayerCharacter()];
        playerUIHints[player.GetPlayerID() - 1].bombCapText.text = player.GetBombCap().ToString();
        playerUIHints[player.GetPlayerID() - 1].bombFireLevelText.text = player.GetBombFireLevel().ToString();
        playerUIHints[player.GetPlayerID() - 1].moveSpeedText.text = player.GetPlayerMoveSpeed().ToString("F1") + ""; // Format float value to 2 decimal places
        playerUIHints[player.GetPlayerID() - 1].playerWinsText.text = "Wins: " + player.GetPlayerWins().ToString();


        playerUIHints[player.GetPlayerID() - 1].bombFireLevelImage.color = Color.white;
        playerUIHints[player.GetPlayerID() - 1].bombCapImage.color = Color.white;
        playerUIHints[player.GetPlayerID() - 1].bombTypeImage.color = Color.white;
        playerUIHints[player.GetPlayerID() - 1].bombTypeImage.sprite = GetBombTypeImage(player.GetPlayerBombType());
        playerUIHints[player.GetPlayerID() - 1].playerCharacterPortrait.color = Color.white;


        if (player.GetHasShield()) playerUIHints[player.GetPlayerID() - 1].shieldTypeImage.color = Color.white;
        else playerUIHints[player.GetPlayerID() - 1].shieldTypeImage.color = Color.black;


        if (player.GetPlayerMoveSpeed() > 1) playerUIHints[player.GetPlayerID() - 1].skatesImage.color = Color.white;
        else playerUIHints[player.GetPlayerID() - 1].skatesImage.color = Color.black;

        if (player.GetPlayerPowerup() != PlayerPowerup.None)
        {
            playerUIHints[player.GetPlayerID() - 1].playerPowerupImage.color = Color.white;
            playerUIHints[player.GetPlayerID() - 1].playerPowerupImage.sprite = GetPlayerPowerupImage(player.GetPlayerPowerup());
        }
        else playerUIHints[player.GetPlayerID() - 1].playerPowerupImage.color = Color.black;
        */
    }




    public void LeavePlayerChange(int playerID, PlayerController player)
    {
        /*
        int i = playerID - 1;
        playerUIHints[i].playerUIHint.text = "P" + (i + 1).ToString() + " Tap To Join";
        //playerUIHints[i].shieldCountText.text = "";
        playerUIHints[i].bombCapText.text = "";
        playerUIHints[i].bombFireLevelText.text = "";
        playerUIHints[i].moveSpeedText.text = "";
        playerUIHints[i].playerWinsText.text = "";
        playerUIHints[i].bombTypeImage.color = Color.black;
        playerUIHints[i].bombFireLevelImage.color = Color.black;
        playerUIHints[i].bombCapImage.color = Color.black;
        playerUIHints[i].playerPowerupImage.color = Color.black;
        playerUIHints[i].skatesImage.color = Color.black;
        playerUIHints[i].shieldTypeImage.color = Color.black;
        playerUIHints[i].playerCharacterPortrait.color = Color.black;
        playerUIHints[i].playerCharacterPortrait.sprite = characterPortraits[i];
        */
    }




    public void JoinPlayerUIChange(int playerID, PlayerController player)
    {
        /*
        playerUIHints[player.GetPlayerID() - 1].playerUIHint.text = "P" + player.GetPlayerID() + " Will Join Next Round!";
        playerUIHints[player.GetPlayerID() - 1].bombTypeImage.color = Color.black;
        playerUIHints[player.GetPlayerID() - 1].bombFireLevelImage.color = Color.black;
        playerUIHints[player.GetPlayerID() - 1].bombCapImage.color = Color.black;
        playerUIHints[player.GetPlayerID() - 1].playerPowerupImage.color = Color.black;
        playerUIHints[player.GetPlayerID() - 1].skatesImage.color = Color.black;
        playerUIHints[player.GetPlayerID() - 1].shieldTypeImage.color = Color.black;
        playerUIHints[player.GetPlayerID() - 1].playerCharacterPortrait.sprite = characterPortraits[player.GetPlayerCharacter()];
        playerUIHints[player.GetPlayerID() - 1].playerCharacterPortrait.color = Color.white;
        playerUIHints[player.GetPlayerID() - 1].bombCapText.text = "";
        playerUIHints[player.GetPlayerID() - 1].bombFireLevelText.text = "";
        playerUIHints[player.GetPlayerID() - 1].moveSpeedText.text = "";
        */
    }


    public void EliminatePlayerUIChange(int playerID, PlayerController player)
    {
        /*
        int tempIndex = playerID - 1;
        playerUIHints[tempIndex].playerUIHint.text = "P" + playerID + " Out!";
        //playerUIHints[player.GetPlayerID() - 1].shieldCountText.text = "";
        playerUIHints[tempIndex].bombCapText.text = "";
        playerUIHints[tempIndex].bombFireLevelText.text = "";
        playerUIHints[tempIndex].moveSpeedText.text = "";
        playerUIHints[tempIndex].playerWinsText.text = "Wins: " + player.GetPlayerWins().ToString();
        playerUIHints[tempIndex].bombTypeImage.color = Color.black;
        playerUIHints[tempIndex].bombFireLevelImage.color = Color.black;
        playerUIHints[tempIndex].bombCapImage.color = Color.black;
        playerUIHints[tempIndex].playerPowerupImage.color = Color.black;
        playerUIHints[tempIndex].skatesImage.color = Color.black;
        playerUIHints[tempIndex].shieldTypeImage.color = Color.black;
        playerUIHints[tempIndex].playerCharacterPortrait.color = Color.gray;
        */
    }



    public Sprite GetBombTypeImage(BombTypes bombType)
    {
        switch(bombType) 
        {
            default: return defaultBomb;
            case BombTypes.DefaultBomb:
                return defaultBomb;
            case BombTypes.PierceBomb:
                return pierceBomb;
            case BombTypes.LightningBomb:
                return lightningBomb;
            case BombTypes.RemoteBomb:
                return remoteBomb;
            case BombTypes.SlideBomb:
                return slideBomb;
        }
    }

    public Sprite GetPlayerPowerupImage(PlayerPowerup playerPowerup) 
    {
        switch(playerPowerup) 
        {
            default: return glovesImage;
            case PlayerPowerup.playerFly: return wingsImage;
            case PlayerPowerup.playerPushBombs: return glovesImage;
        }
    }
}
