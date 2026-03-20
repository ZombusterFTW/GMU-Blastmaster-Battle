using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementToggle : MonoBehaviour
{
    private bool isTileBasedMovement = false;
    public bool IsTileBasedMovement => isTileBasedMovement;

    public delegate void ToggleMovementModeEvent(bool isTileBasedMovement);
    public event ToggleMovementModeEvent OnToggleMovementMode;

    public void ToggleMovementMode()
    {
        isTileBasedMovement = !isTileBasedMovement;
        OnToggleMovementMode?.Invoke(isTileBasedMovement);
    }
}
