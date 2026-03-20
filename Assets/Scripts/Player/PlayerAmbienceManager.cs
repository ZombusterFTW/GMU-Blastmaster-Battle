using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerAmbienceManager : MonoBehaviour
{
    //private string[] KCode = {"Up", "Up", "Down", "Down", "Left", "Right", "Left", "Right", "B", "A"};
    public List<int> currentCode = new List<int>();
    public List<int> KCode = new List<int>();
    private bool active = false;
    private PlayerController playerController;
    private float timer = 2f;
    private float currentTimer = 0f;
    private Coroutine powerupTimer;

    private void Start()
    {
        currentCode.Clear();
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (EscMenu.instance.easterEggsEnabled && KCode.Count == currentCode.Count && active == false && Time.timeScale > 0 && (SceneManager.GetActiveScene().buildIndex == 4 || SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 6))
        {
            bool isTheSame = true;
            for(int i = 0; i < KCode.Count - 1; i++)
            {
                if(KCode[i] != currentCode[i])
                {
                    isTheSame = false;
                    break; 
                }
            }
            if(isTheSame)
            {
                Debug.Log("KCode matches currentCode");
                active = true;
                playerController.Secret = 30;
                playerController.ActivateShieldVFX(true);
                playerController.bombCap_Game = 10;
                if (SceneManager.GetActiveScene().buildIndex != 4) playerController.bombCap_Game = 10;
                //Take powerup to player.
                if(SceneManager.GetActiveScene().buildIndex == 4) GameObject.FindObjectOfType<RoundManager>()?.StarPower(playerController.gameObject);
                currentCode.Clear();
            }

            else
            {
                Debug.Log("KCode does not match currentCode");
                currentCode.Clear();
            }

        }


    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if(active == false)
        {
            if(powerupTimer == null)
            {
                powerupTimer = StartCoroutine(enumerator());
            }
            else
            {
                currentTimer = timer;
            }
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Vector2 moveDirection = context.ReadValue<Vector2>();
                    //Debug.Log(moveDirection);
                    if (moveDirection == Vector2.up)
                    {
                        currentCode.Add(1);


                    }
                    else if (moveDirection == Vector2.down)
                    {
                        currentCode.Add(2);

                    }
                    else if (moveDirection == Vector2.left)
                    {
                        currentCode.Add(3);

                    }
                    else if (moveDirection == Vector2.right)
                    {
                        currentCode.Add(4);

                    }


                    break;
            }
        }
        
        
    }


    public void OnButton1(InputAction.CallbackContext context)
    {
        if(active == false)
        {
            if (powerupTimer == null)
            {
                powerupTimer = StartCoroutine(enumerator());
            }
            else
            {
                currentTimer = timer;
            }
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    currentCode.Add(5);
                    break;

            }
        }
        
    }
    public void OnButton2(InputAction.CallbackContext context)
    {
        if(active == false)
        {
            if (powerupTimer == null)
            {
                powerupTimer = StartCoroutine(enumerator());
            }
            else
            {
                currentTimer = timer;
            }
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    currentCode.Add(6);
                    break;

            }
        }
        


    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentCode.Clear();
        active = false;
    }

    IEnumerator enumerator()
    {
        currentTimer = timer;
        while(currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;
            yield return null;
        }
        currentCode.Clear();
        powerupTimer = null;
    }
}