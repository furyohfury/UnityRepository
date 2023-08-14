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
        /* public static CardPropertiesData ConvertToProperties(CardPropertyData data)
        {
            CardPropertiesData propData = new();
            propData.Cost = (ushort) data._cost;
            propData.Texture = data._image;
            propData.Name = data._name;
            propData.Attack = (ushort)data._attack;
            propData.Health = (ushort)data._health;
            propData.Type = data._type;
            return propData;
        } */
        
    }

    [Serializable]
    public struct CardPropertiesData
    {
        [Width(30)]
        public uint Id;
        [Width(20)]
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
    public struct CardsData
    {
        public CardPropertiesData _commonCardPacks;
        public CardPropertiesData _classCardPacks;
    }
}
