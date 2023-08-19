using UnityEngine;
using UnityEngine.EventSystems;
using static Cards.GameCycleManager;

public class EndTurnButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameCycleSingleton.InputOn) OnEndTurn?.Invoke();
    }
    public delegate void EndTurn();
    public event EndTurn OnEndTurn;
}
