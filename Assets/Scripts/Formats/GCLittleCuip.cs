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
    private List<string> bannedAttacks = new List<string>(){ "Mad Party", "Let's All Rollout" };
    protected override List<string> BannedAttacks { get => bannedAttacks; }
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
