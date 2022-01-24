using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using PKMN.Cards;
public abstract class PokemonFormat
{
    // The number of cards with the same name allowed in a deck
    protected virtual int MaxDuplicates { get => 4; }

    protected virtual bool RequireStandardLegal { get => false; }
    protected virtual bool RequireExpandedLegal { get => false; }
    protected virtual bool RequireUnlimitedLegal { get => false; }

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
        if (deck.TotalCards != 60)
        {
            deck.AddNote(NoteType.INVALID, "Deck Needs 60 Cards");
        }
        bool anyBasic = false;
        for(int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck card = deck.DeckCards[i];
            if(card.Reference.Supertype == CardSupertype.POKEMON)
            {
                if(Array.Exists(card.Reference.Subtypes, (e) => e == CardSubtype.BASIC))
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
        int maxCopies = MaxDuplicates;
        Dictionary<string, int> cardCounts = new Dictionary<string, int>();
        for (int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck cid = deck.DeckCards[i];
            Debug.Log("Checking quantity for card " + cid.ToString());
            if (cid.Reference.Supertype != CardSupertype.UNKNOWN &&
               (cid.Reference.Supertype != CardSupertype.ENERGY || !Array.Exists(cid.Reference.Subtypes, (e) => e == CardSubtype.BASIC)))
            {
                string name = cid.Reference.Name;
                if (cardCounts.ContainsKey(name))
                {
                    cardCounts[name] += cid.Quantity;
                }
                else
                {
                    cardCounts.Add(name, cid.Quantity);
                }
                if (cardCounts[name] > maxCopies)
                {
                    cid.AddNote(NoteType.INVALID, "There are more than " + maxCopies + " cards with this name.");
                }
            }   
        }
    }

    private void LegalitiesCheck(PokemonDeck deck)
    {
        for(int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck cid = deck.DeckCards[i];
            if (cid.Reference.Supertype != CardSupertype.UNKNOWN)
            {
                Legalities legal = cid.Reference.Legalities;
                if (RequireUnlimitedLegal)
                {
                    if (legal.Unlimited == Legality.BANNED)
                    {
                        cid.AddNote(NoteType.INVALID, "This card is Banned in the Unlimited format.");
                    }
                    else if (!legal.IsUnlimitedLegal)
                    {
                        cid.AddNote(NoteType.INVALID, "This card is not Legal in the Unlimited format.");
                    }
                }
                if (RequireExpandedLegal)
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
    }

    private void ApplyBanList(PokemonDeck deck)
    {
        List<ShortCard> banlist = FormatBanList;
        if(banlist != null)
        {
            List<PokemonCard> longBanlist = new List<PokemonCard>();
            for(int i = 0, count = banlist.Count; i < count; i++)
            {
                longBanlist.Add(banlist[i].GetDetails());
            }
            for (int i = 0, count = deck.DeckCards.Count; i < count; i++)
            {
                CardInDeck cid = deck.DeckCards[i];
                PokemonCard reference = cid.Reference;
                if(longBanlist.Exists((e) => e.IsReprintOf(reference)))
                {
                    cid.AddNote(NoteType.INVALID, "This card is banned in this format.");
                }
            }
        }
    }

    protected abstract void CustomFormatRules(PokemonDeck deck);
}

public class ShortCard : IEquatable<ShortCard>
{
    private string setCode;
    public string SetCode { get => setCode; }
    private string collectorsId;
    public string CollectorsId { get => collectorsId; }

    public ShortCard(string setCode, string collectorsId)
    {
        this.setCode = setCode;
        this.collectorsId = collectorsId;
    }

    public PokemonCard GetDetails()
    {
        return CardDatabase.LookupCard(setCode, collectorsId);
    }

    public override bool Equals(object obj)
    {
        return obj is ShortCard card && Equals(card);
    }

    public bool Equals(ShortCard card)
    {
        return setCode == card.setCode &&
               collectorsId == card.collectorsId;
    }

    public override int GetHashCode()
    {
        return setCode.GetHashCode() * collectorsId.GetHashCode();
    }

    public override string ToString()
    {
        return string.Format("(ShortCard {0} {1})", setCode, collectorsId);
    }
}

public class StandardFormat : PokemonFormat
{
    protected override bool RequireStandardLegal => true;
    protected override void CustomFormatRules(PokemonDeck deck)
    {
        // None
    }
}

public class ExpandedFormat : PokemonFormat
{
    protected override bool RequireExpandedLegal => true;
    protected override void CustomFormatRules(PokemonDeck deck)
    {
        // None
    }
}
