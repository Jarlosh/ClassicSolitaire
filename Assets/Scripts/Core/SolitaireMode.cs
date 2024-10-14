using System.Collections.Generic;
using Solitaire.Cards;
using Solitaire.Tools;

namespace Solitaire.Core
{
    public interface ISolitaireMode
    {
        bool CanDrag(CardFacade card);
        bool CanDrop(IList<CardFacade> column, CardPile targetPile);
    }
    
    public class ClassicMode: ISolitaireMode
    {
        public bool CanDrop(IList<CardFacade> column, CardPile targetPile)
        {
            if (column.Count == 0)
            {
                return false;
            }
            
            if (column.Count == 1)
            {
                var card = column[0];
                return targetPile.Type switch
                {
                    CardPile.PileType.Column => IsNextInTableColumn(card, targetPile.TopCard),
                    CardPile.PileType.Finish => IsNextInFinishColumn(card, targetPile.TopCard),
                    _ => false
                };
            }
            
            return targetPile.Type == CardPile.PileType.Column && 
                   IsNextInTableColumn(column[0], targetPile.TopCard);
        }

        public bool CanDrag(CardFacade card)
        {
            var pile = card.Pile;
            if (pile == null)
            {
                return true;
            }

            switch (pile.Type)
            {
                case CardPile.PileType.Column:
                    return pile.TopCard == card || IsValidColumn(pile.TraceCardsUp(card), pile);
                
                case CardPile.PileType.Deck:
                case CardPile.PileType.Finish: 
                case CardPile.PileType.Grave: 
                    return pile.TopCard == card;

                default: return false;
            }
        }

        private bool IsValidColumn(IList<CardFacade> cards, CardPile pile)
        {
            var prev = cards[0];
            for (int i = 1; i < pile.Cards.Count; i++)
            {
                var current = pile.Cards[i];
                if (!IsNextInTableColumn(current, prev))
                {
                    return false;
                }

                prev = current;
            }
            return true;
        }

        private bool IsNextInTableColumn(CardFacade current, CardFacade prev)
        {
            if(prev == null)
            {
                return current.Value == CardFacade.Values.King;
            }

            var sameColor = current.Suit.IsSameColorAs(prev.Suit); 
            var succeedes = ((int)prev.Value) - ((int)current.Value) == +1;
            return !sameColor && succeedes;
        }
        
        private bool IsNextInFinishColumn(CardFacade current, CardFacade prev)
        {
            if(prev == null)
            {
                return current.Value == CardFacade.Values.Ace;
            }
            var sameSuit = current.Suit == prev.Suit; 
            var precedes = ((int)prev.Value) - ((int)current.Value) == -1;
            return sameSuit && precedes;
        }
    }
}