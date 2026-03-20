using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableBlock : MonoBehaviour
{
    public GameObject destroyedVersion;//Add Destroyed Block ie. Explosion, Crumble, etc.

    private bool isDestroyed = false;

    public int DropChanceDenominator = 4;//Determins Drop Rate Chance

    BombermanGameManager gameManager;//Access GameManger Script

    public AudioSource blockAudioSource; //Audio Source added to object in script section TE
    public AudioClip destroyBlock;

    private void OnCollisionEnter(Collision collision)
    {
        if (isDestroyed) return;

        if (collision.gameObject.CompareTag("Explosion"))// Detects Contact with Explosion
        {
            DestroyBlock();
        }
    }

    public void DestroyBlock(bool destroyedByPressure = false, bool manuallyDestroyed = false)
    {
        isDestroyed = true;
        System.Random random = new System.Random();
        int randomNumber = random.Next(1, DropChanceDenominator + 1);//Random Number Generator

        if (randomNumber == 1 && !destroyedByPressure)
        {
            DropItem();
        }

        GameObject destroyedBlock = Instantiate(destroyedVersion, transform.position, transform.rotation);//Calls Destroyed Block Asset
        if(manuallyDestroyed) destroyedBlock.GetComponent<AudioSource>().clip = null;

        Destroy(gameObject);//Destroys Block

        //PlayAudioOnDestroy();
    }
    
    private void DropItem()
    {
        gameManager = FindObjectOfType<BombermanGameManager>();
        if (gameManager.SpawnUntrackedItem(transform.position) != null)
        {
            // works with above line so its causing the problem as well
            Debug.Log("Item dropped!");
        }
        else
        {
            Debug.Log("Loot table is empty.");
        }
    }



}
