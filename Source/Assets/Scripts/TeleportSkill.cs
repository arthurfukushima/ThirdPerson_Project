using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportSkill : MonoBehaviour
{
    private PlayerInput _playerInput;
    private StarterAssetsInputs _input;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }
}
