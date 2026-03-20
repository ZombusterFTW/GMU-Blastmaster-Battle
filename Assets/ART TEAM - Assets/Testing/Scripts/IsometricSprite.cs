using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricSprite : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Toggles on billboarding.")]
    private bool isometricOn;


    void Update()
    {
        if (isometricOn)
        {
            Vector3 pos = transform.position;
            pos.z = pos.y;
            transform.position = pos;
        }

    }
}
