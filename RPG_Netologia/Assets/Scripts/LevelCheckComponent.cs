using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RPG {
    public class LevelCheckComponent : MonoBehaviour
    {
        void Start()
        {
            var objects = GetComponentsInChildren<Transform>(true);
            var str = new StringBuilder();
            foreach (var obj in objects)
            {
                bool check1 = !obj.CompareTag(Constants.FloorTag);
                bool check2 = obj.gameObject.layer != Constants.ObstacleLayerInt;
                if (check1 && check2)
                {
                    str.Append($"{obj.name} doesnt have a correct tag and layer");
                }
                else if (check1) str.Append($"{obj.name} doesnt have a correct tag");
                else if (check2) str.Append($"{obj.name} doesnt have a correct layer");
            }
            if (str.Length > 0) Debug.LogError(str.ToString());
        }
    }
}