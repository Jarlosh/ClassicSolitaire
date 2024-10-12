using System.Collections.Generic;
using Core;
using Solitaire.Dragging;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Solitaire.Cards
{
    public class CardPile : MonoBehaviour, IDropHandler
    {
        public enum PileType
        {
            Column,
            Finish,
            Deck,
            Grave,
        }

        [Inject] 
        private DragController _dragController;
        [Inject] 
        private GameController _gameController;

        [SerializeField] 
        private float cardOffset;
        
        [field: SerializeField] public PileType Type { get; private set; }

        public List<CardFacade> Cards { get; } = new();

        public bool Any => Cards.Count > 0;
        public CardFacade TopCard => Any ? Cards[^1] : null;
        public CardFacade BottomCard => Any ? Cards[0] : null;

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag.TryGetComponent<CardFacade>(out var card)
                && _gameController.IsMoveAllowed(card, this))
            {
                _dragController.CompleteDrag();
                _gameController.MoveCard(card, this);
            }
        }

        public void Add(CardFacade card)
        {
            Cards.Add(card);
            card.Pile = this;
            card.IsInteractable = false;
            UpdatePosition(card);
            card.SetOrder(DrawOrders.InPileOrderBase + (Cards.Count - 1) * 5);
        }

        public void Remove(CardFacade card)
        {
            Cards.Remove(card);
            card.Pile = null;
            card.IsInteractable = true;
            card.SetOrder(DrawOrders.DefaultOrder);
        }

        private void UpdatePosition(CardFacade card)
        {
            card.SetPosition(transform.position + Vector3.down * cardOffset * (Cards.Count - 1));
        }
    }
}