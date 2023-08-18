using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Cards.DecksManager;
namespace Cards
{
    public partial class GameCycleManager
    {
        BoardPosition _clickedBoardPosition;
        PlayerPortrait _clickedPlayer;
        private void Battlecry(Card card)
        {
            switch (card.GetCardPropertyData()._name)
            {
                case "Elven Archer":
                    {
                        StartCoroutine(WaitForClickOnAnyone(card));
                    }
                    break;
                case "Voodoo Doctor":
                    {
                        StartCoroutine(WaitForClickOnAnyone(card));
                    }
                    break;
                case "Murloc Tidehunter":
                    {
                        UnitAction(card);                        
                    }
                    break;
                case "Novice Engineer":
                    {
                        GivePlayerACard(card.Player);
                    }
                    break;
                case "Shattered Sun Cleric":
                    {
                        StartCoroutine(WaitForClickSpecified(card, false, Targets.Friendly));
                    }
                    break;
                case "Frostwolf Warlord":
                    {
                        UnitAction(card);
                    }
                    break;
                case "Houndmaster":
                    {
                        StartCoroutine(WaitForClickSpecified(card, false, Targets.Friendly, CardUnitType.Beast));
                    }
                    break;
            }

        }
        private IEnumerator WaitForClickOnAnyone(Card card)
        {
            ResetClicked();
            Debug.Log("Waiting for click");
            InputOn = false;
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    LayerMask mask = LayerMask.GetMask("Board", "Player");
                    Debug.Log("Shooting ray");
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, 1000, mask))
                    {
                        Debug.Log("Clicked on mask");
                        if (hit.collider.TryGetComponent(out BoardPosition boardPosition) && boardPosition.LinkedCard != null && boardPosition.LinkedCard != card)
                        {
                            _clickedBoardPosition = boardPosition;
                            InputOn = true;
                            Debug.Log("Click Made on " + _clickedBoardPosition.gameObject.name);
                            UnitAction(card);
                            yield break;
                        }
                        else if (hit.collider.TryGetComponent(out PlayerPortrait playerPortrait))
                        {
                            _clickedPlayer = playerPortrait;
                            InputOn = true;
                            Debug.Log("Click Made on " + _clickedPlayer.gameObject.name);
                            UnitAction(card);
                            yield break;
                        }
                    }
                }
                yield return null;
            }
        }

        private IEnumerator WaitForClickSpecified(Card card, bool withPlayer, Targets target, CardUnitType type = CardUnitType.Common)
        {
            ResetClicked();
            if (!CheckForTargets(out List<BoardPosition> boardTargets, card, target, type)) yield break;
            Debug.Log("Waiting for click");
            InputOn = false;
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    LayerMask mask = LayerMask.GetMask("Board", "Player");
                    Debug.Log("Shooting ray");
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, 1000, mask))
                    {
                        Debug.Log("Clicked on mask");
                        // Есть ли кликнутая BoardPosition в списке на проверку
                        if (hit.collider.TryGetComponent(out BoardPosition boardPosition) && boardTargets.Contains(boardPosition))
                        {
                            _clickedBoardPosition = boardPosition;
                            InputOn = true;
                            Debug.Log("Click Made on " + _clickedBoardPosition.gameObject.name);
                            UnitAction(card);
                            yield break;
                        }
                        else if (withPlayer && hit.collider.TryGetComponent(out PlayerPortrait playerPortrait) && (target == Targets.Friendly && playerPortrait.Player == card.Player || target == Targets.Enemies && playerPortrait.Player != card.Player))
                        {
                            _clickedPlayer = playerPortrait;
                            InputOn = true;
                            Debug.Log("Click Made on " + _clickedPlayer.gameObject.name);
                            UnitAction(card);
                            yield break;
                        }
                    }
                }
                yield return null;
            }

        }
        private bool CheckForTargets(out List<BoardPosition> boardTargets, Card card, Targets target, CardUnitType type = CardUnitType.Common)
        {
            boardTargets = new();
            switch (target)
            {
                case Targets.Friendly:
                    {
                        // Есть хотя бы одна дружественная карта с нужным типом если он задан
                        foreach (var boardPositions in BoardDict.Values)
                        {
                            boardTargets.AddRange(boardPositions.Where(boardPos => boardPos.LinkedCard != null && boardPos.LinkedCard != card && boardPos.LinkedCard.Player == card.Player && (type == CardUnitType.Common || type == boardPos.LinkedCard.GetCardPropertyData()._type)).ToList());
                        }
                        if (boardTargets.Count > 0) return true;
                        return false;
                    }
                case Targets.Enemies:
                    {
                        // Есть хотя бы одна вражеская карта с нужным типом если он задан
                        foreach (var boardPositions in BoardDict.Values)
                        {
                            boardTargets.AddRange(boardPositions.Where(boardPos => boardPos.LinkedCard != null && boardPos.LinkedCard != card && boardPos.LinkedCard.Player != card.Player && (type == CardUnitType.Common || type == boardPos.LinkedCard.GetCardPropertyData()._type)).ToList());
                        }
                        if (boardTargets.Count > 0) return true;
                        return false;
                    }
                case Targets.All:
                    {
                        // Есть хотя бы одна карта с нужным типом если он задан
                        foreach (var boardPositions in BoardDict.Values)
                        {
                            boardTargets.AddRange(boardPositions.Where(boardPos => boardPos.LinkedCard != null && boardPos.LinkedCard != card && (type == CardUnitType.Common || type == boardPos.LinkedCard.GetCardPropertyData()._type)).ToList());
                        }
                        if (boardTargets.Count > 0) return true;
                        return false;
                    }
                default:
                    {
                        boardTargets = null;
                        return false;
                    }
            }
        }
        private void UnitAction(Card card)
        {
            switch (card.GetCardPropertyData()._name)
            {
                case "Elven Archer":
                    {
                        if (_clickedBoardPosition != null)
                        {
                            _clickedBoardPosition.LinkedCard.ChangeHP(-1);
                            CheckAliveAndDestroyCard(_clickedBoardPosition.LinkedCard);
                        }
                        if (_clickedPlayer != null) _clickedPlayer.ChangePlayerHealth(-1);
                    }
                    break;
                case "Voodoo Doctor":
                    {
                        if (_clickedBoardPosition != null) _clickedBoardPosition.LinkedCard.ChangeHP(2, true);
                        if (_clickedPlayer != null) _clickedPlayer.ChangePlayerHealth(2, true);
                    }
                    break;
                case "Shattered Sun Cleric":
                    {
                        _clickedBoardPosition.LinkedCard.ChangeHP(1);
                        _clickedBoardPosition.LinkedCard.ChangeAttack(1);
                    }
                    break;
                case "Houndmaster":
                    {
                        _clickedBoardPosition.LinkedCard.ChangeAttack(2);
                        _clickedBoardPosition.LinkedCard.ChangeHP(2);
                    }
                    break;
                case "Murloc Tidehunter":
                    {
                        SummonMinion(card, "Murloc Scout");
                    }
                    break;
                case "Frostwolf Warlord":
                    {
                        CheckForTargets(out List<BoardPosition> boardPositions, card, Targets.Friendly);
                        card.ChangeAttack(boardPositions.Count);
                        card.ChangeHP(boardPositions.Count);
                    }
                    break;
            }
        }
        private void SummonMinion(Card card, string summonName)
        {
            // Поиск свободного места
            BoardPosition emptyPosition = BoardDict[card.Player].FirstOrDefault(boardPos => boardPos.LinkedCard == null);
            if (emptyPosition == default) return;
            Vector3 position = new Vector3(emptyPosition.transform.position.x, _cardPrefab.transform.position.y, emptyPosition.transform.position.z);
            GameObject summonCardGO = card.Player == PlayerSide.One ? Instantiate(_cardPrefab, position, Quaternion.Euler(new Vector3(0, 0, 180))) : Instantiate(_cardPrefab, position, Quaternion.Euler(new Vector3(0, 180, 180)));
            CardPropertyData summonData = Converting.ConvertToProperty(DeckManagerSingleton.AllCards.SingleOrDefault(minion => minion.Name == summonName));
            Card summonCardComp = summonCardGO.GetComponent<Card>();
            summonCardComp.SetCardDataAndVisuals(summonData);
            // Присвоение свойств карты
            summonCardComp.Player = card.Player;
            if (Charge.ChargeCards.Contains(summonCardComp.GetCardPropertyData()._name))
            {
                summonCardComp.Charge = true;
                summonCardComp.CanAttack = true;
            }
            if (Taunt.TauntCards.Contains(summonCardComp.GetCardPropertyData()._name)) summonCardComp.Taunt = true;
            summonCardGO.name = summonCardComp.GetCardPropertyData()._name;
            // Привязка к позиции
            emptyPosition.SetLinkedCard(summonCardComp);
            summonCardComp.SetLinkedBoardPosition(emptyPosition);
            // Подписка на ивенты 
            summonCardComp.OnDragBegin += DragBegin;
            summonCardComp.OnDragging += DraggingCard;
            summonCardComp.OnDragEnd += DragEnd;
        }
        private void ResetClicked()
        {
            _clickedBoardPosition = null;
            _clickedPlayer = null;
        }
    }
}
