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
    protected virtual bool RuleBoxesBanned { get => false; }

    protected virtual List<ShortCard> FormatBanList { get => null; }
    protected virtual List<string> BannedAttacks { get => null; }
    protected virtual List<string> BannedAbilities { get => null; }

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
        // Ban List for attacks and abilities
        ApplyAttackAbilityBanlist(deck);
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
        for (int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck card = deck.DeckCards[i];
            if (card.Reference.Supertype == CardSupertype.POKEMON)
            {
                if (Array.Exists(card.Reference.Subtypes, (e) => e == CardSubtype.BASIC))
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

        HashSet<string> prismStars = new HashSet<string>();
        bool deckHasAceSpec = false;

        for (int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck cid = deck.DeckCards[i];
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
                if (cid.Reference.Rules.Length != 0 && Array.Exists(cid.Reference.Rules, (e) => e.Contains("(Prism Star)")))
                {
                    if (cid.Quantity > 1)
                    {
                        cid.AddNote(NoteType.INVALID, "You can only have 1 copy of a Prism Star card");
                    }
                    else
                    {
                        if (prismStars.Contains(name))
                        {
                            cid.AddNote(NoteType.INVALID, "You can only have 1 copy of a Prism Star card");
                        }
                        else
                        {
                            prismStars.Add(name);
                        }
                    }
                }
                if (cid.Reference.Rules.Length != 0 && Array.Exists(cid.Reference.Rules, (e) => e.Contains("ACE SPEC")))
                {
                    if (deckHasAceSpec)
                    {
                        deck.AddNote(NoteType.INVALID, "Decks can only have 1 ACE SPEC card.");
                        cid.AddNote(NoteType.INVALID, "Decks can only have 1 ACE SPEC card.");
                    }
                    else
                    {
                        deckHasAceSpec = true;
                        if (cid.Quantity > 1)
                        {
                            deck.AddNote(NoteType.INVALID, "Decks can only have 1 ACE SPEC card.");
                            cid.AddNote(NoteType.INVALID, "Decks can only have 1 ACE SPEC card.");
                        }
                    }
                }
            }
        }
    }

    private void LegalitiesCheck(PokemonDeck deck)
    {
        for (int i = 0, count = deck.DeckCards.Count; i < count; i++)
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
                if (RuleBoxesBanned)
                {
                    if (cid.Reference.HasRulebox())
                    {
                        cid.AddNote(NoteType.INVALID, "Cards with Ruleboxes are banned in this format.");
                    }
                }
            }
        }
    }

    private void ApplyBanList(PokemonDeck deck)
    {
        List<ShortCard> banlist = FormatBanList;
        if (banlist != null)
        {
            List<PokemonCard> longBanlist = new List<PokemonCard>();
            for (int i = 0, count = banlist.Count; i < count; i++)
            {
                longBanlist.Add(banlist[i].GetDetails());
            }
            for (int i = 0, count = deck.DeckCards.Count; i < count; i++)
            {
                CardInDeck cid = deck.DeckCards[i];
                PokemonCard reference = cid.Reference;
                if (longBanlist.Exists((e) => e.IsReprintOf(reference)))
                {
                    cid.AddNote(NoteType.INVALID, "This card is banned in this format.");
                }
            }
        }
    }

    private void ApplyAttackAbilityBanlist(PokemonDeck deck)
    {
        for (int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck cid = deck.DeckCards[i];
            PokemonCard reference = cid.Reference;

            if (reference.Supertype == CardSupertype.POKEMON)
            {
                if (BannedAttacks != null)
                {
                    if (CardHasBannedAttack(cid))
                    {
                        cid.AddNote(NoteType.INVALID, "This card has a banned attack.");
                    }
                }
                if (BannedAbilities != null)
                {
                    if (CardHasBannedAbility(cid))
                    {
                        cid.AddNote(NoteType.INVALID, "This card has a banned ability.");
                    }
                }
            }
        }
    }

    protected bool CardHasBannedAttack(CardInDeck card)
    {
        PokemonCard reference = card.Reference;
        PokemonAttack[] attacks = reference.Attacks;
        for (int i = 0, count = attacks.Length; i < count; i++)
        {
            if (BannedAttacks.Contains(attacks[i].Name))
            {
                return true;
            }
        }
        return false;
    }

    protected bool CardHasBannedAbility(CardInDeck card)
    {
        PokemonCard reference = card.Reference;
        PokemonAbility[] abilities = reference.Abilities;
        for (int i = 0, count = abilities.Length; i < count; i++)
        {
            if (BannedAbilities.Contains(abilities[i].Name))
            {
                return true;
            }
        }
        return false;
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
