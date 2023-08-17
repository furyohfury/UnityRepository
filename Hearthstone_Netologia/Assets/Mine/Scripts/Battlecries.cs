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
                        SummonMinion(card, "Murloc Scout");
                    }
                    break;
                case "Novice Engineer":
                    {
                        GivePlayerACard(card.Player);
                    }
                    break;
                case "Shattered Sun Cleric":
                    {
                        StartCoroutine(WaitForClickSpecified(card, false, BattlecryTargets.Friendly));
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

        private IEnumerator WaitForClickSpecified(Card card, bool withPlayer, BattlecryTargets target, CardUnitType type = CardUnitType.Common)
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
                        // ≈сть ли кликнута€ BoardPosition в списке на проверку
                        if (hit.collider.TryGetComponent(out BoardPosition boardPosition) && boardTargets.Contains(boardPosition))
                        {
                            _clickedBoardPosition = boardPosition;
                            InputOn = true;
                            Debug.Log("Click Made on " + _clickedBoardPosition.gameObject.name);
                            UnitAction(card);
                            yield break;
                        }
                        else if (withPlayer && hit.collider.TryGetComponent(out PlayerPortrait playerPortrait) && (target == BattlecryTargets.Friendly && playerPortrait.Player == card.Player || target == BattlecryTargets.Enemies && playerPortrait.Player != card.Player))
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
        private bool CheckForTargets(out List<BoardPosition> boardTargets, Card card, BattlecryTargets target, CardUnitType type = CardUnitType.Common)
        {
            boardTargets = new();
            switch (target)
            {
                case BattlecryTargets.Friendly:
                    {
                        // ≈сть хот€ бы одна дружественна€ карта с нужным типом если он задан
                        foreach (var boardPositions in _boardDict.Values)
                        {
                            boardTargets.AddRange(boardPositions.Where(boardPos => boardPos.LinkedCard != null && boardPos.LinkedCard != card && boardPos.LinkedCard.Player == card.Player && (type == CardUnitType.Common || type == boardPos.LinkedCard.GetCardPropertyData()._type)).ToList());
                        }
                        if (boardTargets.Count > 0) return true;
                        return false;
                    }
                case BattlecryTargets.Enemies:
                    {
                        // ≈сть хот€ бы одна вражеска€ карта с нужным типом если он задан
                        foreach (var boardPositions in _boardDict.Values)
                        {
                            boardTargets.AddRange(boardPositions.Where(boardPos => boardPos.LinkedCard != null && boardPos.LinkedCard != card && boardPos.LinkedCard.Player != card.Player && (type == CardUnitType.Common || type == boardPos.LinkedCard.GetCardPropertyData()._type)).ToList());
                        }
                        if (boardTargets.Count > 0) return true;
                        return false;
                    }
                case BattlecryTargets.All:
                    {
                        // ≈сть хот€ бы одна карта с нужным типом если он задан
                        foreach (var boardPositions in _boardDict.Values)
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
            }
        }
        private void SummonMinion(Card card, string summonName)
        {
            BoardPosition emptyPosition = _boardDict[card.Player].FirstOrDefault(boardPos => boardPos.LinkedCard == null);
            if (emptyPosition == default) return;
            Vector3 position = new Vector3(emptyPosition.transform.position.x, _cardPrefab.transform.position.y, emptyPosition.transform.position.z);
            GameObject summonCardGO = Instantiate(_cardPrefab, position, Quaternion.Euler(new Vector3(0, 0, 180)));
            CardPropertyData summonData = Converting.ConvertToProperty(DeckManagerSingleton.AllCards.SingleOrDefault(minion => minion.Name == summonName));
            Card summonCardComp = summonCardGO.GetComponent<Card>();
            summonCardComp.SetCardDataAndVisuals(summonData);
            // ѕрисвоение свойств карты
            summonCardComp.Player = card.Player;
            if (Charge.ChargeCards.Contains(summonCardComp.GetCardPropertyData()._name))
            {
                summonCardComp.Charge = true;
                summonCardComp.CanAttack = true;
            }
            if (Taunt.TauntCards.Contains(summonCardComp.GetCardPropertyData()._name)) summonCardComp.Taunt = true;
            summonCardGO.name = summonCardComp.GetCardPropertyData()._name;
            // ѕрив€зка к позиции
            emptyPosition.SetLinkedCard(summonCardComp);
            summonCardComp.SetLinkedBoardPosition(emptyPosition);
        }
        private void ResetClicked()
        {
            _clickedBoardPosition = null;
            _clickedPlayer = null;
        }
    }
}
