using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Announcer : MonoBehaviour
{

    [Tooltip("Set this to true if you want a new clip to interupt the current playing one.")] private bool nextClipInteruptsCurrent = true;
    //A voice line has a array reference if it has multiple variations. One is randomly chosen at runtime.
    public AudioClip[] allPlayersCentaurship;
    public AudioClip[] allPlayersGraduate;
    public AudioClip[] allPlayersPatriot;
    public AudioClip[] allPlayersProfessor;

    public AudioClip[] centaurShipIntro;
    public AudioClip[] graduateIntro;
    public AudioClip[] patriotIntro;
    public AudioClip[] professorIntro;

    public AudioClip[] centaurShipEliminated;
    public AudioClip[] graduateEliminated;
    public AudioClip[] patriotEliminated;
    public AudioClip[] professorEliminated;

    public AudioClip[] player1Intro;
    public AudioClip[] player2Intro;
    public AudioClip[] player3Intro;
    public AudioClip[] player4Intro;

    public AudioClip[] player1Victory;
    public AudioClip[] player2Victory;
    public AudioClip[] player3Victory;
    public AudioClip[] player4Victory;

    public AudioClip[] player1RoundWin;
    public AudioClip[] player2RoundWin;
    public AudioClip[] player3RoundWin;
    public AudioClip[] player4RoundWin;

    public AudioClip[] stadiumIntro;
    public AudioClip[] gardenIntro;
    public AudioClip[] forestIntro;

    public AudioClip[] lowTimeHint;
    public AudioClip[] pressureBlockHint;


    public AudioClip gameCountdown;
    public AudioClip chooseCharacter;
    public AudioClip chooseStage;
    public AudioClip count321;
    public AudioClip go;

    public AudioSource announcerAudioSource;


    public AnnouncerLine currentAnnouncerLine { get; private set; }


    public static Announcer instance { get; private set; }


    private void Awake()
    {
        //Create a singleton reference to the announcer class. This allows this class to be referenced by anything.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null && instance != this)
        {
            Debug.Log("Announcer object already exists. Deleting clone");
            DestroyImmediate(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void AnnouncerStopCurrentLine()
    {
        announcerAudioSource.Stop();
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (this != null) announcerAudioSource.Stop();
        else return;
        switch (arg0.buildIndex) 
        {
            default:
                //if the scene doesn't match do nothing.
                break;
            case 0:
                //Add case for game title said here.
                break;
            case 2:
                PlayAnnouncerLine(AnnouncerLine.ChooseYourCharacter);
                break;
            case 3:
                PlayAnnouncerLine(AnnouncerLine.ChooseYourStage);
                break;
                 
        }
    }

    public AudioClip PlayPlayerIntro(PlayerController playerController)
    {
        //Pass in a player controller and play the correct player intro contextually. Player ID is 1-4
        switch(playerController.GetPlayerID())
        {
            default: return PlayAnnouncerLine(AnnouncerLine.Player1Intro);
            case 1:
                return PlayAnnouncerLine(AnnouncerLine.Player1Intro);
            case 2:
                return PlayAnnouncerLine(AnnouncerLine.Player2Intro);
            case 3:
                return PlayAnnouncerLine(AnnouncerLine.Player3Intro);
            case 4:
                return PlayAnnouncerLine(AnnouncerLine.Player4Intro);
        }
    }

    public AudioClip PlayerPlayerRoundWin(PlayerController playerController)
    {
        //Pass in a player controller and play the correct player intro contextually. Player ID is 1-4
        switch (playerController.GetPlayerID())
        {
            default: return PlayAnnouncerLine(AnnouncerLine.Player1RoundWin);
            case 1:
                return PlayAnnouncerLine(AnnouncerLine.Player1RoundWin);
            case 2:
                return PlayAnnouncerLine(AnnouncerLine.Player2RoundWin);
            case 3:
                return PlayAnnouncerLine(AnnouncerLine.Player3RoundWin);
            case 4:
                return PlayAnnouncerLine(AnnouncerLine.Player4RoundWin);
        }
    }

    public AudioClip PlayLevelIntro(int sceneID)
    {
        switch(sceneID)
        {
            default:
                return PlayAnnouncerLine(AnnouncerLine.StadiumIntro);
            case 4:
                return PlayAnnouncerLine(AnnouncerLine.StadiumIntro);
            case 5:
                return PlayAnnouncerLine(AnnouncerLine.GardenIntro);
            case 6:
                return PlayAnnouncerLine(AnnouncerLine.DarkForestIntro);
        }
    }

    public AudioClip PlayerPlayerVictory(PlayerController playerController)
    {
        //Pass in a player controller and play the correct player intro contextually. Player ID is 1-4
        switch (playerController.GetPlayerID())
        {
            default: return PlayAnnouncerLine(AnnouncerLine.Player1Victory);
            case 1:
                return PlayAnnouncerLine(AnnouncerLine.Player1Victory);
            case 2:
                return PlayAnnouncerLine(AnnouncerLine.Player2Victory);
            case 3:
                return PlayAnnouncerLine(AnnouncerLine.Player3Victory);
            case 4:
                return PlayAnnouncerLine(AnnouncerLine.Player4Victory);
        }
    }

    public AudioClip PlayCharacterIntro(PlayerController playerController)
    {
        //Pass in a player controller and play the correct player intro contextually. Player ID is 1-4
        switch (playerController.GetPlayerCharacter())
        {
            default: return PlayAnnouncerLine(AnnouncerLine.PatriotIntro);
            case 0:
                return PlayAnnouncerLine(AnnouncerLine.PatriotIntro);
            case 1:
                return PlayAnnouncerLine(AnnouncerLine.CentaurshipIntro);
            case 2:
                return PlayAnnouncerLine(AnnouncerLine.GraduateIntro);
            case 3:
                return PlayAnnouncerLine(AnnouncerLine.ProfessorIntro);
        }
    }

    public AudioClip PlayCharacterElimLine(PlayerController playerController)
    {
        //Pass in a player controller and play the correct player intro contextually. Player ID is 1-4
        switch (playerController.GetPlayerCharacter())
        {
            default: return PlayAnnouncerLine(AnnouncerLine.PatriotElim);
            case 0:
                return PlayAnnouncerLine(AnnouncerLine.PatriotElim);
            case 1:
                return PlayAnnouncerLine(AnnouncerLine.CentaurshipElim);
            case 2:
                return PlayAnnouncerLine(AnnouncerLine.GraduateElim);
            case 3:
                return PlayAnnouncerLine(AnnouncerLine.ProfessorElim);
        }
    }


    public AudioClip AreAllPlayersTheSame(GameObject[] playerControllers) 
    {
        //Set a temp value to check if all players are the same
        int tempChar = (int)PlayerCharacter.Patriot;
        //Assume first that all players are the same
        bool isTheSame = true;
        //Ensure that the playerController list has something on it
        if (playerControllers.Length > 0)
        {
            //If we can get a player controller reference on the first player in the array we set our temp PlayerCharacter value to their playerchar value
            if (playerControllers[0].GetComponent<PlayerController>() != null)
            {
                tempChar = playerControllers[0].GetComponent<PlayerController>().GetPlayerCharacter();
            }
        }
        //If we return null something bad happened. Should be impossible as players cannot leave during the intro to prevent this issue
        else return null;
        //We iterate over the passed in list of game objects turning them into a list preventing a possible run time error. 
        foreach(GameObject playerController in playerControllers.ToList()) 
        { 
            //Check if the current players character type equals the last one
            if(tempChar != playerController.GetComponent<PlayerController>().GetPlayerCharacter())
            {
                //if it doesn't all players aren't the same and we can break the loop
                isTheSame = false;
                Debug.Log(isTheSame.ToString());
                break;
            }
        }
        //if all players are the same we switch on their character type to player the correct line
        if(isTheSame)
        {
            switch(tempChar) 
            {
                default:
                    return null;
                case 0:
                    Debug.Log("All players patriot");
                    return PlayAnnouncerLine(AnnouncerLine.AllPlayersPatriot);
                case 1:
                    Debug.Log("All players centaur");
                    return PlayAnnouncerLine(AnnouncerLine.AllPlayersCentaurship);
                case 2:
                    Debug.Log("All players graduate");
                    return PlayAnnouncerLine(AnnouncerLine.AllPlayersGraduate);
                case 3:
                    Debug.Log("All players professor");
                    return PlayAnnouncerLine(AnnouncerLine.AllPlayersProfessor);
            }
        }
        //if we get here characters are not the same.
        return null;
    }










    //This class handles playing announcer lines and the randomization of said lines
    public AudioClip PlayAnnouncerLine(AnnouncerLine lineToPlay)
    {
        //Don't play if a line is already playing.
        if (announcerAudioSource.isPlaying && !nextClipInteruptsCurrent) return null;

        announcerAudioSource.Stop();
        currentAnnouncerLine = lineToPlay;
        //If we play a line we find it and randomize it if applicable
        AudioClip clipToPlay;
        switch (lineToPlay)
        {
            case AnnouncerLine.AllPlayersCentaurship:
                clipToPlay = allPlayersCentaurship[Random.Range(0, allPlayersCentaurship.Length)];
                break;
            case AnnouncerLine.AllPlayersGraduate:
                clipToPlay = allPlayersGraduate[Random.Range(0, allPlayersGraduate.Length)];
                break;
            case AnnouncerLine.AllPlayersPatriot:
                clipToPlay = allPlayersPatriot[Random.Range(0, allPlayersPatriot.Length)];
                break;
            case AnnouncerLine.AllPlayersProfessor:
                clipToPlay = allPlayersProfessor[Random.Range(0, allPlayersProfessor.Length)];
                break;
            case AnnouncerLine.GameCountdown:
                clipToPlay = gameCountdown;
                break;
            case AnnouncerLine.Go:
                clipToPlay = go;
                break;
            case AnnouncerLine.Count321:
                clipToPlay = count321;
                break;
            case AnnouncerLine.CentaurshipIntro:
                clipToPlay = centaurShipIntro[Random.Range(0, centaurShipIntro.Length)];
                break;
            case AnnouncerLine.GraduateIntro:
                clipToPlay = graduateIntro[Random.Range(0, graduateIntro.Length)];
                break;
            case AnnouncerLine.PatriotIntro:
                clipToPlay = patriotIntro[Random.Range(0, patriotIntro.Length)];
                break;
            case AnnouncerLine.ProfessorIntro:
                clipToPlay = professorIntro[Random.Range(0, professorIntro.Length)];
                break;
            case AnnouncerLine.Player1Intro:
                clipToPlay = player1Intro[Random.Range(0, player1Intro.Length)];
                break;
            case AnnouncerLine.Player2Intro:
                clipToPlay = player2Intro[Random.Range(0, player2Intro.Length)];
                break;
            case AnnouncerLine.Player3Intro:
                clipToPlay = player3Intro[Random.Range(0, player3Intro.Length)];
                break;
            case AnnouncerLine.Player4Intro:
                clipToPlay = player4Intro[Random.Range(0, player4Intro.Length)];
                break;
            case AnnouncerLine.ChooseYourCharacter:
                clipToPlay = chooseCharacter;
                break;
            case AnnouncerLine.DarkForestIntro:
                clipToPlay = forestIntro[Random.Range(0, forestIntro.Length)];
                break;
            case AnnouncerLine.CentaurshipElim:
                clipToPlay = centaurShipEliminated[Random.Range(0, centaurShipEliminated.Length)];
                break;
            case AnnouncerLine.GraduateElim:
                clipToPlay = graduateEliminated[Random.Range(0, graduateEliminated.Length)];
                break;
            case AnnouncerLine.PatriotElim:
                clipToPlay = patriotEliminated[Random.Range(0, patriotEliminated.Length)];
                break;
            case AnnouncerLine.ProfessorElim:
                clipToPlay = professorEliminated[Random.Range(0, professorEliminated.Length)];
                break;
            case AnnouncerLine.GardenIntro:
                clipToPlay = gardenIntro[Random.Range(0, gardenIntro.Length)];
                break;
            case AnnouncerLine.LowTimeHint:
                clipToPlay = lowTimeHint[Random.Range(0, lowTimeHint.Length)];
                break;
            case AnnouncerLine.Player1Victory:
                clipToPlay = player1Victory[Random.Range(0, player1Victory.Length)];
                break;
            case AnnouncerLine.Player2Victory:
                clipToPlay = player2Victory[Random.Range(0, player2Victory.Length)];
                break;
            case AnnouncerLine.Player3Victory:
                clipToPlay = player3Victory[Random.Range(0, player3Victory.Length)];
                break;
            case AnnouncerLine.Player4Victory:
                clipToPlay = player4Victory[Random.Range(0, player4Victory.Length)];
                break;
            case AnnouncerLine.Player1RoundWin:
                clipToPlay = player1RoundWin[Random.Range(0, player1RoundWin.Length)];
                break;
            case AnnouncerLine.Player2RoundWin:
                clipToPlay = player2RoundWin[Random.Range(0, player2RoundWin.Length)];
                break;
            case AnnouncerLine.Player3RoundWin:
                clipToPlay = player3RoundWin[Random.Range(0, player3RoundWin.Length)];
                break;
            case AnnouncerLine.Player4RoundWin:
                clipToPlay = player4RoundWin[Random.Range(0, player4RoundWin.Length)];
                break;
            case AnnouncerLine.PressureBlockSpawn:
                clipToPlay = pressureBlockHint[Random.Range(0, pressureBlockHint.Length)];
                break;
            case AnnouncerLine.ChooseYourStage:
                clipToPlay = chooseStage;
                break;
            case AnnouncerLine.StadiumIntro:
                clipToPlay = stadiumIntro[Random.Range(0, stadiumIntro.Length)];
                break;
            default:
                clipToPlay = chooseStage;
                break;
        }
        //We set clipToPlay to the clip in the announcer audio source.
        announcerAudioSource.clip = clipToPlay;
        if (clipToPlay != null)
        {
            //play the clip
            announcerAudioSource.Play();
            //return the clip so we can get its total length in seconds in other scripts
        }
        else Debug.Log("clip was null. Annoucer script");
        return clipToPlay;
    }
}

public enum AnnouncerLine
{
    //The enums that can be called for a line to play. Call PlayAnnouncerLine with one of these enums.
    AllPlayersCentaurship = 0,
    AllPlayersGraduate = 1,
    AllPlayersPatriot = 2,
    AllPlayersProfessor = 3,
    GameCountdown = 4,
    CentaurshipIntro = 5,
    GraduateIntro = 6,
    PatriotIntro = 7,
    ProfessorIntro = 8,
    Player1Intro = 9,
    Player2Intro = 10,
    Player3Intro = 11,
    Player4Intro = 12,
    ChooseYourCharacter = 13,
    DarkForestIntro = 14,
    CentaurshipElim = 15,
    GraduateElim = 16,
    PatriotElim = 17,
    ProfessorElim = 18,
    GardenIntro = 19,
    LowTimeHint = 20,
    Player1Victory = 21,
    Player2Victory = 22,
    Player3Victory = 23,
    Player4Victory = 24,
    Player1RoundWin = 25,
    Player2RoundWin = 26,
    Player3RoundWin = 27,
    Player4RoundWin = 28,
    PressureBlockSpawn = 29,
    ChooseYourStage = 30,
    StadiumIntro = 31,
    Go = 32,
    Count321 = 33
}
