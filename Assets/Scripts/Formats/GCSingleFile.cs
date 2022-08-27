using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PKMN.Cards;

public class GCSingleFile : PokemonFormat
{
    protected override bool RequireExpandedLegal { get => true; }

    protected override List<ShortCard> FormatBanList { get => bannedCards; }
    private List<ShortCard> bannedCards = new List<ShortCard>()
    {
        // TODO
    };

    protected override void CustomFormatRules(PokemonDeck deck)
    {
        for (int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck current = deck.DeckCards[i];

            if (current.Reference.Supertype == CardSupertype.POKEMON)
            {
                if (Array.Exists(current.Reference.Subtypes, (st) =>
                {
                    return st == CardSubtype.EX || st == CardSubtype.MEGA || st == CardSubtype.GX || st == CardSubtype.V ||
                           st == CardSubtype.VMAX || st == CardSubtype.V_UNION || st == CardSubtype.VSTAR;
                }))
                {
                    current.AddNote(NoteType.INVALID, "Pokemon worth more than 1 prize are not valid in this format.");
                }
            }
        }
    }
}
