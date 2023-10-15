using UnityEngine;

namespace Cars
{
    public class FinishLine : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            OnFinish?.Invoke();
        }
        public delegate void Finish();
        public event Finish OnFinish;
    }
}
    