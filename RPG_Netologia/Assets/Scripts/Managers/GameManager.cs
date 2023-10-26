using RPG.Units.NPC;
using RPG.Units.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace RPG.Managers
{
    public class GameManager : MonoInstaller
    {
        public override void InstallBindings()
        {
            

            var player = FindObjectOfType<PlayerUnit>();

            Container.BindInstance(player).AsSingle();
        }
    }
}