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
    public struct Charge
    {
        public static readonly string[] ChargeCards = { "Stonetusk Boar", "Bluegill Warrior", "Wolfrider", "Stormwind Knight", "Reckless Rocketeer", "Kor'kron Elite" };
    }
    public struct Taunt
    {
        public static readonly string[] TauntCards = { "Goldshire Footman", "Frostwolf Grunt", "Ironfur Grizzly", "Silverback Patriarch", "Sen'jin Shieldmasta", "Booty Bay Bodyguard", "Lord of the Arena" };
    }
    public struct BattlecryList
    {
        public static readonly string[] BattlecryCards = { "Elven Archer", "Voodoo Doctor", "Acidic Swamp Ooze", "Murloc Tidehunter", "Novice Engineer", "Ironforge Rifleman", "Razorfen Hunter", "Shattered Sun Cleric", "Dragonling Mechanic", "Gnomish Inventor", "Darkscale Healer", "Frostwolf Warlord", "Nightblade", "Stormpike Commando" };
    }
    public struct PassiveList
    {
        public static readonly string[] PassiveEfectCards = { "Raid Leader" , "Gurubashi Berserer", "Stormwind Champion" };
    }
}
