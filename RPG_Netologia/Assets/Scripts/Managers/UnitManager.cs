using RPG.Units;
using RPG.Units.NPC;
using RPG.Units.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace RPG.Managers
{
    public class UnitManager : MonoBehaviour
    {
        [Inject]
        public PlayerUnit GetPlayer { get; private set; }
        public IReadOnlyCollection<NPCUnit> GetNPCs => _bots;

        private LinkedList<NPCUnit> _bots = new();

        [SerializeField]
        private Transform _npcPool;
        private void Start()
        {
            var units = _npcPool.GetComponentsInChildren<Unit>();

            foreach (var npc in units)
            {
                var bot = npc as NPCUnit;
                if (bot == null) Debug.LogError($"Unit " + npc.gameObject.name + " isn't a NPC");

                foreach (NPCUnit npcunit in units) _bots.AddLast(npcunit);

                if (FindObjectsOfType<NPCUnit>().Length != _bots.Count) Debug.LogError("NPCs must be in pool");

            }
        }
    }
}