using GMUBMB.Utilities.DevConsole.Commands;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

namespace GMUBMB.Utilities.DevConsole
{
    public class DeveloperConsoleBehavior : MonoBehaviour
    {
        [SerializeField] private string prefix = string.Empty;
        [SerializeField] private ConsoleCommand[] commands = new ConsoleCommand[0];

        [Header("UI")]
        [SerializeField] private GameObject uiCanvas = null;
        [SerializeField] private TMP_InputField inputField = null;
        [SerializeField] private PlayerInputManager BMBInputManager = null;
        [SerializeField] private ScrollRect scrollRect = null;
        [SerializeField] private TextMeshProUGUI consoleText = null;
        public bool consoleActive { get; private set; } = false;


        private float pausedTimeScale;

        public static DeveloperConsoleBehavior instance;

        private DevConsole developerConsole;


        public PlayerInputManager gameInputManager;
        public PlayerInputManager devCamInputManager;

        private DevConsole DeveloperConsole
        {
            get
            {
                if (developerConsole != null) { return developerConsole; }
                return developerConsole = new DevConsole(prefix, commands);
            }
        }


        private void Awake()
        {
            if (instance != null && instance != this)
            {
                //Destroy(gameObject);
                return;
            }
            instance = this;

            SceneManager.activeSceneChanged += ActiveSceneChanged;





           // DontDestroyOnLoad(gameObject);
        }

        private void ActiveSceneChanged(Scene arg0, Scene arg1)
        {
            if(GameObject.FindObjectOfType<FreeCameraBMB>() != null)
            {
                Destroy(GameObject.FindObjectOfType<FreeCameraBMB>().gameObject);
            }
            devCamInputManager.DisableJoining();
            gameInputManager.EnableJoining();
        }


        private void Update()
        {
            //Project uses new and old input systems.
            if(Input.GetKeyDown(KeyCode.Home)) 
            {
                if(uiCanvas.activeSelf)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    Time.timeScale = pausedTimeScale;
                    foreach(AudioSource audioSource in FindObjectsOfType<AudioSource>())
                    {
                        audioSource.pitch = Time.timeScale;
                    }
                    uiCanvas.SetActive(false);
                    BMBInputManager.EnableJoining();
                    foreach (PlayerController player in GameObject.FindObjectsOfType<PlayerController>())
                    {
                        player.playerInput.ActivateInput();
                    }
                    inputField.text = string.Empty;
                    inputField.DeactivateInputField();
                    consoleActive = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    pausedTimeScale = Time.timeScale;
                    Time.timeScale = 0;
                    foreach (AudioSource audioSource in FindObjectsOfType<AudioSource>())
                    {
                        audioSource.pitch = Time.timeScale;
                    }
                    uiCanvas.SetActive(true);
                    inputField.ActivateInputField();
                    BMBInputManager.DisableJoining();
                    foreach(PlayerController player in GameObject.FindObjectsOfType<PlayerController>()) 
                    {
                        player.playerInput.DeactivateInput();
                    }
                    consoleActive = true;
                }
            }
        }

        public void ProcessCommand()
        {
            if (!DeveloperConsole.ProcessCommand(inputField.text) && inputField.text != string.Empty) AddMessageToConsole(inputField.text + " is an invalid command");
            inputField.text = string.Empty;
        }

        public void SetSavedTimeScale(float inTimeScale)
        {
            pausedTimeScale = inTimeScale;  
            EscMenu.instance.SetSavedTimeScale(inTimeScale);
        }


        public void AddMessageToConsole(string message)
        {
            if(message != string.Empty)
            {
                if (consoleText.text == string.Empty) consoleText.text += message;
                else consoleText.text += ("\n" + message);
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }

        public void ClearDevConsole()
        {
            StartCoroutine(WaitToClear());
        }

        IEnumerator WaitToClear()
        {
            yield return null;
            consoleText.text = string.Empty;
        }
    }
}
