using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using static KeyboardSplitter;
using Unity.VisualScripting;
using GMUBMB.Utilities.DevConsole;
//using static UnityEngine.RuleTile.TilingRuleOutput;


public class RoundManager : MonoBehaviour
{
    [Tooltip("The max time per round, measured in seconds.")]
    [SerializeField] float roundTime = 30;
    private float roundTimeCountdown_InGame = 0;
    [Tooltip("The countdown before a round starts measured in seconds.")]
    [SerializeField] int roundTimeCountdown_Start = 3;
    private int startingRoundTimeCountdown = 0;
    [Tooltip("The amount of rounds a player must win before the match ends.")]
    [SerializeField] int roundsToWin = 3;
    [Tooltip("Player spawn points")]
    [SerializeField] private GameObject[] playerSpawns;
    [Tooltip("How long the winning players name will be on screen.")]
    [SerializeField] int winTime = 5;
    private int winTimeCountdown = 0;
    [Tooltip("If player can win a round. Set to false to test with 1 or no players")]
    [SerializeField] bool playerCanWin = true;
    [Tooltip("If the round time is less than this value, pressure blocks will begin to spawn.")]
    public float pressureBlockSpawnTime  = 30f;
    private bool levelFilledWithBlocks = false;
    [SerializeField][Tooltip("If true powerups will not spawn until all destroyable blocks are gone.")] bool powerupSpawnsOffTillBlocksGone = false;
    [SerializeField][Tooltip("The time after a round begins before powerup spawn is enabled. powerupSpawnsOffTillBlocksGone must be false for this to work.")] float powerupSpawnTime = 30f;

    //An array that contains all players in the level. Joining, eliminated, or playing
    GameObject[] playerArray;
    //A list that contains all of the players that will join next round
    List<PlayerController> joiningPlayerList;
    //A list that contains all of the players that are spawned during a round
    List<PlayerController> playerControllers;
    //A list that contains all of the eliminated players per round
    List<PlayerController> eliminatedPlayers;
    //A list that contains that player that won each round. At the end of a game, these references will be used to determine which player won the whole game.
    List<PlayerController> winningPlayersList;
    //Reference to tile manager to give spawns that are snapped to a world tile. 
    private TileManager tileManager;
    public GameObject destroyableBlock;
    private int drawCounter = 0;
    //Reference to the UI manager that runs the games temp UI
    public PlayerUIManager playerUIManager;
    //Reference to the game manager so powerups can be wiped at the end of each round.
    private BombermanGameManager gameManager;
    //This +1 equals current round. This gets iterated each round
    public int roundCounter { get; private set; } = 0;
    //Wether or not the round start countdown is active
    public bool roundStartCountdownActive = true;
    //wether or not a round is in progress
    private bool isRoundInProgress = false;
    //wether or not a game is over, this happens after roundCount rounds have passed
    private bool gameOver = false;
    //This is true when the win or draw text shows on the screen
    private bool winInProgress = false;
    //This string has which player has won or if the game was a draw. Used by the UI manager.
    private string whoWon = "";
    //This sting says which player is being introduced
    private string whoDis = "";
    public bool blocksActive { get; private set; }
    //This is where you put the name of the scene that will load (StartMenu).
    public string sceneToLoad;
    private DestroyableBlock[] destroyableBlocks;
    private bool isOvertimeActive = false;  
    private List<DestroyableBlock> destroyableList;
    private GameMusicMenu gameMusic;
    private GameMusicMenu gameAmbience;
    public bool gameIntroActive {  get; private set; } = false;
    private CinemachineVirtualCamera cineCamera;
    private Transform ogCameraPos;
    AudienceMember[] audience;
    float cameraFOV = 53.3f;
    float cameraFOV2 = 15f;
    public ParticleSystem celebrationMaterial;
    public GamemodeManager gamemodeManager;
    public GameObject gameModeIdentifier;
    private int lastRoundAltMode = -1;
    private int blockChanceDropDenominator = 4;
    private Coroutine pauseResume;
    /*Round starts with countdown to zero, then the roundtime starts to tick down. Players can join during the countdown and are set to their spawns. If a player attempts joining during a round they will
     * spawn in the "player corall" an area out of view of the camera. When the next round begins they will spawn at their designated spawn.
     * update UI to say that the player will join next round.
     */
    //Powerup references for console spawning
    public GameObject fireUp;
    public GameObject bombUp;
    public GameObject wings;
    public GameObject lightningBomb;
    public GameObject slimeBomb;
    public GameObject pierceBomb;
    public GameObject gloves;
    public GameObject remoteBomb;
    public GameObject shield;
    public GameObject skates;
    public Canvas inGameUI;

// Start is called before the first frame update
void Awake()
    {
        DOTween.SetTweensCapacity(3000, 200);
        //Get tilemanager ref
        tileManager = GameObject.FindObjectOfType<TileManager>();
        cineCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        if (cineCamera != null) ogCameraPos = cineCamera.transform;
        //init lists
        joiningPlayerList = new List<PlayerController>();
        playerControllers = new List<PlayerController>();
        eliminatedPlayers = new List<PlayerController>();
        winningPlayersList = new List<PlayerController>();
        destroyableList = new List<DestroyableBlock>();
        //get game manger ref
        gameManager = FindObjectOfType<BombermanGameManager>(); 
        //set the round counter to zero
        roundCounter = 0;
        //hide the player spawns
        HidePlayerSpawns();
        //start the core game loop
        destroyableBlocks = GameObject.FindObjectsOfType<DestroyableBlock>();
        audience = GameObject.FindObjectsOfType<AudienceMember>();

    }

    public void ResumeRoundAfterGamePause()
    {
        if(pauseResume == null)
        {
            pauseResume = StartCoroutine(ResumeAfterPause());
        }
    }

    IEnumerator ResumeAfterPause()
    {
       // isRoundInProgress = false;
        roundStartCountdownActive = true;
        gameIntroActive = true;
        startingRoundTimeCountdown = roundTimeCountdown_Start;
        //pitch override so audio plays regardless of timescale
        Announcer.instance.announcerAudioSource.pitch = 1;
        Announcer.instance.PlayAnnouncerLine(AnnouncerLine.Count321);
        //Make countdown as normal. 
        while (roundStartCountdownActive == true)
        {
            //Wait one second then subtract 1 from the startingRoundCountdown variable. If we hit zero, break out of the loop and the round starts.
            yield return new WaitForSecondsRealtime(1);
            startingRoundTimeCountdown -= 1;
            if (startingRoundTimeCountdown <= 0) break;
        }
        gameIntroActive = false;
        startingRoundTimeCountdown = -1;
        Announcer.instance.PlayAnnouncerLine(AnnouncerLine.Go);
        yield return new WaitForSecondsRealtime(0.5f);
        yield return null;
        while (DeveloperConsoleBehavior.instance.consoleActive)
        {
            yield return null;
        }
        roundStartCountdownActive = false;
       //isRoundInProgress = true;
        pauseResume = null;
        Time.timeScale = EscMenu.instance.savedTimeScale;
        foreach (AudioSource audioSource in FindObjectsOfType<AudioSource>())
        {
            audioSource.pitch = Time.timeScale;
        }
    }


    public void SetLevelIsFilledWithBlock(bool isFilled)
    { levelFilledWithBlocks = isFilled; }
    public bool GetLevelIsFilledWithBlock()
    { return levelFilledWithBlocks; }

    public bool GetIntroActive()
    { return gameIntroActive; }

    public string GetWhoDis()
    { return whoDis; }

    private void Start()
    {
        //Set Tween capacity
        //if (GameObject.FindGameObjectWithTag("GameMusicMenu").GetComponent<GameMusicMenu>() != null) gameMusic = GameObject.FindGameObjectWithTag("GameMusicMenu").GetComponent<GameMusicMenu>();
        gameMusic = GameMusicMenu.instance;
        gameAmbience = GameMusicMenu.instance;
        gamemodeManager = GamemodeManager.instance;
        StartCoroutine(RoundCountdown());
        PlayerCrownManager();
    }


    private void PlayerCrownManager()
    {
        //Figure out who should have a crown if any players.
        List<PlayerController> loadedPlayers = FindObjectsOfType<PlayerController>().ToList();
        if(loadedPlayers.Count == 0) { return; }    
        List<int> playerMatchWins = new List<int>();
        foreach(PlayerController player in loadedPlayers)
        {
            player.playerAnimationObject.GetComponent<PlayerCrownBillboarder>().ToggleCrown(false);
            playerMatchWins.Add(player.playerMatchWins);
        }
        int matchWinsMax = playerMatchWins.Max(); ;
        if (matchWinsMax > 0) 
        {
            foreach (PlayerController player in loadedPlayers)
            {
                if(player.playerMatchWins ==  matchWinsMax)
                {
                    player.playerAnimationObject.GetComponent<PlayerCrownBillboarder>().ToggleCrown(true);
                }
            }
        }
    }


    private void Update()
    {
        //if the round counter equals the round count and its not a game over, we reset the game to round 1. Will be changed later to take player to the menu.
        //remove null blocks from list.
        destroyableList.RemoveAll(x => x == null);
        
    }

    private void GrabAllDestroyableBlocks()
    {
        //Clear the destroyable block list in case it isn't full
        destroyableList.Clear();
        //Grab every destroyable block in the level and add it to an array
        destroyableBlocks = GameObject.FindObjectsOfType<DestroyableBlock>();
        //Add each block in the array to the destroyable block list. This list is used to tell how many destroyable blocks still remain.
        foreach (DestroyableBlock block in destroyableBlocks) 
        {
            destroyableList.Add(block);
            block.DropChanceDenominator = blockChanceDropDenominator;
        }
    }

    public List<DestroyableBlock> GetDestroyableBlocks() 
    { return destroyableList; }


    private void WipeAllBombs()
    {
        //Wipe any bombs that may remain on the battlefield.
        GameObject[] allBombs = GameObject.FindGameObjectsWithTag("Bomb");
        foreach(GameObject bomb in allBombs)
        {
            if(bomb != null) Destroy(bomb);
        }
    }

    private void ReadGamemodeData(Gamemode_TemplateScriptableObject gameMode)
    {
        roundTime = gameMode.roundTime;
        roundTimeCountdown_Start = gameMode.roundStartCountdownTime;
        pressureBlockSpawnTime = gameMode.pressureBlockSpawnTime;
        winTime = gameMode.winTime;
        powerupSpawnTime = gameMode.powerupSpawnTime;
        powerupSpawnsOffTillBlocksGone = gameMode.dontSpawnRandomPowerupsTillAllBlocksGone;
        blockChanceDropDenominator = gameMode.powerupBlockDropChance;
        gameManager.ReadGamemodeData(gameMode);
    }

    private void ReadGameModePlayerData()
    {
        //Edit gamemode determined player options here
        foreach (PlayerController player in GetJoinedPlayers())
        {
            if (player != null)
            {
                player.bombCap_Game = gamemodeManager.currentGameMode.startingBombCap;
                player.bombFireLevel_Game = gamemodeManager.currentGameMode.startingBombRadius;
                player.playerPowerup = gamemodeManager.currentGameMode.startingPlayerPowerup;
                player.playerBombType = gamemodeManager.currentGameMode.startingBombType;
                player.moveSpeed_Game = gamemodeManager.currentGameMode.startingMoveSpeed;
                player.flightSpeed = gamemodeManager.currentGameMode.startingFlightSpeed;
                if (gamemodeManager.currentGameMode.startWithShield)
                {
                    player.hasShield = true;
                    player.ActivateShieldVFX(true);
                    if(gamemodeManager.currentGameMode.shieldStartingCount > 1)
                    {
                        player.Secret = gamemodeManager.currentGameMode.shieldStartingCount;
                    }
                }
                if (gamemodeManager.currentGameMode.automaticBombShift)
                {
                    //Start coroutine on the player controller here.
                    player.StartBombShift(gamemodeManager.currentGameMode.bombShiftTime);
                }
                player.UpdatePlayerUI();
            }
        }
    }

    IEnumerator RoundCountdown()
    {
        int altGamemodeChanceDenominator = 5;
        //Moved ambience so it begins before a round does
        if (gameAmbience != null) gameAmbience.PlayLevelAmbience(SceneManager.GetActiveScene().buildIndex);
        //Clear the winning players list
        winningPlayersList.Clear();
        //Loop while the round counter is less than round count and game over is false
        while (!gameOver)
        {
            playerUIManager.UpdateRoundCounter();
            //Cease player bombshift loop if it is active
            foreach (PlayerController player in GetJoinedPlayers())
            {
                if (player != null)
                {
                    if(player.bombShiftCoroutine != null)
                    {
                        player.StopBombShift();
                        playerUIManager.PlayerEnterMatchUIChange(player.GetPlayerID(), player);
                    }
                   
                }
            }
            gameIntroActive = false;
            playerCanWin = true;
            //clear who won
            whoWon = "";
            //clear player lists
            eliminatedPlayers.Clear();
            joiningPlayerList.Clear();
            playerControllers.Clear();
            //Wipe all spawned power ups
            blocksActive = false;
            gameManager.ErasePressure();
            gameManager.SpawnInPressureBlocks();
            gameManager.WipeAllItems();
            gameManager.EnableItemSpawn(false);
            WipeAllBombs();
            levelFilledWithBlocks = false;
            isOvertimeActive = false;
            //Set countdown variables to their editor selected values
            startingRoundTimeCountdown = roundTimeCountdown_Start;
            roundTimeCountdown_InGame = roundTime;
            winTimeCountdown = winTime;
            //Set round start countdown active to true. This will allow players to hop in
            roundStartCountdownActive = true;
            //Countdown is happening so round is not in progress
            isRoundInProgress = false;
            //Countdown is happening so a win is not in progress
            winInProgress = false;
            //Set joined already players to their starting positions.
            SetPlayerStartingPositions();
            //Print current round
            Debug.Log("Round: " + (roundCounter + 1).ToString());
            Debug.Log("Countdown started");
            GameObject[] DestroyedBlocks = GameObject.FindGameObjectsWithTag("BlockDestroyed");
            foreach (GameObject brokenBlock in DestroyedBlocks)
            {
                Instantiate(destroyableBlock, brokenBlock.transform.position, brokenBlock.transform.rotation);
                DestroyImmediate(brokenBlock, true);
            }
            
            //Loop while the countdown is active

            //check what round it is and determine if an alternate gamemode should be rolled. alt game mode can't happen on round 0

            if(roundCounter == 0)
            {

                if(PlayerPrefs.GetInt("AlternateGamemodeChance", 1) == 2)
                {
                    //This is if we have guaranteed altmodes 100% of the time on
                    Debug.Log("Round SUS");
                    ReadGamemodeData(gamemodeManager.GetRandomAltGamemode());
                }
                else ReadGamemodeData(gamemodeManager.GetDefaultGamemode());
                
            }
            else
            {
                //by default there is a 1/5 chance for a random gamemode to occur. The chance for a random game mode increases with subsequent draws. Prevent 
                if((((Random.Range(0, altGamemodeChanceDenominator+1) == altGamemodeChanceDenominator && lastRoundAltMode != roundCounter - 1) || lastRoundAltMode + 3 <= roundCounter)) && PlayerPrefs.GetInt("AlternateGamemodeChance", 1) != 0)
                {
                    ReadGamemodeData(gamemodeManager.GetRandomAltGamemode());
                    altGamemodeChanceDenominator = 5;
                    lastRoundAltMode = roundCounter;
                }
                else
                {
                    if (PlayerPrefs.GetInt("AlternateGamemodeChance", 1) == 2)
                    {
                        //This is if we have guaranteed altmodes 100% of the time on
                        ReadGamemodeData(gamemodeManager.GetRandomAltGamemode());
                    }
                    else ReadGamemodeData(gamemodeManager.GetDefaultGamemode());
                }
            }
            GrabAllDestroyableBlocks();

            //First ever round of match. Do camera intro
            if (roundCounter == 0 && cineCamera != null)
            {
                SetPlayerStartingPositions();
                startingRoundTimeCountdown = 0;
                AudioClip mapIntro = Announcer.instance.PlayLevelIntro(SceneManager.GetActiveScene().buildIndex);
                if (mapIntro != null) yield return new WaitForSeconds(mapIntro.length);
                //Set all player positions and add all current players to the joined array
                float introTimePerChar = roundTimeCountdown_Start;
                GameObject cameraParent = cineCamera.gameObject;
                gameIntroActive = true;
                whoDis = "";

                List<GameObject> sortedPlayers = playerArray.ToList();


                //Sort the list in descending order then reverse it so we begin with the player with the lowest player ID
                while (gameIntroActive)
                {
                    foreach(GameObject player in sortedPlayers.OrderByDescending(player => player.GetComponent<PlayerController>().playerID).ToArray().Reverse())
                    {
                        if(player !=  null)
                        {
                            //Look at a player position tween over half a second
                            cameraParent.transform.DOLookAt(player.transform.position, 0.5f);
                            //Zoom in on them
                            DOTween.To(() => cineCamera.m_Lens.FieldOfView, x => cineCamera.m_Lens.FieldOfView = x, cameraFOV2, 0.75f);
                            GameMusicMenu.instance.transitionSoundPlayer.pitch = Random.Range(1.2f, 1.8f);
                            GameMusicMenu.instance.transitionSoundPlayer.Play();
                            //wait and say player name
                            AudioClip playerIntro = Announcer.instance.PlayPlayerIntro(player.GetComponent<PlayerController>());
                            if (playerIntro != null)
                            {
                                yield return new WaitForSeconds(playerIntro.length);
                            }
                            else yield return new WaitForSeconds(0.75f);
                            //Introduce character name
                            Debug.Log("Introducing the " + player.GetComponent<PlayerController>().ReturnPlayerCharacterString() + "!");
                            whoDis = "Introducing the " + player.GetComponent<PlayerController>().ReturnPlayerCharacterString() + "!";
                            AudioClip characterIntro = Announcer.instance.PlayCharacterIntro(player.GetComponent<PlayerController>());
                            if (characterIntro != null) yield return new WaitForSeconds(characterIntro.length);
                            else yield return new WaitForSeconds(introTimePerChar / 3);
                            //Zoom out
                            whoDis = "";
                            //only do the zoom out if the the player isn't the last in the array. If they are we skip to the big zoomout.
                            if(sortedPlayers.OrderByDescending(player => player.GetComponent<PlayerController>().playerID).ToArray().Reverse().Last() != player)
                            {
                                GameMusicMenu.instance.transitionSoundPlayer.pitch = Random.Range(1.2f, 1.8f);
                                GameMusicMenu.instance.transitionSoundPlayer.Play();
                                DOTween.To(() => cineCamera.m_Lens.FieldOfView, x => cineCamera.m_Lens.FieldOfView = x, cameraFOV / 2, 1f);
                                yield return new WaitForSeconds(introTimePerChar / 3);
                                //Go to next character
                            }
                        }
                    }
                    //Go back to normal FOV
                    DOTween.To(() => cineCamera.m_Lens.FieldOfView, x => cineCamera.m_Lens.FieldOfView = x, cameraFOV, 1f);
                    GameMusicMenu.instance.transitionSoundPlayer.pitch = Random.Range(.9f, 1f);
                    GameMusicMenu.instance.transitionSoundPlayer.Play();
                    //Rotate to standard pos
                    cameraParent.transform.DORotate(new Vector3(52.7f, 0, 0), 1f);
                    //play line if all players are the same
                    AudioClip allSameEE = Announcer.instance.AreAllPlayersTheSame(playerArray);
                    if(allSameEE != null) yield return new WaitForSeconds(allSameEE.length);
                    else yield return new WaitForSeconds(1f);
                    Announcer.instance.PlayAnnouncerLine(AnnouncerLine.Count321);
                    startingRoundTimeCountdown = roundTimeCountdown_Start;
                    //Make countdown as normal. 
                    while (roundStartCountdownActive == true)
                    {
                        //Wait one second then subtract 1 from the startingRoundCountdown variable. If we hit zero, break out of the loop and the round starts.
                        yield return new WaitForSeconds(1);
                        startingRoundTimeCountdown -= 1;
                        if (startingRoundTimeCountdown <= 0) break;
                    }
                    gameIntroActive = false;
                    startingRoundTimeCountdown = -1;
                    Announcer.instance.PlayAnnouncerLine(AnnouncerLine.Go);
                    yield return new WaitForSeconds(0.5f);
                    roundStartCountdownActive = false;
                }
            }
            else
            {
                int index = 0;
                while (roundStartCountdownActive == true)
                {
                    if (index == 0) Announcer.instance.PlayAnnouncerLine(AnnouncerLine.Count321);
                    //Wait one second then subtract 1 from the startingRoundCountdown variable. If we hit zero, break out of the loop and the round starts.
                    yield return new WaitForSeconds(1);
                    startingRoundTimeCountdown -= 1;
                    if (startingRoundTimeCountdown <= 0) break;
                    index++;
                }
                startingRoundTimeCountdown = -1;
                Announcer.instance.PlayAnnouncerLine(AnnouncerLine.Go);
                yield return new WaitForSeconds(0.5f);
                roundStartCountdownActive = false;
            }
            //Only check for star power after round 1 and ONLY on the arena level.
            if(roundCounter != 0 && SceneManager.GetActiveScene().buildIndex == 4) StarPower();
            //Play level music
            if (gameMusic!= null)gameMusic.PlayLevelMusic(SceneManager.GetActiveScene().buildIndex);
            
            //The round is now in progress
            isRoundInProgress = true;
            //Allow players to move
            AllowPlayerMovement();
            //print that round has begun
            Debug.Log("Round has begun");
            if(gamemodeManager.currentGameMode.showGamemodeIndentifierOnRoundStart)
            {
                //Pull mode name and desc here and spawn text prefab.
                GameModeIndentifierText textPrefab = Instantiate(gameModeIdentifier).GetComponent<GameModeIndentifierText>();
                textPrefab.gameModeTitleText = gamemodeManager.currentGameMode.gamemodeName;
                textPrefab.gameModeDescriptionText = gamemodeManager.currentGameMode.gamemodeDescription;
                textPrefab.UpdateText();
                GameMusicMenu.instance.transitionSoundPlayer.pitch = Random.Range(1.2f, 1.8f);
                GameMusicMenu.instance.transitionSoundPlayer.Play();
            }
            if(gamemodeManager.currentGameMode.beginWithNoBlocks)
            {
                //Manually destroy blocks if wanted
                foreach (DestroyableBlock block in destroyableList)
                {
                    //Prevent powerup drop and sound on these manual destructions.
                    block.DestroyBlock(true, true);
                }
            }
            //Assign data to player accordingly.
            ReadGameModePlayerData();
            yield return null;
            //Loop while round is active.
            while (isRoundInProgress == true)
            {
                //Wait until the end of a frame then subtract delta time from the roundTimeCountdown_InGame variable.
                yield return new WaitForEndOfFrame();

                if (roundTimeCountdown_InGame > 0) roundTimeCountdown_InGame -= Time.deltaTime;
                else
                {
                    roundTimeCountdown_InGame = 0;
                    isOvertimeActive = true;
                }
                if (roundTimeCountdown_InGame <= pressureBlockSpawnTime && !blocksActive)
                {
                    gameManager.Pressure();
                    playerUIManager.ShakeClock(true);
                    blocksActive = true;
                    gameMusic.PlaySpeedUpLevelTheme();
                }
                //if(roundTimeCountdown_InGame == pressureBlockSpawnTime + 10)
               // {
                  //  Announcer.instance.PlayAnnouncerLine(AnnouncerLine.LowTimeHint);
               // }

                //Prevent powerups from spawning unless one of these values is true.
                if(powerupSpawnsOffTillBlocksGone || (roundTime - powerupSpawnTime > roundTimeCountdown_InGame))
                {

                    if((roundTime - powerupSpawnTime > roundTimeCountdown_InGame) && !powerupSpawnsOffTillBlocksGone)
                    {
                       // Debug.Log("Time less than roundTime - powerupSpawnTime spawning powerups");
                        gameManager.EnableItemSpawn(true);
                    }
                    else if(powerupSpawnsOffTillBlocksGone)
                    {
                        if(destroyableList.Count == 0) 
                        {
                            Debug.Log("all blocks gone, spawning powerups true");
                            gameManager.EnableItemSpawn(destroyableList.Count == 0);
                        }
                        
                    }
                    else
                    {
                        gameManager.EnableItemSpawn(false);
                    }
                    
                }

                PlayerController winPlayer = null;
                //If we hit zero the round is over. No edge case needed to see if all players are alive as pressure blocks will fully fill the stage by that point.
                if (roundTimeCountdown_InGame <= 0 && levelFilledWithBlocks)
                {
                    if(gameMusic != null) gameMusic.levelMusicPlayer.Stop();
                    if (gameAmbience != null) gameAmbience.levelMusicPlayer.Stop();
                    gameManager.EnableItemSpawn(false);
                    isRoundInProgress = false;
                    gameManager.StopPressure();
                    Debug.Log("Draw");
                    whoWon = "Draw";
                }
                //if this is true all joined players have been eliminated or only one player remains
                if ((eliminatedPlayers.Count == playerControllers.Count || eliminatedPlayers.Count == playerControllers.Count - 1 || playerControllers.Count == 1) && playerCanWin)
                {
                    if(gameMusic != null) gameMusic.levelMusicPlayer.Stop();
                    if (gameAmbience != null) gameAmbience.levelMusicPlayer.Stop();

                    gameManager.StopPressure();
                    gameManager.EnableItemSpawn(false);
                    //Stop round
                    isRoundInProgress = false;
                    //Figure out who won if anyone.
                    if (eliminatedPlayers.Count == playerControllers.Count)
                    {
                        //if the count of eliminated players equals the count of joined players everyone is dead :(
                        Debug.Log("Draw");
                        //Set who won to draw
                        whoWon = "Draw";
                        //Play draw music
                        winTimeCountdown = 5;
                        gameMusic.PlayVictoryTheme(5);
                        //Announcer.instance.PlayAnnouncerLine(AnnouncerLine.Draw)
                        //Track how many draws have occured.
                        drawCounter++;
                        //Subtract from the altGamemodeChanceDenominator to increase the chance of the next round being an alt gamemode
                        altGamemodeChanceDenominator--;
                        //Prevent this number from ever becoming negative.
                        if (altGamemodeChanceDenominator < 0) altGamemodeChanceDenominator = 5;
                    }
                    else
                    {
                        //If the count of eliminated players doesn't equal the amount of joined players. We have a winner!

                        //Loop over all players to find which player isn't in the eliminated players array
                        foreach (var playerController in playerControllers)
                        {
                            if (!eliminatedPlayers.Contains(playerController))
                            {
                                //Grab reference to the winning player controller
                                winPlayer = playerController;
                                //Add the player to the winning players list so their wins can be tallied at the end of a match.
                                winPlayer.AddToWins();
                                if (!winningPlayersList.Contains(winPlayer)) winningPlayersList.Add(winPlayer);

                                //I think this line is where the never ending round bug occurs. Two players may elim each other on close to the same frame
                                //Wasn't sure if I needed to add the gameAmbience code here as well ~Kaleb
                                if (gameMusic != null && winPlayer != null)
                                {
                                    //set who won to the name of the player who won. 
                                    winTimeCountdown = (int)gameMusic.PlayVictoryTheme(winPlayer.GetPlayerCharacter()) + 1;
                                    //whoWon = "Player " + winPlayer.GetPlayerID() + " Won!";
                                    Debug.Log("Player " + winPlayer.GetPlayerID() + " Won!");
                                }
                                else
                                {
                                    winTimeCountdown = 4;
                                    whoWon = "THE THING JUST HAPPENED!";
                                    Debug.Log("THE THING JUST HAPPENED!");
                                }
                                break;
                            }
                        }
                    }
                    if (celebrationMaterial != null) celebrationMaterial.Play();
                    playerUIManager.ShakeClock(false);
                    //set win in progress to true as a win/draw is happening
                    winInProgress = true;
                    //set round start countdown active to false.
                    roundStartCountdownActive = false;
                    foreach (AudienceMember member in audience)
                    {
                        member.Cheer();
                    }
                    if(SceneManager.GetActiveScene().buildIndex == 4) gameAmbience.PlayCheer();
                    if (winPlayer != null)
                    {
                       //Look at a player position tween over half a second
                       cineCamera.gameObject.transform.DOLookAt(winPlayer.transform.position, 0.45f);
                       //Zoom in on them
                       DOTween.To(() => cineCamera.m_Lens.FieldOfView, x => cineCamera.m_Lens.FieldOfView = x, cameraFOV2, 0.45f);
                       cineCamera.LookAt = winPlayer.transform;
                       playerUIManager.PlayWinBanner(true);
                    }
                    

                    //Loop while win is in progress
                    while (winInProgress == true)
                    {
                        //Wait for 1 second then subtract from the winTimeCountdown variable.
                        yield return new WaitForSeconds(1);
                        winTimeCountdown -= 1;
                        //Only zoom the camera back out if a player won a round and the not the match, since we will transition to the results screen.
                        //If we hit zero the win/draw fanfare is over and we break out of the loop
                        if (winTimeCountdown <= 0)
                        {
                            winInProgress = false;
                            if (!HasPlayerWon() && winPlayer != null)
                            {
                                cineCamera.LookAt = null;
                                //Go back to normal FOV
                                DOTween.To(() => cineCamera.m_Lens.FieldOfView, x => cineCamera.m_Lens.FieldOfView = x, cameraFOV, 0.583f);
                                //Rotate to standard pos
                                cineCamera.gameObject.transform.DORotate(new Vector3(52.7f, 0, 0), 0.583f);
                                playerUIManager.PlayWinBanner(false);
                            }
                        }
                    }
                    if(celebrationMaterial != null) celebrationMaterial.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    foreach (AudienceMember member in audience)
                    {
                        member.Cheer(false);
                    }
                }
            }
            //Increase the round by 1
            //Plus failsafe to return to the main menu if no players are loaded in
            if (HasPlayerWon() || (eliminatedPlayers.Count == 0 && playerControllers.Count == 0))
            {
                Debug.Log("Player won or failsafe active");
                gameOver = true;
            }
            else
            {
                roundCounter++;
            }
        }
        //Take player back to menu here
        Debug.Log("Player has won, going to results screen");
        PlayerController winner = null;

        for(int i = 0; i < winningPlayersList.Count; i++) 
        {
            if(winningPlayersList[i].GetPlayerWins() >= roundsToWin)
            {
                winner = winningPlayersList[i];
                break;
            }
        }
        if (winner != null)
        {
            Debug.Log("Player " + winner.GetPlayerID() + " wins the game!");
            //whoWon = "Player " + winner.GetPlayerID() + " Won the match!";
            //We track the player's match wins so we can add a crown to them later...
            winner.AddToPlayerMatchWins();
        }
        else
        {
            Debug.Log("It's a tie!");
            //whoWon = "Its a tie!";
        }
        /*
        if(gameMusic != null) gameMusic.PlayVictoryTheme(4);
        winTimeCountdown = winTime;
        //set win in progress to true as a win/draw is happening
        winInProgress = true;
        //set round start countdown active to false.
        roundStartCountdownActive = false;
        //Loop while win is in progress


        while (winInProgress == true)
        {
            //Wait for 1 second then subtract from the winTimeCountdown variable.
            yield return new WaitForSecondsRealtime(1);
            winTimeCountdown -= 1;
            //If we hit zero the win/draw fanfare is over and we break out of the loop
            if (winTimeCountdown <= 0) break;
        }
        */
        //Loads StartMenu
        StartCoroutine(LevelTransition());

    }
    IEnumerator LevelTransition()
    {
        
        SceneTransitionerManager.instance.StartTransition();
        AsyncOperation asyncLoad = null;
        if (playerControllers.Count == 0)
        {
            GameMusicMenu.instance.BeginMusic(0);
            asyncLoad = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        }
        else
        {
            GameMusicMenu.instance.BeginMusic(10);
            asyncLoad = SceneManager.LoadSceneAsync(8, LoadSceneMode.Single);
        }
        asyncLoad.allowSceneActivation = false;
        yield return new WaitForSeconds(1.5f);
        asyncLoad.allowSceneActivation = true;
    }


    private bool HasPlayerWon()
    {
        for (int i = 0; i < winningPlayersList.Count; i++)
        {
            if (winningPlayersList[i].GetPlayerWins() >= roundsToWin)
            {
                return true;
            }
        }
        return false;
    }

    private void HidePlayerSpawns()
    {
        //Disable the rendering component of the player spawns
        foreach (GameObject player in playerSpawns) 
        {
            player.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void SetPlayerStartingPositions()
    {
        //Grab all of the players 
        playerArray = GameObject.FindGameObjectsWithTag("Player");

        if (playerArray != null)
        {
            int index = 0;
            //Loop over each player
            foreach (GameObject player in playerArray)
            {
                //Get a reference to the player's player controller script
                PlayerController playerController = player.GetComponent<PlayerController>();
                //Reset powerup effects
                playerController.ResetPlayerPowerups();
                //Add them to the joined player list
                if(!playerControllers.Contains(playerController)) playerControllers.Add(playerController);
                //Disable their PlayerController script to prevent them moving
                //playerController.enabled = false;
                //player.transform.position = playerSpawns[playerController.GetPlayerID() - 1].transform.position;
                //Set their spawn to their designated spawn
                player.transform.position = tileManager.GetTileCenterFromWorldPosPlayer(playerSpawns[playerController.GetPlayerID() - 1].transform.position) + new Vector3(0, 5, 0);
                index++;
            }
        }
    }



    private void AllowPlayerMovement()
    {
        if (playerArray != null)
        {
            foreach (GameObject player in playerArray)
            {
                //Loop over all joined players and get a reference to their PlayerController component
                PlayerController playerController = player.GetComponent<PlayerController>();
                //Enable their PlayerController component.
                //playerController.enabled = true;
            }
        } 
    }

    



    //For UI script 
    public bool IsRoundActive()
    {
        //Return the value of isRoundInProgress
        return isRoundInProgress;
    }

    public bool IsStartingCountdownActive()
    {
        //Return the value of roundStartCountdownActive
        return roundStartCountdownActive;
    }

    public int GetStartingCountdownTime()
    {
        //return the value of startingRoundTimeCountdown
        return startingRoundTimeCountdown;
    }

    public float GetRemainingRoundTime()
    {
        //return the value of roundTimeCountdown_InGame
        return roundTimeCountdown_InGame;
    }

    public int GetRound()
    {
        //Return the value of the round counter plus one. This gives the actual round.
        return roundCounter + 1;
    }

    public bool GetIsOvertimeActive()
    {
        return isOvertimeActive;
    }

    public Vector3 GetCoralleSpawn(int index, GameObject player)
    {
        //IF the index is the players index plus 4 add them to the joining list.
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (index == (playerController.GetPlayerID() - 1) + 4)
        {
            //Add player to the joining player list
            playerUIManager.JoinPlayerUIChange(playerController.GetPlayerID(), playerController);
            joiningPlayerList.Add(playerController);
            //Disable their PlayerController component to prevent movement 
            //playerController.enabled = false;
            return new Vector3(playerSpawns[index].transform.position.x, playerSpawns[index].transform.position.y, playerSpawns[index].transform.position.z);
        }
        //Add the player to the joined player list if a round isn't active
        else
        {
            playerUIManager.PlayerEnterMatchUIChange(playerController.GetPlayerID(), playerController);
            playerControllers.Add(playerController);
        }

        //Return the player's spawn. Either their out of map spawn or in map starting position.
        return tileManager.GetTileCenterFromWorldPosPlayer(new Vector3(playerSpawns[index].transform.position.x, 0, playerSpawns[index].transform.position.z));
    }

    public Vector3 GetDeathSpawn(int index, GameObject player)
    {
        //get a reference to the players PlayerController component
        PlayerController playerController = player.GetComponent<PlayerController>();
        //Check if the player is already in the eliminated player list and if they aren't add them to it
        if(!eliminatedPlayers.Contains(playerController)) eliminatedPlayers.Add(playerController);
        //Change audience poses
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        SceneManager.GetActiveScene();
        if(sceneName == "L01_Arena")
        {

            
            foreach (AudienceMember member in audience)
            {
                member.Cheer(true, 1f, 2f);
            }
            //Play's the cheer audio, then set's it back to nonrmal ambience
            gameAmbience.PlayCheer();


        }
        //Return the specific player's death spawn.
        return new Vector3(playerSpawns[index].transform.position.x, 0, playerSpawns[index].transform.position.z);

    }

    public List<PlayerController> GetJoiningPlayers()
    {
        //Return the joining player list
        return joiningPlayerList;
    }

    public List<PlayerController> GetJoinedPlayers()
    {
        //Return the joined player list
        return playerControllers;
    }

    public List<PlayerController> GetEliminatedPlayers()
    {
        //Return the eliminated player list
        return eliminatedPlayers;
    }

    public bool IsWinInProgress()
    {
        //Return the value of winInProgress
        return winInProgress;
    }

    public string GetWhoWon()
    {
        //Return the value of whoWon
        return whoWon;
    }


    public void HandlePlayerDropout(PlayerController playerController)
    {
        playerUIManager.LeavePlayerChange(playerController.GetPlayerID(), playerController);
        if(joiningPlayerList.Contains(playerController)) joiningPlayerList.Remove(playerController);
        if(playerControllers.Contains(playerController)) playerControllers.Remove(playerController);    
        if(eliminatedPlayers.Contains(playerController)) eliminatedPlayers.Remove(playerController);
    }

    public void StarPower(GameObject player = null)
    {
        if(!EscMenu.instance.easterEggsEnabled)
        {
            Debug.Log("Star power was triggered but easter eggs were disabled in the settings menu");
            return;
        }


        //At the start of a round there is a 1/10 chance for a random audience member to throw a powerup at the best and worst preforming player
        if(Random.Range(1, 11) == 10 && player == null && GetJoinedPlayers().Count > 1)
        {
            Debug.Log("Star power rolled");
            //Pick 2 random audience members
            int firstPlayerIndex = Random.Range(0, audience.Length);
            int secondPlayerIndex = Random.Range(0, audience.Length);
            //Ensure that they are not the same audience member
            while (firstPlayerIndex == secondPlayerIndex)
            {
                secondPlayerIndex = Random.Range(0, audience.Length);
            }
            //Figure out who is the best and who is the worst based on (wins+elims) - self elims. The person with the highest and lowest value of this are our choices.
            PlayerController worstPlayer;
            PlayerController bestPlayer;
            int[] array = new int[GetJoinedPlayers().Count];
            List<PlayerController> playersCopy = GetJoinedPlayers();
            for (int i = 0; i < array.Length-1; i++)
            {
                //A players average is based off of this calculation. Higher is better.
                array[i] = (playersCopy[i].GetPlayerWins() + playersCopy[i].GetPlayerEliminations()) - playersCopy[i].GetPlayerSelfElims();
            }
            //Find the max and min values in this array and their index
            int maxIndex = array.ToList().IndexOf(Mathf.Max(array));
            int minIndex = array.ToList().IndexOf(Mathf.Min(array));
            if (array[maxIndex] == 0 && array[minIndex] == 0)
            {
                return;
            }
            //Spawn in powerups
            GameObject firstPlayerPowerup = gameManager.SpawnUntrackedItem(audience[Random.Range(0, audience.Length)].transform.position, true);
            GameObject secondPlayerPowerup = gameManager.SpawnUntrackedItem(audience[Random.Range(0, audience.Length)].transform.position, true);

            if(firstPlayerPowerup == null || secondPlayerPowerup == null)
            {
                //if either of the drops are null we return to prevent errors. 
                return;
            }


            //Set the cheer anim to play on the gifters.
            gameAmbience.PlayCheer();
            audience[firstPlayerIndex].Cheer(true, 1f, 1.5f);
            audience[secondPlayerIndex].Cheer(true, 1f, 1.5f);
            //Using that index we then set to players
            bestPlayer = playersCopy[maxIndex];
            worstPlayer = playersCopy[minIndex];
            //We now move the two powerups toward the recipient players.
            firstPlayerPowerup.AddComponent<ParabolicMover>().endPos = bestPlayer.transform.position;
            secondPlayerPowerup.AddComponent<ParabolicMover>().endPos = worstPlayer.transform.position;
        }
        else
        {
            Debug.Log("Star power not rolled");
        }
        if(player != null && player.GetComponent<PlayerController>()?.isEliminated == false) 
        {
            gameAmbience.PlayCheer();
            Debug.Log("Star power manual activation");
            GameObject firstPlayerPowerup = gameManager.SpawnUntrackedItem(audience[Random.Range(0, audience.Length)].transform.position, true);
            if(firstPlayerPowerup == null) 
            {
                return;
            }
            firstPlayerPowerup.AddComponent<ParabolicMover>().endPos = player.transform.position;
        }
    }


    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
