using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public class FreeCameraBMB : MonoBehaviour
{
    public float sensitivity;
    public float slowSpeed, normalSpeed, sprintSpeed;
    float currentSpeed;

    [SerializeField] private Camera m_Camera;
    [SerializeField] private PlayerInput m_PlayerInput;



    private Vector2 cameraRotationVector;
    private Vector2 cameraMovementVector;
    private Vector2 cameraUpDownVector;


    private readonly int cameraMaxFOV = 120;
    private readonly int cameraMinFOV = 10;
    private int currentCameraFOV = 85;

    public static FreeCameraBMB instance;

    private bool fovadjustinprogress =false;
    private bool sprintPressed = false;
    private Coroutine fovCoroutine;

    private void Awake()
    {

        if(FreeCameraBMB.instance == null)
        {
            FreeCameraBMB.instance = this;
        }
        else Destroy(gameObject);


        if (m_PlayerInput.devices.Count != 0 && m_PlayerInput.devices[0] is not UnityEngine.InputSystem.XInput.XInputController)
        {
            Debug.Log("Invalid input for freecam");
            Destroy(gameObject);
        }

      
        
    }

    private void Start()
    {
        foreach (Billboardable billboarder in GameObject.FindObjectsOfType<MonoBehaviour>().OfType<Billboardable>())
        {
            billboarder.ReassignCameraTarget(m_Camera);
        }
        m_Camera.fieldOfView = currentCameraFOV;
    }


    private void Update()
    {
        Rotation();
        Movement();
    }


    private void Rotation()
    {
        Vector2 lookInput = new Vector3(cameraRotationVector.y, cameraRotationVector.x, 0);
        transform.Rotate(lookInput * sensitivity * Time.unscaledDeltaTime * 50);
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0);
    }

    private void Movement()
    {
        Vector3 input = new Vector3(cameraMovementVector.x, cameraUpDownVector.y, cameraMovementVector.y);
        if (!sprintPressed) currentSpeed = normalSpeed;
        else currentSpeed = sprintSpeed;
        transform.Translate(input * currentSpeed * Time.unscaledDeltaTime);
    }

   

  

    public void OnCameraRotation(InputAction.CallbackContext context)
    {
        cameraRotationVector = context.ReadValue<Vector2>();
        //Debug.Log("Camera look around");
    }

    public void OnCameraMovement(InputAction.CallbackContext context)
    {
        cameraMovementVector = context.ReadValue<Vector2>();
        //Debug.Log("Camera Move Flat");
    }

    public void OnCameraUpDownMovement(InputAction.CallbackContext context) 
    {
        cameraUpDownVector = context.ReadValue<Vector2>();
        //Debug.Log("Camera Look Up or Down");
    }

    public void OnCameraFOVUP(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Started)
        {
            fovadjustinprogress = true;
            if (fovCoroutine != null) StopCoroutine(fovCoroutine);
            fovCoroutine = StartCoroutine(FOVUpDownLoop(true));
            //Debug.Log("Camera FOV UP");
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            if (fovCoroutine != null) StopCoroutine(fovCoroutine);
            fovadjustinprogress = false;
        }
    }

    public void OnCameraFOVDown(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Started)
        {
            fovadjustinprogress = true;
            if (fovCoroutine != null) StopCoroutine(fovCoroutine);
            fovCoroutine = StartCoroutine(FOVUpDownLoop(false));
           // Debug.Log("Camera FOV Down");
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            if(fovCoroutine != null) StopCoroutine(fovCoroutine);
            fovadjustinprogress = false;
        }   
    }


    public void OnSprintPressed(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            sprintPressed = true;
        }
        if(context.phase == InputActionPhase.Canceled)
        {
            sprintPressed = false;
        }
    }



   void OnDestroy()
    {
       
    }




    IEnumerator FOVUpDownLoop(bool up)
    {
        while(fovadjustinprogress) 
        {
            if (up)
            {
                if (currentCameraFOV < cameraMaxFOV) currentCameraFOV++;
            }
            else
            {
                if (currentCameraFOV > cameraMinFOV) currentCameraFOV--;
            }
            m_Camera.fieldOfView = currentCameraFOV;
            yield return new WaitForSecondsRealtime(0.15f);
        }
        fovCoroutine = null;
    }
}
