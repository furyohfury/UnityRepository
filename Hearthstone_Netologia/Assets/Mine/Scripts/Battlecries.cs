using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
                        ResetClicked();
                        StartCoroutine(WaitForClickOnAnyone(card));
                    }
                    break;
                case "Voodoo Doctor":
                    {
                        ResetClicked();
                        StartCoroutine(WaitForClickOnAnyone(card));
                    }
                    break;
                case "Murloc Tidehunter":
                    {
                        HandPosition emptyHandPosition = _handsDict[card.Player].FirstOrDefault(pos => pos.LinkedCard == null);
                        if (emptyHandPosition == default) return;
                        //todo заспавнить мурвока

                    }
                    break;
                case "Novice Engineer":
                    {
                        ResetClicked();
                        GivePlayerACard(card.Player);
                    }
                    break;
                case "Shattered Sun Cleric":
                    {
                        ResetClicked();
                        // StartCoroutine(WaitForClickOnSideMinion(card, true));
                    }
                    break;
            }

        }
        private IEnumerator WaitForClickOnAnyone(Card card)
        {
            Debug.Log("Waiting for click");
            InputOn = false;
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    LayerMask mask = LayerMask.GetMask("Board", "Player");
                    // Vector3 v = 10 * (_camera.transform.position - _camera.ScreenToWorldPoint(Input.mousePosition));
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

        private IEnumerator WaitForClickOnFriendlyOrEnemy(Card card, bool isFriendly)
        {
            yield return null;
        }
        private IEnumerator WaitForClickOnAnyMinion(Card card, CardUnitType type = CardUnitType.Common)
        {
            // Есть ли существа
            bool thereAreMinions = false;
            foreach (var boardPositions in _boardDict.Values)
            {
                if (boardPositions.FirstOrDefault(boardpos => boardpos.LinkedCard != null) != default) thereAreMinions = true;
            }
            if (!thereAreMinions) yield break;
            Debug.Log("Waiting for click");
            InputOn = false;
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    LayerMask mask = LayerMask.GetMask("Board");
                    if (Physics.Raycast(_camera.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out RaycastHit hit, 20, mask))
                    {
                        Debug.Log("Clicked on mask");
                        if (hit.collider.TryGetComponent(out BoardPosition boardPosition) && boardPosition.LinkedCard != null)
                        {
                            _clickedBoardPosition = boardPosition;
                            Debug.Log("Click Made on " + _clickedBoardPosition.gameObject.name);
                            InputOn = true;
                            UnitAction(card);
                            yield break;
                        }
                    }
                    yield return null;
                }
            }
        }
        private IEnumerator WaitForClickOnSideMinion(Card card, bool isFriendly, CardUnitType type = CardUnitType.Common)
        {
            // Есть ли существа указанной стороны
            bool thereAreMinions = false;
            foreach (var boardPositions in _boardDict.Values)
            {
                if (isFriendly && boardPositions.FirstOrDefault(boardpos => boardpos.LinkedCard != null && boardpos.LinkedCard.Player == card.Player) != default) thereAreMinions = true;
                if (!isFriendly && boardPositions.FirstOrDefault(boardpos => boardpos.LinkedCard != null && boardpos.LinkedCard.Player != card.Player) != default) thereAreMinions = true;
            }
            if (!thereAreMinions) yield break;

            InputOn = false;
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    LayerMask mask = LayerMask.GetMask("Board");
                    if (Physics.Raycast(_camera.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out RaycastHit hit, 20, mask))
                    {
                        if (hit.collider.TryGetComponent(out BoardPosition boardPosition) && boardPosition.LinkedCard != null && (isFriendly && boardPosition.LinkedCard.Player == card.Player || !isFriendly && boardPosition.LinkedCard.Player != card.Player))
                        {
                            _clickedBoardPosition = boardPosition;
                            InputOn = true;
                            UnitAction(card);
                            yield break;
                        }
                    }
                    yield return null;
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
        /* private IEnumerator WaitForClickOnFriendlyMinion(Card card)
        {
            // если есть дружественные 
            if (_handsDict[card.Player].FirstOrDefault(handPos => handPos.LinkedCard != null) == default) yield break;
            InputOn = false;
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    LayerMask mask = LayerMask.GetMask("Board");
                    if (Physics.Raycast(_camera.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out RaycastHit hit, 20, mask))
                    {
                        if (hit.collider.TryGetComponent(out BoardPosition boardPosition) && boardPosition.LinkedCard != null && boardPosition.LinkedCard.Player == card.Player)
                        {
                            _clickedBoardPosition = boardPosition;
                            InputOn = true;
                            UnitAction(card);
                            yield break;
                        }
                    }
                    yield return null;
                }
            }
        }
        private IEnumerator WaitForClickOnType(Card card, CardUnitType type, bool isFriendly = true)
        {
            // если есть дружественные такого типа
            if (isFriendly && _handsDict[card.Player].FirstOrDefault(handPos => handPos.LinkedCard != null && handPos.LinkedCard.GetCardPropertyData()._type == type) == default) yield break;
            if (_handsDict[card.Player].FirstOrDefault(handPos => handPos.LinkedCard != null && handPos.LinkedCard.GetCardPropertyData()._type == type) == default) yield break;
            InputOn = false;
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    LayerMask mask = LayerMask.GetMask("Board");
                    if (Physics.Raycast(_camera.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out RaycastHit hit, 20, mask))
                    {
                        if (hit.collider.TryGetComponent(out BoardPosition boardPosition) && boardPosition.LinkedCard != null && boardPosition.LinkedCard.Player == card.Player)
                        {
                            _clickedBoardPosition = boardPosition;
                            InputOn = true;
                            UnitAction(card);
                            yield break;
                        }
                    }
                    yield return null;
                }
            }
        } */
        private void ResetClicked()
        {
            _clickedBoardPosition = null;
            _clickedPlayer = null;
        }
    }
}
