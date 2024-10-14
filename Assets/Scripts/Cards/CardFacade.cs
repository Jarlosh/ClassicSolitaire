using System;
using Solitaire.Dragging;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Solitaire.Cards
{
    public class CardFacade : MonoBehaviour, 
        IBeginDragHandler, 
        IDragHandler, 
        IEndDragHandler, 
        IDropHandler,
        IPointerClickHandler,
        IPoolable<CardFacade.Values, CardFacade.Suits, bool, IMemoryPool>,
        IDisposable
    {
        [Inject] private DragController _dragController;
        [SerializeField] private CardView _view;
        [SerializeField] private CardFloating _movement;
        [SerializeField] private Collider2D _collider;

        private IMemoryPool _pool;

        public Suits Suit { get; private set; }
        public Values Value { get; private set; }
        public CardPile Pile { get; set; }

        public Vector3 Position
        {
            get => _movement.Position;
        }
        
        public bool IsInteractable
        {
            get => _collider.enabled;
            set => _collider.enabled = value;
        }
        
        public bool IsDragged
        {
            get => _movement.IsDragged;
            set => _movement.IsDragged = value;
        }

        public bool IsRevealed
        {
            get => _view.IsRevealed;
            set => _view.IsRevealed = value;
        }

        private void SetData(Values value, Suits suit, bool isRevealed)
        {
            gameObject.name = $"({value} {suit})";
            Value = value;
            Suit = suit;
            _view.SetVisual(Value, Suit);
            IsRevealed = isRevealed;
        }

        public void SetPosition(Vector3 position)
        {
            _movement.TargetPosition = position;
        }

        private void SetOrder(int order)
        {
            _view.SetOrder(order);
        }

        public void ResetOrder()
        {
            if (!IsDragged && Pile == null)
            {
                SetOrder(DrawOrders.Default);
                return;
            }
            UpdateOrder(IsDragged 
                ? _dragController.IndexOf(this) 
                : Pile.Cards.IndexOf(this));
        }
        
        public void UpdateOrder(int columnIndex)
        {
            if (IsDragged)
            {
                SetOrder(DrawOrders.Drag + columnIndex * DrawOrders.TypeOffset);   
            }
            else if (Pile != null)
            {
                SetOrder(DrawOrders.InPileBase + columnIndex * DrawOrders.TypeOffset);
            }
            else
            {
                SetOrder(DrawOrders.Default);
            }
        }
        
        public void ResetPosition()
        {
            _movement.ResetPosition();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (Pile != null)
            {
                Pile.OnPointerClick(eventData);
            }
        }
        
        public class Factory : PlaceholderFactory<Values, Suits, bool, CardFacade>
        {
        }

        #region Drag event handlers 
        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragController.OnDragStart(this, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _dragController.OnDrag(this, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _dragController.OnEndDrag(this, eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
            _dragController.OnDropOnCard(this, eventData);
        }
        #endregion
        
        #region IDisposable
        public void Dispose()
        {
            _pool.Despawn(this);
        }
        #endregion IDisposable

        #region IPoolable
        public void OnSpawned(Values value, Suits suit, bool isRevealed, IMemoryPool pool)
        {
            _pool = pool;
            SetData(value, suit, isRevealed);
        }

        public void OnDespawned()
        {
            _pool = null;
        }
        #endregion IPoolable


        public enum Suits: byte
        {
            Spade,
            Club,
            Heart,
            Diamond
        }

        public enum Values: byte
        {
            Ace,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King
        }
    }
}