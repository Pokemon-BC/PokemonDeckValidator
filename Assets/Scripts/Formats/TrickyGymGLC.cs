using PKMN.Cards;

using System;
using System.Collections.Generic;

public class TrickyGymGLC : PokemonFormat
{
    // Applies Singleton Rule
    protected override int MaxDuplicates => 1;

    private List<ShortCard> banList = new List<ShortCard>() {
        new ShortCard("PHF", "99"), // Lysandre's Trump Card
        new ShortCard("UPR", "114") // Oranguru
    };
    protected override List<ShortCard> FormatBanList { get => banList; }


    protected override void CustomFormatRules(PokemonDeck deck)
    {
        ApplySingleTypeRule(deck);
        ApplyRuleBoxRule(deck);
        ApplyNewnessRule(deck);
    }

    private void ApplySingleTypeRule(PokemonDeck deck)
    {
        PokemonType deckType = PokemonType.NONE;
        // If we encounter a dual type card before the type is set it's an issue so go back to them afterwards.
        List<CardInDeck> callbacks = new List<CardInDeck>();

        for(int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck current = deck.DeckCards[i];
            if(current.Reference.Supertype == CardSupertype.POKEMON)
            {
                if(deckType == PokemonType.NONE)
                {
                    // First card, define type
                    if(current.Reference.Types.Length == 1)
                    {
                        deckType = current.Reference.Types[0];
                    }
                    else
                    {
                        callbacks.Add(current);
                    }
                }
                else
                {
                    // Subsequent cards, check type
                    if(!Array.Exists(current.Reference.Types, (e) => e == deckType))
                    {
                        current.AddNote(NoteType.INVALID, "This Pokemon is not in the deck's primary type.");
                    }
                }
            }
        }
        if(deckType == PokemonType.NONE)
        {
            deck.AddNote(NoteType.ERROR, "There are no single type pokemon in the deck. This is not supported by the checker.");
        }
        for(int i = 0, count = callbacks.Count; i < count; i++)
        {
            CardInDeck current = callbacks[i];
            // Subsequent cards, check type
            if(!Array.Exists(current.Reference.Types, (e) => e == deckType))
            {
                current.AddNote(NoteType.INVALID, "This Pokemon is not in the deck's primary type.");
            }
        }
    }

    // Includes ACE SPEC Rule
    private void ApplyRuleBoxRule(PokemonDeck deck)
    {
        for(int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck current = deck.DeckCards[i];
            if(current.Reference.Supertype != CardSupertype.UNKNOWN)
            {
                if(CardHasRulebox(current))
                {
                    current.AddNote(NoteType.INVALID, "Cards with Rule Boxes are not allowed.");
                }
            }
        }
    }

    private bool CardHasRulebox(CardInDeck card)
    {
        if(card.Reference.Supertype == CardSupertype.POKEMON)
        {
            if(Array.Exists(card.Reference.Subtypes, (e) => {
                return e == CardSubtype.EX || e == CardSubtype.MEGA || e == CardSubtype.BREAK || e == CardSubtype.GX || e == CardSubtype.V || e == CardSubtype.VMAX || e == CardSubtype.V_UNION;
            }))
            {
                return true;
            }
        }
        else if(card.Reference.Supertype == CardSupertype.TRAINER)
        {
            if(card.Reference.Rules != null)
            {
                foreach(string s in card.Reference.Rules)
                {
                    if(s.Contains("ACE SPEC") || s.Contains("Prism Star"))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        else if(card.Reference.Supertype == CardSupertype.ENERGY)
        {
            if(card.Reference.Rules != null)
            {
                foreach(string s in card.Reference.Rules)
                {
                    if(s.Contains("Prism Star"))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    // BLW Onwards but includes banned expanded cards.
    private void ApplyNewnessRule(PokemonDeck deck)
    {
        for(int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck current = deck.DeckCards[i];
            if(current.Reference.Supertype != CardSupertype.UNKNOWN)
            {
                if(current.Reference.Legalities.Expanded == Legality.LEGAL || current.Reference.Legalities.Expanded == Legality.BANNED ||
                   current.Reference.Legalities.Standard == Legality.LEGAL || current.Reference.Legalities.Standard == Legality.BANNED)
                {
                    // Pass
                }
                else
                {
                    current.AddNote(NoteType.INVALID, "The card must be defined in expanded or standard legality.");
                }
            }
        }
    }
}
