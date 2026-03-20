using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchCrownManager : MonoBehaviour
{
    [SerializeField] GameObject playerCrown;


    public void ToggleCrown(bool isVisible)
    {
        playerCrown.SetActive(isVisible);
    }


}
