using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIAssignerPlayer : MonoBehaviour
{
    public Canvas[] playerUICanvasesCSS;
    public TextMeshProUGUI[] cssPNames;
    public TextMeshProUGUI[] lssPNames;
    public Canvas[] playerUICanvasesLS;

    public GameObject[] firstObSelectCSS;
    public GameObject[] firstObSelectLS;
    public MultiplayerEventSystem mpEventSystem;
    private int playerID;
    private Canvas playerCanvas;
    private PlayerController owningPlayer;
    public InputSystemUIInputModule inputSystemUIInputModule;
 


    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }



    public void AssignPlayerUI(int playerIDIn, PlayerController controller)
    {
        playerID = playerIDIn;
        owningPlayer = controller;
        inputSystemUIInputModule.enabled = true;
        mpEventSystem.enabled = true;

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            Debug.Log("First Create");
            playerUICanvasesCSS[playerID - 1].gameObject.SetActive(true);
            owningPlayer.gameObject.GetComponent<PlayerInput>().uiInputModule = inputSystemUIInputModule;
            playerUICanvasesCSS[playerID - 1].gameObject.GetComponent<ButtonHolder>().EnableButtons(true);
            playerUICanvasesCSS[playerID - 1].gameObject.GetComponent<ButtonHolder>().BeginArrowBlinking();
            if (owningPlayer.playerMatchWins > 0)
            {
                playerUICanvasesCSS[playerID - 1].gameObject.GetComponent<ButtonHolder>().matchWinsTracker.GetComponentInChildren<TextMeshProUGUI>().text = "Match Wins: " + owningPlayer.playerMatchWins;
                playerUICanvasesCSS[playerID - 1].gameObject.GetComponent<ButtonHolder>().matchWinsTracker.gameObject.SetActive(true);
            }
            mpEventSystem.playerRoot = playerUICanvasesCSS[playerID - 1].gameObject;
            playerCanvas = playerUICanvasesCSS[playerID - 1];
            mpEventSystem.firstSelectedGameObject = firstObSelectCSS[playerID - 1].gameObject;
            mpEventSystem.SetSelectedGameObject(firstObSelectCSS[playerID - 1].gameObject);
            cssPNames[playerID - 1].text = "Player " + playerID;
            StartCoroutine(PlayerInputDelay());

        }
        else if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            Debug.Log("First Create");
            playerUICanvasesLS[playerID - 1].gameObject.SetActive(true);
            owningPlayer.gameObject.GetComponent<PlayerInput>().uiInputModule = inputSystemUIInputModule;
            playerUICanvasesLS[playerID - 1].gameObject.GetComponent<ButtonHolder>().EnableButtons(true);
            playerUICanvasesLS[playerID - 1].gameObject.GetComponent<ButtonHolder>().BeginArrowBlinking();
            if (owningPlayer.playerMatchWins > 0)
            {
                playerUICanvasesLS[playerID - 1].gameObject.GetComponent<ButtonHolder>().matchWinsTracker.GetComponentInChildren<TextMeshProUGUI>().text = "Match Wins: " + owningPlayer.playerMatchWins;
                playerUICanvasesLS[playerID - 1].gameObject.GetComponent<ButtonHolder>().matchWinsTracker.gameObject.SetActive(true);
            }
            mpEventSystem.playerRoot = playerUICanvasesLS[playerID - 1].gameObject;
            playerCanvas = playerUICanvasesLS[playerID - 1];
            mpEventSystem.firstSelectedGameObject = firstObSelectLS[playerID - 1].gameObject;
            mpEventSystem.SetSelectedGameObject(firstObSelectLS[playerID - 1].gameObject);
            lssPNames[playerID - 1].text = "Player " + playerID;
            StartCoroutine(PlayerInputDelay());

        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (this != null && owningPlayer != null) 
        {
            
            mpEventSystem.enabled = true;
            playerUICanvasesLS[playerID - 1].gameObject.SetActive(false);
            playerUICanvasesCSS[playerID - 1].gameObject.SetActive(false);
            inputSystemUIInputModule.enabled = true;
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                Debug.Log("new scene");
                playerUICanvasesCSS[playerID - 1].gameObject.SetActive(true);
                owningPlayer.gameObject.GetComponent<PlayerInput>().uiInputModule = inputSystemUIInputModule;
                playerUICanvasesCSS[playerID - 1].gameObject.GetComponent<ButtonHolder>().EnableButtons(true);
                playerUICanvasesCSS[playerID - 1].gameObject.GetComponent<ButtonHolder>().BeginArrowBlinking();
                if (owningPlayer.playerMatchWins > 0)
                {
                    playerUICanvasesCSS[playerID - 1].gameObject.GetComponent<ButtonHolder>().matchWinsTracker.GetComponentInChildren<TextMeshProUGUI>().text = "Match Wins: " + owningPlayer.playerMatchWins;
                    playerUICanvasesCSS[playerID - 1].gameObject.GetComponent<ButtonHolder>().matchWinsTracker.gameObject.SetActive(true);
                }
                mpEventSystem.playerRoot = playerUICanvasesCSS[playerID - 1].gameObject;
                playerCanvas = playerUICanvasesCSS[playerID - 1];
                mpEventSystem.firstSelectedGameObject = firstObSelectCSS[playerID - 1].gameObject;
                mpEventSystem.SetSelectedGameObject(firstObSelectCSS[playerID - 1].gameObject);
                cssPNames[playerID - 1].text = "Player " + playerID;
                StartCoroutine(PlayerInputDelay());
            }
            else if (SceneManager.GetActiveScene().buildIndex == 3)
            {
                Debug.Log("new scene");
                playerUICanvasesLS[playerID - 1].gameObject.SetActive(true);
                owningPlayer.gameObject.GetComponent<PlayerInput>().uiInputModule = inputSystemUIInputModule;
                playerUICanvasesLS[playerID - 1].gameObject.GetComponent<ButtonHolder>().EnableButtons(true);
                playerUICanvasesLS[playerID - 1].gameObject.GetComponent<ButtonHolder>().BeginArrowBlinking();
                if(owningPlayer.playerMatchWins > 0)
                {
                    playerUICanvasesLS[playerID - 1].gameObject.GetComponent<ButtonHolder>().matchWinsTracker.GetComponentInChildren<TextMeshProUGUI>().text = "Match Wins: " + owningPlayer.playerMatchWins;
                    playerUICanvasesLS[playerID - 1].gameObject.GetComponent<ButtonHolder>().matchWinsTracker.gameObject.SetActive(true);
                }
                mpEventSystem.playerRoot = playerUICanvasesLS[playerID - 1].gameObject;
                playerCanvas = playerUICanvasesLS[playerID - 1];
                mpEventSystem.firstSelectedGameObject = firstObSelectLS[playerID - 1].gameObject;
                mpEventSystem.SetSelectedGameObject(firstObSelectLS[playerID - 1].gameObject);
                lssPNames[playerID - 1].text = "Player " + playerID;
                StartCoroutine(PlayerInputDelay());
            }
            else
            {
                playerUICanvasesCSS[playerID - 1].gameObject.SetActive(false);
                playerUICanvasesLS[playerID - 1].gameObject.SetActive(false);
            }
            

        }
    }

    public void EnablePlayerCanvas(bool enable)
    {
        playerUICanvasesCSS[playerID - 1].gameObject.SetActive(enable);
        playerUICanvasesLS[playerID - 1].gameObject.SetActive(enable);
    }


    //needs delay before player can pick to prevent them from moving the selection early.
    IEnumerator PlayerInputDelay()
    {
        //if (playerUICanvasesCSS[playerID - 1].gameObject.activeSelf == false) yield break;
       // else if (playerUICanvasesLS[playerID - 1].gameObject.activeSelf == false) yield break;
        mpEventSystem.enabled = false;
        yield return new WaitForSecondsRealtime(0.15f);
        Debug.Log("Select Delay");
        mpEventSystem.enabled = true;
    }

    public void ForceButtonLockIn()
    {
        GameObject selectedButton = mpEventSystem.currentSelectedGameObject;
       // selectedButton.GetComponent<Button>().clicked.Invoke();
        ExecuteEvents.Execute(selectedButton, new BaseEventData(mpEventSystem), ExecuteEvents.submitHandler);
    }
   
}
