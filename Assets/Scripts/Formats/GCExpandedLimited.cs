using System;
using System.Collections.Generic;

using PKMN.Cards;

public class GCExpandedLimited : PokemonFormat
{
    protected override bool RequireExpandedLegal => true;

    private List<string> bannedAttacks = new List<string> { "Lost March", "Night March", "Mad Party", "Let's All Rollout", "Bee Revenge", "Vengeance" };
    protected override List<string> BannedAttacks => bannedAttacks;

    protected override void CustomFormatRules(PokemonDeck deck)
    {
        for (int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck current = deck.DeckCards[i];
            if (current.Reference.Supertype != CardSupertype.UNKNOWN)
            {
                if (current.Reference.HasRulebox())
                {
                    current.AddNote(NoteType.INVALID, "Cards with Rule Boxes are not allowed.");
                }
            }
        }
    }
}
