using Solitaire.Cards;
using Solitaire.Tools;

namespace Core
{
    public class GameController
    {
        public bool IsMoveAllowed(CardFacade card, CardPile pile)
        {
            switch (pile.Type)
            {
                case CardPile.PileType.Column when !pile.Any:
                    return true;
                
                case CardPile.PileType.Column:
                    var top = pile.TopCard;
                    return !top.Suit.IsSameColorAs(card.Suit) &&
                           card.Value.IsGreaterThan(top.Value);
                
                case CardPile.PileType.Finish:
                    return !pile.Any || card.Value is CardFacade.Values.Ace;

                case CardPile.PileType.Deck:
                case CardPile.PileType.Grave:
                default:
                    return false;
            }
        }

        public void MoveCard(CardFacade card, CardPile pile)
        {
            if (card.Pile == pile)
            {
                return;
            }

            if (card.Pile != null)
            {
                card.Pile.Remove(card);
            }
            pile.Add(card);
        }
    }
}