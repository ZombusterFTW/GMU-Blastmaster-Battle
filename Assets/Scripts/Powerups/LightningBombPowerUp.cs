using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class LightningBombPowerUp : MonoBehaviour
{
    private PlayerController playerController;

    public AudioSource lightningPickUp;
    public new Collider collider;
    public GameObject spriteObject;



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
        //Set player bombType to Lightning Bomb. lightning bomb logic will be in the bomb behavior script
        playerController.playerBombType = BombTypes.LightningBomb;
        Debug.Log("Power up picked up by Player " + playerID);
        playerController.UpdatePlayerUI();

        lightningPickUp.Play();
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
