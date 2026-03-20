

//#define WINNITRON_BUILD
#define NORMAL_BUILD

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static KeyboardSplitter;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using Cinemachine;
using SpriteGlow;


public class PlayerController : MonoBehaviour
{
    public Vector3 targetPosition { get; private set; }
    public bool isMoving { get; private set; } = false;
    public bool isFlying { get; private set; } = false;
    public bool isPushing { get; private set; } = false;
    public bool isEliminated { get; private set; } = false;
    public PlayerInput playerInput;
    private Vector2 moveDirection;
    private Vector2 flightDir;
    private Vector2 pushDir;
    //Get reference to the round manager
    private RoundManager roundManager;
    private TileManager tileManager;
    private Vector3 lastDirection;
    private Rigidbody playerRB;
    private int playerWins = 0;
    public UIAssignerPlayer playerUIAssigner;
    private PlayerCharacter playerCharacter;
    private CinemachineImpulseSource impulser;
    private SpriteGlowEffect shieldVFX;

    public ParticleSystem dust;
    public ParticleSystem feathersVFX;
    public ParticleSystem wifiBombVFX;
    public ParticleSystem speedVFX;

    [Header("Player Settings")]
    [Tooltip("Speed of the player.")] public float moveSpeed = 25f;
    [Tooltip("Speed of the players flight.")] public float flightSpeed = 25f;
    [Tooltip("The starting cap of how many bombs a player can drop.")][SerializeField] private int bombCap = 3;
    [Tooltip("The starting bomb fire level.")][SerializeField] private int bombFireLevel = 3;
    [Tooltip("The prefab that will spawn when the player hits the bomb button")] public GameObject bombStash;
    [Tooltip("The scale value set on the tilemap in the inspector. For tile movement.")] private int tileSize = 5;
    [Tooltip("float to stop player from clipping a wall before they are stopped by the line trace")] private float freeMoveCapsuleSize = 2f;
    [Tooltip("Wether or not tile based movement is active. Non-tile movement does not currently function as its behavior with bomb push and fly were unruly.")] private bool isMovementTileBased = true;
    [Tooltip("The player's possible colors. The color is set at runtime and determined by their input scheme. Temporary")] public Material[] playerColors;
    [Tooltip("The player's number, assigned at runtime. 1-4")] public int playerID { get; private set; } = 0;
    [Tooltip("The player's ID, assigned at runtime. 1-4")][SerializeField] private string playerStringID;
    [Tooltip("The layers the player collision check will collide with")][SerializeField] private LayerMask playerCollMask;

    [Header("Player Powerup Settings(values are reset each round)")]
    //Variables effected by powerups
    [Tooltip("Edit this value at runtime, cap logic checks this number. This value is reset to bombCap's value at the start of a new round.")][SerializeField] public int bombCap_Game;
    [Tooltip("Speed of the player. Effected by the skates powerup")][SerializeField] public float moveSpeed_Game = 0f;
    [Tooltip("Contains all instantiated bombs. Count is used in logic for bomb cap.")] private List<BombBehavior> bombList;
    [Tooltip("The type of bomb the player will drop. These bomb types replace each other.")] public BombTypes playerBombType = BombTypes.DefaultBomb;
    [Tooltip("Determines whether the player as picked up a shield or not, and how many they have")] public bool hasShield = false;
    [Tooltip("Edit this value at runtime, this value is passed to the bomb to increase its fire level.")] public int bombFireLevel_Game;
    [Tooltip("The fly and push bomb powerups replace each other")] public PlayerPowerup playerPowerup = PlayerPowerup.None;
    [Tooltip("Array of possible Patriot player characters. One of these prefabs will be instantiated and parented to the player object")][SerializeField] private GameObject[] playerAnimPrefabsPatriot;
    [Tooltip("Array of possible Centaur player characters. One of these prefabs will be instantiated and parented to the player object")][SerializeField] private GameObject[] playerAnimPrefabsCentaur;
    [Tooltip("Array of possible Toph player characters. One of these prefabs will be instantiated and parented to the player object")][SerializeField] private GameObject[] playerAnimPrefabsToph;
    [Tooltip("Array of possible Professor player characters. One of these prefabs will be instantiated and parented to the player object")][SerializeField] private GameObject[] playerAnimPrefabsProf;





    private Animator playerAnimator = null;
    public GameObject playerAnimationObject { get; private set; } = null;
    private Coroutine footstepsRoutine;
    private int playerEliminations = 0;
    private int selfEliminations = 0;


    [Header("Audio Settings")]
    public AudioSource footstepSource; // Audio Source added to object in script section TE
    public AudioSource footstepSourceSqueak; // Audio Source added to object in script section TE
    public AudioSource voiceSource;
    public AudioSource eliminationSource;
    public AudioSource bombPlaceSource;
    public AudioSource playerPowerupSource;
    public AudioSource centuarSounds;
    public AudioSource victorySource;
    public AudioSource selectionSource;
    [Tooltip("array of sounds that are randomly chosen to be the sound at runtime")] public AudioClip[] footstepSounds;
    [Tooltip("array of sounds that are randomly chosen to be the sound at runtime")] public AudioClip[] squeakyFootstepSounds;
    [Tooltip("array of sounds that are randomly chosen to be the sound at runtime")] public AudioClip[] eliminationSound;
    [Tooltip("array of sounds that are randomly chosen to be the sound at runtime")] public AudioClip[] bombPlaceSound;
    [Tooltip("Sound that plays during flight")] public AudioClip wingsLoop;

    [Tooltip("array of sounds for the patriot character")][SerializeField] public AudioClip[] patriotVoiceLines;
    [Tooltip("array of sounds for the centaur character")][SerializeField] public AudioClip[] centaurVoiceLines;
    [Tooltip("array of sounds for the toph character")][SerializeField] public AudioClip[] tophVoiceLines;
    [Tooltip("array of sounds for the prof character")][SerializeField] public AudioClip[] profVoiceLines;
    public int Secret { get; set; } = 0;

    //For shoe toggle
    public EscMenu playerManagerClass;
    public Coroutine bombShiftCoroutine;

    //New crown feature
    public int playerMatchWins { get; private set; } = 0;

    [SerializeField] PlayerBillboarder playerBillboarderClass;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        impulser = GetComponent<CinemachineImpulseSource>();
        playerRB = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void AddToPlayerMatchWins()
    {
        playerMatchWins++;
    }

    public void ResetPlayerStats()
    {
        playerWins = 0;
        playerEliminations = 0;
        selfEliminations = 0;
    }

    public string ReturnPlayerCharacterString()
    {
        switch (playerCharacter)
        {
            //Set temp array to the correct array
            case PlayerCharacter.Patriot:
                return "The Patriot";
            case PlayerCharacter.Centaur:
                return "The Centaurship";
            case PlayerCharacter.Toph:
                return "The Graduate";
            case PlayerCharacter.Professor:
                return "The Professor";
            default:
                return "The Patriot";
        }
    }

    //For UI later
    public bool GetHasShield()
    { return hasShield; }
    public int GetBombCap()
    { return bombCap_Game; }
    public int GetBombFireLevel()
    { return bombFireLevel_Game; }
    public PlayerPowerup GetPlayerPowerup()
    { return playerPowerup; }
    public BombTypes GetPlayerBombType()
    { return playerBombType; }
    public float GetPlayerMoveSpeed()
    { return moveSpeed_Game / moveSpeed; }
    public int GetPlayerWins()
    { return playerWins; }

    public int GetPlayerSelfElims()
    { return selfEliminations; }

    public int GetPlayerEliminations()
    { return playerEliminations; }

    public void AddElimination(bool selfElim)
    {
        //player char voice here
        if (!selfElim)
        {
            playerEliminations++;
            PlayPlayerVoiceline(CharacterVoiceLine.Elimination);
        }
        else selfEliminations++;
    }

    public void AddToWins()
    {
        //Play char voice here
        playerWins++;
        UpdatePlayerUI();
        StartCoroutine(DelayCallout(CharacterVoiceLine.RoundWin));
    }

    IEnumerator DelayCallout(CharacterVoiceLine voiceType)
    {
        if (voiceType == CharacterVoiceLine.RoundWin)
        {
            Announcer.instance.AnnouncerStopCurrentLine();
            PlayPlayerVoiceline(CharacterVoiceLine.RoundWin);
            yield return new WaitForSeconds(3);
            Announcer.instance.PlayerPlayerRoundWin(this);
        }
        else if (voiceType == CharacterVoiceLine.Eliminated)
        {
            //Announcer.instance.AnnouncerStopCurrentLine();
            PlayPlayerVoiceline(CharacterVoiceLine.Eliminated);
            yield return new WaitForSeconds(3);
            //Only play line if the active one is not a victory line
            if(!roundManager.IsWinInProgress())
            {
                Announcer.instance.PlayCharacterElimLine(this);
            }
        }
    }

    public void ToggleMovementMode(InputAction.CallbackContext context)
    {
        if (roundManager != null && !roundManager.IsStartingCountdownActive())
        {
            //This function is called by a unity event on the playerinput component on the player prefab.
            //Player movement toggle is per player. End for P1, 5 for P2, 9 for P3, Numpad 2 for P4.
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    // isMovementTileBased = !isMovementTileBased;
                    Debug.Log(playerStringID + " Pressed Tile Movement Toggle. Toggle is disabled in this build.");
                    break;
                case InputActionPhase.Started:
                    break;
                case InputActionPhase.Canceled:
                    break;
            }
        }
    }

    public void PlayPlayerVoiceline(CharacterVoiceLine type)
    {
        AudioClip[] tempArray;
        //Grab the player voice array of the character this player is
        switch (playerCharacter)
        {
            //Set temp array to the correct array
            case PlayerCharacter.Patriot:
                tempArray = patriotVoiceLines;
                break;
            case PlayerCharacter.Centaur:
                tempArray = centaurVoiceLines;
                break;
            case PlayerCharacter.Toph:
                tempArray = tophVoiceLines;
                break;
            case PlayerCharacter.Professor:
                tempArray = profVoiceLines;
                break;
            default:
                tempArray = patriotVoiceLines;
                break;
        }
        if (type == CharacterVoiceLine.Victory || type == CharacterVoiceLine.RoundWin)
        {
            PlayerController[] otherPlayers = GameObject.FindObjectsOfType<PlayerController>();
            foreach (var player in otherPlayers)
            {
                player.voiceSource.Stop();
            }
            victorySource.Stop();
            victorySource.clip = tempArray[(int)type];
            victorySource.Play();
            return;
        }
        else if (type == CharacterVoiceLine.CharSelect)
        {
            bool otherPlaying = false;
            PlayerController[] otherPlayers = GameObject.FindObjectsOfType<PlayerController>();
            foreach (var player in otherPlayers)
            {
                if (player.victorySource.isPlaying) otherPlaying = true;
            }
            if (!otherPlaying)
            {
                //During char select we play through the player's victory audio source
                Debug.Log("Playing");
                victorySource.clip = tempArray[(int)type];
                victorySource.Play();
            }
            return;
        }
        //Check if the voice source is already playing, if so we do nothing. If not we play the line.
        else
        {
            if (!voiceSource.isPlaying && tempArray[(int)type] != null)
            {
                voiceSource.clip = tempArray[(int)type];
                voiceSource.Play();
            }
        }
    }

    private void Start()
    {
        playerManagerClass = GameObject.FindGameObjectWithTag("PlayerInputManager").GetComponent<EscMenu>();
        playerRB = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        //Get ref to round manager to handle a join and leave.
        roundManager = GameObject.FindObjectOfType<RoundManager>();
        //Get ref to tile manager to allow tile based movement.
        tileManager = GameObject.FindObjectOfType<TileManager>();
        //This will set all required values to their defaults when a player spawns.
        bombList = new List<BombBehavior>();
        //Check if a round is active, if one is the player's position will be set to a spot out of the playable area. If one is inactive spawn them at their designated spawn. Add height so player doesn't clip through floor
        if (roundManager != null)
        {
            ResetPlayerPowerups();
            if (!roundManager.IsRoundActive() && !roundManager.IsWinInProgress())
            {
                gameObject.transform.position = roundManager.GetCoralleSpawn(playerID - 1, gameObject) + new Vector3(0, 5, 0);
            }
            else
            {
                gameObject.transform.position = roundManager.GetCoralleSpawn((playerID - 1) + 4, gameObject) + new Vector3(0, 5, 0);
                isEliminated = true;
            }
        }
        else
        {
            Debug.Log("No round manager detected. Player is spawned in menu or level is missing roundmanager.");
            gameObject.transform.position = Vector3.zero;
            playerRB.isKinematic = false;
        }
        playerWins = 0;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (this != null)
        {
            if(bombShiftCoroutine != null) StopCoroutine(bombShiftCoroutine);
            isMoving = false;
            isFlying = false;
            isPushing = false;
            targetPosition = transform.position;
            playerRB = GetComponent<Rigidbody>();
            playerInput = GetComponent<PlayerInput>();
            //Get ref to round manager to handle a join and leave.
            roundManager = GameObject.FindObjectOfType<RoundManager>();
            //Get ref to tile manager to allow tile based movement.
            tileManager = GameObject.FindObjectOfType<TileManager>();
            //This will set all required values to their defaults when a player spawns.
            bombList = new List<BombBehavior>();
            //Check if a round is active, if one is the player's position will be set to a spot out of the playable area. If one is inactive spawn them at their designated spawn. Add height so player doesn't clip through floor
            if (roundManager != null)
            {
                ActivateShieldVFX(false);
                ResetPlayerPowerups();
                if (!roundManager.IsRoundActive() && !roundManager.IsWinInProgress())
                {
                    gameObject.transform.position = roundManager.GetCoralleSpawn(playerID - 1, gameObject) + new Vector3(0, 5, 0);
                }
                else gameObject.transform.position = roundManager.GetCoralleSpawn((playerID - 1) + 4, gameObject) + new Vector3(0, 5, 0);
            }
            else
            {
                Debug.Log("No round manager detected. Player is spawned in menu or level is missing roundmanager.");
                gameObject.transform.position = Vector3.zero;
                playerRB.isKinematic = false;
            }
            if (scene == SceneManager.GetSceneByBuildIndex(0))
            {
                //Reset player stats if they go back to the main menu
                ResetPlayerStats();
            }
        }

    }


    public void ActivateShieldVFX(bool activate)
    {
        UpdatePlayerUI();
        //When a player pickups a shield they will receive an outline that matches their player ID.
        if (activate)
        {
            var shieldTween = DOTween.To(() => shieldVFX.OutlineWidth, x => shieldVFX.OutlineWidth = x, 5, 0.25f);
            Color glowColor;
            if (playerID == 1) glowColor = Color.red;
            else if (playerID == 2) glowColor = Color.blue;
            else if (playerID == 3) glowColor = Color.green;
            else if (playerID == 4) glowColor = Color.yellow;
            else glowColor = Color.white;

            shieldVFX.GlowColor = glowColor;
        }
        else
        {
            if(Secret <= 0)
            {
                shieldVFX.OutlineWidth = 0;
            }
        }
    }


    private void Update()
    {
        //Rotate the direction of the player speed line vfx so it faces away from the player
        if(lastDirection != Vector3.zero) speedVFX.transform.forward = -lastDirection;
        if (!isEliminated && roundManager != null && !roundManager.IsStartingCountdownActive())
        {
            if (!isFlying && !isPushing)
            {
                if (!isMoving)
                {
                    HandleInput();
                }
                else if (isMoving)
                {
                    MoveToTarget();
                }
            }

           // ParticleSystem.EmissionModule chunguis = speedVFX.emission;
            // Play audio when the player is moving   TE
            if (isMoving)
            {

                if (footstepsRoutine == null) footstepsRoutine = StartCoroutine(Footsteps());
                if (!dust.isPlaying) dust.Play();
                //Play the skates vfx
                if (moveSpeed_Game > moveSpeed)
                {
                    if(!speedVFX.isPlaying) speedVFX.Play();
                }
            }
            else if (!isMoving)
            {
                if(speedVFX.isPlaying) speedVFX.Stop();
                if (playerCharacter == PlayerCharacter.Centaur) centuarSounds.Stop();
                if (footstepsRoutine != null)
                {
                    StopCoroutine(footstepsRoutine);
                    footstepsRoutine = null;
                }
                //Debug.LogWarning("No AudioSource assigned to stop playing.");
            }
        }
    }

    public void StartBombShift(float shiftTime = 1)
    {
        //Start the bomb shift loop
        if(bombShiftCoroutine == null) bombShiftCoroutine = StartCoroutine(AutoBombShift(shiftTime));
        else
        {
            StopCoroutine(bombShiftCoroutine);
            bombShiftCoroutine = StartCoroutine(AutoBombShift(shiftTime));
        }
    }

    public void StopBombShift()
    {
        if (bombShiftCoroutine != null) StopCoroutine(bombShiftCoroutine);
    }

    IEnumerator AutoBombShift(float shiftTime)
    {
        int currentBombType = 0;
        UpdatePlayerUI();
        //Only shift bombtypes while the player is alive and if they are in a level.
        while (!isEliminated && roundManager != null)
        {
            yield return new WaitForSeconds(shiftTime);
            UpdatePlayerUI();
            playerBombType = (BombTypes)currentBombType;
            currentBombType++;
            if (currentBombType > 4) currentBombType = 0;
        }
        playerBombType = BombTypes.DefaultBomb;
        bombShiftCoroutine = null;
    }

    IEnumerator Footsteps()
    {
        int lastIndex = 0;
        if (playerCharacter == PlayerCharacter.Centaur) centuarSounds.Play();
        while (isMoving && !isEliminated && !isFlying)
        {
            if (playerCharacter != PlayerCharacter.Centaur)
            {
                //Only play shoe squeaks if the toggle is active and the option in the menu was chosen
                if(SceneManager.GetActiveScene().buildIndex == 4 && playerManagerClass != null && playerManagerClass.squeakyStepsOnLvl1 == true)
                {
                    //Debug.Log("Squeak");
                    int generated = Random.Range(0, squeakyFootstepSounds.Length);
                    while(generated == lastIndex)
                    {
                        generated = Random.Range(0, squeakyFootstepSounds.Length);
                    }
                    lastIndex = generated;
                    footstepSourceSqueak.clip = squeakyFootstepSounds[generated];
                    footstepSourceSqueak.Play();
                    yield return new WaitForSeconds(footstepSourceSqueak.clip.length);
                    //footstepSourceSqueak.Stop();
                }
                else
                {
                    int generated = Random.Range(0, squeakyFootstepSounds.Length);
                    while (generated == lastIndex)
                    {
                        generated = Random.Range(0, squeakyFootstepSounds.Length);
                    }
                    lastIndex = generated;
                    footstepSource.clip = footstepSounds[generated];
                    footstepSource.Play();
                    yield return new WaitForSeconds(footstepSource.clip.length);
                    //footstepSource.Stop();
                }
            }
            yield return null;

        }
        centuarSounds.Stop();
    }


    private void HandleInput()
    {
        float horizontalInput = moveDirection.x;
        float verticalInput = moveDirection.y;


        // Ensure that diagonal movement is prevented by prioritizing one axis
        if (Mathf.Abs(horizontalInput) > Mathf.Abs(verticalInput))
        {
            //We are moving left or right
            verticalInput = 0f;
            //If the horizontal input is less than zero player was going left, if it wasn't they were going right.
            lastDirection = horizontalInput < 0f ? Vector3.left : Vector3.right;

            if (lastDirection == Vector3.right)
            {
                if (playerCharacter == PlayerCharacter.Centaur)
                {
                    playerAnimationObject.GetComponent<SpriteRenderer>().flipX = false;
                }
                //player is going right. 
                //set animator bool here
                playerAnimator.SetBool("IsMovingRight", true);
                playerAnimator.SetBool("IsMovingLeft", false);
                playerAnimator.SetBool("IsMovingUp", false);
                playerAnimator.SetBool("IsMovingDown", false);

            }
            else
            {
                //player is going left.
                //set animator bool here
                if (playerCharacter == PlayerCharacter.Centaur)
                {
                    playerAnimationObject.GetComponent<SpriteRenderer>().flipX = true;
                }
                playerAnimator.SetBool("IsMovingRight", false);
                playerAnimator.SetBool("IsMovingLeft", true);
                playerAnimator.SetBool("IsMovingUp", false);
                playerAnimator.SetBool("IsMovingDown", false);
            }
        }
        //return if no input.
        else if (horizontalInput == 0 && verticalInput == 0)
        {
            //Player is idle here. 
            //set animator bool here
            playerAnimator.SetBool("IsMovingRight", false);
            playerAnimator.SetBool("IsMovingLeft", false);
            playerAnimator.SetBool("IsMovingUp", false);
            playerAnimator.SetBool("IsMovingDown", false);
            return;
        }
        else
        {
            //We are moving up or down
            horizontalInput = 0f;
            lastDirection = verticalInput < 0f ? Vector3.back : Vector3.forward;

            if (lastDirection == Vector3.forward)
            {
                //player is going north. 
                //set animator bool here
                playerAnimator.SetBool("IsMovingRight", false);
                playerAnimator.SetBool("IsMovingLeft", false);
                playerAnimator.SetBool("IsMovingUp", true);
                playerAnimator.SetBool("IsMovingDown", false);
            }
            else
            {
                //player is going south.
                //set animator bool here
                playerAnimator.SetBool("IsMovingRight", false);
                playerAnimator.SetBool("IsMovingLeft", false);
                playerAnimator.SetBool("IsMovingUp", false);
                playerAnimator.SetBool("IsMovingDown", true);
            }

        }



        if (isMovementTileBased)
        {
            // Calculate the target position based on input(Grid based)
            targetPosition = transform.position + new Vector3(horizontalInput, 0f, verticalInput) * tileSize;
            //Find the tile target pos is on and get the center of that tile
            Vector3 tilePos = tileManager.GetTileCenterFromWorldPosPlayer(targetPosition);

            //We also need to check if another player is headed toward that tile
            bool otherPlayerWillBlock = false;
            PlayerController[] otherPlayers = GameObject.FindObjectsOfType<PlayerController>();
            foreach (var player in otherPlayers)
            {
                if (player != this && player.targetPosition == targetPosition && (player.isMoving || player.isFlying))
                {
                    otherPlayerWillBlock = true;
                }
            }


            //IF the tile is null, a vector3.zero will be returned by the tilemanager.
            if (tilePos == new Vector3(0, 0, 0) || otherPlayerWillBlock)
            {
                //If there is no tile to move to we reset the target pos and stop movement.
                targetPosition = gameObject.transform.position;
                isMoving = false;
            }
            else
            {
                //If the tile exists we move to it
                targetPosition = new Vector3(tilePos.x, transform.position.y, tilePos.z);
                isMoving = true;
            }
            //not nessicary to trace from the outside of the capsule for grid movement.
            if (Physics.Linecast(transform.position + new Vector3(0, 10, 0), new Vector3(targetPosition.x, 0, targetPosition.z), playerCollMask))
            {
                //Debug.Log("flying is being attempted?");
                isMoving = false;
                if (playerPowerup == PlayerPowerup.playerFly && !isFlying && !isPushing)
                {
                    Debug.Log("isFlying set to true");
                    isFlying = true;
                    flightDir = new Vector2(horizontalInput, verticalInput);
                    flightDir.Normalize();
                    bool cantFly = false;
                    //First check if the block the player is attempting to jump is jumpable
                    RaycastHit[] hitColliders = Physics.BoxCastAll(new Vector3(targetPosition.x, 0f, targetPosition.z), Vector3.one * tileSize / 2.5f, Vector3.up, Quaternion.identity, tileSize, playerCollMask);
                    if (hitColliders.Length > 0)
                    {
                        foreach (RaycastHit hit in hitColliders)
                        {
                            if (hit.collider.gameObject.tag == "IndestructibleBlock" || hit.collider.gameObject.tag == "DestroyableBlock" /*|| hit.collider.gameObject.tag == "Player"*/)
                            {
                                cantFly = true;
                                isFlying = false;
                                break;
                            }
                        }
                        if (!cantFly)
                        {
                            isFlying = true;
                            StopCoroutine(FlyPlayer());
                            StartCoroutine(FlyPlayer());
                            return;
                        }


                    }
                    else
                    {
                        isFlying = false;
                    }

                }
                else if (playerPowerup == PlayerPowerup.playerPushBombs && !isFlying && !isPushing)
                {
                    isPushing = true;
                    pushDir = new Vector2(horizontalInput, verticalInput);
                    pushDir.Normalize();
                    bool cantPush = false;
                    GameObject bomb = null;
                    //First check if the item the player is attempting to push is a bomb.
                    RaycastHit[] hitColliders = Physics.BoxCastAll(new Vector3(targetPosition.x, 0f, targetPosition.z), Vector3.one * tileSize / 2.5f, Vector3.up, Quaternion.identity, tileSize, playerCollMask);
                    if (hitColliders.Length > 0)
                    {
                        foreach (RaycastHit hit in hitColliders)
                        {
                            if (hit.collider.gameObject.tag == "Bomb")
                            {
                                cantPush = false;
                                isPushing = true;
                                bomb = hit.collider.gameObject;
                            }
                            else if (hit.collider.gameObject.tag == "IndestructibleBlock" || hit.collider.gameObject.tag == "DestroyableBlock" || hit.collider.gameObject.tag == "Player")
                            {
                                cantPush = true;
                                isPushing = false;
                                break;
                            }
                        }
                        if (!cantPush)
                        {
                            isPushing = true;
                            isMoving = false;
                            StopCoroutine(PushBombs(bomb));
                            StartCoroutine(PushBombs(bomb));
                            return;
                        }

                    }
                    else
                    {
                        isPushing = false;
                    }

                }
            }
        }
        else
        {
            // Calculate the target position based on input(free movement)
            targetPosition = transform.position + new Vector3(horizontalInput, 0f, verticalInput);
            //Linecast to prevent clipping.
            if (Physics.Linecast(transform.position, targetPosition + new Vector3(horizontalInput * freeMoveCapsuleSize, 0f, verticalInput * freeMoveCapsuleSize), playerCollMask))
            {
                transform.position = transform.position;
                isMoving = false;
            }
            else isMoving = true;
        }
    }

    IEnumerator FlyPlayer()
    {
        //start at player position. See if something obstructs their path.
        //if so check the next block. Keep start and end positions saved and find a point in the middle that has a y value of the height of a tile.
        Vector3 newTargetPos = targetPosition;
        bool keepFlying = true;
        List<Vector3> positions = new List<Vector3>();
        positions.Add(targetPosition + new Vector3(0, tileSize * 2, 0));
        playerRB.useGravity = false;

        while (true)
        {
            //Target position is the players position added to the point they are attempting to fly to
            newTargetPos += new Vector3(flightDir.x, 0f, flightDir.y) * tileSize;
            //Check if that spot has any objects in it that could prevent flight
            RaycastHit[] hitColliders = Physics.BoxCastAll(new Vector3(newTargetPos.x, 0f, newTargetPos.z), Vector3.one * tileSize / 2.5f, Vector3.up, Quaternion.identity, tileSize, playerCollMask);
            if (hitColliders.Length > 0)
            {
                foreach (RaycastHit hit in hitColliders)
                {
                    //If we hit a player or bomb we keep looping as we can jump overthose
                    if ((hit.collider.gameObject.tag == "Player" && hit.collider.gameObject != this.gameObject) || hit.collider.gameObject.tag == "Bomb")
                    {
                        keepFlying = true;
                    }
                    //If we hit any block flight cannot proceed and the loop breaks
                    else if (hit.collider.gameObject.tag == "IndestructibleBlock" || hit.collider.gameObject.tag == "DestroyableBlock"/* || hit.collider.gameObject.tag == "Player"*/)
                    {
                        keepFlying = false;
                        break;
                    }
                }
                //If we can fly we add that point with added height to the positions list.
                if (keepFlying)
                {
                    Debug.Log("Added flight position " + newTargetPos + new Vector3(0, tileSize * 2.5f, 0));
                    positions.Add(newTargetPos + new Vector3(0, tileSize * 2.5f, 0));
                }
                else break;
            }
            else
            {
                //If we hit nothing with the boxCast, we do a second check to ensure that that tile is playable. 
                if (tileManager.IsTileBlocked(newTargetPos))
                {
                    keepFlying = false;
                    break;
                }
                //If that tile is playable we have found the player's landing spot.
                else positions.Add(newTargetPos);
                break;
            }
            yield return null;
        }


        if (keepFlying)
        {
            PlayerController[] otherPlayers = GameObject.FindObjectsOfType<PlayerController>();
            foreach (var player in otherPlayers)
            {
                if (player != this && player.targetPosition == positions[positions.Count - 1] && (player.isMoving || player.isFlying))
                {
                    keepFlying = false;
                }
            }
        }


        //If there is no landing spot, we disallow flight and set variables back to their defaults
        if (!keepFlying)
        {

            //No spot to land at
            isFlying = false;
            isMoving = false;
            playerRB.useGravity = true;
            Debug.Log("No spot to land at");
        }
        //If we can fly, we loop over the list of positions. This flys the player over the spots and allows them to land.
        else
        {
            ParticleSystem.EmissionModule em = feathersVFX.emission;
            em.enabled = true;
            targetPosition = positions[positions.Count - 1];
            playerPowerupSource.clip = wingsLoop;
            playerPowerupSource.Play();
            int currentIndex = 0;
            while (true)
            {
                //fly to spots and check if a spot under the player has opened. If it has cancel flight early.
                if (positions[currentIndex] != transform.position && !isEliminated)
                {
                    transform.position = Vector3.MoveTowards(transform.position, positions[currentIndex], flightSpeed * Time.deltaTime);
                }
                else
                {
                    if (currentIndex + 1 < positions.Count)
                    {
                        currentIndex++;
                    }
                    else break;
                }
                yield return null;
            }
            isFlying = false;
            isMoving = false;
            playerRB.useGravity = true;
            playerPowerupSource.Stop();
            //feathersVFX.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            em.enabled = false;
            targetPosition = transform.position;
        }
        yield break;
    }

    IEnumerator PushBombs(GameObject bomb)
    {
        if (bomb == null)
        {
            yield break;
        }
        //Check from bomb location to the next tile in the player's last moved direction. If nothing is there we move the bomb to that tile. If another bomb is there we need to move the current bomb
        //and it as well. If a player, indestructible block, or destroyable block is there we break and stop movement. 
        List<GameObject> bombs = new List<GameObject>();
        Vector3 newTargetPos = bomb.transform.position;
        Vector3 startingBombPos = bomb.transform.position;
        bombs.Add(bomb);
        bool keepPushing = true;

        while (true)
        {
           // Debug.Log("Finding bomb positions");
            newTargetPos += new Vector3(pushDir.x, 0f, pushDir.y) * tileSize;
            RaycastHit[] hitColliders = Physics.BoxCastAll(new Vector3(newTargetPos.x, 0f, newTargetPos.z), Vector3.one * tileSize / 2.5f, Vector3.up, Quaternion.identity, tileSize, playerCollMask);
            if (hitColliders.Length > 0)
            {
                foreach (RaycastHit hit in hitColliders)
                {
                    if (hit.collider.gameObject.tag == "Bomb")
                    {
                        yield return null;
                        //Attempt to fix bug where a slime bomb could become stuck and not explode if a player pushed it.
                        BombBehavior bombBehavior = hit.collider.gameObject.GetComponent<BombBehavior>();
                        //Make sure the bomb isn't a moving slide bomb
                        if (bombBehavior != null && bombBehavior.IsMoving() == false)
                        {
                            keepPushing = true;
                            bombs.Add(hit.collider.gameObject);
                        }
                        else
                        {
                            keepPushing = false;
                            break;
                        }
                    }
                    else if (hit.collider.gameObject.tag == "IndestructibleBlock" || hit.collider.gameObject.tag == "DestroyableBlock" || hit.collider.gameObject.tag == "Player")
                    {
                        keepPushing = false;
                        break;
                    }
                }
            }
            else
            {
                //Nothing at this spot. Bombs can now move.
               // Debug.Log("Can move bombs.");
                break;
            }
            if (!keepPushing)
            {
                //Debug.Log("Can't move bombs.");
                break;
            }
            yield return null;
        }


        if (keepPushing)
        {
            foreach (GameObject bombOb in bombs)
            {
               // Debug.Log("moving bomb");
                bombOb.GetComponent<BombBehavior>().MoveBombToPosition(bombOb.transform.position + new Vector3(pushDir.x, 0f, pushDir.y) * tileSize, moveSpeed_Game);
            }
            while (isPushing)
            {
                Vector3 tilePos = tileManager.GetTileCenterFromWorldPosPlayer(startingBombPos);
                tilePos.y = transform.position.y;
                //move player to where bomb was
                if (tilePos != transform.position && !isEliminated)
                {
                    //Debug.Log("Moving to " + tilePos);
                    // Debug.Log("At " + transform.position);
                    transform.position = Vector3.MoveTowards(transform.position, tilePos, moveSpeed_Game * Time.deltaTime);
                }
                else
                {
                   // Debug.Log("Completed Player move.");
                    break;
                }
                yield return null;
            }
        }
        else
        {
            //Debug.Log("Can't push bombs");
        }
        isPushing = false;
        isMoving = false;
    }









    private void MoveToTarget()
    {
        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed_Game * Time.deltaTime);
        // Check if we've reached the target position
        if (Vector3.Distance(transform.position, targetPosition) == 0f)
        {
            isMoving = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //disallow input while game is active
        if (roundManager != null && !roundManager.IsStartingCountdownActive())
        {
            //This function is called by a unity event when any player hits their movement input. Read the data it sends in a vector 2 value.
            moveDirection = context.ReadValue<Vector2>();
            moveDirection.Normalize();
        }
        else moveDirection = Vector2.zero;
    }

    public void DeleteBombFromList(BombBehavior bomb)
    {
        //Make sure that this does not run while the player's bomb list is being iterated over or an exception will occur.
        if (bomb == null) return;
        if (bombList.Contains(bomb))
        {
            bombList.Remove(bomb);
        }
    }

    public void OnButton1(InputAction.CallbackContext context)
    {
        //disallow input while game is active and player is waiting to join. Stops players from eliming themselves when a player wins
        if (roundManager != null && (!roundManager.IsStartingCountdownActive() && !roundManager.IsWinInProgress() && !isFlying))
        {
            //Added switch as I realize this function is called 3 times(preformed, canceled, started). Now a bomb will only drop once.
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    //Debug.Log(playerStringID + " Pressed Button 1");

                    //Bomb Cap Logic
                    //remove null bombs that have exploded
                    bombList.RemoveAll(x => x == null);
                    if (bombList.Count < bombCap_Game && !isEliminated)
                    {
                        //Prevent player from placing bomb on surface that has one.
                        BombBehavior[] bombArray = GameObject.FindObjectsOfType<BombBehavior>();
                        bool spotHasBomb = false;
                        foreach (BombBehavior bomb in bombArray)
                        {
                            if (!bomb.scheduledForDelete && tileManager.GetTileCenterFromWorldPosBomb(bomb.transform.position) == tileManager.GetTileCenterFromWorldPosBomb(transform.position)) spotHasBomb = true;
                        }

                        if (!spotHasBomb)
                        {
                            //Add further check to ensure there isn't a bomb already on the tile. Tag all bombs and check all of their locations.
                            GameObject bomb = Instantiate(bombStash, tileManager.GetTileCenterFromWorldPosBomb(transform.position), bombStash.transform.rotation);
                            BombBehavior bombScript = bomb.GetComponent<BombBehavior>();
                            bombList.Add(bombScript);
                            //Add fire level here. editable by bombScript.canPierce and bombScript.fireLevel
                            bombScript.fireLevel = bombFireLevel_Game;
                            bombScript.bombType = playerBombType;
                            if (playerBombType == BombTypes.RemoteBomb)
                            {
                                if (wifiBombVFX.isPlaying) wifiBombVFX.Stop();
                                wifiBombVFX.Play();
                            }
                            bombScript.bombDirection = lastDirection;
                            bombScript.bombPlayerID = (int)playerCharacter;
                            bombScript.bombOwner = this;
                            bombPlaceSource.clip = bombPlaceSound[Random.Range(0, bombPlaceSound.Length)];
                            bombPlaceSource.Play();
                        }
                        else Debug.Log("Spot has a bomb already");
                    }
                    else Debug.Log("Player bomb cap reached or player is eliminated");
                    break;
                case InputActionPhase.Started:
                    break;
                case InputActionPhase.Canceled:
                    break;
            }
        }
    }

    public void OnButton2(InputAction.CallbackContext context)
    {
        // disallow input while game is active

        // This function is called when a player presses their button one
        switch (context.phase)
        {
            //Since button 2 has an interaction set in the input action list(hold for 5 seconds to leave game), we check if the action was started, completed, or canceled.
            case InputActionPhase.Performed:
                if (roundManager != null && roundManager.IsWinInProgress() || SceneManager.GetActiveScene().buildIndex == 8 || (roundManager != null && roundManager.roundStartCountdownActive && roundManager.roundCounter == 0))
                {
                    Debug.Log("Cannot leave during a win, round zero, or on the results screen");
                    return;
                }
                Debug.Log(playerStringID + " Left The Game");
                //Player has held button 1 for 5 seconds, remove player 
                if (roundManager != null) roundManager.HandlePlayerDropout(this);
                if (SceneManager.GetActiveScene().buildIndex == 2) { GameObject.FindAnyObjectByType<CSSTimerLogic>().LeavePlayer(this); }
                if (SceneManager.GetActiveScene().buildIndex == 3) { GameObject.FindAnyObjectByType<CSSTimerLogicVote>().LeavePlayer(this); }
                playerUIAssigner.EnablePlayerCanvas(false);
                playerUIAssigner.mpEventSystem.SetSelectedGameObject(null);
                bombList.RemoveAll(x => x == null);
                foreach (BombBehavior bomb in bombList.ToList())
                {
                    if (bomb != null) Destroy(bomb.gameObject);
                }
                if(roundManager != null)
                {
                    //We are in a match so spawn in death clone
                    StartCoroutine(SpawnElimationPlayerClone());
                }
                Destroy(gameObject);
                break;
            case InputActionPhase.Started:
                Debug.Log(playerStringID + " Pressed Button 2");
                if (roundManager != null && !isEliminated && (!roundManager.IsStartingCountdownActive() && !roundManager.IsWinInProgress()))
                {
                    bombList.RemoveAll(x => x == null);
                    List<BombBehavior> bombsToRemove = new List<BombBehavior>();
                    for (int i = 0; i < bombList.Count; i++)
                    {
                        if (bombList[i] != null)
                        {
                            bombList[i].ManualExplode();
                            bombsToRemove.Add(bombList[i]);
                        }
                    }
                    /*
                    foreach (BombBehavior bomb in bombList.ToList())
                    {
                        if (bomb != null)
                        {
                            bomb.ManualExplode();
                            bombsToRemove.Add(bomb);
                        }
                    }
                    */
                    //Delete all now detonated bombs from the list.
                    foreach (BombBehavior bomb in bombsToRemove)
                    {
                        DeleteBombFromList(bomb);
                    }
                }
                break;
            case InputActionPhase.Canceled:
                break;
        }
    }

    public void GetPlayerInput(PlayerInput input)
    {
#if WINNITRON_BUILD
        //Get the name of the new keyboard split device made from the keyboard splitter script.
        //This gives us the player input and since its connected to their number, we also get the player ID
        var device = input.devices[0];
        playerStringID = "Player " + device.name.ToString();
        string playerNumber = device.name.ToString();
#endif

#if NORMAL_BUILD
        //Input is not linked to the players number in non winnitron builds. Player number is linked to how recently the player joined. Etc 4th player to join will be player 4
        //Input also needs to fill if a player leaves. So if player 2 leaves and another player joins they need to be P2
        PlayerInputManager inputManager = GameObject.FindObjectOfType<PlayerInputManager>();
        string playerNumber = (input.playerIndex + 1).ToString();
        playerStringID = "Player " + (input.playerIndex + 1).ToString();
        // We execute this code on `playerInput.onControlsChanged`
        /*
        var gamepad = input.devices[0];
        if (gamepad is UnityEngine.InputSystem.Switch.SwitchProControllerHID || gamepad is UnityEngine.InputSystem.DualShock.DualShock4GamepadHID || gamepad is UnityEngine.InputSystem.DualShock.DualShock3GamepadHID || gamepad is UnityEngine.InputSystem.DualShock.DualSenseGamepadHID)
        {
            foreach (var item in Gamepad.all)
            {
                if ((item is UnityEngine.InputSystem.XInput.XInputController) && (Math.Abs(item.lastUpdateTime - gamepad.lastUpdateTime) < 0.1))
                {
                    Debug.Log($"Switch Pro controller detected and a copy of XInput was active at almost the same time. Disabling XInput device. `{gamepad}`; `{item}`");
                    InputSystem.DisableDevice(item);
                    //Destroy(gameObject);
                    break;
                }
            }
        }
        */
#endif





        //Check if the player number string is one of these numbers
        if (playerNumber == "1" || playerNumber == "2" || playerNumber == "3" || playerNumber == "4")
        {
            //If its a certain number set it. Super hacky but I couldn't parse an int from the string for some reason?
            if (playerNumber == "1") { playerID = 1; playerCharacter = PlayerCharacter.Patriot; }
            if (playerNumber == "2") { playerID = 2; playerCharacter = PlayerCharacter.Centaur; }
            if (playerNumber == "3") { playerID = 3; playerCharacter = PlayerCharacter.Toph; }
            if (playerNumber == "4") { playerID = 4; playerCharacter = PlayerCharacter.Professor; }
            //playerModel.material = playerColors[playerID - 1];

            //Uncomment later logic to spawn character anims at player spawn. If a player has already selected their character(like in the menu) we dont spawn in a new animator on them. 
            if (playerAnimationObject == null)
            {
                //Set the index of this to [playerID - 1] once more prefabs have been created. Sprites should look at camera aswell but that script can be added to sprite prefab later(Like amelie did). 
                //Some scaling and movement of the sprite is done cause it takes the scale of the player prefab which is quite large. Will def have to check scaling later.
                //Could use a combination of the playerid and enum casted to int to find alternate colors.


                //Set colored players, pulls from 4 arrays for each player character

                GameObject playerObject;
                switch (playerCharacter)
                {
                    default:
                        {
                            playerObject = playerAnimPrefabsPatriot[playerID - 1];
                            break;
                        }
                    case PlayerCharacter.Patriot:
                        {
                            playerObject = playerAnimPrefabsPatriot[playerID - 1];
                            break;
                        }
                    case PlayerCharacter.Centaur:
                        {
                            playerObject = playerAnimPrefabsCentaur[playerID - 1];
                            break;
                        }
                    case PlayerCharacter.Toph:
                        {
                            playerObject = playerAnimPrefabsToph[playerID - 1];
                            break;
                        }
                    case PlayerCharacter.Professor:
                        {
                            playerObject = playerAnimPrefabsProf[playerID - 1];
                            break;
                        }
                }
                playerAnimationObject = Instantiate(playerObject, transform);
                playerAnimationObject.GetComponent<PlayerCrownBillboarder>().ToggleCrown(false);
                //Add anim object to the correct layer to make sure outlines show behind geo
                switch (playerID)
                {
                    case 1:
                        playerAnimationObject.layer = LayerMask.NameToLayer("Player1");
                        break;
                    case 2:
                        playerAnimationObject.layer = LayerMask.NameToLayer("Player2");
                        break;
                    case 3:
                        playerAnimationObject.layer = LayerMask.NameToLayer("Player3");
                        break;
                    case 4:
                        playerAnimationObject.layer = LayerMask.NameToLayer("Player4");
                        break;
                }
                shieldVFX = playerAnimationObject.GetComponent<SpriteGlowEffect>();
                shieldVFX.OutlineWidth = 0;
                shieldVFX.GlowBrightness = 1;
                playerAnimationObject.transform.localScale = Vector3.one / 4;
                playerAnimationObject.transform.localPosition = new Vector3(0, -0.38f, 0);
                playerAnimator = playerAnimationObject.GetComponent<Animator>();
            }
            playerUIAssigner.AssignPlayerUI(playerID, this);
            if (SceneManager.GetActiveScene().buildIndex == 2) { GameObject.FindAnyObjectByType<CSSTimerLogic>().JoinPlayer(this); }
            if (SceneManager.GetActiveScene().buildIndex == 3) { GameObject.FindAnyObjectByType<CSSTimerLogicVote>().JoinPlayer(this); }
        }

    }

    public void ChangePlayerCharacter(int charID)
    {
        //If we receive a 4 a player opted to be a random character.
        if (charID < 4)
        {
            playerCharacter = (PlayerCharacter)charID;
        }
        else
        {
            playerCharacter = (PlayerCharacter)Random.Range(0, 4);
        }

        if (playerAnimationObject != null) Destroy(playerAnimationObject);
        //Set the index of this to [playerID - 1] once more prefabs have been created. Sprites should look at camera aswell but that script can be added to sprite prefab later(Like amelie did). 
        //Some scaling and movement of the sprite is done cause it takes the scale of the player prefab which is quite large. Will def have to check scaling later.

        GameObject playerObject;
        switch (playerCharacter)
        {
            default:
                {
                    playerObject = playerAnimPrefabsPatriot[playerID - 1];

                    break;
                }
            case PlayerCharacter.Patriot:
                {
                    playerObject = playerAnimPrefabsPatriot[playerID - 1];
                    break;
                }
            case PlayerCharacter.Centaur:
                {
                    playerObject = playerAnimPrefabsCentaur[playerID - 1];
                    break;
                }
            case PlayerCharacter.Toph:
                {
                    playerObject = playerAnimPrefabsToph[playerID - 1];
                    break;
                }
            case PlayerCharacter.Professor:
                {
                    playerObject = playerAnimPrefabsProf[playerID - 1];
                    break;
                }
        }
        PlayPlayerVoiceline(CharacterVoiceLine.CharSelect);
        playerAnimationObject = Instantiate(playerObject, transform);
        playerAnimationObject.GetComponent<PlayerCrownBillboarder>().ToggleCrown(false);
        //Add anim object to the correct layer to make sure outlines show behind geo
        switch (playerID)
        {
            case 1:
                playerAnimationObject.layer = LayerMask.NameToLayer("Player1");
                break;
            case 2:
                playerAnimationObject.layer = LayerMask.NameToLayer("Player2");
                break;
            case 3:
                playerAnimationObject.layer = LayerMask.NameToLayer("Player3");
                break;
            case 4:
                playerAnimationObject.layer = LayerMask.NameToLayer("Player4");
                break;
        }
        shieldVFX = playerAnimationObject.GetComponent<SpriteGlowEffect>();
        shieldVFX.OutlineWidth = 0;
        shieldVFX.GlowBrightness = 1;
        playerAnimationObject.transform.localScale = Vector3.one / 4;
        playerAnimationObject.transform.localPosition = new Vector3(0, -0.38f, 0);
        playerAnimator = playerAnimationObject.GetComponent<Animator>();
        if (SceneManager.GetActiveScene().buildIndex == 2) { GameObject.FindAnyObjectByType<CSSTimerLogic>().LockInPlayer(this); }
    }

    public void VoteForMap(int mapID)
    {
        Debug.Log(mapID);
        if (SceneManager.GetActiveScene().buildIndex == 3) { GameObject.FindAnyObjectByType<CSSTimerLogicVote>().LockInPlayer(this, mapID); }
    }

    public int GetPlayerID()
    {
        return playerID;
    }

    public int GetPlayerCharacter()
    {
        return (int)playerCharacter;
    }

    public void EliminatePlayer(string deathCause)
    {
        if (roundManager != null && !roundManager.IsWinInProgress() && !isEliminated)
        {
            roundManager.playerUIManager.EliminatePlayerUIChange(GetPlayerID(), this);
            //Announcer.instance.PlayCharacterElimLine(this);
            StartCoroutine(SpawnElimationPlayerClone());
            StartCoroutine(DelayCallout(CharacterVoiceLine.Eliminated));
            //PlayPlayerVoiceline(CharacterVoiceLine.Eliminated);
            //Camera.main.transform.DOComplete();
            //Camera.main.transform.DOShakePosition(0.35f, 0.5f, 14, 19, false, true);
            impulser.GenerateImpulseAtPositionWithVelocity(transform.position, Vector3.one);
            eliminationSource.clip = eliminationSound[Random.Range(0, eliminationSound.Length)];
            eliminationSource.Play();
            playerAnimator.SetBool("IsMovingRight", false);
            playerAnimator.SetBool("IsMovingLeft", false);
            playerAnimator.SetBool("IsMovingUp", false);
            playerAnimator.SetBool("IsMovingDown", false);
            Debug.Log("Player " + GetPlayerID() + deathCause);
            transform.position = roundManager.GetDeathSpawn(((GetPlayerID() - 1) + 4), gameObject);
            StopCoroutine(FlyPlayer());
            isEliminated = true;
            isMoving = false;
            isFlying = false;
            isPushing = false;
            //note for later
            //Undetonated bombs from a player should be detonated when they die, or removed same with when a round ends. Players need to be invincible when a round ends.
            bombList.RemoveAll(x => x == null);
            foreach (BombBehavior bomb in bombList.ToList())
            {
                if (bomb != null)
                {
                    bomb.ManualExplode();
                }
            }
        }
    }



    public void ResetPlayerPowerups()
    {
        //Reset player values to their default values.
        //Set the players bombcap to its default
        if(Secret <= 0) bombCap_Game = bombCap;
        //Set the players speed to its default
        moveSpeed_Game = moveSpeed;
        //Set the players bomb to default.
        playerBombType = BombTypes.DefaultBomb;
        //Set player's shield to none if they have one
        hasShield = false;
        //Reset player's bomb fire level
        bombFireLevel_Game = bombFireLevel;
        //Reset bomb move powerup
        //Reset wings powerup
        playerPowerup = PlayerPowerup.None;
        playerRB.useGravity = true;
        StopCoroutine(FlyPlayer());
        isEliminated = false;
        //Detonate all remote bombs if player has any.
        bombList.RemoveAll(x => x == null);
        foreach (BombBehavior bomb in bombList.ToList())
        {
            Destroy(bomb.gameObject);
        }
        playerAnimator.SetBool("IsMovingRight", false);
        playerAnimator.SetBool("IsMovingLeft", false);
        playerAnimator.SetBool("IsMovingUp", false);
        playerAnimator.SetBool("IsMovingDown", false);
        playerRB.isKinematic = true;
        //Clear the players bomblist.
        bombList.Clear();
        isMoving = false;
        isFlying = false;
        isPushing = false;
        targetPosition = transform.position;
        ActivateShieldVFX(false);
        if (!GameObject.ReferenceEquals(roundManager, null))
        {
            UpdatePlayerUI();
        }
    }

    public void UpdatePlayerUI()
    {
        //use sparingly. Updates the player UI this logic has been decoupled to prevent it from running on everyframe
        //if (isEliminated) return;
        //roundManager.playerUIManager.PlayerEnterMatchUIChange(GetPlayerID(), this);
        
    }

    public void CreateDust()
    {
        Instantiate(dust, gameObject.transform);
        if (!dust.isPlaying) dust.Play();
        //Currently kind of empty/useless but I figured it'd be nice to keep it around just in case we use it for color changing?
    }



    private void OnEnable()
    {
        // playerInput.ActivateInput();
        isMoving = false;

    }

    private void OnDisable()
    {
        //playerInput.DeactivateInput();
        isMoving = false;
    }

    private void OnDestroy()
    {

        bombList.RemoveAll(x => x == null);
        foreach (BombBehavior bomb in bombList.ToList())
        {
            Destroy(bomb);
        }
        bombList.Clear();
    }


    public GameObject GetPlayerAnimObject()
    {
        return playerAnimationObject;
    }



    private IEnumerator SpawnElimationPlayerClone()
    {
        //This function spawns a clone of the players sprite object. This clone spawns on their death pos and plays the death anim and fades away and is then deleted.

        GameObject playerObject;
        //First we grab the correct sprite prefab from the arrays. This is determined by the players character type enum and their playerID(1-4)
        switch (playerCharacter)
        {
            default:
                {
                    playerObject = playerAnimPrefabsPatriot[playerID - 1];
                    break;
                }
            case PlayerCharacter.Patriot:
                {
                    playerObject = playerAnimPrefabsPatriot[playerID - 1];
                    break;
                }
            case PlayerCharacter.Centaur:
                {
                    playerObject = playerAnimPrefabsCentaur[playerID - 1];
                    break;
                }
            case PlayerCharacter.Toph:
                {
                    playerObject = playerAnimPrefabsToph[playerID - 1];
                    break;
                }
            case PlayerCharacter.Professor:
                {
                    playerObject = playerAnimPrefabsProf[playerID - 1];
                    break;
                }
        }
        //We instantiate the clone
        GameObject playerClone = Instantiate(playerObject);
        switch (playerID)
        {
            case 1:
                playerClone.layer = LayerMask.NameToLayer("Player1");
                break;
            case 2:
                playerClone.layer = LayerMask.NameToLayer("Player2");
                break;
            case 3:
                playerClone.layer = LayerMask.NameToLayer("Player3");
                break;
            case 4:
                playerClone.layer = LayerMask.NameToLayer("Player4");
                break;
        }
        playerClone.GetComponent<SpriteGlowEffect>().OutlineWidth = 0;
        //Add a billboarder to the clone so it is accurately billboarded
        playerClone.AddComponent<PlayerElimCloneBillboarder>().ReassignCameraTarget(playerBillboarderClass.ReturnCurrentCamTarget());
        //Disable crown
        playerClone.GetComponent<PlayerCrownBillboarder>().ToggleCrown(false);
        //Queue destruction of this object
        Destroy(playerClone, 5);
        //Set rotation of the clone
        playerClone.transform.rotation = playerAnimationObject.transform.rotation;
        //Get a reference to the clones animator
        Animator playerCloneAnimator = playerClone.GetComponent<Animator>();
        //Set the clone position to the death pos of the player
        playerClone.transform.position = gameObject.transform.position;
        playerClone.transform.localScale = playerAnimationObject.transform.lossyScale;
        //Activate the eliminated animation on the player clone's animator
        playerCloneAnimator.SetBool("Eliminated", true);
        //Get a reference to the player clone sprite's sprite renderer
        SpriteRenderer playerCloneSprite = playerClone.GetComponent<SpriteRenderer>();
        //Wait one second
        //yield return new WaitForSeconds(2);
        //Tween its alpha to zero so it fades away 
        playerCloneSprite.DOFade(0, 3f);
        yield return null;  
    }

    public void OnBack()
    {
#if NORMAL_BUILD
        GameObject.FindObjectOfType<EscMenu>().CallEscBack(this);
#endif 
    }
}

    public enum CharacterVoiceLine
{
    Elimination = 0,
    Eliminated = 1,
    Victory = 2,
    RoundWin = 3,
    CharSelect = 4,
    GetHit = 5
}

public enum PlayerPowerup
{
    None,
    playerFly,
    playerPushBombs
}

public enum PlayerCharacter
{
    Patriot = 0,
    Centaur = 1,
    Toph = 2,
    Professor = 3,
}
