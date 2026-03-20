using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrownBillboarder : MonoBehaviour
{
    [SerializeField] GameObject playerCrown;


    private void Start()
    {
        //playerCrown.SetActive(false);
    }

    public void ToggleCrown(bool enabled)
    {
        if (enabled) 
        {
            playerCrown.SetActive(true);
        }
        else
        {
            playerCrown.SetActive(false);
        }
    }
}
