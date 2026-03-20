using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class BombermanGameManager : MonoBehaviour
{
    //This script will take care of spawning items at random intervals and ending the game when only player is left alive
    //Will include logic to ensure that items only spawn on open areas


    [Tooltip("The minimum time between item spawns")]
    [SerializeField] private float minTimeForSpawn = 10;
    [Tooltip("The maximum time between item spawns")]
    [SerializeField] private float maxTimeForSpawn = 25;
    [Tooltip("This bool determines wether or not items can spawn at all, can be changed at run time.")]
    private bool canItemsSpawn = true;
    [Tooltip("The maximum amount of items that can be spawned in at once. Number is abitrary currently. Design hasn't decided if this is feature yet.")]
    [SerializeField] private int maxItemsSpawned = 3;
    [Tooltip("The items that can spawn.")]
    [SerializeField] private GameObject[] itemArray;
    [Tooltip("The bombs that can spawn.")]
    [SerializeField] private GameObject[] bombArray;
    [SerializeField] private GameObject[] specialItemArray;
    [SerializeField] private bool weightedItemSpawnsEnabled = true;

    [SerializeField] public GameObject pressureBlock;
    private Coroutine pressureSpawn;
    private List<PressureBlock> pressureBlocksSpawned = new List<PressureBlock>();
    private bool pressureBlocksActive = false;


    [SerializeField] private List<GameObject> spawnedItems = new List<GameObject>();

    //The amount of items currently spawned in at a given moment. Will be updated at runtime
    private int currentItemsSpawned = 0;
    private bool itemSpawning = false;

    public Tilemap tilemap;
    public List<Vector3> tileWorldLocations;
    public List<Vector3> takenTiles;
    public List<Vector3> pressureBlockLocations;
    public List<GameObject> destroyableBlocks;
    private List<Vector3> pressureBlockSpiral;
    public List<Vector3> blockLocationsNoIndestructibles;
    public RoundManager roundManager {get; private set;}


    public void ReadGamemodeData(Gamemode_TemplateScriptableObject gameMode)
    {
        //Read data from the gamemode and set values accordingly
        itemArray = gameMode.commonPowerupDrops;
        bombArray = gameMode.rarePowerupDrops;
        specialItemArray = gameMode.legendaryPowerupDrops;
        minTimeForSpawn = gameMode.minTimeForPowerupSpawn;
        maxTimeForSpawn = gameMode.maxTimeForPowerupSpawn;
        maxItemsSpawned = gameMode.maxPowerupsSpawned;
        weightedItemSpawnsEnabled = gameMode.weightedPowerupSpawnsEnabled;
    }
    private void Awake()
    {
        itemSpawning = false;
        canItemsSpawn = false;
        currentItemsSpawned = spawnedItems.Count;
        tileWorldLocations = new List<Vector3>();
        pressureBlockSpiral = new List<Vector3>();

        /*foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = tilemap.CellToWorld(localPlace);
            if (tilemap.HasTile(localPlace))
            {
                tileWorldLocations.Add(place);
            }
        }
        print(tileWorldLocations);*/

        Tilemap tilemap = GetComponent<Tilemap>();
        tilemap.CompressBounds();

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                //TileBase tile = allTiles[x + z * bounds.size.x];
                Vector3Int localPlace = new Vector3Int(x, y, (int)tilemap.transform.position.y);

                Vector3 place = tilemap.CellToWorld(localPlace);

                if (tilemap.HasTile(localPlace))
                {
                    Vector3 centerCell = tilemap.GetCellCenterWorld(localPlace);
                    tileWorldLocations.Add(centerCell);
                    // Debug.Log("x:" + x + " z:" + y + " tile:");
                }
                else
                {
                    //Debug.Log("x:" + x + " z:" + y + " tile: (null)");

                }
                Vector3 centerCell2 = tilemap.GetCellCenterWorld(localPlace);
                pressureBlockSpiral.Add(centerCell2);
            }
        }
        pressureBlockSpiral = TranslateToSpiral(pressureBlockSpiral);
    }

    void Start()
    {
        roundManager = GameObject.FindObjectOfType<RoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!itemSpawning && canItemsSpawn)
        {
            //If there isn't an item spawning and items can spawn, we schedule one to drop
            Debug.Log("Trigger item spawn");
            itemSpawning = true;
            StartCoroutine(HandleItemSpawn());
        }
        else if(itemSpawning && !canItemsSpawn)
        {
            Debug.Log("cancel qued item spawn");
            itemSpawning = false;
            StopCoroutine(HandleItemSpawn());
        }
    }

    public void EnableItemSpawn(bool enable)
    {
        if(enable) canItemsSpawn = true;
        else canItemsSpawn = false;

    }

    private IEnumerator HandleItemSpawn()
    {
        //Currently the max powerup cap will not intefere with the powerup random countdown. Say the max has been reached and a player grabs a powerup reducing current ones on screen. A countdown is still happening in the back ground so there is a chance that a powerup will instantly spawn after one is grabbed. 
        //I'll bring it up to design if thats what they want or not. But this is script is really temp and can be thrown out.

        float randTime = Random.Range(minTimeForSpawn, maxTimeForSpawn);
        Debug.Log("An item is spawning in " + randTime + " seconds.");
        yield return new WaitForSeconds(randTime);
        if (canItemsSpawn)
        {
            //check again if items can spawn.
            SpawnTrackedItem();
            itemSpawning = false;
        }
    }


    public int GetCurrentItemCount()
    {
        //Returns the current number of items on screen and clears the object list of null o
        spawnedItems.RemoveAll(x => x == null);
        
        return spawnedItems.Count;
    }

 

    private void SpawnTrackedItem()
    {
        //Grabs a random item from the item array(TBD) and spawns it if currentItemsSpawned is not equal to maxItemsSpawned
        //Randomize the item in this function

        if (GetCurrentItemCount() < maxItemsSpawned)
        {
            Debug.Log("Spawned item sucessfully");
            //add code to instantiate the item prefab and set a bool on the item that says it is tracked
            int itemIndex = Random.Range(0, (tileWorldLocations.Count - 1));

            //temp   


            //Set bool to check in function
            bool doesNotHaveSameLocation = true;
            bool doesNotHaveSameLocation2 = true;
            bool doesNotHaveSameLocation3 = true;
            //Add bad locations to a list to prevent item spawn there
            List<Vector3> Badlocations = tileWorldLocations.ToArray().ToList();
            //Loop indefininetly
            while (Badlocations.Count > 0)
            {
                //At the start of a loop set doesNotHaveSameLocation to true.
                doesNotHaveSameLocation = true;
                doesNotHaveSameLocation2 = true;
                doesNotHaveSameLocation3 = true;
                //Loop over every current spawned item.
                //Check if destroyable block is on that tile
                for (int i = 0; i < spawnedItems.Count; i++)
                {
                    if (Badlocations.Count == 0)
                    {
                        doesNotHaveSameLocation3 = false;
                        break;
                    }
                    //Grab the reference to a spawned item in the spawned items list and see if its location equals the location we grabbed from the location list.
                    else if (spawnedItems[i].transform.position == new Vector3(Badlocations[itemIndex].x, 3, Badlocations[itemIndex].z))
                    {
                        Badlocations.RemoveAt(itemIndex);
                        //If these equal then we need to grab a new location from the list.
                        itemIndex = Random.Range(0, (Badlocations.Count - 1));
                        //Set our doesNotHaveSameLocation to false as we got a location that was the same.
                        doesNotHaveSameLocation = false;
                        //break out of the for loop, so we can then loop again
                        break;
                    }
                }
                //Prevent powerups from spawning beneath destroyable blocks.
                for(int i = 0; i < roundManager.GetDestroyableBlocks().Count; i++)
                {
                    if (Badlocations.Count == 0)
                    {
                        doesNotHaveSameLocation3 = false;
                        break;
                    }
                    //Grab the reference to a spawned item in the spawned items list and see if its location equals the location we grabbed from the location list.
                    else if (roundManager.GetDestroyableBlocks()[i].transform.position == Badlocations[itemIndex] + new Vector3(0, 2.5f, 0))
                    {
                        Badlocations.RemoveAt(itemIndex);
                        //If these equal then we need to grab a new location from the list.
                        itemIndex = Random.Range(0, (Badlocations.Count - 1));
                        //Set our doesNotHaveSameLocation to false as we got a location that was the same.
                        doesNotHaveSameLocation2 = false;
                        //break out of the for loop, so we can then loop again
                        break;
                    }
                }
                //Prevent powerups from spawning beneath presuure blocks.
                for (int i = 0; i < pressureBlocksSpawned.Count; i++)
                {
                    if(Badlocations.Count == 0)
                    {
                        doesNotHaveSameLocation3 = false;
                        break;
                    }
                    //Grab the reference to a spawned item in the spawned items list and see if its location equals the location we grabbed from the location list.
                    else if (pressureBlocksSpawned[i].targetPosition == Badlocations[itemIndex] + new Vector3(0, 2.5f, 0) && pressureBlocksSpawned[i].IsLanded() == true)
                    {   
                        Badlocations.RemoveAt(itemIndex);
                        //If these equal then we need to grab a new location from the list.
                        itemIndex = Random.Range(0, (Badlocations.Count - 1));
                        //Set our doesNotHaveSameLocation to false as we got a location that was the same.
                        doesNotHaveSameLocation3 = false;
                        //break out of the for loop, so we can then loop again
                        break;
                    }
                }
                if(roundManager.GetLevelIsFilledWithBlock())
                {
                    canItemsSpawn = false;
                    break;
                }
                //Check if doesNotHaveSameLocation is true and if it is we break the while loop as we don't have a location conflict any longer.
                if ((doesNotHaveSameLocation && doesNotHaveSameLocation2 && doesNotHaveSameLocation3)) break;
            }

            if (Badlocations.Count > 0)
            {
                GameObject powerupToSpawn = GrabRandomItem();
                if (powerupToSpawn != null)
                {
                    GameObject powerUp = Instantiate(powerupToSpawn, new Vector3(Badlocations[itemIndex].x, 3, Badlocations[itemIndex].z), Quaternion.identity) as GameObject;
                    spawnedItems.Add(powerUp);
                    powerUp.AddComponent<PowerupJumper>();
                }
                else
                {
                    Debug.Log("Couldn't find item to spawn.");
                }
            }
            else Debug.Log("No valid spots to spawn a powerup");
        }
        else Debug.Log("Too many items on screen to spawn another.");
    }


    public GameObject SpawnUntrackedItem(Vector3 pos, bool isGift = false, GameObject powerupIn = null)
    {
        //Grabs a random item from the item array(TBD) and spawns it regardless of how many items are already on screen.
        //This will be used for powerups that randomly drop from exploding destructible tiles.
        Debug.Log("Spawned item from brick successfully. Not tracked in currentItemCount.");
        //add code to instantiate the item prefab and set a bool on the item that says it isn't tracked
        //temp replace vector zero with block location
        GameObject powerupToSpawn = powerupIn;
        if (powerupToSpawn == null) powerupToSpawn = GrabRandomItem();
        if (powerupToSpawn != null)
        {
            GameObject powerUp = Instantiate(powerupToSpawn, pos, Quaternion.identity);
            if (!isGift) powerUp.AddComponent<PowerupJumper>();
            return powerUp;
        }
        Debug.Log("Bad thing happened :3 check 278 in bombermangamemanager");
        return null;
    }

    private GameObject GrabRandomItem()
    {
        int itemRoll;
        if (weightedItemSpawnsEnabled) itemRoll = Random.Range(1, 100);
        //if weighted spawns are disabled we always choose from the common array.
        else itemRoll = 51;

        //We handle the case of one of the arrays being empty to prevent any errors. 
        //We prevent an infinite loop in the case of all of the arrays being empty, even if that case occurs at runtime.

        if (specialItemArray.Length == 0 && itemArray.Length == 0 && bombArray.Length == 0)
        {
            Debug.Log("Loot table is empty but spawning is enabled.");
            return null;
        }
        while(true) 
        {
            if ((itemRoll <= 50 && itemRoll > 17) && !(specialItemArray.Length == 0 && itemArray.Length == 0 && bombArray.Length == 0))
            {
                Debug.Log("Rolled a bomb");
                //End is exclusive so minus 1 isn't needed
                int bombendIndex = bombArray.Length;
                int bombrandIndex = Random.Range(0, bombendIndex);
                if (bombArray.Length <= bombrandIndex)
                {
                    Debug.LogWarning("No prefab found to spawn.");
                    itemRoll = 17;
                }
                return bombArray[bombrandIndex];
            }
            else if (itemRoll <= 17 && !(specialItemArray.Length == 0 && itemArray.Length == 0 && bombArray.Length == 0))
            {
                int specialendIndex = specialItemArray.Length;
                int specialrandIndex = Random.Range(0, specialendIndex);
                if (specialItemArray.Length <= specialrandIndex)
                {
                    Debug.LogWarning("No prefab found to spawn.");
                    itemRoll = 51;
                }
                return specialItemArray[specialrandIndex];
            }
            else if (itemRoll > 50 && !(specialItemArray.Length == 0 && itemArray.Length == 0 && bombArray.Length == 0))
            {
                Debug.Log("Rolled an item");
                int endIndex = itemArray.Length;
                int randIndex = Random.Range(0, endIndex);
                if (itemArray.Length <= randIndex)
                {
                    Debug.LogWarning("No prefab found to spawn.");
                    itemRoll = 18;
                }
                return itemArray[randIndex];
            }
            else if (specialItemArray.Length == 0 && itemArray.Length == 0 && bombArray.Length == 0)
            {
                Debug.Log("Loot table is empty but spawning is enabled.");
                return null;
            }
         
        }
        
    }

    public void WipeAllItems()
    {
        //Get all of powerups with a tag here.
        GameObject[] items = GameObject.FindGameObjectsWithTag("PowerUp");
        foreach (GameObject powerUp in items) 
        {
            Destroy(powerUp);
        }
    }
    private List<Vector3> TranslateToSpiral(List<Vector3> pressureBlockSpiral)
    {
        int n = (int)Mathf.Sqrt(pressureBlockSpiral.Count);
        int nSquare = pressureBlockSpiral.Count;

        if (n <= 1) return pressureBlockSpiral;

        List<Vector3> newList = pressureBlockSpiral.GetRange(0, n);

        for (int i = 2; i <= n; i++)
        {
            newList.Add(pressureBlockSpiral[n * i - 1]);
        }
        newList.AddRange(pressureBlockSpiral.GetRange(nSquare - n, (nSquare - 2) - (nSquare - n - 1)).ToArray().Reverse());
        for (int i = 2; i < n; i++)
        {
            newList.Add(pressureBlockSpiral[nSquare - n * i]);
        }
        List<Vector3> nextList = new List<Vector3>();
        for (int i = 1; i < n - 1; i++)
        {
            nextList.AddRange(pressureBlockSpiral.GetRange(n * i + 1, (n * i + 1 + n - 2) - (n * i + 1)));
        }
        return newList.Concat(TranslateToSpiral(nextList)).ToList();
    }

    public void Pressure()
    {
        pressureSpawn = StartCoroutine(PressureBlockLoop());
    }

    public void StopPressure()
    {
        if(pressureSpawn != null)
        {
            StopCoroutine(pressureSpawn);
            PressureBlock[] pressureBlocks = GameObject.FindObjectsOfType<PressureBlock>();
            foreach (PressureBlock block in pressureBlocks)
            {
               block.enabled = false;
            }
            pressureBlocksActive = false;
        }
    }


    public void SpawnInPressureBlocks()
    {
        PressureBlock block = null;
        blockLocationsNoIndestructibles = tileWorldLocations.ToArray().ToList();
        for (int i = 0; i < pressureBlockSpiral.Count; i++)
        {
            if (tileWorldLocations.Contains(pressureBlockSpiral[i]))
            {
                GameObject pressureBlockReal = Instantiate(pressureBlock, pressureBlockSpiral[i] + new Vector3(0, 100, 0), Quaternion.identity);
                block = pressureBlockReal.GetComponent<PressureBlock>();
                pressureBlockReal.GetComponent<PressureBlock>().targetPosition = pressureBlockSpiral[i] + new Vector3(0, 2.5f, 0);
                pressureBlockLocations.Add(pressureBlockReal.GetComponent<PressureBlock>().targetPosition);
                pressureBlocksSpawned.Add(block);
                block.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Indestrutible Tile at that location");
            }
        }
    }


    private IEnumerator PressureBlockLoop()
    {
        pressureBlocksActive =true;
        for (int i = 0; i < pressureBlocksSpawned.Count; i++)
        {
            yield return new WaitForSeconds(0.5f);
            if (i == 0) Announcer.instance.PlayAnnouncerLine(AnnouncerLine.LowTimeHint);
            if (i == 15) Announcer.instance.PlayAnnouncerLine(AnnouncerLine.PressureBlockSpawn);
            pressureBlocksSpawned[i].gameObject.SetActive(true);
            pressureBlocksSpawned[i].ActivateBlock();
        }

        //This checks if the last block has landed and ends the round if it has. Allows for the overtime feature
        while (!pressureBlocksSpawned[pressureBlocksSpawned.Count-1].IsLanded())
        {
            yield return null;
        }
        roundManager.SetLevelIsFilledWithBlock(true);

        
    }
    public void ErasePressure()
    {
        if(pressureSpawn != null) StopCoroutine(pressureSpawn);
        foreach (PressureBlock block in pressureBlocksSpawned)
        {
            Destroy(block.gameObject);
        }
        pressureBlocksSpawned.Clear();
    }

}
