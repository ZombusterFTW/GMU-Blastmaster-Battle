using Cinemachine;
using DG.Tweening;
using SpriteGlow;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static KeyboardSplitter;

public class BombBehavior : MonoBehaviour
{
    [Header("Bomb Settings")]
    private TileManager tileManager;
    private RoundManager roundManager;
    private bool exploded = false;
    //How long before a bomb explodes
    [Tooltip("How long before bomb detonation")] public float explosionTime = 2f;
    [Tooltip("Explosion volume")] public float explosionVolume = 2f;
    [Tooltip("The explosion fx prefab that will spawn on detonation")] private GameObject explosionPrefab;
    [Tooltip("Min time before lightning bomb delayed explosion")][SerializeField] private float minLightningTime = 1f;
    [Tooltip("Max time before lightning bomb delayed explosion")][SerializeField] private float maxLightningTime = 1f;
    [Tooltip("I think our problem with the inconsistent bomb radius is due to this value. By default bombs wont explode a full tile radius to prevent from hitting adjacent tiles. This value should be set closer to 2 to increase tile explosion radius")][SerializeField][Range(2, 2.5f)] private float bombGrace = 2.5f;
    [Tooltip("Speed that bomb slides at")][SerializeField] private float bombSlideSpeed = 5f;
    [Tooltip("This is how many units up the bomb will check for objects and players. Will need to find a good number that keeps flight balanced.")][SerializeField] private float bombTraceHeight = 2.5f;
    [Header("Bomb Powerup Settings(changed to a player's values on spawn).")]
    [Tooltip("The type of bomb. These bomb types replace each other.")]
    public BombTypes bombType = BombTypes.DefaultBomb;
    [Tooltip("How many blocks in each direction the bomb will explode")]
    public int fireLevel = 3;
    [Tooltip("The scale of each tile in the tilemap")]
    private float tileSize = 5f;
    [Tooltip("If the bomb can pierce through destroyable blocks")]
    public bool canPierce = false;
    public Vector3 bombDirection;
    private List<PlayerController> hitPlayerControllers = new List<PlayerController>();
    [Tooltip("The layer that the bomb will check collisions for.")] [SerializeField] private LayerMask bombLayerMask;
    [Tooltip("How long bomb collision is disabled after bomb creation")] public float noCollTime = 0f;
    private SphereCollider bombCollider;
    private bool isMoving = false;
    private SpriteRenderer bombSprite;
    
    public int bombPlayerID;
    private Camera mainCamera;
    public PlayerController bombOwner;

    [Tooltip("The sprite that will show for the default bomb for players, add sprites prefabs to list in order")] public GameObject[] defaultBombSprites;
    private Animator bombAnimator;
    private GameObject bombSpritePrefab;

    public GameObject Slide;
    public GameObject Explosion;


    public GameObject SpikeDown;
    public GameObject SpikeUp;
    public GameObject SpikeLeft;
    public GameObject SpikeRight;

    public GameObject lightningBombUp;
    public GameObject lightningBombDown;
    public GameObject lightningBombLeft;
    public GameObject lightningBombRight;

    private GameObject bombExploUp;
    private GameObject bombExploDown;
    private GameObject bombExploLeft;
    private GameObject bombExploRight;

    public AudioClip[] explosionSounds;
    public AudioClip slideBombLoop;
    public AudioClip lightningBombBuzz;
    public AudioClip slimeBombArrival;
    public AudioSource bombExplosionAudio; //Audio Source added to object in script section TE
    public AudioSource bombLoopAudio; //Audio Source added to object in script section TE
    public AudioSource lightingAudioSource; //Audio Source added to object in script section TE
    public bool scheduledForDelete = false;
    private CinemachineImpulseSource impulser;
    public float explosionSpreadTime = 0.1f;
    private Coroutine exploUp;
    private Coroutine exploDown;
    private Coroutine exploLeft;
    private Coroutine exploRight;
    private bool isSecondExplosion = false;




    private void Awake()
    {

        mainCamera = Camera.main;
        bombSprite = GetComponent<SpriteRenderer>();
        bombCollider = GetComponent<SphereCollider>();    
        hitPlayerControllers.Clear();
        tileManager = GameObject.FindObjectOfType<TileManager>();
        roundManager = GameObject.FindObjectOfType<RoundManager>(); 
        impulser = GetComponent<CinemachineImpulseSource>();
        //bombCollider.enabled = false;
        //StartCoroutine(CollCountDown());
    }

    private void Start()
    {
        //Set sprite to specific player.
        if (defaultBombSprites[bombPlayerID] != null)
        {
            bombSpritePrefab = Instantiate(defaultBombSprites[(bombPlayerID)], gameObject.transform);
            bombAnimator = bombSpritePrefab.GetComponent<Animator>();
            var shieldVFX = bombSpritePrefab.AddComponent<SpriteGlowEffect>();
            shieldVFX.OutlineWidth = 5;
            shieldVFX.GlowBrightness = 1;
            Color glowColor;
            switch (bombOwner.GetPlayerID())
            {
                default: glowColor = Color.white; 
                    break;
                case 1:
                    bombSpritePrefab.layer = LayerMask.NameToLayer("Player1");
                    glowColor = Color.red;
                    break;
                case 2:
                    bombSpritePrefab.layer = LayerMask.NameToLayer("Player2");
                    glowColor = Color.blue;
                    break;
                case 3:
                    bombSpritePrefab.layer = LayerMask.NameToLayer("Player3");
                    glowColor = Color.green;
                    break;
                case 4:
                    bombSpritePrefab.layer = LayerMask.NameToLayer("Player4");
                    glowColor = Color.yellow;
                    break;
            }
            shieldVFX.GlowColor = glowColor;
            bombSprite.enabled = false;
        }



        explosionPrefab = Explosion;
        switch (bombType)
        {
            //other bombs can be added here. for pierce bombs to work canPierce just needs to be set to true.
            case BombTypes.DefaultBomb:
                Debug.Log("Default bomb dropped");
                explosionPrefab = Explosion;
                break;
            case BombTypes.LightningBomb:
                Debug.Log("Lightning bomb dropped");
                //Do lightning bomb stuff, todo.
                bombExploUp = lightningBombUp;
                bombExploDown = lightningBombDown;
                bombExploLeft = lightningBombLeft;
                bombExploRight = lightningBombRight;
                break;
            case BombTypes.PierceBomb:
                canPierce = true;
                Debug.Log("Pierce bomb dropped");
                bombExploUp = SpikeUp;
                bombExploDown = SpikeDown; 
                bombExploLeft = SpikeLeft;
                bombExploRight = SpikeRight;
                break;
            case BombTypes.SlideBomb:
                Debug.Log("Slide bomb dropped");
                explosionPrefab = Slide;
                break;
            case BombTypes.RemoteBomb:
                Debug.Log("Remote bomb dropped");
                //Commented for testing flight logic with inert bombs.
                //bombType = BombTypes.DefaultBomb;   
                explosionPrefab = Explosion;
                break;
        }

        //Bomb explosion proceeds normally for any bombtypes except these 2.
        if(bombType != BombTypes.SlideBomb && bombType != BombTypes.RemoteBomb)
        {
            bombAnimator.SetTrigger("Explode");
            Invoke("Explode", explosionTime);
        }

        if (bombType == BombTypes.SlideBomb) StartCoroutine(BombSlide());
    }

    IEnumerator CollCountDown()
    {
        yield return new WaitForSeconds(noCollTime);
        bombCollider.enabled = true;
    }


    IEnumerator BombSlide()
    {
        bombLoopAudio.clip = slideBombLoop;
        bombLoopAudio.Play();
        isMoving = true;
        Vector3 targetPosition = gameObject.transform.position + (bombDirection * tileSize);
        bool bombIsNotBlocked = true;
        while(bombIsNotBlocked)
        {
            if ((Physics.Linecast(transform.position, targetPosition, bombLayerMask)))
            {
                bombIsNotBlocked = false;
                break;
            }
            if (!tileManager.IsTileBlocked(targetPosition)) gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPosition, bombSlideSpeed * Time.deltaTime);
            else bombIsNotBlocked = false;
            if (gameObject.transform.position == targetPosition)
            {
                targetPosition = gameObject.transform.position + (bombDirection * tileSize);
            }
            yield return null;
        }

        if(!bombIsNotBlocked)
        {
            //snap bomb to nearest tile if it stops early
            Vector3 earlyStop = tileManager.GetTileCenterFromWorldPosBomb(transform.position);
            transform.position = new Vector3(earlyStop.x, transform.position.y, earlyStop.z);
        }
        isMoving = false;
        bombLoopAudio.Stop();
        bombLoopAudio.PlayOneShot(slimeBombArrival);
        bombAnimator.SetTrigger("Explode");
        Invoke("Explode", explosionTime);
    }

    public bool IsMoving()
    {
        return isMoving;
    }


    public void Explode()
    {
        if (!exploded)
        {
            exploded = true;
            //Explode bomb
            BombAlgo();
            bombExplosionAudio.clip = explosionSounds[(int)bombType];
            bombExplosionAudio.Play();
            float audioLength = bombExplosionAudio.clip.length;
            //add lightning bomb stuff
            impulser.GenerateImpulseAtPositionWithVelocity(transform.position, Vector3.one);
            if (bombType == BombTypes.LightningBomb)
            {
                StartCoroutine(DelayedExplosion());
            }
            else
            {

                //Destroy the bomb.
                PostExplodeHandle(audioLength);
            }
        }

    }
    

    IEnumerator DelayedExplosion()
    {
        //Fake out, normal explosion then lightning
        
        //Delay the explosion for a lightning bomb
        lightingAudioSource.clip = lightningBombBuzz;
        lightingAudioSource.Play();
        yield return new WaitForSeconds(Random.Range(minLightningTime, maxLightningTime));
        isSecondExplosion = true;
        //explosionPrefab = lightningBomb;
        impulser.GenerateImpulseAtPositionWithVelocity(transform.position, Vector3.one);
        bombExplosionAudio.clip = explosionSounds[(int)bombType];
        bombExplosionAudio.Play();
        float audioLength = bombExplosionAudio.clip.length;
        BombAlgo();
        bombCollider.enabled = false;
        bombSpritePrefab.gameObject.SetActive(false);
        PostExplodeHandle(3);
    }

    private void PostExplodeHandle(float timeBeforeDelete)
    {
        exploded = true;
        scheduledForDelete = true;
        //This keeps the bomb alive albiet invisible and disabled
        //remove bomb from players list to make their cap update (Possible fix for never ending round?)
        //bombOwner.DeleteBombFromList(this.gameObject);
        //Disable collison
        bombCollider.enabled = false;
        //Make the bomb invisible
        bombSpritePrefab.gameObject.SetActive(false);
        bombOwner.DeleteBombFromList(this);
        GetComponent<BombDeleter>().DeleteMeIn(timeBeforeDelete);
        Destroy(this, explosionSpreadTime*fireLevel + 1);
        //Destroy(gameObject, timeBeforeDelete);
    }

    public void ManualExplode()
    {
        //Dont explode until player hits Button2
        if(bombType == BombTypes.RemoteBomb && !exploded)
        {
            exploded = true;
            //Delay frame to prevent bomb from being placed and exploded on the same frame.
            StartCoroutine(BombExplosionBugDelay());
        }
    }

    IEnumerator BombExplosionBugDelay()
    {
        yield return null;
        BombAlgo();
        bombExplosionAudio.clip = explosionSounds[(int)bombType];
        bombExplosionAudio.Play();
        float audioLength = bombExplosionAudio.clip.length;
        PostExplodeHandle(audioLength);
    }

    public bool GetIsExploded()
    { return exploded; }
    void BombAlgo()
    {
        //Clear the hit controller list so a player cannot be hit by a bomb twice
        hitPlayerControllers.Clear();
        //Bomb algo
        exploded = true;
        //Make the explosion at the bomb's center
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Vector3 targetPosition = transform.position;
        //Check if there are any bombs or players at the bombs position
        CheckBombHit(targetPosition, Vector3.zero, true);
        /*
        //Loop for the fire level of the bomb. Each loop does the same thing up, down, left, and right.
        for (int i = 0; i < fireLevel; i++)
        {
            if (i == 0) targetPosition = transform.position;
            //Go up 1 tile at a time
            targetPosition = targetPosition + (Vector3.forward * tileSize);
            //Check if the bomb will hit a indestructible block, if it does we break out of the loop
            if (tileManager.IsTileBlocked(targetPosition)) break;
            //Check if the bomb hit a destructible tile, if it did and it can't pierce we break the loop stopping the explosion in that direction.
            //Check bomb hit returns true if a destructable block is hit. 
            if (CheckBombHit(targetPosition) && !canPierce) break;
        }
        //down
        for (int i = 0; i < fireLevel; i++)
        {
            if (i == 0) targetPosition = transform.position;
            //Go up 1 tile at a time
            targetPosition = targetPosition + (Vector3.back * tileSize);
            if (tileManager.IsTileBlocked(targetPosition)) break;
            if (CheckBombHit(targetPosition) && !canPierce) break;
        }
        //left
        for (int i = 0; i < fireLevel; i++)
        {
            if (i == 0) targetPosition = transform.position;
            //Go up 1 tile at a time
            targetPosition = targetPosition + (Vector3.left * tileSize);
            if (tileManager.IsTileBlocked(targetPosition)) break;
            if (CheckBombHit(targetPosition) && !canPierce) break;
        }
        //right
        for (int i = 0; i < fireLevel; i++)
        {
            if (i == 0) targetPosition = transform.position;
            //Go up 1 tile at a time
            targetPosition = targetPosition + (Vector3.right * tileSize);
            if (tileManager.IsTileBlocked(targetPosition)) break;
            if (CheckBombHit(targetPosition) && !canPierce) break;
        }
        */
        StartCoroutine(LatentBombExplosionWrapper());

        
    }

    IEnumerator LatentBombExplosionWrapper()
    {
        //Starts a coroutine for each direction so explosion happens in each direction at once
        exploUp = StartCoroutine(LatentExplosionUp());
        exploDown = StartCoroutine(LatentExplosionDown());
        exploLeft = StartCoroutine(LatentExplosionLeft());
        exploRight = StartCoroutine(LatentExplosionRight());
        //Wait for explosion to finish
        //Can tell if explosion is completed by if the coroutines are null. This value is set to null at the end of each directional coroutine.
        while(exploUp != null && exploDown != null && exploRight != null && exploLeft != null) 
        {
            yield return null;
        }
       Debug.Log("explosion completed");
    }


    IEnumerator LatentExplosionUp()
    {
        Vector3 targetPosition = transform.position;
        for (int i = 0; i < fireLevel; i++)
        {
            //Go up 1 tile at a time
            targetPosition = targetPosition + (Vector3.forward * tileSize);
            yield return new WaitForSeconds(explosionSpreadTime);
            if (tileManager.IsTileBlocked(targetPosition)) break;
            if (CheckBombHit(targetPosition, Vector3.forward) && !canPierce) break;
        }
        exploUp = null;
    }
    IEnumerator LatentExplosionDown()
    {
        Vector3 targetPosition = transform.position;
        for (int i = 0; i < fireLevel; i++)
        {
            targetPosition = targetPosition + (Vector3.back * tileSize);
            yield return new WaitForSeconds(explosionSpreadTime);
            if (tileManager.IsTileBlocked(targetPosition)) break;
            if (CheckBombHit(targetPosition, Vector3.back) && !canPierce) break;
        }
        exploDown = null;
    }
    IEnumerator LatentExplosionLeft()
    {
        Vector3 targetPosition = transform.position;
        for (int i = 0; i < fireLevel; i++)
        {
            targetPosition = targetPosition + (Vector3.left * tileSize);
            yield return new WaitForSeconds(explosionSpreadTime);
            if (tileManager.IsTileBlocked(targetPosition)) break;
            if (CheckBombHit(targetPosition, Vector3.left) && !canPierce) break;
        }
        exploLeft = null;
    }
    IEnumerator LatentExplosionRight()
    {
        Vector3 targetPosition = transform.position;
        for (int i = 0; i < fireLevel; i++)
        {
            targetPosition = targetPosition + (Vector3.right * tileSize);
            yield return new WaitForSeconds(explosionSpreadTime);
            if (tileManager.IsTileBlocked(targetPosition)) break;
            if (CheckBombHit(targetPosition, Vector3.right) && !canPierce) break;
        }
        exploRight = null;
    }

    bool CheckBombHit(Vector3 targetPosition, Vector3 direction, bool first = false)
    {
        //This value will be returned when this function terminates.
        bool hitDestroyableBlock = false;
        //Box trace for the size of the tile but slightly smaller as tracing the full tile size will contact items in other tiles. 
        //Will likely need to tweak the boxcast distance to see what design likes. Setting this low will likely make the flight powerup OP
        RaycastHit[] hitColliders = Physics.BoxCastAll(new Vector3(targetPosition.x, 0f, targetPosition.z), Vector3.one * tileSize / bombGrace, Vector3.up, Quaternion.identity, bombTraceHeight);
        //Check what colliders the box trace hit
        foreach (RaycastHit hit in hitColliders)
        {
            //if we hit a destroyable block
            if (hit.collider.gameObject.tag == "DestroyableBlock")
            {
                Debug.Log("Broke block");
                //Break the block
                hit.collider.gameObject.GetComponent<DestroyableBlock>().DestroyBlock();
                //Set hit destroyable block to true
                hitDestroyableBlock = true;
            }
            //Only check if we hit a player if the round is active. This will prevent players from dying after they won a round. 
            else if (hit.collider.gameObject.tag == "Player" && roundManager.IsRoundActive() && !roundManager.IsWinInProgress() && !roundManager.IsStartingCountdownActive())
            {
                //Get reference to the player controller
                PlayerController playerController = hit.collider.gameObject.GetComponent<PlayerController>();


                //elim player logic.
                //Bomb checks to see if player is shielded
                if (playerController.hasShield == false && playerController.Secret <= 0)
                {
                    //Bomb will only elim a player if they aren't on its elim list
                    if (!hitPlayerControllers.Contains(playerController) && roundManager.IsRoundActive() && !roundManager.IsWinInProgress() && !roundManager.IsStartingCountdownActive())
                    {
                        hitPlayerControllers.Add(playerController);
                        Debug.Log(playerController.GetPlayerID() + " Hit by bomb.");
                        //Eliminate the player
                        playerController.EliminatePlayer("Blew up!");
                        //Change to track elims and self elims
                        bombOwner.AddElimination(playerController == bombOwner);
                        //Disable their PlayerController component.
                        //playerController.enabled = false;
                        //Possibly add a variable to a bomb to say who's it is so we can track elims.
                    }
                }
                else if(playerController.hasShield == true || playerController.Secret > 0)
                {
                    //Fixed a bug where a player could lose their shield and get hit by the bomb causing an elimination if they moved too fast.
                    if(!hitPlayerControllers.Contains(playerController)) hitPlayerControllers.Add(playerController);
                    if (playerController.Secret > 0)
                    {
                        playerController.Secret--;
                    }
                    playerController.PlayPlayerVoiceline(CharacterVoiceLine.GetHit);
                    //Removes shield instead of eliminating player
                    playerController.hasShield = false;
                    playerController.ActivateShieldVFX(false);
                }
            }
            //If explosion hits any bomb, then explode that bomb as well 
            else if (hit.collider.gameObject.tag == "Bomb" && hit.collider.gameObject != this.gameObject && hit.collider.gameObject.GetComponent<BombBehavior>() != null)
            {
                if(hit.collider.gameObject.GetComponent<BombBehavior>().bombType != BombTypes.LightningBomb)
                {
                    Debug.Log("Exploded another bomb");
                    hit.collider.gameObject.GetComponent<BombBehavior>().Explode();
                }
            }
            //Change to disallow the second explosion of a lightning bomb from destroying powerups as the tradeoff was unfair/
            else if(hit.collider.gameObject.tag == "PowerUp" && isSecondExplosion == false)
            {
                Debug.Log("Exploded powerup");
                Destroy(hit.collider.gameObject);
            }
        }

        //Rotate the spawned game objects forward vector to be in the direction of the explosion if the explosion is not the center
        if (!first && (bombType != BombTypes.RemoteBomb && bombType != BombTypes.DefaultBomb && bombType != BombTypes.SlideBomb))
        {
            //Spawn in explosion vfx
            //Hard coding this because Quanterions failed me
            if (direction == Vector3.forward)
            {
                GameObject explosionObject = Instantiate(bombExploUp, targetPosition, Quaternion.identity);
            }
            else if(direction == Vector3.back)
            {
                GameObject explosionObject = Instantiate(bombExploDown, targetPosition, Quaternion.identity);
            }
            else if (direction == Vector3.left)
            {
                GameObject explosionObject = Instantiate(bombExploLeft, targetPosition, Quaternion.identity);
            }
            else if (direction == Vector3.right)
            {
                GameObject explosionObject = Instantiate(bombExploRight, targetPosition, Quaternion.identity);
            }
        }
        else
        {
            GameObject explosionObject = Instantiate(explosionPrefab, targetPosition, Quaternion.identity);
        }
        //return if a destroyable block was hit. This is how the above loops can check if they can let an explosion proceed. 
        return hitDestroyableBlock;
    }


    public void MoveBombToPosition(Vector3 pos, float moveSpeed)
    {
       // StopCoroutine(MoveBomb(pos, moveSpeed));
        StartCoroutine(MoveBomb(pos, moveSpeed));
    }

    IEnumerator MoveBomb(Vector3 pos, float moveSpeed)
    {
        Vector3 spot = new Vector3(pos.x, gameObject.transform.position.y, pos.z); 
        while (spot != gameObject.transform.position)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, spot, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }


    private void OnDestroy()
    {
        if(bombSpritePrefab != null) Destroy(bombSpritePrefab);
    }


    void LateUpdate()
    {
        bombSpritePrefab.transform.transform.rotation = mainCamera.transform.rotation;
    }
}

public enum BombTypes
{
    DefaultBomb = 0,
    PierceBomb = 1,
    LightningBomb = 2,
    RemoteBomb = 3,
    SlideBomb = 4
}


