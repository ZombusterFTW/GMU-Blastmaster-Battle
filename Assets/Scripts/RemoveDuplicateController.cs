using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RemoveDuplicateController : MonoBehaviour
{
    [SerializeField] private PlayerInputManager _playerInputManager;

    private void Start()
    {
        _playerInputManager.notificationBehavior = PlayerNotifications.InvokeCSharpEvents; // Make sure it invokes c# events
        _playerInputManager.onPlayerJoined += _playerInputManager_onPlayerJoined;
    }

    private void _playerInputManager_onPlayerJoined(PlayerInput obj)
    {
        PlayerInput_onControlsChanged(obj);
        obj.onControlsChanged += PlayerInput_onControlsChanged;
    }

    /// <summary>
    /// Ugly workaround to prevent input being copied with an xinput controller
    /// </summary>
    private void PlayerInput_onControlsChanged(PlayerInput obj)
    {
        // Disable all XInput Controllers
        foreach (var item in Gamepad.all)
        {
            if ((item is UnityEngine.InputSystem.XInput.XInputController) && (Math.Abs(item.lastUpdateTime - item.lastUpdateTime) < 0.1))
            {
                //Debug.Log($"XInput was detected and deactived. Disabling XInput device. `{item}`");
                InputSystem.DisableDevice(item);
            }
        }
        Destroy(obj.gameObject);    
    }
}
