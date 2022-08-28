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
        new ShortCard("PGO", "21"), // Magikarp PGO
        new ShortCard("PGO", "22"), // Gyarados PGO
        new ShortCard("PGO", "53"), // Ditto PGO
        new ShortCard("AOR", "69"), // Ace Trainer
        new ShortCard("AOR", "10"), // Vespiquen
        new ShortCard("PLF", "12"), // Flareon (Vengeance)
        new ShortCard("CPA", "26") // Machamp (Revenge)
    };

    private List<string> bannedAttacks = new List<string>() { "Mad Party", "Night March", "Lost March", "Matron's Anger" };
    protected override List<string> BannedAttacks { get => bannedAttacks; }
    private List<string> bannedAbilities = new List<string>() { "Shady Dealings" };
    protected override List<string> BannedAbilities { get => bannedAbilities; }

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
