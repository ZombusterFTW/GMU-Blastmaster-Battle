using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolicMover : MonoBehaviour
{
    Vector3 startPos;
    public Vector3 endPos;
    bool landed = false;
    PowerupJumper jumpa;
    //This function will move a gifted powerup towards the recipient player
    void Start()
    {
        startPos = transform.position;
        transform.DOLocalJump(endPos, 10f, 1, 3f);
        jumpa = gameObject.AddComponent<PowerupJumper>();
    }

    private void Update()
    {
        if(transform.position == endPos && !landed) 
        {
            landed = true;
            jumpa.enabled = true;
        }
    }



    public void ParbolicMoverCleanup()
    {
        transform.DOComplete();
    }


}
