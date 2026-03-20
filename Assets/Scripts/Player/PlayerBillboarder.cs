using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static KeyboardSplitter;

public class PlayerBillboarder : MonoBehaviour, Billboardable
{
    public PlayerController player;
    private GameObject playerAnimationObject;
    private Camera mainCamera;

    public void ReassignCameraTarget(Camera camera)
    {
        mainCamera = camera;
    }

    private void Awake()
    {
        mainCamera = Camera.main;
    }
    // Start is called before the first frame update
    void Start()
    {
        playerAnimationObject = player.GetPlayerAnimObject();
    }

    // Update is called once per frame
    void Update()
    {
        if(mainCamera == null) mainCamera = Camera.main;
        if (playerAnimationObject != null) playerAnimationObject.transform.transform.rotation = mainCamera.transform.rotation;
        else playerAnimationObject = player.GetPlayerAnimObject();
    }

    public Camera ReturnCurrentCamTarget()
    {
        return mainCamera;
    }
    
}

public interface Billboardable
{
    public void ReassignCameraTarget(Camera camera);
}
