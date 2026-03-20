using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerUp : MonoBehaviour
{
    private PlayerController playerController;

    public AudioSource shieldPickUp;
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

        void PickUp(int playerID)
        {
            if (playerController.hasShield == false)
            {
                playerController.hasShield = true;
                Debug.Log("Shield given to Player " + playerID);
                playerController.ActivateShieldVFX(true);
                playerController.UpdatePlayerUI();
            }
            else
            {
                Debug.Log("Shield cap hit");
            }

            shieldPickUp.Play();
            Debug.Log("pick up");

            collider.enabled = false;
            spriteObject.SetActive(false);
            Destroy(gameObject, 5f);
            if (GetComponent<PowerupJumper>() != null) GetComponent<PowerupJumper>().HoverCleanUp();
            //Destroy(gameObject);
        }
    }
        private void OnDisable()
        {
            if (GetComponent<PowerupJumper>() != null) GetComponent<PowerupJumper>().HoverCleanUp();
        }

    }
