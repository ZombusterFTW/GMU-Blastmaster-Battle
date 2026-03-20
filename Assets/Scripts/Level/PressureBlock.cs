using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CinemachineImpulseSource))]
[RequireComponent(typeof(AudioSource))]
public class PressureBlock : MonoBehaviour
{
    public Vector3 targetPosition = Vector3.zero;
    public float blockSpeed = 100f;
    private bool isLanded = false;
    private CinemachineImpulseSource impusler;
    float timeToland = 0f;
    private AudioSource audioSource;
    public AudioClip[] blockFallClips;

    public float pitchMax = 1.2f;
    public float pitchMin = 0.8f;
    private bool isActive = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        impusler = GetComponent<CinemachineImpulseSource>();
        audioSource.clip = blockFallClips[Random.Range(0, blockFallClips.Length)];
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
    }


    public void ActivateBlock() 
    {
        audioSource.Play();
        isActive = true;
    }

    public bool IsLanded()
    {
        return isLanded;
    }

    void Update()
    {
        if(isActive)
        {
            if (transform.position != targetPosition)
            {
                timeToland += Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, blockSpeed * Time.deltaTime);

            }
            if (transform.position == targetPosition && isLanded == false)
            {
                //Debug.Log(timeToland);
                Landing();
                isLanded = true;
                impusler.GenerateImpulseAtPositionWithVelocity(transform.position, Vector3.one);
            }
        }
    }

    
    private void Landing()
    {
        RaycastHit[] hitColliders = Physics.BoxCastAll(new Vector3(targetPosition.x, 0f, targetPosition.z), Vector3.one * 5 / 2.5f, Vector3.up, Quaternion.identity, 2.5f);
        foreach (RaycastHit hit in hitColliders)
        {
            if (hit.collider.gameObject.GetComponent<PlayerController>() != null)
            {
                hit.collider.gameObject.GetComponent<PlayerController>().EliminatePlayer("Crushed by block");
            }
            else if (hit.collider.gameObject.GetComponent<DestroyableBlock>() != null)
            {
                hit.collider.gameObject.GetComponent<DestroyableBlock>().DestroyBlock(true);
            }
            //added this rq.
            else if (hit.collider.gameObject.tag == "PowerUp")
            {
                Destroy(hit.collider.gameObject);
            }
            else if (hit.collider.gameObject.GetComponent<BombBehavior>() != null)
            {
                //Bomb explode after crush by block
                //hit.collider.gameObject.GetComponent<BombBehavior>().Explode();
                Destroy(hit.collider.gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //In case player some how gets under a block after it has done its landing checks
        if (collision.gameObject.GetComponent<PlayerController>() != null && isLanded)
        {
            collision.gameObject.GetComponent<PlayerController>().EliminatePlayer("Crushed by block");
        }
    }
}
