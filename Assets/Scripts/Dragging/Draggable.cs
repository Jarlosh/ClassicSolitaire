using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Debug = UnityEngine.Debug;

namespace Solitaire.Dragging
{
    [RequireComponent(typeof(Collider2D))]
    public class Draggable: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [Inject] private DragController _dragController;
        private Collider2D _collider; 
        
        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }
        
        public bool IsInteractable
        {
            get => _collider.enabled;
            set => _collider.enabled = value;
        }

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Log("OnBeginDrag");
            var list = new List<Draggable> { this };
            _dragController.OnDragStart(eventData, list);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Log("OnDrag");
            _dragController.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Log("OnEndDrag");
            _dragController.CancelDrag();
        }

        public void OnDrop(PointerEventData eventData)
        {
            Log("OnDrop");
            _dragController.CompleteDrag();
        }
        
        [Conditional("UNITY_EDITOR")]
        private void Log(string str)
        {
            Debug.Log($"{gameObject.name}\n" + str, this);
        }
    }
}