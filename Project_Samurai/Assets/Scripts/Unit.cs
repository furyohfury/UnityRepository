using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Samurai
{
    public abstract class Unit: MonoBehaviour
    {
        protected UnitStatsStruct UnitStats {get; private set;}

        public UnitStats GetUnitStats()
        {
            return UnitStats;
        }
    }
}