using RPG.Units.NPC;
using RPG.Units.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;
namespace RPG.Managers
{
    public class GameManager : MonoInstaller
    {
        public override void InstallBindings()
        {
            

            var player = FindObjectOfType<PlayerUnit>();

            Container.BindInstance(player).AsSingle();
        }
        public override void Start()
        {
            base.Start();
            Constants.Construct();
            GetComponentsInChildren<Transform>(true).First(t => t.name == "Level").gameObject.AddComponent<LevelCheckComponent>();
        }
        [ContextMenu("Conf Level")]
        private void ConfigurationLevel()
        {
            var level = GetComponentsInChildren<Transform>(true).First(t => t.name == "Level").gameObject.AddComponent<LevelCheckComponent>();

            if (level == null) Debug.LogError("there's no level in scene");
            foreach (var obj in level.GetComponentsInChildren<Transform>(true))
            {
                obj.gameObject.layer = LayerMask.NameToLayer(Constants.ObstaclesTagName);
                obj.gameObject.tag = Constants.FloorTag;
            }
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
    }
}