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
    protected override bool RuleBoxesBanned => true;

    private List<string> bannedAttacks = new List<string>(){ "Mad Party", "Let's All Rollout" };
    protected override List<string> BannedAttacks { get => bannedAttacks; }
    private List<string> bannedAbilities = new List<string>(){ "Shady Dealings" };
    protected override List<string> BannedAbilities { get => bannedAbilities; }

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
