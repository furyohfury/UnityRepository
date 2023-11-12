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

        }
        [ContextMenu("Get NPCs in pool")]
        public void GetNPCsInPool()
        {
            _bots = new();
            var units = _npcPool.GetComponentsInChildren<Unit>();

            foreach (var npc in units)
            {
                NPCUnit bot = npc as NPCUnit;
                if (bot == null) Debug.LogError($"Unit " + npc.gameObject.name + " isn't a NPC");
            }
            foreach (NPCUnit npcunit in units)
            {
                _bots.AddLast(npcunit);
                Debug.Log($"{npcunit} added to NPCPool");
            }

            if (FindObjectsOfType<NPCUnit>().Length != _bots.Count) Debug.LogError("All NPCs must be in pool");


        }
    }
}