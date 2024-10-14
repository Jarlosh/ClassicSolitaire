using System.Collections.Generic;
using System.Linq;
using Solitaire.Cards;
using Solitaire.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Solitaire.Dragging
{
    public class DragController
    {
        private record DraggedObject(CardFacade Card, Vector3 Origin, Vector3 Offset);

        private readonly Camera _camera;

        private List<DraggedObject> _dragged = new();
        
        [Inject] private ISolitaireMode _solitaireMode;
        [Inject] private GameController _gameController;
        private bool IsDragging => _dragged.Any();
        private DraggedObject DragStarter => _dragged.FirstOrDefault();

        public DragController()
        {
            _camera = Camera.main;
        }
        
        public void OnDragStart(CardFacade first, PointerEventData data)
        {
            var pile = first.Pile;
            if (!_solitaireMode.CanDrag(first))
            {
                return;
            }

            var column = pile != null 
                ? pile.TraceCardsUp(first)
                : new List<CardFacade> { first };

            var pointerOrigin = PointerToWorldPoint(data);
            for (var i = 0; i < column.Count; i++)
            {
                var card = column[i];
                var originPosition = card.Position;
                var info = new DraggedObject(card, originPosition, originPosition - pointerOrigin);
                _dragged.Add(info);
                card.IsDragged = true;
                card.IsInteractable = false;
                card.UpdateOrder(i);
            }
        }

        public void OnDrag(CardFacade card, PointerEventData data)
        {
            if (!IsDragging || card != DragStarter.Card)
            {
                return;
            }
            var pointerOrigin = PointerToWorldPoint(data);
            foreach (var dragged in _dragged)
            {
                dragged.Card.IsDragged = true;
                dragged.Card.SetPosition(pointerOrigin + dragged.Offset);
            }
        }

        public void OnDropOnCard(CardFacade accepting, PointerEventData eventData)
        {
            OnDropOnPile(accepting.Pile, eventData);
        }

        public void OnDropOnPile(CardPile pile, PointerEventData eventData)
        {
            if (pile == null || !IsDragging)
            {
                return;
            }

            var starter = DragStarter;
            if (eventData.pointerDrag != starter.Card.gameObject || starter.Card.Pile == pile)
            {
                return;
            }

            using (UnityEngine.Pool.ListPool<CardFacade>.Get(out var cards))
            {
                cards.Clear();
                foreach (var dragged in _dragged)
                {
                    cards.Add(dragged.Card);
                }
                
                if (_solitaireMode.CanDrop(cards, pile))
                {
                    CompleteDrag();
                    _gameController.MoveCards(cards, pile, reveal: true);
                }
            }
        }
        
        public void OnEndDrag(CardFacade card, PointerEventData eventData)
        {
            CancelDrag();
        }
        
        private void CancelDrag()
        {
            foreach (var dragged in _dragged)
            {
                dragged.Card.IsDragged = false;
                dragged.Card.IsInteractable = true;
                dragged.Card.SetPosition(dragged.Origin);
                dragged.Card.ResetOrder();
            }
            _dragged.Clear();
        }

        private void CompleteDrag()
        {
            foreach (var dragged in _dragged)
            {
                dragged.Card.IsDragged = false;
                dragged.Card.IsInteractable = true;
            }
            _dragged.Clear();
        }

        private Vector3 PointerToWorldPoint(PointerEventData eventData)
        {
            var screenPoint = new Vector3(
                eventData.position.x,
                eventData.position.y,
                -_camera.transform.position.z
            );
            return _camera.ScreenToWorldPoint(screenPoint);
        }

        public int IndexOf(CardFacade card)
        {
            for (int i = 0; i < _dragged.Count; i++)
            {
                if (_dragged[i].Card == card)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}