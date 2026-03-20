using UnityEngine;
[CreateAssetMenu(fileName = "GameModeScriptableObject", menuName = "ScriptableObjects/GameMode")]
public class Gamemode_TemplateScriptableObject : ScriptableObject
{
    //Class that represents a given gamemode. By default this class shows the "normal" gamemode
    [Header("GameMode Name and Description")]
    [Tooltip("The name of the game mode")] public string gamemodeName = "Default";
    [Tooltip("A short 1 line description of the gamemode")] public string gamemodeDescription = "The classic way to enjoy GMU Blastmaster Battle.";
    [Tooltip("Wether or not the game will present the game mode and desc. Default game mode will not show this text.")] public bool showGamemodeIndentifierOnRoundStart = false;

    [Header("Round Options")]
    [Tooltip("Round time in seconds")] public float roundTime = 120f;
    [Tooltip("Round starting countdown time in seconds")] public int roundStartCountdownTime = 3;
    //[Tooltip("How many rounds to win. This isn't used in gamemode options but is here for posterity.")] public int roundsToWin = 3;
    [Tooltip("The default win time in seconds. This time can be extended during runtime and is dependent on how long a given characters victory theme is.")] public int winTime = 5;
    [Tooltip("If the remaining round time is less than this value, pressure blocks will begin to fall.")] public float pressureBlockSpawnTime = 30f;
    [Tooltip("If dontSpawnPowerupsTillAllBlocksGone is false, powerups will begin to spawn randomly after this amount of time has passed.")] public float powerupSpawnTime = 30f;
    [Tooltip("If this value is true, random powerup spawn will only happen after all blocks have been destroyed.")] public bool dontSpawnRandomPowerupsTillAllBlocksGone = true;
    [Tooltip("If this value is set to true a game will begin with no destroyable blocks")] public bool beginWithNoBlocks = false;

    [Header("Random Powerup Spawn Options")]
    [Tooltip("The max amount of randomly spawned powerups that can exist at once. Note this only counts powerups that are not spawned from blocks.")][Range(1, 100)] public int maxPowerupsSpawned = 3;
    [Tooltip("The minimum time between powerup spawns")] public float minTimeForPowerupSpawn = 10;
    [Tooltip("The maximum time between powerup spawns")] public float maxTimeForPowerupSpawn = 25;

    [Header("Powerup Blockspawn Options")]
    [Tooltip("The odds of a powerup spawning from a block being destroyed")] public int powerupBlockDropChance = 4;

    [Header("Powerup Spawn Table Options")]
    [Tooltip("List of common drops to spawn")] public GameObject[] commonPowerupDrops;
    [Tooltip("List of rare drops to spawn")] public GameObject[] rarePowerupDrops;
    [Tooltip("List of legendary drops to spawn")] public GameObject[] legendaryPowerupDrops;

    [Tooltip("If false weighted powerup spawns are disabled. Note the game will only spawn items from the common powerup list if this is false.")] public bool weightedPowerupSpawnsEnabled = true;
    
    [Header("Player Round Options")]
    [Tooltip("Players will have their starting bomb cap set to this value")] public int startingBombCap = 1;
    [Tooltip("Players will have their starting bomb radius set to this value")] public int startingBombRadius = 2;
    [Tooltip("Players will have their starting move speed set to this value")] public float startingMoveSpeed = 25f;
    [Tooltip("Players will have their starting flight speed set to this value")] public float startingFlightSpeed = 50f;
    [Tooltip("If true players will begin the round with a shield.")] public bool startWithShield = false;
    [Tooltip("If start with shield is true the player will start with this many shields")] public int shieldStartingCount = 1;
    [Tooltip("Players will begin a round with this bombtype.")] public BombTypes startingBombType = BombTypes.DefaultBomb;
    [Tooltip("Players will begin a round with this player powerup.")] public PlayerPowerup startingPlayerPowerup = PlayerPowerup.None;
    [Tooltip("If true players will have their bombtype change every bombShiftTime seconds.")] public bool automaticBombShift = false;
    [Tooltip("The amount of time between bombShifts, only works if automaticBombShift is set to true. MUST be greater than 0")] public float bombShiftTime = 1.53f;



}
