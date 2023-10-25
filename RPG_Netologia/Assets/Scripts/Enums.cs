using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    public enum SideType : byte
    {
        None = 0,
        Friendly = 1,
        Enemy = 2
    }
    public enum WeaponType : byte
    {
        None = 0,
        SwordAndShield = 1,
        Bow = 2,
        TwoHandedSword = 3,
        Mage = 4
    }
}