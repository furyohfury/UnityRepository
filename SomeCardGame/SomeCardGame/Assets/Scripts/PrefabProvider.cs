using UnityEngine;
namespace Pasians
{
    public class PrefabProvider : MonoBehaviour
    {
        private Prefabs _prefabs;

        public GameObject InstantiatePrefab(string name, Vector3 pos, Quaternion rot = Quaternion.identity)
        {
            return Instantiate(_prefabs.PrefabsDictionary[name], pos, rot);
        }
    }
}