using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneLine;

namespace Cards
{
    [System.Serializable]
    public struct CardPropertyData
    {
        [SerializeField]
        public int _cost;
        [SerializeField]
        public Texture _image;
        [SerializeField]
        public string _name;
        [SerializeField]
        public string _description;
        [SerializeField]
        public int _attack;
        [SerializeField]
        public int _health;
        [SerializeField]
        public CardUnitType _type;
    }

    [Serializable]
    public struct CardPropertiesData
    {
        [Width(30)]
        public uint Id;
        [NonSerialized]
        public ushort Cost;
        public string Name;
        [Width(50)]
        public Texture Texture;
        [Width(40)]
        public ushort Attack;
        [Width(40)]
        public ushort Health;
        [Width(65)]
        public CardUnitType Type;

        public CardParamsData GetParams()
        {
            return new CardParamsData(Cost, Attack, Health);
        }
    }

    public struct CardParamsData
    {
        public ushort Cost;
        public ushort Attack;
        public ushort Health;

        public CardParamsData(ushort cost, ushort attack, ushort health)
        {
            Cost = cost; Attack = attack; Health = health;
        }
    }
    public struct ClassCardsPaths
    {

    }
}
