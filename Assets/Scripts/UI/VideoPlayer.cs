using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    //This array will be in order of the vids etc, htp, controls, meet the bombs.
    [SerializeField] VideoClip[] tutorialVideos;
    UnityEngine.Video.VideoPlayer videoPlayer;
    [SerializeField] string[] videoTitles;
    [SerializeField] TextMeshProUGUI videoTitleText;
    [SerializeField] private int videoIndex = 0;
    private AudioSource videoPlayerAudioSource;
    [SerializeField] Image progressBar;
    [SerializeField] AudioMixerSnapshot startMenu;
    [SerializeField] AudioMixerSnapshot videoPlaying;
    private Coroutine videoWaitToLoad;

    void Start()
    {
        videoPlayerAudioSource = GetComponent<AudioSource>();
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        //Set to first video by default
        videoPlayer.clip = tutorialVideos[0];
        videoTitleText.text = videoTitles[0] + "/<color=\"yellow\">Paused";
        videoPlayer.Prepare();
        videoWaitToLoad = StartCoroutine(WaitForVideoToFullyLoadOnStart());
    }


    private void Update()
    {
        if (!videoPlayer.isPaused) progressBar.fillAmount = (float)(videoPlayer.time/tutorialVideos[videoIndex].length);
        //Debug.Log((float)(videoPlayer.time / tutorialVideos[videoIndex].length));
    }


    private void UpdateText(UnityEngine.Video.VideoPlayer source)
    {
        videoPlayerAudioSource.Play();
        videoTitleText.text = videoTitles[videoIndex] + "/<color=\"yellow\">Finished";
        progressBar.fillAmount = 1;
        startMenu.TransitionTo(0.5f);
    }


    public void PlaySelectedVideo(int videoIndexIn)
    {
        videoPlayerAudioSource.Play();
        videoIndex = videoIndexIn;
        if (videoPlayer.clip != tutorialVideos[videoIndex] && videoWaitToLoad == null)
        {
            //We get here if the player wants to play a different video
            videoPlayer.Stop();
            videoPlayer.clip = tutorialVideos[videoIndex];
            videoPlayer.Prepare();
            videoWaitToLoad = StartCoroutine(WaitForVideoToFullyLoad());
        }
        else if (videoPlayer.clip == tutorialVideos[videoIndex] && (ulong)videoPlayer.frame == tutorialVideos[videoIndex].frameCount && videoWaitToLoad == null)
        {
            //We get here if the player has pressed play on the same video that has just finished playing
            videoPlayer.Prepare();
            videoWaitToLoad = StartCoroutine(WaitForVideoToFullyLoad());
        }
        else if(videoPlayer.clip == tutorialVideos[videoIndex] && (ulong)videoPlayer.frame < tutorialVideos[videoIndex].frameCount && videoWaitToLoad == null)
        {
            //We get here if the Player wants to pause or play video
            if (videoPlayer.isPaused) 
            {
                videoPlayer.Prepare();
                videoWaitToLoad = StartCoroutine(WaitForVideoToFullyLoad());
            }
            else if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
            }
        }
        if(!videoPlayer.isPaused)
        {
            videoTitleText.text =  videoTitles[videoIndex] + "/<color=\"green\">Playing";
            videoPlaying.TransitionTo(0.5f);
        }
        else
        {
            videoTitleText.text =  videoTitles[videoIndex] + "/<color=\"red\">Paused";
            startMenu.TransitionTo(0.5f);
        }
    }

    IEnumerator WaitForVideoToFullyLoad()
    {
        while(videoPlayer.isPrepared == false) 
        {
            //Debug.Log("preparing video");
            yield return null;
        }
        videoPlayer.Play();
        videoWaitToLoad = null;
    }

    IEnumerator WaitForVideoToFullyLoadOnStart()
    {
        while (videoPlayer.isPrepared == false)
        {
            //Debug.Log("preparing video");
            yield return null;
        }
        videoPlayer.Play();
        videoPlayer.Pause();
        videoPlayer.loopPointReached += UpdateText;
        videoWaitToLoad = null;
    }
}
