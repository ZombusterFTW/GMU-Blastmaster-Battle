using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboarder : MonoBehaviour, Billboardable
{
    /* Main Variables */

    [SerializeField]
    [Tooltip("The tag of the scene's main camera. Targets said camera for the sake of billboarding.")]
    private string cameraTag;
    [SerializeField]
    [Tooltip("A boolean which toggles billboarding on or off for the sake of use in other scripts or testing should the need arise.")]
    public bool canBillboard;
    [SerializeField]
    [Tooltip("A boolean which toggles the locking of X axis rotation. Stops sprites from looking at the camera but still allows them to be angled upwards at the camera's angle.")]
    public bool lockRotationX;
    [SerializeField]
    [Tooltip("A boolean which toggles the locking of Y axis rotation. Stops sprites from looking at the camera but still allows them to be angled upwards at the camera's angle.")]
    public bool lockRotationY;
    [SerializeField]
    [Tooltip("A boolean which toggles the locking of Z axis rotation. Stops sprites from looking at the camera but still allows them to be angled upwards at the camera's angle.")]
    public bool lockRotationZ;

    /* Private Variables */

    private GameObject cam;

    void Start()
    {
        cam = GameObject.FindWithTag(cameraTag);
    }

    private void LateUpdate()
    {
        if (canBillboard)
        {

            var target = cam.transform.position;
            //target.y = transform.position.y;
            if (lockRotationX)
            {
                target.x = 0;
            }
            if (lockRotationY)
            {
                target.y = 0;
            }
            if (lockRotationZ)
            {
                target.z = 0;
            }
            transform.LookAt(target);
            transform.Rotate(0, 180, 0);
        }// If billboarding is set to active.
    }

    public void ReassignCameraTarget(Camera camera)
    {
        cam = camera.gameObject;
    }
}
