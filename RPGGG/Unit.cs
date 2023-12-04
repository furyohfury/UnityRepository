using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPGMine
{
    public abstract class Unit
    {
        protected UnitStats _unitStats;

        public UnitStats GetUnitStats()
        {
            return _unitStats;
        }
    }
}