

//#define WINNITRON_BUILD
#define NORMAL_BUILD
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Windows;
using static KeyboardSplitter;

public class FirstJoinPrivelege : MonoBehaviour
{
    // Start is called before the first frame update

    private bool firstJoined = false;
    public string playerInControl;
    public PlayerController playerController;
    public Canvas canvas;
    public GameObject firstSelected;
    public Canvas quitCanvas;
    public GameObject secondSelected;
    public TextMeshProUGUI whoHasControl;
    private GameObject selected;
    private Canvas currentCanvas;
    private PlayerController[] loadedPlayers;
    public TextMeshProUGUI buildIdentifier;
    public string normalBuildText;
    public string winnitronBuildText;
    public TextMeshProUGUI buildNumber;

    //Ignore this function
    public void PlayerFirstJoin(GameObject player)
    {
        if (!firstJoined)
        {
            firstJoined = true;
            playerController = player.GetComponent<PlayerController>();
            playerInControl = player.GetComponent<PlayerController>().GetPlayerID().ToString();
            player.GetComponent<PlayerController>().playerUIAssigner.mpEventSystem.playerRoot = canvas.gameObject;
            player.GetComponent<PlayerController>().playerUIAssigner.mpEventSystem.firstSelectedGameObject = firstSelected;
        }
        else Debug.Log("Player " + playerInControl + " has menu control");
    }

    private void Start()
    {
        if(buildNumber != null)
        {
            buildNumber.text = "Vers: " + Application.version;
        }
        loadedPlayers = GameObject.FindObjectsOfType<PlayerController>();
        selected = firstSelected;
        currentCanvas = canvas;
#if NORMAL_BUILD
        if(buildIdentifier != null) buildIdentifier.text = normalBuildText;
#endif
#if WINNITRON_BUILD
        if (buildIdentifier != null) buildIdentifier.text = winnitronBuildText;
#endif
    }

    private void Update()
    {
        loadedPlayers = GameObject.FindObjectsOfType<PlayerController>();
        //Check if the controlling player becomes null
        if (playerController == null) 
        {
            //if they do a player left and we need to find another player to control the menu
            firstJoined = false;
            //We attempt to find another controlling player. 
            if (GameObject.FindGameObjectWithTag("Player") != null) playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            //if we fail we update the widget text
            else
            {
               // Debug.Log("No players");
                whoHasControl.text = "No players detected";
            }
        }
        if (playerController != null && !firstJoined)
        {
            //if we are successful we set first joined to true to prevent running on every frame
            firstJoined = true;
            //We grab their player id
            playerInControl = playerController.GetPlayerID().ToString();
            //Update the widget. In non-winnitron builds the widget will also note the player's input device

            Debug.Log("Player " + playerInControl + playerController.GetComponent<PlayerInput>().devices[0] + " has menu control | " + loadedPlayers.Length + " Players Detected");
            //Make their event system select the button
            //Add delay so player stays on the play button
            StartCoroutine(MenuStartDelay());
        }
        if(playerController != null)
        {
            string deviceName = string.Empty;
            if (playerController.GetComponent<PlayerInput>().enabled)
            {
                deviceName = playerController.GetComponent<PlayerInput>().devices[0].ToString();
            }


#if NORMAL_BUILD
            whoHasControl.text = "Player " + playerInControl + " (" + deviceName + ") has menu control | " + loadedPlayers.Length + " Players Detected";
#endif
#if WINNITRON_BUILD
            whoHasControl.text = "Player " + playerInControl + " has menu control" + " | " + loadedPlayers.Length + " Players Detected";
#endif
        }
    }

    //Allow for manual override of first joined player to allow the player who pressed pause to be the controlling player in this circumstance.
    public void OverwriteControllingPlayer(PlayerController playerControllerIn)
    {
        if (playerControllerIn == null) return;
#if NORMAL_BUILD
        playerController = playerControllerIn;
        playerInControl = playerController.GetPlayerID().ToString();
        StartCoroutine(MenuStartDelay());
        //whoHasControl.text = "Player " + playerInControl + " (" + playerController.GetComponent<PlayerInput>().devices[0] + ") has menu control | " + loadedPlayers.Length + " Players Detected";
#endif
#if WINNITRON_BUILD
        return;
#endif

    }

    IEnumerator MenuStartDelay()
    {

        Debug.Log("select delay");
        yield return new WaitForSecondsRealtime(0.15f);
        playerController.playerUIAssigner.mpEventSystem.playerRoot = currentCanvas.gameObject;
        playerController.playerUIAssigner.mpEventSystem.SetSelectedGameObject(selected);
    }




    public void EnableQuitCanvas(bool enable)
    {
        if(quitCanvas == null) return;
        //If the player hits escape on the quit menu we quit anyway
        if(quitCanvas.enabled && enable) Application.Quit();
        quitCanvas.enabled = enable;

        if(enable)
        {
            selected = secondSelected;
            currentCanvas = quitCanvas;
        }
        else
        {
            selected = firstSelected;
            currentCanvas = canvas;
            if (GameObject.FindObjectOfType<EscMenu>() != null) GameObject.FindObjectOfType<EscMenu>().SetIsTransitioning(false);
        }
        if(playerController != null)
        {
            playerController.playerUIAssigner.mpEventSystem.playerRoot = currentCanvas.gameObject;
            playerController.playerUIAssigner.mpEventSystem.SetSelectedGameObject(selected);
        }
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.playerUIAssigner.mpEventSystem.playerRoot = null;
            playerController.playerUIAssigner.mpEventSystem.SetSelectedGameObject(null);
            playerController = null;
        }
    }

    private void OnEnable()
    {
        
    }

}
