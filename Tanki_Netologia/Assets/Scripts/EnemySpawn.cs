using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
    public class EnemySpawn : MonoBehaviour 
    {
        public bool isBusy = false;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Tank _)) isBusy = true;
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out Tank _)) isBusy = false;
        }
    }
}