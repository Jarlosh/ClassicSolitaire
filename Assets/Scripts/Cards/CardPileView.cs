using System.Diagnostics;
using Solitaire.Dragging;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Debug = UnityEngine.Debug;

namespace Solitaire.Cards
{
    public class CardPileView: MonoBehaviour, IDropHandler
    {
        [Inject] private DragController _dragController;
        
        public void OnDrop(PointerEventData eventData)
        {
            Log("OnDrop");
            if (eventData.pointerDrag.TryGetComponent<Draggable>(out var cardView))
            {
                _dragController.CompleteDrag();
                cardView.Position = transform.position;
            }
        }

        [Conditional("UNITY_EDITOR")]
        private void Log(string str)
        {
            Debug.Log($"{gameObject.name}\n" + str, this);
        }
    }
}