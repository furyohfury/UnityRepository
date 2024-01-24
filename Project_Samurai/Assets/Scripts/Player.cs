using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
namespace Samurai
{
    [RequireComponent(typeof(RigidBody), typeof(SpriteRenderer))]
    public class Player: Unit, IChangeColor
    {
        public PhaseColor CurrentColor {get; private set;}
        public SpriteRenderer Sprite {get; private set;}

        private void Awake()
        {

        }
        private void OnEnable()
        {

        }
        private void Start()
        {
            Sprite = GetComponent<SpriteRenderer>();
        }
        private void OnDisable()
        {
            
        }
        public void ChangeColor()
        {
            if (CurrentColor == PhaseColor.Red)
            {
                CurrentColor = PhaseColor.Blue;
            }
            else if (CurrentColor == PhaseColor.Blue)
            {
                CurrentColor = PhaseColor.Red;
            }
            OnSwapColor?.Invoke();
        }
        public void ChangeColor(PhaseColor color)
        {
            if (color == PhaseColor.Green) CurrentColor = PhaseColor.Green;
        }
    }
}