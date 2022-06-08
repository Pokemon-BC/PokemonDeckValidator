using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PKMN.Cards;

public class GCAllFourOne : PokemonFormat
{
    private List<ShortCard> banList = new List<ShortCard>()
    {
        new ShortCard("BRS", "7"),  // Grottle
        new ShortCard("BRS", "8"),  // Torterra
        new ShortCard("BRS", "10"), // Wormadam
        new ShortCard("BRS", "77"), // Wormadam 2
        new ShortCard("BRS", "98")  // Wormadam 3
    };

    protected override List<ShortCard> FormatBanList { get => banList; }
    protected override bool RequireStandardLegal { get => true; }

    public string[] bannedAttacks = new string[] { "Mad Party", "Let's All Rollout" };

    protected override void CustomFormatRules(PokemonDeck deck)
    {
        Dictionary<string, StashedCard> cardCounts = new Dictionary<string, StashedCard>();

        for (int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck current = deck.DeckCards[i];
            
            if (cardCounts.ContainsKey(current.Reference.Name))
            {
                cardCounts[current.Reference.Name].qty += current.Quantity;
                cardCounts[current.Reference.Name].cards.Add(current);
            }
            else
            {
                if (!current.Reference.IsBasicEnergy())
                {
                    cardCounts.Add(current.Reference.Name, new StashedCard(current.Quantity, current));
                }
            }

            if (current.Reference.Supertype != CardSupertype.UNKNOWN)
            {
                if (current.Reference.HasRulebox())
                {
                    current.AddNote(NoteType.INVALID, "Cards with Rule Boxes are not allowed.");
                }
                if (current.Reference.Supertype == CardSupertype.POKEMON && CardHasBannedAttack(current))
                {
                    current.AddNote(NoteType.INVALID, "This card has a banned attack.");
                }
                // Ability check
                if (current.Reference.Supertype == CardSupertype.POKEMON)
                {
                    if (Array.Exists(current.Reference.Abilities, (a) => a.Name == "Shady Dealings"))
                    {
                        current.AddNote(NoteType.INVALID, "This card has a banned ability.");
                    }
                }
            }
        }

        foreach (KeyValuePair<string, StashedCard> combination in cardCounts)
        {
            if (combination.Value.qty != 1 && combination.Value.qty != 4)
            {
                foreach (CardInDeck cid in combination.Value.cards)
                {
                    cid.AddNote(NoteType.INVALID, "Cards must be played in a quantity of 1 or 4.");
                }
            }
        }
    }

    private bool CardHasBannedAttack(CardInDeck card)
    {
        PokemonCard reference = card.Reference;
        PokemonAttack[] attacks = reference.Attacks;
        for (int i = 0, count = attacks.Length; i < count; i++)
        {
            if (Array.Exists(bannedAttacks, (e) =>
            {
                return e == attacks[i].Name;
            }))
            {
                return true;
            }
        }
        return false;
    }

    private class StashedCard
    {
        public StashedCard(int startQty, CardInDeck firstCard)
        {
            qty = startQty;
            cards = new List<CardInDeck>();
            cards.Add(firstCard);
        }

        public int qty;
        public List<CardInDeck> cards;
    }
}
