using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireUpPowerup : MonoBehaviour
{
    private PlayerController playerController;
    public int fireLevelsToGrant = 1;
    [SerializeField]private int maxFireLevel = 6;

    public AudioSource firePickUp;
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
        if(playerController.bombFireLevel_Game < maxFireLevel)
        {
            playerController.bombFireLevel_Game += fireLevelsToGrant;
            Debug.Log("Increased player bomb fire level by " + fireLevelsToGrant);
            playerController.UpdatePlayerUI();
        }
        else
        {
            Debug.Log("Bomb fire level cap hit");
        }

        Debug.Log("Fire Up Power up picked up by Player " + playerID);
        
        firePickUp.Play();
        Debug.Log("Pick up");

        collider.enabled = false;
        spriteObject.SetActive(false);
        Destroy(gameObject, 5f);
        if(GetComponent<PowerupJumper>() != null) GetComponent<PowerupJumper>().HoverCleanUp();
        //Destroy(gameObject);
    }

    private void OnDisable()
    {
        if (GetComponent<PowerupJumper>() != null) GetComponent<PowerupJumper>().HoverCleanUp();
    }



}
