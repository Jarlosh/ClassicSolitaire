using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solitaire.Dragging
{
    public class DragController
    {
        private record DraggedObject(Draggable Draggable, Vector3 Origin, Vector3 Offset);

        private readonly Camera _camera;

        private List<DraggedObject> _dragged = new();
        
        public DragController()
        {
            _camera = Camera.main;
        }
        
        public void OnDragStart(PointerEventData data, IList<Draggable> dragged)
        {
            var pointerOrigin = PointerToWorldPoint(data);
            foreach (var draggable in dragged)
            {
                draggable.IsInteractable = false;
                var originPosition = draggable.Position;
                var info = new DraggedObject(draggable, originPosition, originPosition - pointerOrigin);
                _dragged.Add(info);
            }
        }
        
        public void OnDrag(PointerEventData data)
        {
            var pointerOrigin = PointerToWorldPoint(data);
            foreach (var dragged in _dragged)
            {
                dragged.Draggable.Position = pointerOrigin + dragged.Offset;
            }
        }

        public void CancelDrag()
        {
            foreach (var dragged in _dragged)
            {
                dragged.Draggable.Position = dragged.Origin;
                dragged.Draggable.IsInteractable = true;
            }
            _dragged.Clear();
        }

        public void CompleteDrag()
        {
            foreach (var dragged in _dragged)
            {
                dragged.Draggable.IsInteractable = true;
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