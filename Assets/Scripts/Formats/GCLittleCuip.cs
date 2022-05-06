using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PKMN.Cards;

public class GCLittleCuip : PokemonFormat
{
    private List<ShortCard> banList = new List<ShortCard>()
    {
        new ShortCard("BST", "131"), // Rapid Strike Scroll of Swirls
    };

    protected override List<ShortCard> FormatBanList { get => banList; }

    protected override bool RequireStandardLegal { get => true; }

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
            // Shady Dealings
            if (current.Supertype == CardSupertype.POKEMON && current.Abilities.Length != 0)
            {
                if (Array.Exists(current.Abilities, (e) => e.Name == "Shady Dealings"))
                {
                    cid.AddNote(NoteType.INVALID, "Cards with the ability Shady Dealings are not allowed in this deck.");
                }
            }
            // Let's All Rollout & Mad Party
            if (current.Supertype == CardSupertype.POKEMON && current.Attacks.Length != 0)
            {
                if (Array.Exists(current.Attacks, (e) => e.Name == "Let's All Rollout" || e.Name == "Mad Party"))
                {
                    cid.AddNote(NoteType.INVALID, "Cards with the attack \"Let's All Rollout\" or \"Mad Party\" are not allowed in this deck.");
                }
            }
            // HP Requirement
            if (current.Supertype == CardSupertype.POKEMON)
            {
                if (current.HP > 100)
                {
                    cid.AddNote(NoteType.INVALID, "Pokemon must have 100HP or less in this format.");
                }
            }
        }
    }
}
