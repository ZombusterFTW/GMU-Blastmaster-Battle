using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class SkatesPowerUp : MonoBehaviour
{
    [Tooltip("How much to multiply speed by.")][SerializeField] float speedBuff = 2f;
    [Tooltip("If the player's speed is equal to their default speed times this number, speed caps.")][SerializeField] float speedCap = 5;
    private PlayerController playerController;

    public AudioSource skatePickUp;
    public new Collider collider;
    public GameObject spriteObject;


    //With its current setup the player can grab up to five skates before their movement speed caps.

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
        //Goes into PlayerController script and finds the float "moveSpeed_Game" adds the player controller moveSpeed times speedBuff to the player's speed.
        //Cap logic for later. if the player's move speed is equal to their default speed times this value no more effect will be given to them.

        if (playerController.moveSpeed_Game * speedBuff < playerController.moveSpeed * speedCap)
        {
            playerController.moveSpeed_Game *= speedBuff;
        }
        else Debug.Log("Speed cap hit");
        playerController.UpdatePlayerUI();




        Debug.Log("Skates Powerup picked up by Player " + playerID);

        skatePickUp.Play();
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
