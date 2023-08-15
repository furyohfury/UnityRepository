using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class PlayerPortrait : MonoBehaviour
    {
        [field: SerializeField]
        public SideType Hero { get; private set; }
        [field: SerializeField]
        public PlayerSide Player { get; private set; }
        [field : SerializeField]
        public int Health { get; private set; } = 10;
        // Start is called before the first frame update
        void Start()
        {
            Health = 10;
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void ChangePlayerHealth(int delta)
        {
            Health += delta;
        }
    }
}