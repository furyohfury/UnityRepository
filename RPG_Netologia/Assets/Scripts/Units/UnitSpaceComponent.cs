using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Units
{
    public class UnitSpaceComponent : MonoBehaviour
    {
        [SerializeField]
        private BaseTriggerComponent _trigger;

        private void Start()
        {

        }
#if UNITY_EDITOR
        [ContextMenu("Create or Find Collider")]
        private void Construct()
        {
            var triggers = GetComponentsInChildren<BaseTriggerComponent>().Where(t => t.name == EditorConstants.AirColliderName).ToArray();

            if (triggers.Length == 1)
            {
                _trigger = triggers[0];
                return;
            }
            else if (triggers.Length > 1)
            {
                Debug.LogError("Unit" + name + " hase more than 1 aircollider");
                return;
            }

            var GO = new GameObject(EditorConstants.AirColliderName);
            GO.transform.parent = transform;
            GO.transform.position = Vector3.zero;
            GO.transform.rotation = new Quaternion();
            GO.transform.localScale = Vector3.one;

            var box = GO.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = EditorConstants.AirColliderBounds.size;
            box.center = EditorConstants.AirColliderBounds.center;

            _trigger = GO.AddComponent<BaseTriggerComponent>();
            Debug.Log($"Unit {name} an air collider was created");

            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
#endif
    }
}