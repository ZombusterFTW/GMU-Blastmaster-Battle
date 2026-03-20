using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombUpPowerup : MonoBehaviour
{
    private PlayerController playerController;
    private BombBehavior bombBehavior;
    public int bombsToAdd = 1;
    public int maxBombsAllowed = 6;

    public AudioSource bombPickUp;
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
        if(playerController.bombCap_Game < maxBombsAllowed)
        {
            playerController.bombCap_Game += bombsToAdd;
            Debug.Log("Added 1 bomb to player");
            playerController.UpdatePlayerUI();
        }
        else
        {
            Debug.Log("Bomb cap hit");
        }

        Debug.Log("BombUp Power up picked up by Player " + playerID);

        bombPickUp.Play();
        Debug.Log("pick up");

        collider.enabled = false;
        spriteObject.SetActive(false);
        Destroy(gameObject, 5f);
        //Run cleanup if the powerup has a powerup jumper script
        if (GetComponent<PowerupJumper>() != null) GetComponent<PowerupJumper>().HoverCleanUp();

        //Destroy(gameObject);
    }

    private void OnDisable()
    {
        if (GetComponent<PowerupJumper>() != null) GetComponent<PowerupJumper>().HoverCleanUp();
    }



}
