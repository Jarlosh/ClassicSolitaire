using DG.Tweening;
using UnityEngine;

namespace Solitaire.Cards
{
    public class CardFloating: MonoBehaviour
    {
        private Vector3 _targetPosition;
        private Tweener _tweenMove;

        public bool IsDragged { get; set; }
        
        public Vector3 Position
        {
            get => transform.position;
            private set => transform.position = value;
        }

        public Vector3 TargetPosition
        {
            get => _targetPosition;
            set => SetTargetPosition(value);
        }

        private void SetTargetPosition(Vector3 targetPosition)
        {
            if (CanMove(targetPosition))
            {
                _targetPosition = targetPosition;
                AnimateMove(_targetPosition);
            }
        }
        
        private bool CanMove(Vector3 position)
        {
            return Vector3.SqrMagnitude(position - Position) > 0.001;
        }

        private void AnimateMove(Vector3 position)
        {
            if (IsDragged)
            {
                Position = position;
            }
            else
            {
                if (_tweenMove == null)
                    _tweenMove = transform
                        .DOMove(position, 0.1f)
                        .SetEase(Ease.OutQuad)
                        .SetAutoKill(false)
                        .OnRewind(() =>
                        {
                            // todo: update order
                        })
                        .OnComplete(() =>
                        {
                            // todo: restore order
                        });
                else
                    _tweenMove.ChangeEndValue(position, true).Restart();
            }
        }
    }
}