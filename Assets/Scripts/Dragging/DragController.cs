using System.Collections.Generic;
using Solitaire.Cards;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solitaire.Dragging
{
    public class DragController
    {
        private record DraggedObject(CardFacade CardFacade, Vector3 Origin, Vector3 Offset);

        private readonly Camera _camera;

        private List<DraggedObject> _dragged = new();
        
        public DragController()
        {
            _camera = Camera.main;
        }
        
        public void OnDragStart(PointerEventData data, IList<CardFacade> dragged)
        {
            var pointerOrigin = PointerToWorldPoint(data);
            foreach (var card in dragged)
            {
                var originPosition = card.Position;
                var info = new DraggedObject(card, originPosition, originPosition - pointerOrigin);
                _dragged.Add(info);
                card.IsDragged = true;
            }
        }
        
        public void OnDrag(PointerEventData data)
        {
            var pointerOrigin = PointerToWorldPoint(data);
            foreach (var dragged in _dragged)
            {
                dragged.CardFacade.SetPosition(pointerOrigin + dragged.Offset);
            }
        }

        public void CancelDrag()
        {
            foreach (var dragged in _dragged)
            {
                dragged.CardFacade.IsDragged = false;
                dragged.CardFacade.SetPosition(dragged.Origin);
            }
            _dragged.Clear();
        }

        public void CompleteDrag()
        {
            foreach (var dragged in _dragged)
            {
                dragged.CardFacade.IsDragged = false;
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
    }
}