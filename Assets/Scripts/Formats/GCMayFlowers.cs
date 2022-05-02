using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKMN.Cards;

public class GCMayFlowers : PokemonFormat
{
    protected override bool RequireStandardLegal { get => true; }

    public List<ShortCard> banList = new List<ShortCard>{
        new ShortCard("BRS", "10"),
        new ShortCard("CEL", "3"),
        new ShortCard("BRS", "7"),
        new ShortCard("BRS", "8")
    };
    protected override List<ShortCard> FormatBanList { get => banList; }

    protected override void CustomFormatRules(PokemonDeck deck)
    {
        for (int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck cid = deck.DeckCards[i];
            PokemonCard current = cid.Reference;
            // Rulebox
            if (current.HasRulebox())
            {
                cid.AddNote(NoteType.INVALID, "Rulebox Cards are not allowed in this format.");
            }
            // * Strike Cards
            if (current.Supertype == CardSupertype.POKEMON)
            {
                if (Array.Exists(current.Subtypes, (e) => e == CardSubtype.SINGLE_STRIKE || e == CardSubtype.RAPID_STRIKE || e == CardSubtype.FUSION_STRIKE))
                {
                    cid.AddNote(NoteType.INVALID, "Strike Cards are not allowed in this format.");
                }
            }
            // Shady Dealings
            if (current.Supertype == CardSupertype.POKEMON && current.Abilities.Length != 0)
            {
                if (Array.Exists(current.Abilities, (e) => e.Name == "Shady Dealings"))
                {
                    cid.AddNote(NoteType.INVALID, "Cards with the ability Shady Dealings are not allowed in this deck");
                }
            }
            // Type clause
            if (current.Supertype == CardSupertype.POKEMON)
            {
                if (!Array.Exists(current.Types, (e) => e == PokemonType.WATER || e == PokemonType.GRASS))
                {
                    cid.AddNote(NoteType.INVALID, "Only Grass and Water Type Pokemon are allowed in this format.");
                }
            }
        }
    }
}
