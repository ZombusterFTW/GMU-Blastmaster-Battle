using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDestroyedSound : MonoBehaviour
{
    public AudioSource randomPitchAudio;
    public float minPitch = 0.7f;
    public float maxPitch = 1.5f;

    private void Awake()
    {
        PlayRandomPitch();
    }

    private void PlayRandomPitch()
    {
        float randomPitch = Random.Range(minPitch, maxPitch);
        randomPitchAudio.pitch = randomPitch;
        randomPitchAudio.Play();
    }
}
