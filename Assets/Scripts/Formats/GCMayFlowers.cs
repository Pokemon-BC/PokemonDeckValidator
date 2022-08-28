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

    private List<string> bannedAbilities = new List<string>(){ "Shady Dealings" };
    protected override List<string> BannedAbilities { get => bannedAbilities; }

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
