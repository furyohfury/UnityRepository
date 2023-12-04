using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGMine
{
    public class PlayerUnit : Unit
    {
        private PlayerInput _playerInput;
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }
    }
}