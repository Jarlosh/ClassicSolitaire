﻿using Solitaire.Cards;

namespace Solitaire.Tools
{
    public static class CardEnumExtensions
    {
        public static int Compare(this CardFacade.Values value, CardFacade.Values other)
        {
            return ((int)value).CompareTo((int)other);
        }
        
        public static bool IsGreaterThan(this CardFacade.Values value, CardFacade.Values other)
        {
            return Compare(value, other) > 0;
        }
        
        public static bool IsRed(this CardFacade.Suits suit)
        {
            return suit is CardFacade.Suits.Heart or CardFacade.Suits.Diamond;
        }

        public static bool IsBlack(this CardFacade.Suits suits)
        {
            return !IsRed(suits);
        }

        public static bool IsSameColorAs(this CardFacade.Suits suit, CardFacade.Suits value)
        {
            return IsRed(suit) == IsRed(value);
        }
    }
}