using UnityEngine;
namespace Samurai
{
    public class ColorObstacle: Obstacle, IChangeColor
    {
        private void OnEnable()
        {
            OnSwapColor += ChangeColor;
        }
        private void OnDisable()
        {
            OnSwapColor -= ChangeColor;
        }
        public void ChangeColor()
        {
            
        }
        public void ChangeColor(PhaseColor color){}
        
    }
}