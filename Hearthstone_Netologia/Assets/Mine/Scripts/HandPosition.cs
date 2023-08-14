using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class HandPosition : MonoBehaviour
    {
        [field: SerializeField]
        public PlayerSide Player { get; private set; }
        public Card LinkedCard { get; private set; }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}