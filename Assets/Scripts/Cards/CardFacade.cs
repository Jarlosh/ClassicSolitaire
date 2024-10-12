using System.Collections.Generic;
using Solitaire.Dragging;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Solitaire.Cards
{
    public class CardFacade: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [Inject] private DragController _dragController;
        [SerializeField] private CardView _view;
        [SerializeField] private CardFloating _movement;

        private Collider2D _collider;
        private int _order;

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
            set
            {
                _movement.IsDragged = value;
                SetOrder(value ? DrawOrders.DragOrder : _order);
            }
        }

        public Suits StartSuit;
        public Values StartValue;
        
        private void SetData(Values value, Suits suit)
        {
            Value = value;
            Suit = suit;
            _view.SetVisual(Value, Suit);
        }

        public void SetPosition(Vector3 position)
        {
            _movement.TargetPosition = position;
        }

        private void Awake()
        {
            SetData(StartValue, StartSuit);
            _collider = GetComponent<Collider2D>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            var list = new List<CardFacade> { this };
            IsInteractable = false;
            _dragController.OnDragStart(eventData, list);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _dragController.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _movement.IsDragged = false;
            _dragController.CancelDrag();
            IsInteractable = Pile == null;
        }

        public void OnDrop(PointerEventData eventData)
        {
            _dragController.CompleteDrag();
        }

        public void SetOrder(int order)
        {
            _order = order;
            _view.SetOrder(order);
        }

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