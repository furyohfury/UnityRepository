using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
namespace Samurai
{
    [RequireComponent(typeof(RigidBody))]
    public class Player: Unit, IHaveColor
    {
        public PhaseColor CurrentColor {get; private set;}

        public ChangePlayerColor()
        {
            if (CurrentColor == PhaseColor.Red) CurrentColor = PhaseColor.Blue;
            else if (CurrentColor == PhaseColor.Blue) CurrentColor = PhaseColor.Red;
        }
    }
}