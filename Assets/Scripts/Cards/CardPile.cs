using System.Collections.Generic;
using JetBrains.Annotations;
using Solitaire.Core;
using Solitaire.Dragging;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Solitaire.Cards
{
    public class CardPile : MonoBehaviour, IDropHandler, IPointerClickHandler
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

        public bool HasCards => Cards.Count > 0;
        
        [CanBeNull] 
        public CardFacade TopCard => HasCards ? Cards[^1] : null;

        [CanBeNull] 
        public CardFacade BottomCard => HasCards ? Cards[0] : null;

        public void OnDrop(PointerEventData eventData)
        {
            _dragController.OnDropOnPile(this, eventData);
        }

        public void Add(CardFacade card)
        {
            Cards.Add(card);
            card.Pile = this;
            UpdatePosition(card);
            card.UpdateOrder(Cards.Count - 1 - 1);
        }

        public void Remove(CardFacade card)
        {
            Cards.Remove(card);
            card.Pile = null;
            card.UpdateOrder(0);
        }

        private void UpdatePosition(CardFacade card)
        {
            card.SetPosition(transform.position + Vector3.down * cardOffset * (Cards.Count - 1));
        }

        public IList<CardFacade> TraceCardsUp(CardFacade first)
        {
            var index = Cards.IndexOf(first);
            if (index == -1)
            {
                return null;
            }

            var list = new List<CardFacade>();
            for (int i = index; i < Cards.Count; i++)
            {
                list.Add(Cards[i]);
            }

            return list;
        }

        public void Reset()
        {
            foreach (var card in Cards.ToArray())
            {
                Remove(card);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _gameController.OnPileClicked(this);
        }
    }
}