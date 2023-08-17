using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public enum CardUnitType : byte
    {
        Common = 0,
        Murloc = 1,
        Beast = 2,
        Elemental = 3,
        Mech = 4
    }

    public enum SideType : byte
    {
        Common = 0,
        Mage = 11,
        Warrior = 12,
        Priest = 13,
        Hunter = 14
    }
    public enum PlayerSide : byte
    {
        One = 0,
        Two = 1
    }
    public enum PlayedEffects : byte
    {
        Charge = 0,
        Taunt = 1,
        Battlecry = 2,
        Unique = 100
    }
}
