using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static KeyboardSplitter;

public class PlayerElimCloneBillboarder : MonoBehaviour, Billboardable
{
    public PlayerController player;
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
   

    // Update is called once per frame
    void Update()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        gameObject.transform.transform.rotation = mainCamera.transform.rotation;
    }
}
