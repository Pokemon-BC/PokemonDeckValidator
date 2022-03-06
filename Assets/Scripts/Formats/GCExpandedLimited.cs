using System;
using System.Collections.Generic;

using PKMN.Cards;

public class GCExpandedLimited : PokemonFormat
{
    protected override bool RequireExpandedLegal => true;

    public string[] bannedAttacks = new string[]{"Lost March", "Night March", "Mad Party", "Let's All Rollout", "Bee Revenge", "Vengeance"};

    protected override void CustomFormatRules(PokemonDeck deck)
    {
        for(int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck current = deck.DeckCards[i];
            if(current.Reference.Supertype != CardSupertype.UNKNOWN)
            {
                if(current.Reference.HasRulebox())
                {
                    current.AddNote(NoteType.INVALID, "Cards with Rule Boxes are not allowed.");
                }
                if(current.Reference.Supertype == CardSupertype.POKEMON && CardHasBannedAttack(current))
                {
                    current.AddNote(NoteType.INVALID, "This card has a banned attack.");
                }
            }
        }
    }

    private bool CardHasBannedAttack(CardInDeck card)
    {
        PokemonCard reference = card.Reference;
        PokemonAttack[] attacks = reference.Attacks;
        for(int i = 0, count = attacks.Length; i < count; i++)
        {
            if(Array.Exists(bannedAttacks, (e) => 
            {
                return e == attacks[i].Name;
            }))
            {
                return true;
            }
        }
        return false;
    }
}
