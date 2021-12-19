using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using PKMN.Cards;
public abstract class PokemonFormat
{
    // The number of cards with the same name allowed in a deck
    public virtual int MaxDuplicates { get => 4; }

    public virtual bool RequireStandardLegal { get => false; }
    public virtual bool RequireExpandedLegal { get => false; }
    public virtual bool RequireUnlimitedLegal { get => false; }

    protected virtual List<ShortCard> FormatBanList { get => null; }

    public void CheckDeckInFormat(PokemonDeck deck)
    {
        // Apply base rules
        BaseDeckRules(deck);
        // Apply Quantity Rules
        CheckQuantityRules(deck);
        // Legality Rules
        LegalitiesCheck(deck);
        // Ban List
        ApplyBanList(deck);
        // Allow the format to apply custom rules
        CustomFormatRules(deck);
    }

    private void BaseDeckRules(PokemonDeck deck)
    {
        if (deck.totalCards != 60)
        {
            deck.AddNote(NoteType.INVALID, "Deck Needs 60 Cards");
        }
        bool anyBasic = false;
        for(int i = 0, count = deck.deckCards.Count; i < count; i++)
        {
            CardInDeck card = deck.deckCards[i];
            if(card.reference.Supertype == CardSupertype.POKEMON)
            {
                if(Array.Exists(card.reference.Subtypes, (e) => e == CardSubtype.BASIC))
                {
                    anyBasic = true;
                    break;
                }
            }
        }
        if (!anyBasic)
        {
            deck.AddNote(NoteType.INVALID, "Deck must have at least 1 basic Pokemon");
        }
    }

    private void CheckQuantityRules(PokemonDeck deck)
    {
        // TODO idk how this works for like "Professor's Research (Juniper)"
        Dictionary<string, int> cardCounts = new Dictionary<string, int>();
        for (int i = 0, count = deck.deckCards.Count; i < count; i++)
        {
            CardInDeck cid = deck.deckCards[i];
            if (cid.reference.Supertype != CardSupertype.ENERGY || Array.Exists(cid.reference.Subtypes, (e) => e == CardSubtype.BASIC))
            {
                string name = cid.reference.Name;
                if (cardCounts.ContainsKey(name))
                {
                    cardCounts[name] += cid.quantity;
                }
                else
                {
                    cardCounts.Add(name, cid.quantity);
                }
                if (cardCounts[name] >= 4)
                {
                    cid.AddNote(NoteType.INVALID, "There are more than 4 cards with this name.");
                }
            }   
        }
    }

    private void LegalitiesCheck(PokemonDeck deck)
    {
        for(int i = 0, count = deck.deckCards.Count; i < count; i++)
        {
            CardInDeck cid = deck.deckCards[i];
            Legalities legal = cid.reference.Legalities;
            if(RequireUnlimitedLegal)
            {
                if(legal.Unlimited == Legality.BANNED)
                {
                    cid.AddNote(NoteType.INVALID, "This card is Banned in the Unlimited format.");
                }
                else if (!legal.IsUnlimitedLegal)
                {
                    cid.AddNote(NoteType.INVALID, "This card is not Legal in the Unlimited format.");
                }
            }
            if(RequireExpandedLegal)
            {
                if (legal.Expanded == Legality.BANNED)
                {
                    cid.AddNote(NoteType.INVALID, "This card is Banned in the Expanded format.");
                }
                else if (!legal.IsExpandedLegal)
                {
                    cid.AddNote(NoteType.INVALID, "This card is not Legal in the Expanded format.");
                }
            }
            if (RequireStandardLegal)
            {
                if (legal.Standard == Legality.BANNED)
                {
                    cid.AddNote(NoteType.INVALID, "This card is Banned in the Standard format.");
                }
                else if (!legal.IsStandardLegal)
                {
                    cid.AddNote(NoteType.INVALID, "This card is not Legal in the Standard format.");
                }
            }
        }
    }

    private void ApplyBanList(PokemonDeck deck)
    {
        List<ShortCard> banlist = FormatBanList;
        if(banlist != null)
        {
            for (int i = 0, count = deck.deckCards.Count; i < count; i++)
            {
                CardInDeck cid = deck.deckCards[i];
                ShortCard testCard = ShortCard.TestCard(cid.setId, cid.reference.Number);
                if(banlist.Contains(testCard))
                {
                    cid.AddNote(NoteType.INVALID, "This card is banned in this format.");
                }
            }
        }
    }

    protected abstract void CustomFormatRules(PokemonDeck deck);
}

public class ShortCard
{
    public string setCode;
    public int collectorsId;

    public ShortCard(string setCode, int collectorsId)
    {
        this.setCode = setCode;
        this.collectorsId = collectorsId;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ShortCard);
    }

    public bool Equals(ShortCard other)
    {
        return other != null &&
               setCode == other.setCode &&
               collectorsId == other.collectorsId;
    }

    public override int GetHashCode()
    {
        return setCode.GetHashCode() * collectorsId;
    }

    private static ShortCard _testCard;
    public static ShortCard TestCard(string set, int collId)
    {
        if(_testCard == null)
        {
            _testCard = new ShortCard(set, collId);
            return _testCard;
        }
        else
        {
            _testCard.setCode = set;
            _testCard.collectorsId = collId;
            return _testCard;
        }
    }
}

public class StandardFormat : PokemonFormat
{
    public override bool RequireStandardLegal => true;
    protected override void CustomFormatRules(PokemonDeck deck)
    {
        // None
    }
}