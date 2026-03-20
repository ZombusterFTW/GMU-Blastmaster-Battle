using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AudienceMember : MonoBehaviour, Billboardable
{
    private RoundManager roundManager;
    //private Vector3 whereToLook;
    public Sprite cheer;
    public Sprite netural;
    private SpriteRenderer spriteRenderer;
    private float cheerDelayMin = 0.1f;
    private float cheerDelayMax = 0.35f;


    private float randomCheerRollDelayMin = 10.0f;
    private float randomCheerRollDelayMax = 25.0f;
    private bool cheerInProg = false;
    private Coroutine randomCheerChance;

    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        roundManager = GameObject.FindObjectOfType<RoundManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = netural;
        mainCamera = Camera.main;

        randomCheerChance = StartCoroutine(RandomCheerLoop());


    }

    // Update is called once per frame
    void Update()
    {







        //Scrapped ability for crowd to follow the average movement of all players. Was just not possible to get it feeling right without an insane amount of R&D
        /*
        List<PlayerController> players = roundManager.GetJoinedPlayers();
        whereToLook = FindAveragePos(players);
        whereToLook.x = 0;
        transform.LookAt(whereToLook);
        //transform.localEulerAngles = new Vector3(0, 180, 0);
        */
    }

    private void LateUpdate()
    {
        transform.LookAt(mainCamera.transform);
        transform.Rotate(0, 180, 0);

    }


    private Vector3 FindAveragePos(List<PlayerController> players)
    {
        if (players.Count == 0)
        {
            return Vector3.zero;
        }
        Vector3 meanVector = Vector3.zero;  

        foreach (PlayerController player in players) 
        {
            meanVector += player.transform.position;
        }

        return(meanVector/players.Count);
    }


    public void Cheer(bool cheer = true, float cheerTimeMin = 0, float cheerTimeMax = 0)
    {
        cheerInProg = true;
        //StopCoroutine(CheerFunction(Random.Range(cheerTimeMin, cheerTimeMax)));
        if (randomCheerChance != null) StopCoroutine(randomCheerChance);
        if (cheer) 
        {
            StartCoroutine(CheerFunction(Random.Range(cheerTimeMin, cheerTimeMax)));
        }
        else spriteRenderer.sprite = netural;
    }



    IEnumerator CheerFunction(float cheerTime) 
    {
        spriteRenderer.sprite = netural;
        yield return new WaitForSeconds(Random.Range(cheerDelayMin, cheerDelayMax));
        spriteRenderer.sprite = cheer;
        CheerAnim();
        if (cheerTime > 0)
        {
            yield return new WaitForSeconds(cheerTime);
            Cheer(false);
            cheerInProg = false;
            randomCheerChance = StartCoroutine(RandomCheerLoop());
        }
    }

    private void CheerAnim()
    {
        spriteRenderer.transform.DOComplete();
        spriteRenderer.transform.DOPunchPosition(new Vector3(0, Random.Range(cheerDelayMin, cheerDelayMax), 0), Random.Range(cheerDelayMin, cheerDelayMax));
        spriteRenderer.transform.DOPunchScale(new Vector3(0, Random.Range(cheerDelayMin, cheerDelayMax)/10, 0), Random.Range(cheerDelayMin, cheerDelayMax));
    }


    private IEnumerator RandomCheerLoop() 
    {
        while (!cheerInProg) 
        {
            yield return new WaitForSeconds(Random.Range(randomCheerRollDelayMin, randomCheerRollDelayMax));
            if(Random.Range(1, 3) == 1)
            {
                int rand = Random.Range(0, 3);
                if (rand == 0)
                {
                    spriteRenderer.sprite = cheer;
                    yield return new WaitForSeconds(Random.Range(cheerDelayMin*4, cheerDelayMax*3));
                    spriteRenderer.sprite = netural;
                }
                else if (rand == 1)
                {
                    spriteRenderer.transform.DOComplete();
                    spriteRenderer.transform.DOPunchScale(new Vector3(0, Random.Range(cheerDelayMin, cheerDelayMax) / 10, 0), Random.Range(cheerDelayMin, cheerDelayMax));
                }
                else if (rand == 2)
                {
                    spriteRenderer.transform.DOComplete();
                    spriteRenderer.transform.DOPunchPosition(new Vector3(0, Random.Range(cheerDelayMin, cheerDelayMax), 0), Random.Range(cheerDelayMin, cheerDelayMax));
                }
            }
        }
    }

    public void ReassignCameraTarget(Camera camera)
    {
        mainCamera = camera;
       // Debug.Log("reassignedcamera");
    }
}
