
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Solitaire.Cards;
using Solitaire.Tools;
using UnityEngine;
using Zenject;

namespace Solitaire.Core
{
    // todo: add commands
    // todo: move "move", "draw", "refill" to ISolitaireMode
    public class GameController: IInitializable
    {
        [Inject] private Map _map;
        [Inject] private CardFacade.Factory _factory;

        private List<CardFacade> _cards = new();

        public void Initialize()
        {
            Reset();
            Deal().Forget();
        }

        private async UniTask Deal()
        {
            // move constants to config
            const int StartDelay = 500; 
            const int MoveDelay = 1000 / (13 * 4); // todo: fix int problem
            
            await UniTask.Delay(StartDelay);
            for (var i = 0; i < _map.TablePiles.Length; i++)
            {
                for (var j = 0; j < i + 1; j++)
                {
                    var topCard = _map.DrawPile.TopCard;
                    MoveCard(topCard, _map.TablePiles[i], false);

                    topCard!.IsRevealed = j == i;
                    await UniTask.Delay(MoveDelay);
                }
            }
        }

        public void OnPileClicked(CardPile cardPile)
        {
            if (cardPile.Type is not CardPile.PileType.Deck)
            {
                return;
            }
            if (_map.DrawPile.HasCards)
            {
                Draw();
            }
            else if(_map.GraveyardPile.HasCards)
            {
                Refill().Forget();
            }
        }

        private void Draw()
        {
            var card = _map.DrawPile.TopCard;
            MoveCard(card, _map.GraveyardPile, reveal: true);
            TryRevealTop(_map.GraveyardPile);
            card!.ResetOrder();
        }

        // todo: add check if is refilling
        // todo: update card moving to add awaiting opportunity 
        private async UniTask Refill()
        {
            // move constants to config
            const int DurationMs = 250;
            var grave = _map.GraveyardPile;
            IList<CardFacade> cards = grave.Cards;
            int moveDelay = DurationMs / cards.Count; // todo: fix int problem
            
            cards = cards.Reverse().ToArray();
            foreach (var card in cards)
            {
                MoveCard(card, _map.DrawPile, reveal: false);
                await UniTask.Delay(moveDelay);
                
                // todo: await fly first
                card.IsRevealed = false;
            }
        }

        private void Reset()
        {
            ResetPiles();
            DisposeCards();
            GenerateCards();
            
            void ResetPiles()
            {
                foreach (var pile in _map.TablePiles)
                {
                    pile.Reset();
                }
                foreach (var pile in _map.FinishPiles)
                {
                    pile.Reset();
                }
                _map.DrawPile.Reset();
                _map.GraveyardPile.Reset();
            }
            
            void DisposeCards()
            {
                foreach (var card in _cards)
                {
                    card.Dispose();
                }    
                _cards.Clear();
            }

            void GenerateCards()
            {
                foreach (var suit in Enum.GetValues(typeof(CardFacade.Suits)))
                {
                    foreach (var value in Enum.GetValues(typeof(CardFacade.Values)))
                    {
                        var card = _factory.Create((CardFacade.Values)value, (CardFacade.Suits)suit, false);
                        _cards.Add(card);
                    }
                }

                _cards.Shuffle();
                foreach (var card in _cards)
                {
                    _map.DrawPile.Add(card);
                    card.ResetPosition();
                }
            }
        }

        public void MoveCards(IList<CardFacade> cards, CardPile pile, bool reveal)
        {
            foreach (var card in cards)
            {
                MoveCard(card, pile, reveal);
            }
        }
        
        private void MoveCard(CardFacade card, CardPile pile, bool reveal)
        {
            if (card.Pile == pile)
            {
                return;
            }

            if (card.Pile != null)
            {
                card.Pile.Remove(card);
                if(reveal)
                {
                    TryRevealTop(pile);
                }
            }
            pile.Add(card);
        }

        private void TryRevealTop(CardPile pile)
        {
            if (!pile.HasCards)
            {
                return;
            }

            pile.TopCard!.IsRevealed = true;
        }

        [Serializable]
        public class Map
        {
            [field: SerializeField] public CardPile DrawPile { get; private set; }
            [field: SerializeField] public CardPile GraveyardPile { get; private set; }
            [field: SerializeField] public CardPile[] TablePiles { get; private set; }
            [field: SerializeField] public CardPile[] FinishPiles { get; private set; }
        }
    }
}