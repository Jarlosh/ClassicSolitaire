using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Solitaire.Cards
{
    public class CardView: MonoBehaviour
    {
        [Inject] private VisualSettings _settings;
        [SerializeField] private SpriteRenderer _border;
        [SerializeField] private SpriteRenderer _face;
        [SerializeField] private SpriteRenderer _back;
        [SerializeField] private SpriteRenderer _mainSuitRenderer;
        [SerializeField] private SpriteRenderer _cornerSuitRenderer;
        [SerializeField] private SpriteRenderer _valueRenderer;

        public void SetVisual(CardFacade.Values value, CardFacade.Suits suit)
        {
            var suitSprite = _settings.GetSprite(suit);
            _mainSuitRenderer.sprite = suitSprite;
            _cornerSuitRenderer.sprite = suitSprite;
            _valueRenderer.sprite = _settings.GetSprite(value);
            _valueRenderer.color = suit is CardFacade.Suits.Heart or CardFacade.Suits.Diamond
                ? Color.red
                : Color.black;
        }

        public void SetOrder(int order)
        {
            _border.sortingOrder = order;
            _face.sortingOrder = order + 1;
            _back.sortingOrder = order + 1;
            _mainSuitRenderer.sortingOrder = order + 2;
            _cornerSuitRenderer.sortingOrder = order + 2;
            _valueRenderer.sortingOrder = order + 2;
        }

        [Serializable]
        public class VisualSettings
        {
            [SerializeField] private List<Sprite> _suitSprites;
            [SerializeField] private List<Sprite> _valuesSprites;
            [field: SerializeField] public Color RedColor { get; private set; } 
            [field: SerializeField] public Color BlackColor { get; private set; }
            
            public Sprite GetSprite(CardFacade.Suits value) => _suitSprites[(int)value];
            
            public Sprite GetSprite(CardFacade.Values value) => _valuesSprites[(int)value];
        }
    }
}