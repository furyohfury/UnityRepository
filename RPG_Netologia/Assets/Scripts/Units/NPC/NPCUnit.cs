using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Units.NPC
{
    public class NPCUnit : Unit
    {
        protected override void OnRotate()
        {
            
        }

        protected override void Start()
        {
            base.Start();
            Stats.Name = gameObject.name;

        }
        protected override void Update()
        {

        }
    }
}