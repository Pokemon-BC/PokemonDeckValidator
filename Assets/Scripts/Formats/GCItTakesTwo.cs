using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKMN.Cards;

public class GCItTakesTwo : PokemonFormat
{
    protected override bool RequireStandardLegal { get => true; }
    private List<string> bannedAttacks = new List<string>{ "Mad Party", "Let's All Rollout" };
    protected override List<string> BannedAttacks => bannedAttacks;


    protected override void CustomFormatRules(PokemonDeck deck)
    {
        for (int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck current = deck.DeckCards[i];

            if (current.Reference.Supertype == CardSupertype.POKEMON)
            {
                if (current.Reference.HasRulebox())
                {
                    current.AddNote(NoteType.INVALID, "Cards with Rule Boxes are not allowed.");
                }
                if (Array.Exists(current.Reference.Abilities, (a) => a.Name == "Shady Dealings"))
                {
                    current.AddNote(NoteType.INVALID, "This card has a banned ability.");
                }
                // energy clause
                foreach (PokemonAttack atk in current.Reference.Attacks)
                {
                    if (atk.ConvertedEnergyCost > 2)
                    {
                        current.AddNote(NoteType.INVALID, "This card has an attack which needs too many energy.");
                        break; // we only need 1 invalid attack so we don't need to print it twice.
                    }
                }
            }
        }
    }
}
