using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class MovingBombsPowerUp : MonoBehaviour
{
    private PlayerController playerController;

    public AudioSource movingPickUp;
    public new Collider collider;
    public GameObject spriteObject;
    public Sprite easterEgg;


    private void Awake()
    {
        //Secret alternate sprite easter egg
        if(Random.Range(0, 100) + 1 == 100)
        {
            spriteObject.GetComponent<SpriteRenderer>().sprite = easterEgg;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Checks if the thing colliding with the power-up has a playercontroller attached to it.
        playerController = other.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            PickUp(playerController.GetPlayerID());
        } 
    }
   

    void PickUp(int playerID)
    {
        //Set player bombType to Slide bomb. Sliding bomb logic will be in the bomb behavior script
        playerController.playerBombType = BombTypes.SlideBomb;
        Debug.Log("Power up picked up by Player " + playerID);
        playerController.UpdatePlayerUI();
        movingPickUp.Play();
        Debug.Log("pick up");

        collider.enabled = false;
        spriteObject.SetActive(false);
        Destroy(gameObject, 5f);
        if (GetComponent<PowerupJumper>() != null) GetComponent<PowerupJumper>().HoverCleanUp();
        //Destroy(gameObject);
    }

    private void OnDisable()
    {
        if (GetComponent<PowerupJumper>() != null) GetComponent<PowerupJumper>().HoverCleanUp();
    }

}


/*
Ignore for now, eventually I think we might be able to combine all of the powerups into one file but until then I think making them seperately to prevent merge conflicts is a good idea. 
public enum PowerUpType
{
    PlayerMoveSpeedIncrease,
    PlayerMoveBombs,
    PlayerWings,
    PlayerShield,
    PlayerBombFireLevelIncrease,
    PlayerBombCapIncrease,
    RemoteBombReplacement,
    PierceBombReplacement,
    LightningBombReplacement,
    SlideBombReplacement
}
*/
