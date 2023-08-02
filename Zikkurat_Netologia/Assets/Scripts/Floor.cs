using UnityEngine;
using UnityEngine.EventSystems;
using static Zikkurat.HUDManager;


namespace Zikkurat
{
    public class Floor : MonoBehaviour, IPointerClickHandler
    {
        public Vector3 _newSpawnPosition;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!HUDSingleton.SettingSpawn) return;
            _newSpawnPosition = eventData.position;
            OnNewSpawnPos?.Invoke(eventData.pointerCurrentRaycast.worldPosition);
        }
        public delegate void NewSpawnPos(Vector3 point);
        public event NewSpawnPos OnNewSpawnPos;
    }
}